# Architecture & Design Decisions

## Overview
This document explains the architectural decisions and design patterns used in the Restaurant Review Platform API.

## Architectural Pattern: Layered Architecture

The application follows a clean **Layered Architecture** (also known as N-Tier Architecture) with clear separation of concerns:

### 1. Domain Layer
**Location**: `/Domain`

**Purpose**: Contains the core business entities and business logic interfaces.

**Components**:
- **Entities**: Core business objects (User, Restaurant, Review, ViewedRestaurant)
- **Enums**: Domain-specific enumerations (UserType)
- **Interfaces**: Repository contracts (IRepository<T>, IUnitOfWork)

**Principles**:
- No dependencies on other layers
- Pure business logic and data models
- Framework-agnostic
- Represents the "heart" of the application

### 2. Application Layer
**Location**: `/Application`

**Purpose**: Contains application-specific business logic and orchestration.

**Components**:
- **DTOs**: Data Transfer Objects for API communication
- **Interfaces**: Service contracts
- **Services**: Business logic implementation

**Responsibilities**:
- Orchestrates domain objects
- Implements use cases
- Handles data transformation (Entity ↔ DTO)
- Validates business rules

**Dependencies**: Only depends on Domain layer

### 3. Infrastructure Layer
**Location**: `/Infrastructure`

**Purpose**: Implements technical concerns and external dependencies.

**Components**:
- **Data**: Database context and configurations
- **Repositories**: Data access implementations
- **Services**: External service implementations (e.g., Geocoding)

**Responsibilities**:
- Database access
- External API integration
- File system operations
- Third-party service integration

**Dependencies**: Depends on Domain and Application layers

### 4. Presentation Layer (API)
**Location**: `/Controllers`, `/API`

**Purpose**: HTTP API endpoints and middleware.

**Components**:
- **Controllers**: REST API endpoints
- **Middleware**: Cross-cutting concerns (error handling, logging)

**Responsibilities**:
- HTTP request/response handling
- Authentication/Authorization
- Input validation
- Response formatting

**Dependencies**: Depends on Application layer interfaces

## Design Patterns

### 1. Repository Pattern
**Why**: Abstracts data access logic and provides a collection-like interface for domain entities.

**Implementation**:
```csharp
IRepository<T> → Generic repository interface
IUserRepository : IRepository<User> → Specific repository
Repository<T> → Base implementation
UserRepository : Repository<User> → Specific implementation
```

**Benefits**:
- Testability (easy to mock)
- Centralized data access logic
- Flexibility to change data source

### 2. Unit of Work Pattern
**Why**: Maintains a list of objects affected by a business transaction and coordinates the writing out of changes.

**Implementation**:
```csharp
IUnitOfWork → Aggregates all repositories
UnitOfWork → Manages transaction boundaries
```

**Benefits**:
- Transaction management
- Single point of commit
- Coordinated repository operations

### 3. Dependency Injection
**Why**: Promotes loose coupling and testability.

**Implementation**:
- Constructor injection throughout all layers
- Service registration in Program.cs
- Interface-based dependencies

**Benefits**:
- Testable components
- Easy to swap implementations
- Clear dependencies

### 4. DTO Pattern
**Why**: Separates internal domain models from external API contracts.

**Implementation**:
- Request DTOs (CreateRestaurantRequest, SignUpRequest)
- Response DTOs (RestaurantDto, AuthResponse)
- Separate from domain entities

**Benefits**:
- API versioning flexibility
- Prevents over-posting attacks
- Cleaner API contracts

### 5. Middleware Pattern
**Why**: Handles cross-cutting concerns in the request pipeline.

**Implementation**:
```csharp
ErrorHandlingMiddleware → Global exception handling
```

**Benefits**:
- Centralized error handling
- Consistent error responses
- Separation of concerns

## Security Implementation

### Authentication
- **JWT Tokens**: Stateless authentication
- **Password Hashing**: SHA256 (consider BCrypt/Argon2 for production)
- **Token Expiration**: 7 days

### Authorization
- **Role-Based**: Reviewer vs Owner
- **Resource-Based**: Owners can only modify their own restaurants
- **Attribute-Based**: [Authorize(Roles = "Owner")]

### Input Validation
- **Data Annotations**: Required, Range, MinLength, etc.
- **Model State Validation**: Automatic by ASP.NET Core
- **Business Validation**: In service layer

### Protection Against
- **SQL Injection**: Using EF Core parameterized queries
- **Over-posting**: Using DTOs instead of entities
- **Unauthorized Access**: JWT + Role-based authorization
- **Information Disclosure**: Generic error messages in production

## Database Design

### Entity Relationships
```
User (1) -------- (*) Restaurant
User (1) -------- (*) Review
User (1) -------- (*) ViewedRestaurant
Restaurant (1) -- (*) Review
Restaurant (1) -- (*) ViewedRestaurant
```

### Key Design Decisions
1. **Soft Delete**: Not implemented (can be added if needed)
2. **Audit Fields**: CreatedAt, UpdatedAt on all entities
3. **GUIDs**: Used for all IDs (better for distributed systems)
4. **Cascading**: Reviews cascade on restaurant delete
5. **Indexes**: On frequently queried fields (email, coordinates, restaurantId)

## API Design Principles

### RESTful Conventions
- **Resources**: Nouns in URLs (/restaurants, /reviews)
- **HTTP Verbs**: GET, POST, PUT, DELETE
- **Status Codes**: 200 OK, 201 Created, 400 Bad Request, 401 Unauthorized, 403 Forbidden, 404 Not Found

### Response Structure
```json
{
  "items": [...],      // For paginated lists
  "page": 1,
  "pageSize": 10,
  "totalCount": 100,
  "totalPages": 10
}
```

Error responses:
```json
{
  "message": "Error description"
}
```

### Pagination
- Default page size: 10
- Max page size: 100
- Query parameters: page, pageSize

### Filtering
- Query parameters for filters
- Partial matching for strings (LIKE)
- Geo-spatial filtering with Haversine formula

## Performance Considerations

### Database Queries
- **Eager Loading**: Use Include() for related entities
- **Pagination**: Always paginate large datasets
- **Indexes**: On frequently filtered fields
- **Projection**: Select only needed columns

### Caching Opportunities
- Restaurant listings (Redis)
- Average ratings (cache with TTL)
- Geocoding results (cache addresses)

### Optimization Ideas
1. **Query Optimization**: Use AsNoTracking() for read-only queries
2. **Connection Pooling**: Enabled by default in EF Core
3. **Response Compression**: Add middleware
4. **API Rate Limiting**: Prevent abuse

## Error Handling Strategy

### Layers
1. **Controller**: Catches and returns appropriate HTTP status
2. **Middleware**: Global exception handler for unhandled exceptions
3. **Service**: Throws domain exceptions
4. **Repository**: Throws data exceptions

### Exception Types
- `KeyNotFoundException` → 404 Not Found
- `UnauthorizedAccessException` → 403 Forbidden
- `InvalidOperationException` → 400 Bad Request
- `ArgumentException` → 400 Bad Request
- Other exceptions → 500 Internal Server Error

## Testing Strategy

### Unit Tests (Recommended)
- **Service Layer**: Mock repositories
- **Repository Layer**: In-memory database
- **Controllers**: Mock services

### Integration Tests (Recommended)
- **API Endpoints**: WebApplicationFactory
- **Database**: Test database

### Test Coverage Goals
- Services: 80%+
- Repositories: 70%+
- Controllers: 60%+

## Scalability Considerations

### Horizontal Scaling
- **Stateless API**: JWT tokens enable multiple instances
- **Database**: Connection pooling
- **File Storage**: Use cloud storage (Azure Blob, S3)

### Vertical Scaling
- **Database Optimization**: Indexes, query optimization
- **Caching**: Redis for frequently accessed data
- **CDN**: For static content

### Future Enhancements
1. **CQRS**: Separate read/write models for complex scenarios
2. **Event Sourcing**: For audit trail
3. **Microservices**: Split by bounded contexts
4. **API Gateway**: For routing and rate limiting
5. **Message Queue**: For async operations (email, notifications)

## Technology Choices

### Why .NET?
- Strong typing and compile-time checks
- Excellent performance
- Cross-platform
- Rich ecosystem

### Why Entity Framework Core?
- LINQ for type-safe queries
- Migration support
- Change tracking
- Multiple database support

### Why SQLite?
- Zero configuration
- File-based (easy deployment)
- Good for development/demo
- Can migrate to SQL Server/PostgreSQL

### Why JWT?
- Stateless authentication
- Works well with SPAs
- Scalable
- Standard-based

### Why Swagger?
- Interactive API documentation
- Testing interface
- Code generation support

## Configuration Management

### appsettings.json
- Connection strings
- JWT configuration
- Logging levels

### Environment-Specific
- `appsettings.Development.json` for dev settings
- Environment variables for production secrets
- Azure Key Vault for sensitive data (production)

## Logging & Monitoring

### Built-in Logging
- ASP.NET Core logging framework
- Log levels: Trace, Debug, Info, Warning, Error, Critical

### Recommended Additions
- **Serilog**: Structured logging
- **Application Insights**: Performance monitoring
- **ELK Stack**: Log aggregation and analysis

## Security Checklist

- [x] JWT Authentication
- [x] Role-Based Authorization
- [x] Password Hashing
- [x] HTTPS (configured)
- [x] CORS Configuration
- [x] Input Validation
- [x] SQL Injection Protection (EF Core)
- [x] XSS Protection (automatic in ASP.NET Core)
- [ ] Rate Limiting (recommended)
- [ ] API Versioning (recommended)
- [ ] CSRF Protection (if using cookies)

## Deployment Recommendations

### Development
- SQLite database
- Local file storage
- Debug logging

### Production
- **Database**: SQL Server, PostgreSQL, or Azure SQL
- **Hosting**: Azure App Service, AWS, or Kubernetes
- **Storage**: Azure Blob Storage or AWS S3
- **Monitoring**: Application Insights or similar
- **Secrets**: Azure Key Vault or AWS Secrets Manager
- **SSL**: Let's Encrypt or cloud provider certificates

## Maintenance & Updates

### Regular Tasks
- Update NuGet packages
- Review security vulnerabilities
- Monitor performance metrics
- Backup database
- Review logs

### Breaking Changes
- API versioning prevents breaking existing clients
- Database migrations handle schema changes
- Deprecation notices for removed features

## Conclusion

This architecture provides:
- **Maintainability**: Clear separation of concerns
- **Testability**: Dependency injection and interfaces
- **Scalability**: Stateless design and caching opportunities
- **Security**: Multiple layers of protection
- **Flexibility**: Easy to add new features or change implementations

The design follows SOLID principles and industry best practices, making it suitable for both educational purposes and production use with appropriate hardening.
