# Dynamic Form Builder

A powerful Vue.js application for creating dynamic forms with drag-and-drop functionality, HTML generation, and automated email scheduling.

## Features

### 🎨 Visual Form Builder
- **Drag & Drop Interface**: Intuitive component palette with various form elements
- **Real-time Preview**: See your form as you build it
- **Component Types**:
  - Text Input
  - Textarea
  - Date Picker
  - Select Dropdown
  - Checkbox
  - Radio Buttons
  - Data Tables
  - Interactive Charts (Bar, Line, Pie, Doughnut)

### 📄 HTML Generation
- **Live Preview**: Real-time preview of your form
- **HTML Export**: Generate clean, styled HTML code
- **Download**: Export complete HTML files with embedded CSS
- **Copy to Clipboard**: Quick HTML code copying

### 📧 Email Scheduler
- **Automated Emails**: Schedule form delivery to users
- **Multiple Frequencies**: Daily, Weekly, Monthly, Yearly options
- **Email Preview**: See exactly how your form will appear in emails
- **Enable/Disable**: Toggle schedules on/off
- **SMTP Support**: Works with Gmail, Outlook, and custom SMTP servers

## Tech Stack

- **Frontend**: Vue 3 + TypeScript
- **State Management**: Pinia
- **Routing**: Vue Router
- **Charts**: Chart.js + Vue-ChartJS
- **Drag & Drop**: Sortable.js
- **Styling**: CSS3 with custom styles
- **Icons**: Font Awesome

## Getting Started

### Prerequisites
- Node.js 16+ 
- npm or yarn

### Installation

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd dynamic-form-builder
   ```

2. **Install dependencies**
   ```bash
   npm install
   ```

3. **Run the development server**
   ```bash
   npm run dev
   ```

4. **Build for production**
   ```bash
   npm run build
   ```

### Email Service Setup

The email scheduling feature requires a separate Node.js service:

1. **Download the email service**
   - Go to the "Email Scheduler" tab in the application
   - Click "Download Email Service Code"
   - Extract the `email-service.js` file

2. **Set up the email service**
   ```bash
   # Create a new directory for the email service
   mkdir email-service
   cd email-service
   
   # Initialize npm and install dependencies
   npm init -y
   npm install express nodemailer node-cron cors dotenv
   
   # Copy the downloaded email-service.js file here
   ```

3. **Configure environment variables**
   Create a `.env` file:
   ```env
   EMAIL_USER=your-email@gmail.com
   EMAIL_PASS=your-app-password
   SMTP_HOST=smtp.gmail.com
   SMTP_PORT=587
   ```

4. **Run the email service**
   ```bash
   node email-service.js
   ```

## Usage Guide

### Building Forms

1. **Start Building**
   - Navigate to the Form Builder page
   - Enter a title for your form
   - Drag components from the palette to the canvas

2. **Customize Components**
   - Click on any component to select it
   - Use the Properties Panel to modify:
     - Labels and placeholders
     - Options for select/radio components
     - Chart types for chart components
     - Number of rows for textareas

3. **Generate HTML**
   - Click "Generate & Preview" to see your form
   - Switch between Live Preview and HTML Code tabs
   - Download complete HTML files or copy code to clipboard

### Email Scheduling

1. **Add Email Schedule**
   - Go to the Email Scheduler page
   - Enter recipient email address
   - Choose frequency (Daily/Weekly/Monthly/Yearly)
   - Set the time for email delivery
   - Click "Add Schedule"

2. **Manage Schedules**
   - View all active schedules
   - Enable/disable schedules as needed
   - Remove schedules when no longer needed

3. **Preview Emails**
   - See exactly how your form will appear in emails
   - Check the email subject and formatting

## Project Structure

```
dynamic-form-builder/
├── src/
│   ├── components/
│   │   └── form-components/     # Individual form component renderers
│   ├── stores/                  # Pinia state management
│   ├── views/                   # Main application views
│   │   ├── FormBuilder.vue      # Drag & drop form builder
│   │   ├── PreviewView.vue      # HTML preview and generation
│   │   └── SchedulerView.vue    # Email scheduling interface
│   ├── router/                  # Vue Router configuration
│   └── App.vue                  # Main application component
├── email-service/               # Separate email service (optional)
│   ├── package.json
│   └── email-service.js
└── README.md
```

## Visual Studio 2022 Integration

To open this project in Visual Studio 2022:

1. **Install Node.js Tools for Visual Studio**
   - Ensure you have the Node.js development workload installed

2. **Open the Project**
   - File → Open → Folder
   - Select the `dynamic-form-builder` directory

3. **Configure Debugging**
   - The project includes a `package.json` with npm scripts
   - Use the built-in terminal to run npm commands
   - Set breakpoints in TypeScript/JavaScript files for debugging

## Development

### Available Scripts

```bash
# Development server with hot reload
npm run dev

# Build for production
npm run build

# Preview production build
npm run preview

# Run type checking
npm run type-check

# Run linting
npm run lint

# Format code
npm run format

# Run unit tests
npm run test:unit

# Run end-to-end tests
npm run test:e2e
```

### Adding New Components

1. Create a new component file in `src/components/form-components/`
2. Add the component type to the `FormComponent` interface in `stores/formBuilder.ts`
3. Register the component in `views/FormBuilder.vue`
4. Add HTML generation logic in the store's `formHTML` computed property

## Email Service API

The email service exposes a REST API:

```bash
# Get all schedules
GET /schedules

# Add new schedule
POST /schedules
{
  "email": "user@example.com",
  "frequency": "daily",
  "time": "09:00",
  "enabled": true,
  "formId": "form-123"
}

# Delete schedule
DELETE /schedules/:id
```

## SMTP Configuration

### Gmail Setup
1. Enable 2-factor authentication
2. Generate an app password
3. Use these settings:
   - Host: `smtp.gmail.com`
   - Port: `587`
   - Secure: `false` (uses STARTTLS)

### Outlook/Hotmail Setup
- Host: `smtp-mail.outlook.com`
- Port: `587`
- Secure: `false`

### Custom SMTP
Configure your SMTP server settings in the `.env` file.

## Security Considerations

- **Environment Variables**: Never commit `.env` files with real credentials
- **Email Validation**: The app validates email addresses client-side
- **CORS**: The email service includes CORS middleware
- **Rate Limiting**: Consider adding rate limiting for production use

## Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Add tests if applicable
5. Submit a pull request

## License

This project is licensed under the MIT License - see the LICENSE file for details.

## Support

For issues and questions:
1. Check the GitHub Issues page
2. Create a new issue with detailed description
3. Include steps to reproduce any bugs

## Roadmap

- [ ] Database integration for form storage
- [ ] User authentication system
- [ ] Form submission handling
- [ ] Advanced chart customization
- [ ] Template library
- [ ] Export to other formats (PDF, Word)
- [ ] Form analytics and reporting
