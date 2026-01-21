# Authentication Service Documentation

## Overview

The Authentication Service is a .NET 9 WebAPI service that provides comprehensive user authentication and authorization capabilities for the AI Business Management System. It implements JWT-based authentication, API key management, role-based access control (RBAC), and secure password handling.

## Architecture

### Clean Architecture Implementation

The service follows Clean Architecture principles with clear separation of concerns:

```
AuthenticationService/
├── Controllers/          # API endpoints
├── Services/            # Business logic
├── Models/              # Domain entities
├── DTOs/                # Data transfer objects
├── Middleware/          # Custom middleware
├── Validators/          # Input validation
└── Program.cs           # Application configuration
```

### Key Components

- **AuthService**: Core business logic for authentication operations
- **JwtService**: JWT token generation and validation
- **AuthController**: RESTful API endpoints
- **ApiKeyAuthentication**: Custom middleware for API key authentication
- **AuthDbContext**: Entity Framework Core data context
- **FluentValidation**: Input validation using FluentValidation

## Features

### 1. User Management

#### Registration
- Email and username uniqueness validation
- Password strength requirements
- Automatic role assignment (User role by default)
- Email verification support (placeholder)

#### Authentication
- JWT access tokens (60-minute expiration)
- Refresh tokens (7-day expiration)
- Password-based login with email or username
- Account status validation (active/inactive)

#### User Information
- Profile management (name, avatar, phone)
- Password change functionality
- User info retrieval

### 2. JWT Authentication

#### Token Generation
- Access tokens with user claims (ID, email, username, roles)
- Refresh tokens for token renewal
- Configurable expiration times
- Secure signing with secret key

#### Token Validation
- JWT signature verification
- Expiration checking
- Claim extraction and validation
- Principal creation for authorization

### 3. API Key Management

#### Key Creation
- Secure random key generation (Base64, 32 bytes)
- Prefix-based identification (`ak_`)
- Scoped permissions (read, write, admin)
- Expiration date support
- Maximum 10 keys per user

#### Key Operations
- List user's API keys
- Revoke individual keys
- Usage tracking (last used timestamp)
- Secure storage with BCrypt hashing

### 4. Role-Based Access Control (RBAC)

#### Roles
- **User**: Basic user permissions
- **Admin**: Administrative privileges
- **Moderator**: Content moderation (future)

#### Implementation
- Many-to-many relationship between users and roles
- Role-based authorization attributes
- Flexible role assignment system

### 5. Security Features

#### Password Security
- BCrypt hashing with salt
- Minimum password requirements
- Secure password change process

#### API Security
- API key authentication middleware
- JWT bearer token authentication
- CORS configuration
- Request validation

## API Endpoints

### Authentication Endpoints

#### POST `/api/auth/register`
Register a new user account.

**Request:**
```json
{
  "email": "user@example.com",
  "username": "username",
  "password": "SecurePassword123!",
  "fullName": "Full Name"
}
```

**Response:**
```json
{
  "success": true,
  "data": {
    "user": {
      "id": "guid",
      "email": "user@example.com",
      "username": "username",
      "fullName": "Full Name",
      "roles": ["User"]
    },
    "accessToken": "jwt-token",
    "refreshToken": "refresh-token"
  },
  "message": "User registered successfully"
}
```

#### POST `/api/auth/login`
Authenticate user and return tokens.

**Request:**
```json
{
  "usernameOrEmail": "user@example.com",
  "password": "SecurePassword123!"
}
```

**Response:**
```json
{
  "success": true,
  "data": {
    "user": { /* user info */ },
    "accessToken": "jwt-token",
    "refreshToken": "refresh-token"
  },
  "message": "Login successful"
}
```

#### POST `/api/auth/refresh`
Refresh access token using refresh token.

**Request:**
```json
{
  "refreshToken": "refresh-token"
}
```

### User Management Endpoints

#### GET `/api/auth/me`
Get current user information.

**Headers:** `Authorization: Bearer <access-token>`

#### PUT `/api/auth/me`
Update user information.

**Request:**
```json
{
  "fullName": "Updated Name",
  "avatarUrl": "https://example.com/avatar.jpg",
  "phoneNumber": "+1234567890"
}
```

#### POST `/api/auth/change-password`
Change user password.

**Request:**
```json
{
  "currentPassword": "OldPassword123!",
  "newPassword": "NewPassword123!"
}
```

### API Key Management Endpoints

#### POST `/api/auth/api-keys`
Create a new API key.

**Request:**
```json
{
  "keyName": "My API Key",
  "scopes": ["read", "write"],
  "expiresAt": "2024-12-31T23:59:59Z"
}
```

**Response:**
```json
{
  "success": true,
  "data": {
    "id": "guid",
    "keyName": "My API Key",
    "prefix": "ak_",
    "scopes": ["read", "write"],
    "expiresAt": "2024-12-31T23:59:59Z",
    "apiKey": "ak_actual-key-value"
  }
}
```

#### GET `/api/auth/api-keys`
List user's API keys.

#### DELETE `/api/auth/api-keys/{keyId}`
Revoke an API key.

### Health Check

#### GET `/health`
Service health status.

**Response:**
```json
{
  "status": "Healthy",
  "timestamp": "2024-01-01T00:00:00Z",
  "service": "Authentication Service"
}
```

## Configuration

### Environment Variables

```bash
# JWT Configuration
JWT_SECRET=YourSuperSecretKeyThatIsAtLeast32CharactersLong!
JWT_ISSUER=AuthenticationService
JWT_AUDIENCE=AuthenticationService
JWT_EXPIRATION_MINUTES=60

# Database Configuration
CONNECTION_STRING=Host=localhost;Database=auth_db;Username=postgres;Password=password

# API Key Configuration
API_KEY_PREFIX=ak_
```

### appsettings.json

```json
{
  "Jwt": {
    "Secret": "YourSuperSecretKeyThatIsAtLeast32CharactersLong!",
    "Issuer": "AuthenticationService",
    "Audience": "AuthenticationService",
    "ExpirationMinutes": 60
  },
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=auth_db;Username=postgres;Password=password"
  },
  "ApiKey": {
    "Prefix": "ak_"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```

## Database Schema

### Users Table
```sql
CREATE TABLE Users (
    Id UUID PRIMARY KEY,
    Email VARCHAR(255) UNIQUE NOT NULL,
    Username VARCHAR(50) UNIQUE NOT NULL,
    PasswordHash VARCHAR(255) NOT NULL,
    FullName VARCHAR(100),
    AvatarUrl VARCHAR(500),
    PhoneNumber VARCHAR(20),
    IsEmailVerified BOOLEAN DEFAULT FALSE,
    IsActive BOOLEAN DEFAULT TRUE,
    CreatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    ModifiedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    LastLoginAt TIMESTAMP
);
```

### Roles Table
```sql
CREATE TABLE Roles (
    Id UUID PRIMARY KEY,
    Name VARCHAR(50) UNIQUE NOT NULL,
    Description TEXT,
    CreatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);
```

### UserRoles Table
```sql
CREATE TABLE UserRoles (
    UserId UUID REFERENCES Users(Id) ON DELETE CASCADE,
    Role VARCHAR(50) NOT NULL,
    PRIMARY KEY (UserId, Role)
);
```

### RefreshTokens Table
```sql
CREATE TABLE RefreshTokens (
    Id UUID PRIMARY KEY,
    UserId UUID REFERENCES Users(Id) ON DELETE CASCADE,
    Token VARCHAR(500) NOT NULL,
    ExpiresAt TIMESTAMP NOT NULL,
    IsRevoked BOOLEAN DEFAULT FALSE,
    CreatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    RevokedAt TIMESTAMP
);
```

### ApiKeys Table
```sql
CREATE TABLE ApiKeys (
    Id UUID PRIMARY KEY,
    UserId UUID REFERENCES Users(Id) ON DELETE CASCADE,
    KeyName VARCHAR(100) NOT NULL,
    KeyHash VARCHAR(255) NOT NULL,
    Prefix VARCHAR(10) NOT NULL,
    Scopes TEXT[],
    ExpiresAt TIMESTAMP,
    LastUsedAt TIMESTAMP,
    IsActive BOOLEAN DEFAULT TRUE,
    IsRevoked BOOLEAN DEFAULT FALSE,
    CreatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    RevokedAt TIMESTAMP
);
```

## Security Considerations

### Password Security
- Passwords are hashed using BCrypt with automatic salt generation
- Minimum password requirements enforced
- Password change requires current password verification

### Token Security
- JWT tokens are signed with a secret key
- Access tokens have short expiration (60 minutes)
- Refresh tokens have longer expiration (7 days)
- Tokens are validated on every request

### API Key Security
- API keys are generated using cryptographically secure random number generation
- Keys are hashed before storage using BCrypt
- Keys are only returned in plain text during creation
- Keys can be revoked immediately

### Input Validation
- All inputs are validated using FluentValidation
- SQL injection prevention through Entity Framework Core
- XSS protection through proper encoding
- CORS configuration for cross-origin requests

## Error Handling

### Standard Error Response Format
```json
{
  "success": false,
  "message": "Error description",
  "errors": [
    {
      "field": "fieldName",
      "message": "Field-specific error message"
    }
  ]
}
```

### Common Error Scenarios
- **400 Bad Request**: Invalid input data, validation failures
- **401 Unauthorized**: Invalid credentials, expired tokens
- **403 Forbidden**: Insufficient permissions
- **404 Not Found**: User or resource not found
- **409 Conflict**: Email/username already exists
- **500 Internal Server Error**: Server-side errors

## Testing

### Test Coverage
The service includes comprehensive test coverage:

- **Unit Tests**: Service layer logic testing
- **Integration Tests**: API endpoint testing
- **JWT Service Tests**: Token generation and validation
- **Database Tests**: Entity Framework operations

### Test Categories
1. **AuthServiceRegistrationTests**: User registration scenarios
2. **AuthServiceLoginTests**: Authentication scenarios
3. **AuthServiceApiKeyTests**: API key management
4. **AuthServiceUserManagementTests**: User profile management
5. **JwtServiceTests**: JWT token operations
6. **AuthControllerIntegrationTests**: End-to-end API testing

### Running Tests
```bash
# Run all tests
dotnet test

# Run specific test category
dotnet test --filter "Category=Unit"

# Run with coverage
dotnet test --collect:"XPlat Code Coverage"
```

## Deployment

### Docker Support
The service can be containerized using Docker:

```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["AuthenticationService.csproj", "."]
RUN dotnet restore "AuthenticationService.csproj"
COPY . .
RUN dotnet build "AuthenticationService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AuthenticationService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AuthenticationService.dll"]
```

### Environment Setup
1. Ensure PostgreSQL database is running
2. Set environment variables or update appsettings.json
3. Run database migrations (automatic on startup)
4. Start the service

### Health Monitoring
- Health check endpoint at `/health`
- Structured logging with Serilog
- Performance counters and metrics
- Error tracking and alerting

## Integration with Other Services

### Service Discovery
The authentication service can be discovered by other services through:
- Service registry (Consul, Eureka)
- Load balancer configuration
- API Gateway routing

### Inter-Service Communication
- JWT tokens for service-to-service authentication
- API keys for programmatic access
- Shared user context propagation

### Data Synchronization
- User data synchronization with other services
- Role changes propagation
- Account status updates

## Future Enhancements

### Planned Features
1. **Email Verification**: Complete email verification workflow
2. **Password Reset**: Forgot password functionality
3. **Two-Factor Authentication**: TOTP-based 2FA
4. **Social Login**: OAuth integration (Google, Microsoft)
5. **Audit Logging**: Comprehensive audit trail
6. **Rate Limiting**: API rate limiting per user/key
7. **Session Management**: Active session tracking
8. **Account Lockout**: Brute force protection

### Scalability Considerations
- Database sharding for large user bases
- Redis caching for session data
- Microservice decomposition
- Horizontal scaling support

## Troubleshooting

### Common Issues

#### Database Connection Issues
- Verify PostgreSQL is running
- Check connection string configuration
- Ensure database exists and user has permissions

#### JWT Token Issues
- Verify JWT secret is configured
- Check token expiration settings
- Ensure proper token format in requests

#### API Key Issues
- Verify API key format (starts with `ak_`)
- Check key expiration
- Ensure key is not revoked

### Logging
Enable detailed logging by setting log level to `Debug`:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "AuthenticationService": "Debug"
    }
  }
}
```

### Performance Monitoring
Monitor key metrics:
- Authentication request latency
- Database query performance
- Memory usage
- Token generation/validation times

## Support and Maintenance

### Regular Maintenance Tasks
1. **Database Cleanup**: Remove expired refresh tokens and API keys
2. **Log Rotation**: Manage log file sizes
3. **Security Updates**: Keep dependencies updated
4. **Performance Monitoring**: Monitor service health

### Backup and Recovery
- Regular database backups
- Configuration backup
- Disaster recovery procedures
- Service restoration steps

---

*This documentation is maintained as part of the AI Business Management System project. For updates and contributions, please refer to the project repository.*
