using Domain_layer.Entities;
using Domain_layer.Enums;
using System;
using System.Collections.Generic;

namespace Infrastructure.Persistence
{
    public static class MockDatabase
    {
        public static List<User> Users { get; set; } = new List<User>();
        public static List<Worker> Workers { get; set; } = new List<Worker>();
        public static List<Customer> Customers { get; set; } = new List<Customer>();
        public static List<ServiceRequest> ServiceRequests { get; set; } = new List<ServiceRequest>();
        public static List<Review> Reviews { get; set; } = new List<Review>();
        public static List<Complaint> Complaints { get; set; } = new List<Complaint>();
        public static List<Favorite> Favorites { get; set; } = new List<Favorite>();
        public static List<Notification> Notifications { get; set; } = new List<Notification>();

        static MockDatabase()
        {
            SeedData();
        }

        private static void SeedData()
        {
            var customer1 = new Customer { Id = 101, FullName = "Mona Ahmed", Email = "mona@example.com", PasswordHash = "123456", Role = UserRole.Customer, Address = "Cairo Downtown" };
            var customer2 = new Customer { Id = 102, FullName = "Hady Emad", Email = "hady@example.com", PasswordHash = "123456", Role = UserRole.Customer, Address = "Alexandria Sporting" };
            Customers.Add(customer1);
            Customers.Add(customer2);
            Users.Add(customer1);
            Users.Add(customer2);

            Workers = new List<Worker>
            {
                new Worker { Id = 1, FullName = "Ahmed Ali", Email = "w1@ex.com", PasswordHash = "123", Role = UserRole.Worker, Bio = "Expert plumber", HourlyRate = 150, CategoryId = 1, Category = new Category { Id = 1, Name = "Plumbing" }, Availability = AvailabilityStatus.Available, IsVerified = true },
                new Worker { Id = 2, FullName = "Mohamed Salah", Email = "w2@ex.com", PasswordHash = "123", Role = UserRole.Worker, Bio = "Professional electrician", HourlyRate = 200, CategoryId = 2, Category = new Category { Id = 2, Name = "Electrical" }, Availability = AvailabilityStatus.Busy, IsVerified = true },
                new Worker { Id = 3, FullName = "Omar Hassan", Email = "w3@ex.com", PasswordHash = "123", Role = UserRole.Worker, Bio = "Experienced carpenter", HourlyRate = 120, CategoryId = 3, Category = new Category { Id = 3, Name = "Carpentry" }, Availability = AvailabilityStatus.Available, IsVerified = true },
                new Worker { Id = 4, FullName = "Khaled Youssef", Email = "w4@ex.com", PasswordHash = "123", Role = UserRole.Worker, Bio = "HVAC specialist", HourlyRate = 250, CategoryId = 4, Category = new Category { Id = 4, Name = "HVAC" }, Availability = AvailabilityStatus.Offline, IsVerified = true },
                new Worker { Id = 5, FullName = "Hassan Mostafa", Email = "w5@ex.com", PasswordHash = "123", Role = UserRole.Worker, Bio = "Painter with 10 years experience", HourlyRate = 100, CategoryId = 5, Category = new Category { Id = 5, Name = "Painting" }, Availability = AvailabilityStatus.Available, IsVerified = true },
                new Worker { Id = 6, FullName = "Mahmoud Ezzat", Email = "w6@ex.com", PasswordHash = "123", Role = UserRole.Worker, Bio = "Quick plumbing fixes", HourlyRate = 130, CategoryId = 1, Category = new Category { Id = 1, Name = "Plumbing" }, Availability = AvailabilityStatus.Available, IsVerified = true },
                new Worker { Id = 7, FullName = "Ibrahim Nabil", Email = "w7@ex.com", PasswordHash = "123", Role = UserRole.Worker, Bio = "Smart home installations", HourlyRate = 300, CategoryId = 2, Category = new Category { Id = 2, Name = "Electrical" }, Availability = AvailabilityStatus.Busy, IsVerified = true },
                new Worker { Id = 8, FullName = "Tariq Ziad", Email = "w8@ex.com", PasswordHash = "123", Role = UserRole.Worker, Bio = "Furniture assembly expert", HourlyRate = 110, CategoryId = 3, Category = new Category { Id = 3, Name = "Carpentry" }, Availability = AvailabilityStatus.Available, IsVerified = true },
                new Worker { Id = 9, FullName = "Amr Diab", Email = "w9@ex.com", PasswordHash = "123", Role = UserRole.Worker, Bio = "AC cleaning and repair", HourlyRate = 180, CategoryId = 4, Category = new Category { Id = 4, Name = "HVAC" }, Availability = AvailabilityStatus.Offline, IsVerified = true },
                new Worker { Id = 10, FullName = "Sayed Makkawy", Email = "w10@ex.com", PasswordHash = "123", Role = UserRole.Worker, Bio = "Decorative painting", HourlyRate = 160, CategoryId = 5, Category = new Category { Id = 5, Name = "Painting" }, Availability = AvailabilityStatus.Available, IsVerified = true },
                new Worker { Id = 11, FullName = "Adel Imam", Email = "w11@ex.com", PasswordHash = "123", Role = UserRole.Worker, Bio = "Pipe leak detection", HourlyRate = 140, CategoryId = 1, Category = new Category { Id = 1, Name = "Plumbing" }, Availability = AvailabilityStatus.Available, IsVerified = true },
                new Worker { Id = 12, FullName = "Yasser Galal", Email = "w12@ex.com", PasswordHash = "123", Role = UserRole.Worker, Bio = "Rewiring and electrical safety", HourlyRate = 220, CategoryId = 2, Category = new Category { Id = 2, Name = "Electrical" }, Availability = AvailabilityStatus.Available, IsVerified = true },
                new Worker { Id = 13, FullName = "Wael Gomaa", Email = "w13@ex.com", PasswordHash = "123", Role = UserRole.Worker, Bio = "Custom wooden doors", HourlyRate = 200, CategoryId = 3, Category = new Category { Id = 3, Name = "Carpentry" }, Availability = AvailabilityStatus.Busy, IsVerified = true },
                new Worker { Id = 14, FullName = "Nour El Sherif", Email = "w14@ex.com", PasswordHash = "123", Role = UserRole.Worker, Bio = "Split AC maintenance", HourlyRate = 210, CategoryId = 4, Category = new Category { Id = 4, Name = "HVAC" }, Availability = AvailabilityStatus.Available, IsVerified = true },
                new Worker { Id = 15, FullName = "Ayman Zidan", Email = "w15@ex.com", PasswordHash = "123", Role = UserRole.Worker, Bio = "Exterior house painting", HourlyRate = 120, CategoryId = 5, Category = new Category { Id = 5, Name = "Painting" }, Availability = AvailabilityStatus.Available, IsVerified = true },
                new Worker { Id = 16, FullName = "Tamer Hosny", Email = "w16@ex.com", PasswordHash = "123", Role = UserRole.Worker, Bio = "Emergency plumbing 24/7", HourlyRate = 350, CategoryId = 1, Category = new Category { Id = 1, Name = "Plumbing" }, Availability = AvailabilityStatus.Available, IsVerified = true },
                new Worker { Id = 17, FullName = "Maged El Kedwany", Email = "w17@ex.com", PasswordHash = "123", Role = UserRole.Worker, Bio = "Industrial electrical panels", HourlyRate = 400, CategoryId = 2, Category = new Category { Id = 2, Name = "Electrical" }, Availability = AvailabilityStatus.Offline, IsVerified = true }
            };
            Users.AddRange(Workers);

            Reviews.Add(new Review { Id = 1, Rating = 5, Comment = "Excellent work!", WorkerId = 1, CustomerId = 101, CreatedAt = DateTime.UtcNow.AddDays(-2) });
            Reviews.Add(new Review { Id = 2, Rating = 4, Comment = "Very professional.", WorkerId = 2, CustomerId = 102, CreatedAt = DateTime.UtcNow.AddDays(-1) });
            Workers[0].Reviews.Add(Reviews[0]);
            Workers[1].Reviews.Add(Reviews[1]);

            ServiceRequests.Add(new ServiceRequest { Id = 1, Description = "Fix bathroom pipes", Address = "Cairo Downtown", Status = RequestStatus.Completed, CustomerId = 101, WorkerId = 1, ScheduledDate = DateTime.UtcNow.AddDays(-3), CreatedAt = DateTime.UtcNow.AddDays(-5) });
            ServiceRequests.Add(new ServiceRequest { Id = 2, Description = "Install lighting fixtures", Address = "Alexandria Sporting", Status = RequestStatus.Pending, CustomerId = 102, WorkerId = 2, ScheduledDate = DateTime.UtcNow.AddDays(2), CreatedAt = DateTime.UtcNow });

            Complaints.Add(new Complaint { Id = 1, Title = "Worker arrived late", Content = "The worker was 2 hours late without notice.", CustomerId = 101, ServiceRequestId = 2, CreatedAt = DateTime.UtcNow });

            Favorites.Add(new Favorite { Id = 1, CustomerId = 101, WorkerId = 2, Worker = Workers.FirstOrDefault(w => w.Id == 2), AddedAt = DateTime.UtcNow });

            Notifications.Add(new Notification { Id = 1, Message = "Your request was created successfully.", UserId = 102, CreatedAt = DateTime.UtcNow });
        }
    }
}
