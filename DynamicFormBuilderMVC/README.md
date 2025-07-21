# Dynamic Form Builder - ASP.NET Core MVC

A powerful ASP.NET Core MVC application for creating dynamic forms with drag-and-drop functionality, HTML generation, and automated email scheduling.

## 🎯 Features

### 🎨 Visual Form Builder
- **Drag & Drop Interface**: Intuitive component palette with various form elements
- **Real-time Component Editing**: Click to select and modify component properties
- **8 Component Types**:
  - Text Input
  - Textarea
  - Date Picker
  - Select Dropdown
  - Checkbox
  - Radio Buttons
  - Data Tables
  - Interactive Charts (Bar, Line, Pie, Doughnut)

### 📄 HTML Generation & Preview
- **Live Preview**: Real-time form preview with working components
- **HTML Export**: Generate clean, styled HTML code with embedded CSS and JavaScript
- **Download Functionality**: Export complete HTML files
- **Copy to Clipboard**: Quick HTML code copying

### 📧 Email Scheduler System
- **Automated Emails**: Schedule form delivery to users
- **Multiple Frequencies**: Daily, Weekly, Monthly, Yearly options
- **Quartz.NET Integration**: Professional job scheduling
- **SMTP Support**: Works with Gmail, Outlook, and custom SMTP servers
- **Email Templates**: Professional HTML email templates

## 🛠 Technology Stack

- **Backend**: ASP.NET Core 8.0 MVC
- **Database**: Entity Framework Core with In-Memory Database
- **Email**: MailKit for SMTP communication
- **Scheduling**: Quartz.NET for cron jobs
- **Frontend**: Bootstrap 5, jQuery, Chart.js
- **Drag & Drop**: Native HTML5 with SortableJS support
- **Icons**: Font Awesome

## 📁 Project Structure

```
DynamicFormBuilderMVC/
├── Controllers/
│   ├── HomeController.cs              # Main dashboard
│   └── FormBuilderController.cs       # Form builder operations
├── Models/
│   ├── DynamicForm.cs                 # Form entity
│   ├── FormComponent.cs               # Component entity
│   ├── EmailSchedule.cs               # Email schedule entity
│   └── ViewModels/                    # View models
├── Views/
│   ├── Home/Index.cshtml              # Dashboard
│   ├── FormBuilder/
│   │   ├── Index.cshtml               # Form builder interface
│   │   ├── Preview.cshtml             # Preview & HTML generation
│   │   └── EmailScheduler.cshtml      # Email scheduling
│   └── Shared/
│       ├── _Layout.cshtml             # Main layout
│       └── _ComponentRenderer.cshtml   # Component renderer
├── Services/
│   ├── FormBuilderService.cs         # Form operations
│   └── EmailService.cs               # Email & scheduling
├── Data/
│   └── ApplicationDbContext.cs        # Entity Framework context
├── wwwroot/
│   ├── css/formbuilder.css           # Custom styles
│   └── js/formbuilder.js             # Drag & drop functionality
└── Program.cs                         # Application startup
```

## 🚀 Getting Started

### Prerequisites
- .NET 8.0 SDK
- Visual Studio 2022 (recommended) or VS Code
- SQL Server (optional - uses In-Memory DB by default)

### Installation

1. **Clone or extract the project**
   ```bash
   cd DynamicFormBuilderMVC
   ```

2. **Restore packages**
   ```bash
   dotnet restore
   ```

3. **Configure email settings (optional)**
   Update `appsettings.json`:
   ```json
   {
     "Smtp": {
       "Host": "smtp.gmail.com",
       "Port": "587",
       "Username": "your-email@gmail.com",
       "Password": "your-app-password"
     }
   }
   ```

4. **Run the application**
   ```bash
   dotnet run
   ```

5. **Open in browser**
   Navigate to `https://localhost:5001` or `http://localhost:5000`

## 💻 Visual Studio 2022 Setup

1. **Open the solution**
   - File → Open → Project/Solution
   - Select `DynamicFormBuilderMVC.csproj`

2. **Configure startup**
   - Set `DynamicFormBuilderMVC` as startup project
   - Press F5 to run with debugging

3. **Package management**
   - All NuGet packages are included in the project file
   - Visual Studio will automatically restore packages

## 📖 Usage Guide

### Building Forms

1. **Start Building**
   - Navigate to Form Builder from the main menu
   - The application creates a default form if none exists

2. **Add Components**
   - Drag components from the left palette to the canvas
   - Click on components to select and edit properties

3. **Customize Components**
   - Use the Properties Panel on the right to modify:
     - Labels and placeholders
     - Options for select/radio components
     - Chart types for chart components
     - Number of rows for textareas

4. **Save Changes**
   - All changes are automatically saved to the database
   - Form title updates when you leave the title field

### Generate HTML

1. **Preview Form**
   - Click "Preview & Generate" to see your form
   - Switch between Live Preview and HTML Code tabs

2. **Export Options**
   - **Download**: Get a complete HTML file with embedded styles
   - **Copy**: Copy raw HTML code to clipboard
   - **View Source**: See the generated HTML in the browser

### Email Scheduling

1. **Set up Email Schedules**
   - Go to Email Scheduler from the main menu
   - Add recipient email addresses
   - Choose frequency (Daily/Weekly/Monthly/Yearly)
   - Set delivery time

2. **Configure SMTP**
   - Update `appsettings.json` with your SMTP settings
   - For Gmail: Enable 2FA and use an App Password
   - Test emails to verify configuration

3. **Manage Schedules**
   - Enable/disable schedules as needed
   - Remove old schedules
   - Monitor delivery status

## 🔧 Configuration

### Database Configuration

By default, the application uses Entity Framework In-Memory database. To use SQL Server:

1. Update connection string in `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=DynamicFormBuilderDB;Trusted_Connection=true"
  }
}
```

2. Update `Program.cs`:
```csharp
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
```

3. Add migrations:
```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

### Email Configuration

#### Gmail Setup
1. Enable 2-factor authentication
2. Generate an app password
3. Use these settings:
   ```json
   {
     "Smtp": {
       "Host": "smtp.gmail.com",
       "Port": "587",
       "Username": "your-email@gmail.com",
       "Password": "your-16-character-app-password"
     }
   }
   ```

#### Outlook Setup
```json
{
  "Smtp": {
    "Host": "smtp-mail.outlook.com",
    "Port": "587",
    "Username": "your-email@outlook.com",
    "Password": "your-password"
  }
}
```

## 🎨 Key Features Implementation

### Drag & Drop Functionality
- Uses native HTML5 drag and drop API
- Real-time visual feedback during drag operations
- Automatic component positioning and canvas management

### Component System
- Modular component architecture
- JSON-based property storage
- Extensible design for adding new component types

### HTML Generation Engine
- Server-side HTML generation with embedded CSS
- Chart.js integration for interactive charts
- Responsive design with mobile-first approach

### Email Automation
- Quartz.NET for professional job scheduling
- Cron-based scheduling with multiple frequency options
- HTML email templates with embedded form content
- SMTP configuration with multiple provider support

## 🔒 Security Considerations

- **Input Validation**: All user inputs are validated server-side
- **CSRF Protection**: Anti-forgery tokens for form submissions
- **Email Security**: SMTP credentials stored in configuration
- **SQL Injection**: Entity Framework protects against SQL injection
- **XSS Protection**: HTML encoding for user-generated content

## 📊 API Endpoints

### Form Management
- `GET /FormBuilder` - Form builder interface
- `POST /FormBuilder/AddComponent` - Add new component
- `POST /FormBuilder/UpdateComponent` - Update component properties
- `POST /FormBuilder/DeleteComponent` - Remove component
- `GET /FormBuilder/GenerateHtml` - Generate HTML output

### Email Scheduling
- `POST /FormBuilder/CreateEmailSchedule` - Create new schedule
- `POST /FormBuilder/DeleteEmailSchedule` - Remove schedule
- `POST /FormBuilder/ToggleEmailSchedule` - Enable/disable schedule
- `GET /FormBuilder/GetEmailSchedules` - Get form schedules

## 🧪 Testing

### Manual Testing
1. **Form Creation**: Test all component types and properties
2. **Drag & Drop**: Verify smooth drag and drop operation
3. **HTML Generation**: Validate generated HTML in different browsers
4. **Email Scheduling**: Test email delivery with different frequencies

### Development Testing
```bash
# Run unit tests (if implemented)
dotnet test

# Check code coverage
dotnet test --collect:"XPlat Code Coverage"
```

## 🚀 Deployment

### IIS Deployment
1. Publish the application:
   ```bash
   dotnet publish -c Release -o ./publish
   ```

2. Copy files to IIS server
3. Configure IIS application pool for .NET 8
4. Update `appsettings.json` for production

### Azure App Service
1. Right-click project → Publish
2. Select Azure App Service
3. Configure connection strings and app settings
4. Deploy and test

## 🔄 Future Enhancements

- [ ] User authentication and authorization
- [ ] Form submission data collection
- [ ] Advanced chart customization
- [ ] Template library for common forms
- [ ] Real-time collaboration features
- [ ] Mobile app for form management
- [ ] Advanced email analytics
- [ ] Custom component creation

## 🐛 Troubleshooting

### Common Issues

1. **Email not sending**
   - Check SMTP configuration in `appsettings.json`
   - Verify credentials and firewall settings
   - Test with a simple email client

2. **Drag & drop not working**
   - Ensure JavaScript is enabled
   - Check browser console for errors
   - Verify jQuery and other dependencies are loaded

3. **Components not saving**
   - Check database connection
   - Verify Entity Framework configuration
   - Look for validation errors in browser console

### Debug Mode
Run in development mode to see detailed error messages:
```bash
dotnet run --environment Development
```

## 📝 License

This project is licensed under the MIT License.

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Add tests if applicable
5. Submit a pull request

## 📞 Support

For issues and questions:
1. Check the troubleshooting section
2. Review the project documentation
3. Create an issue in the repository
4. Include detailed error messages and steps to reproduce

---

**Built with ❤️ using ASP.NET Core MVC**