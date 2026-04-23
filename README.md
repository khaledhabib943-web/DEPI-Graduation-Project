# Salahly Backend - Phase 2 API Reference

The backend logic for the Customer Experience is fully implemented using Clean Architecture.

## Important Note on Storage
For Phase 2, we are using a **Mock Database** in memory (`Infrastructure.Persistence.MockDatabase`). This means:
1. All changes (Orders, Reviews, Complaints) are persisted across page loads as long as the server is running.
2. If you restart the server, the data resets to the initial seed state.
3. No SQL database or Entity Framework context is required to run the backend.

## Controllers & Endpoints

### Landing Page (Home)
- **GET /Home/Index**: Returns `HomeDataDto` containing `Categories`, `FeaturedWorkers`, and `RecentReviews`. Use this data to power your main dashboard/landing page.

### Account Cycle
- **GET /Account/Login**: Returns the login view.
- **POST /Account/Login**: Authenticates using `LoginDto` (Email, Password). Success sets `TempData["UserId"]` and redirects to `Home/Index`.
- **GET /Account/Register**: Returns registration view.
- **POST /Account/Register**: Registers using `RegisterDto` and logs in the user.

### Customer Discovery Cycle
- **GET /Customer/Index**: Accepts `WorkerSearchDto` (query string parameters like `MinPrice`, `MaxPrice`, `PageNumber`, etc.). Returns `PagedResult<WorkerDto>`.

### Fulfillment Cycle (Orders)
- **GET /ServiceRequest/Index**: Lists all orders for the mocked logged-in customer (default Customer ID: 101).
- **POST /ServiceRequest/Create**: Creates a new order using `CreateServiceRequestDto`. Redirects to Index upon success.
- **POST /ServiceRequest/UpdateStatus**: Updates the status of an order.

### Feedback & Engagement
- **GET /Review/WorkerReviews**: Lists reviews for a specific worker.
- **POST /Review/Create**: Posts a new review.
- **GET /Complaint/Index**: Lists customer complaints.
- **POST /Complaint/Create**: Creates a new complaint.
- **GET /Favorite/Index**: Lists customer's favorite workers.
- **POST /Favorite/Add**: Adds a worker to favorites.
- **POST /Favorite/Remove**: Removes a worker from favorites.
- **GET /Notification/Index**: Lists customer notifications.
- **POST /Notification/MarkAsRead**: Marks a notification as read.

## AutoMapper
The `Favorite` entity is configured to use `AutoMapper` to map to `FavoriteDto` inside `Application/Mappings/MappingProfile.cs`.
