# Vehicle Auction Platform - ASP.NET Core

A simplified, clean web application built with ASP.NET Core 8 and Razor Pages that consumes an auction API.

## Features

### 🔐 Authentication System
- User registration with Admin/Bidder role selection
- Role-based access control throughout the application

### 🚗 Vehicle Management
- Browse and search vehicles by type, manufacturer, model, and year
- Detailed vehicle information display
- Admin-only vehicle creation with comprehensive form validation
- Responsive vehicle cards with clear information layout

### 🏆 Auction Platform
- View active auctions
- Admin-only auction creation and management
- Bidder-only bid placement with validation
- Complete auction details with bid history

## Configuration

### API Configuration

Update the API base URL in `appsettings.json`:

```json
{
  "ApiSettings": {
    "BaseUrl": "https://your-api-domain.com/api"
  }
}
```

### API Endpoints

API provides these endpoints:

- `POST /api/users` - User registration
- `POST /api/login` - User authentication
- `GET /api/vehicles` - Get all vehicles
- `GET /api/vehicles/{id}` - Get vehicle by ID
- `POST /api/vehicles/search` - Search vehicles
- `POST /api/vehicles` - Create vehicle (Admin only)
- `GET /api/auctions` - Get all auctions
- `GET /api/auctions/{id}` - Get auction by ID
- `POST /api/auctions` - Start auction (Admin only)
- `POST /api/auctions/{id}/stop` - Stop auction (Admin only)
- `POST /api/auctions/{id}/bid` - Place bid (Bidder only)

## Running the Web Application

### Prerequisites
- .NET 8.0 SDK
- Auction API running and accessible

### Local Development

1. **Clone/Download the project**
2. **Configure API URL** in `appsettings.json`
3. **Build the application:**
   ```bash
   dotnet build
   ```
4. **Run the application:**
   ```bash
   dotnet run --urls="http://localhost:5000"
   ```
5. **Open browser** and navigate to `http://localhost:5000`

### Production Deployment

1. **Publish the application:**
   ```bash
   dotnet publish -c Release -o ./publish
   ```
2. **Deploy the published files** to your web server
3. **Configure your web server** to serve the application
4. **Ensure your API** allows CORS requests from your domain

## Usage Guide

### For Administrators
1. **Register** with Admin role
2. **Create vehicles** using the vehicle creation form
3. **Start auctions** for available vehicles
4. **Monitor auctions** and stop them when needed

### For Bidders
1. **Register** with Bidder role
2. **Browse vehicles** and search by criteria
3. **View active auctions** and auction details
4. **Place bids** on active auctions

## API Integration Notes

- The application expects JWT tokens for authentication
- All API responses should be in JSON format
- CORS must be enabled on your API for the web domain
