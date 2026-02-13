# Restaurant Review Platform API

A RESTful API for a restaurant review platform built with .NET and following layered architecture principles.

## Architecture

The project follows a clean layered architecture with clear separation of concerns:

```
├── Domain Layer (Domain/)
│   ├── Entities/          # Domain entities (User, Restaurant, Review, ViewedRestaurant)
│   ├── Enums/            # Domain enumerations (UserType)
│   └── Interfaces/       # Repository interfaces
│
├── Application Layer (Application/)
│   ├── DTOs/             # Data Transfer Objects
│   ├── Interfaces/       # Service interfaces
│   └── Services/         # Business logic implementation
│
├── Infrastructure Layer (Infrastructure/)
│   ├── Data/             # Database context
│   ├── Repositories/     # Repository implementations
│   └── Services/         # External service implementations (Geocoding)
│
└── Presentation Layer (API/)
    ├── Controllers/       # API Controllers
    └── Middleware/        # Custom middleware (Error handling)
```

## Features

### Authentication & Authorization
- JWT-based authentication
- Two user roles: **Reviewer** and **Owner**
- Secure password hashing

### Restaurant Management (Owner Only)
- Create, update, and delete restaurants
- Owners can only manage their own restaurants
- Support for coordinates or address (with automatic geocoding)
- Each restaurant has:
  - Title
  - Preview image
  - Coordinates (latitude/longitude)
  - Description
  - Average rating (calculated)
  - Review count

### Restaurant Listing (Public)
- Paginated listing
- Filter by partial title match
- Filter by coordinates + radius (e.g., "restaurants within 5 miles")
- View restaurant details

### Reviews (Reviewer Only)
- Create reviews with text and rating (1-5)
- Paginated review listing
- Filter reviews by restaurant
- Automatic average rating calculation
- One review per restaurant per user

### Recently Viewed Restaurants (Reviewer Only)
- Automatic tracking of viewed restaurants
- Get last 10 viewed restaurants

## Technology Stack

- **.NET 10.0** - Framework
- **Entity Framework Core** - ORM
- **SQLite** - Database
- **JWT** - Authentication
- **Swagger/OpenAPI** - API Documentation
- **Nominatim (OpenStreetMap)** - Geocoding service

## Getting Started

### Prerequisites
- .NET 10.0 SDK
- Visual Studio 2022 or VS Code

### Installation

1. Restore NuGet packages:
```bash
dotnet restore
```

2. The database will be created automatically on first run using SQLite.

3. Run the application:
```bash
dotnet run
```

The API will be available at:
- HTTPS: `https://localhost:7xxx`
- HTTP: `http://localhost:5xxx`

### Swagger Documentation
Once the application is running, navigate to:
```
https://localhost:7xxx/swagger
```

## API Endpoints

### Authentication
- `POST /api/auth/signup` - Sign up a new user
- `POST /api/auth/login` - Login
- `GET /api/auth/me` - Get current user info (authenticated)

### Restaurants
- `GET /api/restaurants` - Get all restaurants (public, paginated, filterable)
- `GET /api/restaurants/{id}` - Get restaurant by ID (public)
- `POST /api/restaurants` - Create restaurant (Owner only)
- `PUT /api/restaurants/{id}` - Update restaurant (Owner only, own restaurants)
- `DELETE /api/restaurants/{id}` - Delete restaurant (Owner only, own restaurants)
- `GET /api/restaurants/recently-viewed` - Get recently viewed restaurants (Reviewer only)

### Reviews
- `GET /api/reviews` - Get all reviews (public, paginated, filterable by restaurant)
- `POST /api/reviews` - Create review (Reviewer only)

## Query Parameters

### Restaurant Listing
```
GET /api/restaurants?page=1&pageSize=10&titleFilter=pizza&latitude=52.52&longitude=13.405&radiusInMiles=5
```
- `page` (default: 1) - Page number
- `pageSize` (default: 10, max: 100) - Items per page
- `titleFilter` - Partial title match
- `latitude`, `longitude`, `radiusInMiles` - Location-based filtering

### Review Listing
```
GET /api/reviews?page=1&pageSize=10&restaurantId={guid}
```
- `page` (default: 1) - Page number
- `pageSize` (default: 10, max: 100) - Items per page
- `restaurantId` - Filter by restaurant

## Example Usage

### 1. Sign Up as a Reviewer
```json
POST /api/auth/signup
{
  "email": "reviewer@example.com",
  "password": "Password123",
  "fullName": "John Reviewer",
  "userType": 1
}
```

### 2. Sign Up as an Owner
```json
POST /api/auth/signup
{
  "email": "owner@example.com",
  "password": "Password123",
  "fullName": "Jane Owner",
  "userType": 2
}
```

### 3. Login
```json
POST /api/auth/login
{
  "email": "owner@example.com",
  "password": "Password123"
}
```
Response includes a JWT token. Add it to subsequent requests:
```
Authorization: Bearer {token}
```

### 4. Create a Restaurant (Owner)
```json
POST /api/restaurants
Authorization: Bearer {owner_token}

{
  "title": "Amazing Pizza Place",
  "description": "Best pizza in town with authentic Italian recipes",
  "address": "Alexanderplatz, Berlin, Germany",
  "previewImage": "https://example.com/image.jpg"
}
```

Or with direct coordinates:
```json
{
  "title": "Amazing Pizza Place",
  "description": "Best pizza in town",
  "latitude": 52.520008,
  "longitude": 13.404954,
  "previewImage": "https://example.com/image.jpg"
}
```

### 5. Create a Review (Reviewer)
```json
POST /api/reviews
Authorization: Bearer {reviewer_token}

{
  "restaurantId": "{restaurant_guid}",
  "reviewText": "Absolutely loved this place! The pizza was delicious and the service was excellent.",
  "rating": 5
}
```

### 6. Get Restaurants Near Location
```
GET /api/restaurants?latitude=52.520008&longitude=13.404954&radiusInMiles=5
```

## Security Features

- **Password Hashing**: SHA256 hashing for password storage
- **JWT Authentication**: Secure token-based authentication
- **Role-Based Authorization**: Controllers protected by user roles
- **Ownership Validation**: Users can only modify their own resources
- **Input Validation**: Data annotations for request validation
- **Error Handling**: Global exception handling middleware

## Database Schema

The application uses Entity Framework Core with the following entities:

- **User**: Authentication and profile information
- **Restaurant**: Restaurant details with coordinates
- **Review**: User reviews with ratings
- **ViewedRestaurant**: Tracking of recently viewed restaurants

All relationships are properly configured with foreign keys and cascade behaviors.

## Configuration

Update `appsettings.json` to configure:
- Connection string (SQLite by default)
- JWT settings (Key, Issuer, Audience)
- Logging levels

**Important**: Change the JWT Key in production to a secure random value!

## Development Notes

### Geocoding Service
The application uses Nominatim (OpenStreetMap) for free geocoding. For production:
- Consider using paid services (Google Maps API, Azure Maps, etc.)
- Implement rate limiting
- Add caching for frequently queried addresses

### Distance Calculation
The Haversine formula is used for calculating distances between coordinates. This works well for most use cases but may need adjustment for high-precision requirements.

### Password Security
Currently using SHA256 for password hashing. For production, consider:
- BCrypt or Argon2 for better security
- Password complexity requirements
- Account lockout policies

## Testing

To test the API:
1. Use Swagger UI at `/swagger`
2. Use Postman or similar tools
3. Create automated tests (recommended for production)

## Future Enhancements

- Response caching for frequently accessed data
- Redis for session management
- Photo upload support for restaurant images
- Email verification for new accounts
- Password reset functionality
- Review reply functionality for owners
- Advanced filtering and sorting options
- Real-time notifications
- Rate limiting
- API versioning

## License

This project is created for educational/interview purposes.

## Contact

For questions or issues, please open an issue in the repository.
