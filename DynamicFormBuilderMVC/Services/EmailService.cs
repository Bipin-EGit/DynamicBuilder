using DynamicFormBuilderMVC.Data;
using DynamicFormBuilderMVC.Models;
using DynamicFormBuilderMVC.Models.ViewModels;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.EntityFrameworkCore;
using MimeKit;
using Quartz;

namespace DynamicFormBuilderMVC.Services
{
    public class EmailService : IEmailService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly ISchedulerFactory _schedulerFactory;

        public EmailService(ApplicationDbContext context, IConfiguration configuration, ISchedulerFactory schedulerFactory)
        {
            _context = context;
            _configuration = configuration;
            _schedulerFactory = schedulerFactory;
        }

        public async Task<EmailSchedule> CreateScheduleAsync(EmailScheduleViewModel model)
        {
            var timeSpan = TimeSpan.Parse(model.Time);
            var schedule = new EmailSchedule
            {
                Email = model.Email,
                Frequency = model.Frequency,
                Time = timeSpan,
                FormId = model.FormId,
                IsEnabled = true,
                CreatedAt = DateTime.UtcNow,
                NextSendAt = CalculateNextSendTime(model.Frequency, timeSpan)
            };

            _context.EmailSchedules.Add(schedule);
            await _context.SaveChangesAsync();

            // Schedule the job
            await ScheduleEmailJobAsync(schedule);

            return schedule;
        }

        public async Task<bool> DeleteScheduleAsync(int id)
        {
            var schedule = await _context.EmailSchedules.FindAsync(id);
            if (schedule != null)
            {
                // Remove from Quartz scheduler
                await RemoveEmailJobAsync(schedule);

                _context.EmailSchedules.Remove(schedule);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<bool> ToggleScheduleAsync(int id)
        {
            var schedule = await _context.EmailSchedules.FindAsync(id);
            if (schedule != null)
            {
                schedule.IsEnabled = !schedule.IsEnabled;
                await _context.SaveChangesAsync();

                if (schedule.IsEnabled)
                {
                    await ScheduleEmailJobAsync(schedule);
                }
                else
                {
                    await RemoveEmailJobAsync(schedule);
                }
                return true;
            }
            return false;
        }

        public async Task<List<EmailSchedule>> GetFormSchedulesAsync(int formId)
        {
            return await _context.EmailSchedules
                .Where(s => s.FormId == formId)
                .Include(s => s.Form)
                .OrderByDescending(s => s.CreatedAt)
                .ToListAsync();
        }

        public async Task SendFormEmailAsync(EmailSchedule schedule, string formHtml)
        {
            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("Dynamic Form Builder", _configuration["Smtp:Username"]));
                message.To.Add(new MailboxAddress("", schedule.Email));
                message.Subject = $"Form Update - {schedule.Form?.Title ?? "Dynamic Form"}";

                var bodyBuilder = new BodyBuilder
                {
                    HtmlBody = CreateEmailTemplate(schedule, formHtml)
                };

                message.Body = bodyBuilder.ToMessageBody();

                using var client = new SmtpClient();
                await client.ConnectAsync(
                    _configuration["Smtp:Host"], 
                    int.Parse(_configuration["Smtp:Port"] ?? "587"), 
                    SecureSocketOptions.StartTls);
                
                await client.AuthenticateAsync(
                    _configuration["Smtp:Username"], 
                    _configuration["Smtp:Password"]);
                
                await client.SendAsync(message);
                await client.DisconnectAsync(true);

                // Update last sent time
                schedule.LastSentAt = DateTime.UtcNow;
                schedule.NextSendAt = CalculateNextSendTime(schedule.Frequency, schedule.Time);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Log error (in production, use proper logging)
                Console.WriteLine($"Failed to send email to {schedule.Email}: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> TestEmailAsync(string email, string subject, string body)
        {
            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("Dynamic Form Builder", _configuration["Smtp:Username"]));
                message.To.Add(new MailboxAddress("", email));
                message.Subject = subject;

                var bodyBuilder = new BodyBuilder
                {
                    HtmlBody = body
                };

                message.Body = bodyBuilder.ToMessageBody();

                using var client = new SmtpClient();
                await client.ConnectAsync(
                    _configuration["Smtp:Host"], 
                    int.Parse(_configuration["Smtp:Port"] ?? "587"), 
                    SecureSocketOptions.StartTls);
                
                await client.AuthenticateAsync(
                    _configuration["Smtp:Username"], 
                    _configuration["Smtp:Password"]);
                
                await client.SendAsync(message);
                await client.DisconnectAsync(true);

                return true;
            }
            catch
            {
                return false;
            }
        }

        private DateTime CalculateNextSendTime(EmailFrequency frequency, TimeSpan time)
        {
            var now = DateTime.UtcNow;
            var today = now.Date.Add(time);

            return frequency switch
            {
                EmailFrequency.Daily => today > now ? today : today.AddDays(1),
                EmailFrequency.Weekly => GetNextWeekday(today, DayOfWeek.Monday),
                EmailFrequency.Monthly => GetNextMonthly(today),
                EmailFrequency.Yearly => GetNextYearly(today),
                _ => today.AddDays(1)
            };
        }

        private DateTime GetNextWeekday(DateTime date, DayOfWeek targetDay)
        {
            var daysUntilTarget = ((int)targetDay - (int)date.DayOfWeek + 7) % 7;
            if (daysUntilTarget == 0 && date <= DateTime.UtcNow)
                daysUntilTarget = 7;
            return date.AddDays(daysUntilTarget);
        }

        private DateTime GetNextMonthly(DateTime date)
        {
            var firstOfMonth = new DateTime(date.Year, date.Month, 1).Add(date.TimeOfDay);
            return firstOfMonth > DateTime.UtcNow ? firstOfMonth : firstOfMonth.AddMonths(1);
        }

        private DateTime GetNextYearly(DateTime date)
        {
            var firstOfYear = new DateTime(date.Year, 1, 1).Add(date.TimeOfDay);
            return firstOfYear > DateTime.UtcNow ? firstOfYear : firstOfYear.AddYears(1);
        }

        private async Task ScheduleEmailJobAsync(EmailSchedule schedule)
        {
            var scheduler = await _schedulerFactory.GetScheduler();
            
            var jobKey = new JobKey($"email-job-{schedule.Id}", "email-group");
            var triggerKey = new TriggerKey($"email-trigger-{schedule.Id}", "email-group");

            // Remove existing job if any
            await scheduler.DeleteJob(jobKey);

            var job = JobBuilder.Create<EmailJob>()
                .WithIdentity(jobKey)
                .UsingJobData("ScheduleId", schedule.Id)
                .Build();

            var trigger = TriggerBuilder.Create()
                .WithIdentity(triggerKey)
                .StartAt(schedule.NextSendAt ?? DateTime.UtcNow.AddMinutes(1))
                .WithSchedule(CreateSchedule(schedule.Frequency, schedule.Time))
                .Build();

            await scheduler.ScheduleJob(job, trigger);
        }

        private async Task RemoveEmailJobAsync(EmailSchedule schedule)
        {
            var scheduler = await _schedulerFactory.GetScheduler();
            var jobKey = new JobKey($"email-job-{schedule.Id}", "email-group");
            await scheduler.DeleteJob(jobKey);
        }

        private IScheduleBuilder CreateSchedule(EmailFrequency frequency, TimeSpan time)
        {
            return frequency switch
            {
                EmailFrequency.Daily => CronScheduleBuilder.DailyAtHourAndMinute(time.Hours, time.Minutes),
                EmailFrequency.Weekly => CronScheduleBuilder.WeeklyOnDayAndHourAndMinute(DayOfWeek.Monday, time.Hours, time.Minutes),
                EmailFrequency.Monthly => CronScheduleBuilder.MonthlyOnDayAndHourAndMinute(1, time.Hours, time.Minutes),
                EmailFrequency.Yearly => CronScheduleBuilder.CronSchedule($"0 {time.Minutes} {time.Hours} 1 1 ? *"),
                _ => SimpleScheduleBuilder.RepeatHourlyForever()
            };
        }

        private string CreateEmailTemplate(EmailSchedule schedule, string formHtml)
        {
            return $@"
<!DOCTYPE html>
<html lang='en'>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>Form Update - {schedule.Form?.Title}</title>
    <style>
        body {{ font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto; padding: 20px; background: #f5f5f5; }}
        .email-container {{ background: white; padding: 30px; border-radius: 8px; box-shadow: 0 2px 10px rgba(0,0,0,0.1); }}
        .header {{ text-align: center; margin-bottom: 30px; padding-bottom: 20px; border-bottom: 2px solid #3498db; }}
        .header h1 {{ color: #2c3e50; margin: 0; }}
        .header p {{ color: #666; margin: 10px 0 0 0; }}
        .form-content {{ margin: 20px 0; }}
        .footer {{ text-align: center; margin-top: 30px; padding-top: 20px; border-top: 1px solid #ddd; color: #666; font-size: 12px; }}
    </style>
</head>
<body>
    <div class='email-container'>
        <div class='header'>
            <h1>📧 Form Update</h1>
            <p>Your scheduled form report is ready</p>
        </div>
        <div class='form-content'>
            <h2>Form: {schedule.Form?.Title}</h2>
            <p>Frequency: {schedule.Frequency}</p>
            <p>Generated on: {DateTime.UtcNow:MMMM dd, yyyy 'at' HH:mm} UTC</p>
            <hr style='margin: 20px 0; border: none; border-top: 1px solid #ddd;' />
            {formHtml}
        </div>
        <div class='footer'>
            <p>This email was sent automatically by the Dynamic Form Builder system.</p>
            <p>If you no longer wish to receive these updates, please contact your administrator.</p>
        </div>
    </div>
</body>
</html>";
        }
    }

    // Quartz Job for sending emails
    public class EmailJob : IJob
    {
        private readonly IServiceProvider _serviceProvider;

        public EmailJob(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            using var scope = _serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();
            var formService = scope.ServiceProvider.GetRequiredService<IFormBuilderService>();

            var scheduleId = context.JobDetail.JobDataMap.GetInt("ScheduleId");
            var schedule = await dbContext.EmailSchedules
                .Include(s => s.Form)
                .FirstOrDefaultAsync(s => s.Id == scheduleId);

            if (schedule != null && schedule.IsEnabled)
            {
                var formHtml = await formService.GenerateHtmlAsync(schedule.FormId);
                await emailService.SendFormEmailAsync(schedule, formHtml);
            }
        }
    }
}