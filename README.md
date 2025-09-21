# Content App POC - Comments Management System

A Proof of Concept (POC) application built with **Umbraco CMS** and **.NET 8** that demonstrates a comprehensive comments management system with clean architecture principles.

## 🏗️ Architecture Overview

This project follows a **Clean Architecture** pattern with clear separation of concerns:

```
Content App POC/
├── Content App POC/           # Main Umbraco web application
├── CommentsMgt.API/           # API layer - Controllers and endpoints
├── CommentsMgt.Application/   # Application layer - Business logic and services
├── CommentsMgt.Domain/        # Domain layer - Entities and business rules
├── CommentsMgt.DTOs/          # Data Transfer Objects - Shared contracts
└── CommentsMgt.Infra/         # Infrastructure layer - Data access and external services
```

## 🚀 Technology Stack

- **Framework**: .NET 8.0
- **CMS**: Umbraco 13.9.1
- **Database**: SQL Server
- **Architecture**: Clean Architecture with DDD principles
- **Frontend**: HTML, CSS, JavaScript
- **Sync**: uSync 13.2.7 for content synchronization

## 📋 Features

- **Comments Management**: Full CRUD operations for comments
- **User Role Management**: Admin and Viewer user groups
- **Content Integration**: Seamless integration with Umbraco content
- **Portal Display**: Configurable display options for different contexts
- **Status Management**: Comment approval workflow
- **Responsive UI**: Modern, clean interface

## 🛠️ Setup Instructions

### Prerequisites

- .NET 8.0 SDK
- SQL Server (Local or Remote)
- Visual Studio 2022 or VS Code

### Installation Steps

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd "ContentApp POC - ChangedSchema"
   ```

2. **Database Configuration**
   
   Add the following connection string to your `appsettings.json`:
   ```json
   {
     "ConnectionStrings": {
       "CommentsMgt": "Server=10.19.4.4;Database=CommentsMgt;User Id=UmbracoTemplateUser;Password=Dev@123456;Trusted_Connection=True;TrustServerCertificate=True;"
       // Alternative for local development:
       // "CommentsMgt": "Server=(localdb)\\MSSQLLocalDB;Database=CommentsMgt;TrustServerCertificate=True;"
     }
   }
   ```

3. **Comments Management Configuration**
   
   Add this configuration section to `appsettings.json`:
   ```json
   {
     "CommentsManagement": {
       "InitialCommentStatusId": 3,
       "InitialVisbilityStatus": 1,
       "UserGroups": {
         "AdminGroupName": "CommentsAdmin",
         "ViewerGroupName": "CommentsViewer"
       },
       "CommentsMgtToggles": {
         "CMS": "cMSDisplay",
         "Portal": "portalDisplay"
       }
     }
   }
   ```

4. **Register Services in Program.cs**
   
   Add the following service registrations:
   ```csharp
   // Register CommentsMgtContext
   builder.Services.AddDbContext<Content_App_POC.CommentsMgt.CommentsMgtContext>(options =>
       options.UseSqlServer(builder.Configuration.GetConnectionString("CommentsMgt")));
   
   // Register CommentsMgt repository and service
   builder.Services.AddScoped<Content_App_POC.CommentsMgt.ICommentRepository, Content_App_POC.CommentsMgt.CommentRepository>();
   builder.Services.AddScoped<Content_App_POC.CommentsMgt.ICommentService, Content_App_POC.CommentsMgt.CommentService>();
   ```

5. **Build and Run**
   ```bash
   dotnet restore
   dotnet build
   dotnet run --project "Content App POC"
   ```

## 🔧 Integration Guide

### 1. DLL References

Add references to the following assemblies:
- `CommentsMgt.API.dll`
- `CommentsMgt.Application.dll`
- `CommentsMgt.Domain.dll`
- `CommentsMgt.DTOs.dll`
- `CommentsMgt.Infra.dll`

### 2. Umbraco Configuration

1. **Create Composition**: Set up Comments Management Composition with toggles:
   - `CMSDisplay` - For CMS display control
   - `PortalDisplay` - For portal display control

2. **User Groups**: Create the following user groups:
   - `CommentsAdmin` - Full management permissions
   - `CommentsViewer` - Read-only permissions

### 3. Frontend Integration

Create a partial view (`~/Views/Partials/CommentsSection.cshtml`):

```html
<link rel="stylesheet" href="~/clean-assets/css/comments-section.css"/>
<script>
    window.currentContentId = @(Model?.Id ?? 0);
    window.currentUserName = '@(User?.Identity?.Name ?? "")';
    @{
        var parentAlias = Model?.Parent != null ? Model.Parent.ContentType.Alias : "";
    }
    window.currentParentNodeAlias = '@parentAlias';
</script>
<div id="comments-section-container" class="comments-section-social"></div>
<script src="/clean-assets/js/comments-section.js"></script>
```

Use in your views:
```html
@if (Model.Value("portalDisplay") is bool showPortalDisplay && showPortalDisplay)
{
    @await Html.PartialAsync("~/Views/Partials/CommentsSection.cshtml")
}
```

### 4. Required Files

Ensure these files are in place:
- `App_Plugins/CommentsMgt/` - CMS plugin files
- `App_Plugins/PortalDisplayDependency/` - Portal dependency files
- `CommentsMgtApp.cs` - Application configuration
- `comments-section.js` - Frontend JavaScript
- `PortalDisplayDependency.cs` - Portal integration

## 📁 Project Structure

```
├── Content App POC/
│   ├── App_Plugins/
│   │   ├── CommentsMgt/          # CMS plugin for comments management
│   │   └── PortalDisplayDependency/  # Portal display dependencies
│   ├── wwwroot/
│   │   └── clean-assets/
│   │       ├── css/              # Stylesheets
│   │       └── js/               # JavaScript files
│   └── Views/
│       └── Partials/             # Partial views
├── CommentsMgt.API/              # Web API controllers
├── CommentsMgt.Application/      # Business logic and services
├── CommentsMgt.Domain/           # Domain entities and business rules
│   ├── DBEntities/               # Database entities
│   └── Enums/                    # Domain enumerations
├── CommentsMgt.DTOs/             # Data transfer objects
└── CommentsMgt.Infra/            # Data access and infrastructure
```

## 🔍 Key Components

- **Comment Entity**: Core domain model for comments
- **Comment Service**: Business logic for comment operations
- **Comment Repository**: Data access layer
- **Comments API**: RESTful endpoints for comment management
- **Umbraco Integration**: Seamless CMS integration with content types

## 🚦 Status Codes

- **Status ID 2**: Approved
- **Status ID 3**: Pending (Default for new comments)
- **Visibility Status 1**: Visible (when approved)

## 🤝 Contributing

This is a POC project. For production use, consider:
- Adding comprehensive unit tests
- Implementing proper error handling
- Adding logging and monitoring
- Security hardening
- Performance optimization

## 📝 License

This project is a Proof of Concept for internal evaluation purposes.

---

**Note**: This POC demonstrates the integration of a custom comments management system with Umbraco CMS using clean architecture principles. The schema has been modified from the original version to support enhanced functionality.
