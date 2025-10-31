<p align="center" width="100%">
    <img width="150px" src="https://github.com/user-attachments/assets/1653f8d5-23ed-4ca3-b54f-531bcc0dda01"> 
</p>

# Personal Reminder Tool
<b>A lightweight reminder service for scheduling multi-channel notifications using Azure Communication Services.</b>

## Features
- User login with JWT + Refresh Token authentication
- Date + time scheduling with custom messages
- SMS + Email support

## Architecture & Tech Stack
- **Frontend:** Vue.js + TypeScript SPA (hosted on GitHub Pages)
- **Backend:** .NET 9 ASP.NET Core Minimal API handling auth, scheduling, and message dispatch
- **Scheduler:** [TickerQ](https://tickerq.net/) background task scheduler for timed reminders
- **Database:** Azure Database for PostgreSQL + EF Core
- **Messaging:** Azure Communication Service for SMS/Email delivery
- **Infrastructure:** Containerized with Docker, deployed to Azure Container Apps  
- **CI/CD:** Automated build, test, and deploy with GitHub Actions
- **Testing:** XUnit + NSubstitute for unit testing

## Future Improvements
**Messaging Channels**
- Add WhatsApp and Voice reminders

**Authentication**
- Add user registration
- Switch to OAuth provider

**User Experience**
- Add active reminders dashboard
- Enable update/delete actions

**Maintenance**
- Integrate client storage (Pinia)
- Upgrade to .NET 10
