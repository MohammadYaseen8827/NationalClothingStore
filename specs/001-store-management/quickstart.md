# Quick Start Guide: National Clothing Store Management System

**Date**: 2025-02-08  
**Purpose**: Quick setup and development guide for the National Clothing Store Management System

## Prerequisites

### Development Environment
- **.NET 9 SDK** - Latest version with ASP.NET Core Web API
- **Node.js 18+** - For frontend development
- **Docker Desktop** - For containerized development
- **PostgreSQL 15+** - Primary database
- **Redis 7+** - Caching and session storage
- **Git** - Version control

### Development Tools (Recommended)
- **Visual Studio 2022** or **VS Code** - IDE
- **Postman** or **Insomnia** - API testing
- **pgAdmin** or **DBeaver** - Database management
- **Redis Desktop Manager** - Redis management

## Quick Setup

### 1. Clone Repository
```bash
git clone <repository-url>
cd national-clothing-store
```

### 2. Environment Configuration
Create `.env` files in appropriate directories:

**Backend (.env)**:
```env
ASPNETCORE_ENVIRONMENT=Development
ConnectionStrings__DefaultConnection=Host=localhost;Database=NationalClothingStore;Username=postgres;Password=password
Redis__ConnectionString=localhost:6379
JwtSettings__Secret=your-super-secret-jwt-key-here
JwtSettings__Issuer=NationalClothingStore
JwtSettings__Audience=NationalClothingStore
JwtSettings__ExpirationMinutes=60
JwtSettings__RefreshTokenExpirationDays=7
Logging__LogLevel__Default=Information
Logging__LogLevel__Microsoft.AspNetCore=Warning
```

**Frontend (.env)**:
```env
VITE_API_BASE_URL=http://localhost:5000/api/v1
VITE_APP_TITLE=National Clothing Store Management
VITE_APP_VERSION=1.0.0
```

### 3. Database Setup
```bash
# Start PostgreSQL and Redis using Docker
docker-compose -f infrastructure/docker-compose.dev.yml up -d postgres redis

# Run database migrations
cd backend
dotnet ef database update

# Seed initial data (optional)
dotnet run --project src/Infrastructure/Data.Seeding
```

### 4. Start Development Services

#### Using Docker Compose (Recommended)
```bash
# Start all services
docker-compose -f infrastructure/docker-compose.dev.yml up -d

# View logs
docker-compose -f infrastructure/docker-compose.dev.yml logs -f
```

#### Manual Startup
```bash
# Terminal 1: Backend
cd backend
dotnet run

# Terminal 2: Frontend
cd frontend
npm install
npm run dev

# Terminal 3: Nginx (optional)
nginx -c infrastructure/nginx/nginx.conf
```

## Access Points

### Application URLs
- **Frontend**: http://localhost:3000
- **Backend API**: http://localhost:5000
- **API Documentation**: http://localhost:5000/swagger
- **Health Checks**: http://localhost:5000/health

### Database Access
- **PostgreSQL**: localhost:5432
- **Redis**: localhost:6379

## Default Credentials

### Initial Admin User
- **Email**: admin@nationalclothingstore.com
- **Password**: Admin123!
- **Role**: Super Admin

### Test Data
The system includes sample data for:
- 5 branches across different cities
- 50+ products with variations
- Initial inventory levels
- Test users with different roles

## Development Workflow

### 1. Making Changes
```bash
# Create feature branch
git checkout -b feature/your-feature-name

# Make changes
# Commit changes
git add .
git commit -m "feat: add your feature description"

# Push changes
git push origin feature/your-feature-name
```

### 2. Running Tests
```bash
# Backend tests
cd backend
dotnet test

# Frontend tests
cd frontend
npm run test
npm run test:e2e
```

### 3. Code Quality
```bash
# Backend linting and formatting
cd backend
dotnet format
dotnet restore
dotnet build

# Frontend linting and formatting
cd frontend
npm run lint
npm run format
npm run type-check
```

## API Usage Examples

### Authentication
```bash
# Login
POST /api/v1/auth/login
{
  "email": "admin@nationalclothingstore.com",
  "password": "Admin123!"
}

# Response
{
  "accessToken": "eyJhbGciOiJIUzI1NiIs...",
  "refreshToken": "dGhpcy1pcy1yZWZyZXNoLXRva2Vu...",
  "expiresIn": 3600,
  "user": {
    "id": "guid",
    "email": "admin@nationalclothingstore.com",
    "firstName": "Admin",
    "lastName": "User",
    "roles": ["SuperAdmin"]
  }
}
```

### Product Management
```bash
# Get all products
GET /api/v1/products?page=1&pageSize=20

# Create product
POST /api/v1/products
Authorization: Bearer <access-token>
{
  "name": "Classic T-Shirt",
  "description": "Comfortable cotton t-shirt",
  "sku": "TSHIRT-001",
  "basePrice": 29.99,
  "costPrice": 15.50,
  "categoryId": "category-guid"
}

# Add product variation
POST /api/v1/products/{product-id}/variations
Authorization: Bearer <access-token>
{
  "size": "L",
  "color": "Blue",
  "sku": "TSHIRT-001-L-BLUE",
  "price": 29.99,
  "costPrice": 15.50
}
```

### Inventory Management
```bash
# Get inventory for branch
GET /api/v1/inventory?branchId=branch-guid

# Adjust inventory
POST /api/v1/inventory/adjust
Authorization: Bearer <access-token>
{
  "productId": "product-guid",
  "branchId": "branch-guid",
  "quantity": 10,
  "transactionType": "StockIn",
  "reason": "New stock received"
}
```

## Common Development Tasks

### Adding New API Endpoint
1. Define entity in `Domain/Entities/`
2. Create repository interface in `Domain/Interfaces/`
3. Implement repository in `Infrastructure/Data/Repositories/`
4. Create application service in `Application/Services/`
5. Add controller in `API/Controllers/`
6. Write unit tests
7. Update API documentation

### Adding New Frontend Component
1. Create component in `src/components/`
2. Add TypeScript types in `src/types/`
3. Create Pinia store if needed in `src/stores/`
4. Add route in `src/router/`
5. Write component tests
6. Update documentation

### Database Changes
1. Modify entity in `Domain/Entities/`
2. Add migration: `dotnet ef migrations add MigrationName`
3. Apply migration: `dotnet ef database update`
4. Update seeding data if needed

## Troubleshooting

### Common Issues

#### Database Connection Errors
```bash
# Check PostgreSQL status
docker ps | grep postgres

# Check connection string
echo $ConnectionStrings__DefaultConnection

# Restart PostgreSQL
docker-compose restart postgres
```

#### Redis Connection Errors
```bash
# Check Redis status
docker ps | grep redis

# Test Redis connection
redis-cli ping

# Restart Redis
docker-compose restart redis
```

#### Frontend Build Errors
```bash
# Clear node modules
rm -rf node_modules package-lock.json
npm install

# Clear Vite cache
npm run dev -- --force
```

#### Backend Build Errors
```bash
# Clean and rebuild
dotnet clean
dotnet restore
dotnet build

# Check for missing packages
dotnet list package
```

### Performance Issues

#### Slow API Responses
- Check database indexes
- Enable query logging
- Monitor Redis cache hit rate
- Profile with Application Insights

#### Slow Frontend
- Check bundle size with `npm run build`
- Enable lazy loading
- Optimize images
- Check network requests in browser dev tools

### Debugging

#### Backend Debugging
```bash
# Enable detailed logging
export Logging__LogLevel__Default=Debug

# Run with hot reload
dotnet watch run
```

#### Frontend Debugging
```bash
# Run with Vue DevTools
npm run dev

# Debug tests
npm run test:debug
```

## Deployment

### Development Deployment
```bash
# Build for development
docker-compose -f infrastructure/docker-compose.dev.yml build
docker-compose -f infrastructure/docker-compose.dev.yml up -d
```

### Production Deployment
```bash
# Build for production
docker-compose -f infrastructure/docker-compose.prod.yml build
docker-compose -f infrastructure/docker-compose.prod.yml up -d
```

## Support

### Documentation
- **API Documentation**: Available at `/swagger` endpoint
- **Architecture Guide**: `docs/architecture.md`
- **Database Schema**: `docs/database-schema.md`
- **Deployment Guide**: `docs/deployment/`

### Getting Help
- **Development Team**: dev-team@nationalclothingstore.com
- **Technical Support**: support@nationalclothingstore.com
- **Project Wiki**: Internal Confluence space

### Contributing
1. Follow the coding standards defined in the constitution
2. Write tests for all new features
3. Update documentation
4. Submit pull requests with clear descriptions
5. Ensure all CI/CD checks pass

## Next Steps

After completing the quick start:

1. **Explore the System**: Log in and explore all features
2. **Review the Code**: Understand the architecture and patterns
3. **Run Tests**: Ensure all tests pass in your environment
4. **Set Up Development Tools**: Configure your preferred IDE and tools
5. **Join Team Communication**: Set up Slack/Teams notifications
6. **Review Development Guidelines**: Read constitution and coding standards

## Resources

### Technology Documentation
- [.NET Core Documentation](https://docs.microsoft.com/en-us/aspnet/core/)
- [Vue.js Documentation](https://vuejs.org/guide/)
- [Entity Framework Core](https://docs.microsoft.com/en-us/ef/core/)
- [PostgreSQL Documentation](https://www.postgresql.org/docs/)
- [Redis Documentation](https://redis.io/documentation)

### Tools and Utilities
- [Docker Documentation](https://docs.docker.com/)
- [Git Documentation](https://git-scm.com/doc)
- [Postman Documentation](https://learning.postman.com/)
- [VS Code Documentation](https://code.visualstudio.com/docs)

---

**Happy Coding!** 🚀

For any issues or questions, please reach out to the development team or create an issue in the project repository.
