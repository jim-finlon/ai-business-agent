# 🎯 Cursor Rule Sets for AI Agent Business Management System

This directory contains Cursor rule sets designed to enforce modern .NET WebAPI and microservices development practices across the AI Agent Business Management System.

## 📋 Available Rule Sets

### 1. **Clean Architecture Enforcement** (`clean-architecture.mdc`)
- **Scope**: All C# files (`**/*.cs`)
- **Purpose**: Enforces Clean Architecture boundaries and separation of concerns
- **Key Rules**: 
  - Controllers depend only on interfaces
  - Domain models don't reference infrastructure
  - Use MediatR for orchestration
  - Keep controllers thin

### 2. **Service Layer Conventions** (`service-layer.mdc`)
- **Scope**: Service classes (`**/Services/*.cs`)
- **Purpose**: Standardizes service layer patterns and practices
- **Key Rules**:
  - Stateless services with DI
  - Async/await for I/O operations
  - Return DTOs, not EF entities
  - Use FluentValidation for input validation

### 3. **Testing Practices** (`testing-practices.mdc`)
- **Scope**: Test files (`**/Tests/**/*.cs`)
- **Purpose**: Ensures consistent testing patterns and quality
- **Key Rules**:
  - Use xUnit or NUnit
  - Mock dependencies with Moq
  - Follow Arrange-Act-Assert structure
  - Test behavior, not implementation

### 4. **Error Handling & Logging** (`error-handling-logging.mdc`)
- **Scope**: All C# files (`**/*.cs`)
- **Purpose**: Standardizes exception handling and logging practices
- **Key Rules**:
  - Global exception middleware
  - Structured logging with Serilog
  - Use ProblemDetails for API errors
  - Include correlation IDs for tracing

### 5. **NuGet & SDK Hygiene** (`nuget-sdk-hygiene.mdc`)
- **Scope**: Project files (`**/*.csproj`)
- **Purpose**: Maintains clean package management and SDK usage
- **Key Rules**:
  - Target latest LTS SDK (net9.0)
  - Use PackageReference format
  - Lock versions for critical packages
  - Centralized package management

### 6. **Entity Framework Core Rules** (`entity-framework.mdc`)
- **Scope**: Data layer files (`**/Data/**/*.cs`, `**/DbContext.cs`, `**/Models/*.cs`)
- **Purpose**: Ensures proper EF Core usage patterns
- **Key Rules**:
  - DbContext per request via DI
  - Explicit Include() over lazy loading
  - Use migrations for schema changes
  - Separate entity configurations

### 7. **OpenAPI & Swagger** (`openapi-swagger.mdc`)
- **Scope**: Startup and controller files
- **Purpose**: Maintains consistent API documentation
- **Key Rules**:
  - Use Swashbuckle.AspNetCore
  - Annotate with ProducesResponseType
  - Group endpoints by tags
  - Document authentication requirements

## 🚀 Usage

These rule sets are automatically applied by Cursor when working with files matching the specified glob patterns. They provide:

- **Code Quality**: Enforce best practices and patterns
- **Consistency**: Maintain uniform code style across the project
- **Documentation**: Guide developers on proper implementation
- **Performance**: Optimize for scalability and maintainability

## 🔧 Customization

Each rule set can be customized by editing the corresponding `.mdc` file. The rules use:

- **Description**: Brief explanation of the rule set's purpose
- **Globs**: File patterns to which rules apply
- **AlwaysApply**: Whether rules are always enforced (true/false)

## 📚 Additional Resources

- [Cursor Documentation](https://cursor.sh/docs)
- [.NET Clean Architecture Guide](https://docs.microsoft.com/en-us/dotnet/architecture/modern-web-apps-azure/common-web-application-architectures)
- [ASP.NET Core Best Practices](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/best-practices)

## 🎯 Project Alignment

These rules are specifically tailored for the AI Agent Business Management System's microservices architecture, ensuring:

- **Scalability**: Services can be developed and deployed independently
- **Maintainability**: Consistent patterns across all services
- **Security**: Proper authentication and authorization patterns
- **Performance**: Optimized database and API practices
- **Testing**: Comprehensive test coverage and quality
