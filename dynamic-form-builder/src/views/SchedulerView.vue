<template>
  <div class="scheduler-view">
    <div class="scheduler-header">
      <h2>Email Scheduler</h2>
      <p>Schedule automated emails with your form design to be sent to users.</p>
    </div>

    <div class="scheduler-content">
      <!-- Add New Schedule -->
      <div class="add-schedule-section">
        <h3>Add New Email Schedule</h3>
        <form @submit.prevent="addSchedule" class="schedule-form">
          <div class="form-row">
            <div class="form-group">
              <label for="email">Email Address</label>
              <input 
                id="email"
                type="email" 
                v-model="newSchedule.email"
                class="form-control"
                placeholder="recipient@example.com"
                required
              />
            </div>
            
            <div class="form-group">
              <label for="frequency">Frequency</label>
              <select 
                id="frequency"
                v-model="newSchedule.frequency"
                class="form-control"
                required
              >
                <option value="daily">Daily</option>
                <option value="weekly">Weekly</option>
                <option value="monthly">Monthly</option>
                <option value="yearly">Yearly</option>
              </select>
            </div>
            
            <div class="form-group">
              <label for="time">Time</label>
              <input 
                id="time"
                type="time" 
                v-model="newSchedule.time"
                class="form-control"
                required
              />
            </div>
          </div>
          
          <div class="form-actions">
            <button type="submit" class="btn btn-primary">
              Add Schedule
            </button>
            <button type="button" @click="resetForm" class="btn btn-secondary">
              Reset
            </button>
          </div>
        </form>
      </div>

      <!-- Current Schedules -->
      <div class="schedules-section">
        <h3>Current Email Schedules</h3>
        
        <div v-if="formStore.emailSchedules.length === 0" class="empty-state">
          <p>No email schedules configured yet.</p>
        </div>
        
        <div v-else class="schedules-list">
          <div 
            v-for="schedule in formStore.emailSchedules"
            :key="schedule.id"
            class="schedule-card"
          >
            <div class="schedule-info">
              <div class="schedule-email">
                <strong>{{ schedule.email }}</strong>
              </div>
              <div class="schedule-details">
                <span class="schedule-frequency">{{ capitalizeFrequency(schedule.frequency) }}</span>
                <span class="schedule-time">at {{ formatTime(schedule.time) }}</span>
              </div>
              <div class="schedule-status">
                <span :class="['status-badge', schedule.enabled ? 'status-active' : 'status-inactive']">
                  {{ schedule.enabled ? 'Active' : 'Inactive' }}
                </span>
              </div>
            </div>
            
            <div class="schedule-actions">
              <button 
                @click="toggleSchedule(schedule.id)"
                :class="['btn', schedule.enabled ? 'btn-warning' : 'btn-success']"
              >
                {{ schedule.enabled ? 'Disable' : 'Enable' }}
              </button>
              <button 
                @click="removeSchedule(schedule.id)"
                class="btn btn-danger"
              >
                Remove
              </button>
            </div>
          </div>
        </div>
      </div>

      <!-- Preview Email -->
      <div class="email-preview-section">
        <h3>Email Preview</h3>
        <p>This is how your form will appear in the scheduled emails:</p>
        
        <div class="email-preview">
          <div class="email-header">
            <h4>Subject: {{ emailSubject }}</h4>
            <p class="email-meta">From: Form Builder System &lt;noreply@formbuilder.com&gt;</p>
          </div>
          
          <div class="email-body">
            <div v-if="formStore.components.length === 0" class="no-form-content">
              <p>No form content available. Please add components to your form first.</p>
              <router-link to="/" class="btn btn-primary">Go to Form Builder</router-link>
            </div>
            
            <div v-else class="email-form-content">
              <h2>{{ formStore.formTitle }}</h2>
              <p>Here's your scheduled form update:</p>
              
              <div v-html="formStore.formHTML" class="rendered-form"></div>
              
              <div class="email-footer">
                <p><small>This email was sent automatically by the Form Builder system.</small></p>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Email Service Configuration -->
      <div class="service-config-section">
        <h3>Email Service Configuration</h3>
        <div class="config-info">
          <div class="info-card">
            <h4>📧 SMTP Configuration</h4>
            <p>Configure your SMTP server settings to send emails:</p>
            <ul>
              <li>SMTP Host: smtp.gmail.com (for Gmail)</li>
              <li>SMTP Port: 587 (TLS) or 465 (SSL)</li>
              <li>Authentication: Username/Password</li>
            </ul>
          </div>
          
          <div class="info-card">
            <h4>⚙️ Server Setup</h4>
            <p>To enable email scheduling, you need to:</p>
            <ul>
              <li>Set up a Node.js server with the email service</li>
              <li>Configure environment variables for SMTP</li>
              <li>Deploy the scheduler service</li>
            </ul>
          </div>
          
          <div class="info-card">
            <h4>🚀 Quick Start</h4>
            <p>Download the email service configuration:</p>
            <button @click="downloadEmailService" class="btn btn-primary">
              Download Email Service Code
            </button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { useFormBuilderStore } from '../stores/formBuilder'

const formStore = useFormBuilderStore()

const newSchedule = ref({
  email: '',
  frequency: 'daily' as 'daily' | 'weekly' | 'monthly' | 'yearly',
  time: '09:00'
})

const emailSubject = ref('Form Update - ' + formStore.formTitle)

const addSchedule = () => {
  if (!newSchedule.value.email || !newSchedule.value.frequency || !newSchedule.value.time) {
    alert('Please fill in all fields')
    return
  }

  formStore.addEmailSchedule({
    email: newSchedule.value.email,
    frequency: newSchedule.value.frequency,
    time: newSchedule.value.time,
    enabled: true,
    formId: 'current-form'
  })

  resetForm()
}

const resetForm = () => {
  newSchedule.value = {
    email: '',
    frequency: 'daily',
    time: '09:00'
  }
}

const removeSchedule = (id: string) => {
  if (confirm('Are you sure you want to remove this email schedule?')) {
    formStore.removeEmailSchedule(id)
  }
}

const toggleSchedule = (id: string) => {
  const schedule = formStore.emailSchedules.find(s => s.id === id)
  if (schedule) {
    formStore.updateEmailSchedule(id, { enabled: !schedule.enabled })
  }
}

const capitalizeFrequency = (frequency: string) => {
  return frequency.charAt(0).toUpperCase() + frequency.slice(1)
}

const formatTime = (time: string) => {
  const [hours, minutes] = time.split(':')
  const hour = parseInt(hours)
  const ampm = hour >= 12 ? 'PM' : 'AM'
  const displayHour = hour % 12 || 12
  return `${displayHour}:${minutes} ${ampm}`
}

const downloadEmailService = () => {
  const emailServiceCode = `// Email Service for Dynamic Form Builder
// To set up this service:
// 1. npm install nodemailer node-cron express cors
// 2. Set environment variables: EMAIL_USER, EMAIL_PASS, SMTP_HOST, SMTP_PORT
// 3. Run this service: node email-service.js

const express = require('express');
const nodemailer = require('nodemailer');
const cron = require('node-cron');
const cors = require('cors');

const app = express();
app.use(cors());
app.use(express.json());

// Email configuration
const transporter = nodemailer.createTransporter({
  host: process.env.SMTP_HOST || 'smtp.gmail.com',
  port: process.env.SMTP_PORT || 587,
  secure: false,
  auth: {
    user: process.env.EMAIL_USER,
    pass: process.env.EMAIL_PASS
  }
});

// Store schedules (in production, use a database)
let emailSchedules = ${JSON.stringify(formStore.emailSchedules, null, 2)};

// Form content
const formContent = \`${formStore.formHTML}\`;
const formTitle = "${formStore.formTitle}";

// Send email function
async function sendEmail(schedule) {
  try {
    const mailOptions = {
      from: process.env.EMAIL_USER,
      to: schedule.email,
      subject: \`Form Update - \${formTitle}\`,
      html: \`
        <div style="font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;">
          <h2>\${formTitle}</h2>
          <p>Here's your scheduled form update:</p>
          \${formContent}
          <hr>
          <p><small>This email was sent automatically by the Form Builder system.</small></p>
        </div>
      \`
    };

    await transporter.sendMail(mailOptions);
    console.log(\`Email sent to \${schedule.email}\`);
  } catch (error) {
    console.error('Error sending email:', error);
  }
}

// Schedule jobs
function setupSchedules() {
  emailSchedules.forEach(schedule => {
    if (!schedule.enabled) return;

    const [hours, minutes] = schedule.time.split(':');
    
    let cronPattern;
    switch (schedule.frequency) {
      case 'daily':
        cronPattern = \`\${minutes} \${hours} * * *\`;
        break;
      case 'weekly':
        cronPattern = \`\${minutes} \${hours} * * 1\`; // Monday
        break;
      case 'monthly':
        cronPattern = \`\${minutes} \${hours} 1 * *\`; // 1st of month
        break;
      case 'yearly':
        cronPattern = \`\${minutes} \${hours} 1 1 *\`; // Jan 1st
        break;
    }

    cron.schedule(cronPattern, () => {
      sendEmail(schedule);
    });
    
    console.log(\`Scheduled \${schedule.frequency} email to \${schedule.email} at \${schedule.time}\`);
  });
}

// API endpoints
app.get('/schedules', (req, res) => {
  res.json(emailSchedules);
});

app.post('/schedules', (req, res) => {
  const newSchedule = { ...req.body, id: Date.now().toString() };
  emailSchedules.push(newSchedule);
  setupSchedules(); // Restart scheduling
  res.json(newSchedule);
});

app.delete('/schedules/:id', (req, res) => {
  emailSchedules = emailSchedules.filter(s => s.id !== req.params.id);
  setupSchedules(); // Restart scheduling
  res.json({ success: true });
});

// Start server
const PORT = process.env.PORT || 3001;
app.listen(PORT, () => {
  console.log(\`Email service running on port \${PORT}\`);
  setupSchedules();
});

// Environment variables example (.env file):
/*
EMAIL_USER=your-email@gmail.com
EMAIL_PASS=your-app-password
SMTP_HOST=smtp.gmail.com
SMTP_PORT=587
*/`;

  const blob = new Blob([emailServiceCode], { type: 'text/javascript' })
  const url = URL.createObjectURL(blob)
  const a = document.createElement('a')
  a.href = url
  a.download = 'email-service.js'
  document.body.appendChild(a)
  a.click()
  document.body.removeChild(a)
  URL.revokeObjectURL(url)
}
</script>

<style scoped>
.scheduler-view {
  max-width: 1200px;
  margin: 0 auto;
}

.scheduler-header {
  margin-bottom: 2rem;
  text-align: center;
}

.scheduler-header h2 {
  color: #2c3e50;
  margin-bottom: 0.5rem;
}

.scheduler-header p {
  color: #6c757d;
  font-size: 1.1rem;
}

.scheduler-content {
  display: flex;
  flex-direction: column;
  gap: 2rem;
}

.add-schedule-section,
.schedules-section,
.email-preview-section,
.service-config-section {
  background: white;
  border-radius: 8px;
  box-shadow: 0 2px 4px rgba(0,0,0,0.1);
  padding: 2rem;
}

.add-schedule-section h3,
.schedules-section h3,
.email-preview-section h3,
.service-config-section h3 {
  margin: 0 0 1.5rem 0;
  color: #2c3e50;
}

.schedule-form {
  display: flex;
  flex-direction: column;
  gap: 1.5rem;
}

.form-row {
  display: grid;
  grid-template-columns: 2fr 1fr 1fr;
  gap: 1rem;
}

.form-group {
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
}

.form-group label {
  font-weight: 500;
  color: #495057;
}

.form-control {
  padding: 0.75rem;
  border: 1px solid #ced4da;
  border-radius: 4px;
  font-size: 1rem;
}

.form-control:focus {
  outline: none;
  border-color: #3498db;
  box-shadow: 0 0 0 2px rgba(52, 152, 219, 0.2);
}

.form-actions {
  display: flex;
  gap: 1rem;
}

.empty-state {
  text-align: center;
  padding: 2rem;
  color: #6c757d;
}

.schedules-list {
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

.schedule-card {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 1rem;
  border: 1px solid #dee2e6;
  border-radius: 4px;
  background: #f8f9fa;
}

.schedule-info {
  flex: 1;
}

.schedule-email {
  font-size: 1.1rem;
  margin-bottom: 0.25rem;
}

.schedule-details {
  display: flex;
  gap: 1rem;
  font-size: 0.9rem;
  color: #6c757d;
}

.schedule-status {
  margin-top: 0.5rem;
}

.status-badge {
  padding: 0.25rem 0.5rem;
  border-radius: 12px;
  font-size: 0.8rem;
  font-weight: 500;
}

.status-active {
  background: #d4edda;
  color: #155724;
}

.status-inactive {
  background: #f8d7da;
  color: #721c24;
}

.schedule-actions {
  display: flex;
  gap: 0.5rem;
}

.email-preview {
  border: 1px solid #dee2e6;
  border-radius: 4px;
  overflow: hidden;
}

.email-header {
  background: #f8f9fa;
  padding: 1rem;
  border-bottom: 1px solid #dee2e6;
}

.email-header h4 {
  margin: 0 0 0.5rem 0;
  color: #2c3e50;
}

.email-meta {
  margin: 0;
  font-size: 0.9rem;
  color: #6c757d;
}

.email-body {
  padding: 1rem;
}

.no-form-content {
  text-align: center;
  padding: 2rem;
  color: #6c757d;
}

.email-form-content h2 {
  color: #2c3e50;
  margin-bottom: 1rem;
}

.rendered-form {
  border: 1px solid #dee2e6;
  border-radius: 4px;
  padding: 1rem;
  margin: 1rem 0;
  background: #f8f9fa;
}

.email-footer {
  margin-top: 1rem;
  padding-top: 1rem;
  border-top: 1px solid #dee2e6;
  text-align: center;
}

.config-info {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(300px, 1fr));
  gap: 1.5rem;
}

.info-card {
  border: 1px solid #dee2e6;
  border-radius: 4px;
  padding: 1.5rem;
  background: #f8f9fa;
}

.info-card h4 {
  margin: 0 0 1rem 0;
  color: #2c3e50;
}

.info-card ul {
  margin: 0.5rem 0;
  padding-left: 1.5rem;
}

.info-card li {
  margin-bottom: 0.5rem;
}

.btn {
  padding: 0.5rem 1rem;
  border: none;
  border-radius: 4px;
  cursor: pointer;
  text-decoration: none;
  display: inline-block;
  text-align: center;
  font-size: 0.9rem;
  transition: all 0.3s;
}

.btn-primary {
  background: #3498db;
  color: white;
}

.btn-primary:hover {
  background: #2980b9;
}

.btn-secondary {
  background: #6c757d;
  color: white;
}

.btn-secondary:hover {
  background: #5a6268;
}

.btn-success {
  background: #28a745;
  color: white;
}

.btn-success:hover {
  background: #218838;
}

.btn-warning {
  background: #ffc107;
  color: #212529;
}

.btn-warning:hover {
  background: #e0a800;
}

.btn-danger {
  background: #dc3545;
  color: white;
}

.btn-danger:hover {
  background: #c82333;
}

@media (max-width: 768px) {
  .form-row {
    grid-template-columns: 1fr;
  }
  
  .schedule-card {
    flex-direction: column;
    align-items: stretch;
    gap: 1rem;
  }
  
  .schedule-actions {
    justify-content: center;
  }
  
  .config-info {
    grid-template-columns: 1fr;
  }
}
</style>