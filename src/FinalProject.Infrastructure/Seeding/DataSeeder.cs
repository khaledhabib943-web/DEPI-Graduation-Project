using FinalProject.Domain.Entities;
using FinalProject.Domain.Enums;
using FinalProject.Infrastructure.DbContext;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FinalProject.Infrastructure.Seeding
{
    public class DataSeeder
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public DataSeeder(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task SeedAsync()
        {
            await SeedCategoriesAsync();
            await SeedCustomerAsync();
            await SeedAdminAsync();
            await SeedWorkersAsync();
        }

        // ===== Categories =====
        private async Task SeedCategoriesAsync()
        {
            var categoryData = new[]
            {
                ("Plumbing", "All plumbing services including pipe repairs, faucet installation, and drainage.", "plumbing"),
                ("Electrical", "Electrical wiring, outlet installation, circuit breaker repairs, and lighting.", "electrical_services"),
                ("AC & Cooling", "Air conditioning installation, maintenance, cleaning, and repair services.", "ac_unit"),
                ("Carpentry", "Furniture repair, door installation, custom shelving, and woodwork.", "carpenter"),
                ("Painting", "Interior and exterior painting, wall finishing, and decorative coating.", "format_paint"),
                ("Cleaning", "Deep cleaning, regular maintenance cleaning, and specialized cleaning.", "cleaning_services")
            };

            foreach (var (name, desc, icon) in categoryData)
            {
                if (!await _context.Categories.AnyAsync(c => c.Name == name))
                {
                    await _context.Categories.AddAsync(new Category
                    {
                        Name = name, Description = desc, IconUrl = icon,
                        IsActive = true, CreatedAt = DateTime.UtcNow
                    });
                }
            }
            await _context.SaveChangesAsync();
        }

        // ===== Test Customer =====
        private async Task SeedCustomerAsync()
        {
            if (!await _context.Customers.AnyAsync(c => c.UserName == "mohannad"))
            {
                var customer = new Customer
                {
                    FullName = "Mohannad Waleed",
                    Email = "mohannad@test.com",
                    PhoneNumber = "01234567890",
                    NationalId = "29901011234567",
                    Age = 24,
                    UserName = "mohannad",
                    Role = UserRole.Customer,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    Address = "Maadi, Street 9, Building 15, Cairo",
                    EmailConfirmed = true
                };

                var result = await _userManager.CreateAsync(customer, "P@ssword123");
                if (!result.Succeeded)
                {
                    throw new Exception($"Failed to create seed customer: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                }
            }
        }

        // ===== Admins =====
        private async Task SeedAdminAsync()
        {
            // Admin 1
            if (!await _context.Admins.AnyAsync(a => a.UserName == "admin"))
            {
                var admin = new Admin
                {
                    FullName = "Admin User",
                    Email = "admin@salahly.com",
                    PhoneNumber = "01111111111",
                    NationalId = "29801021234567",
                    Age = 35,
                    UserName = "admin",
                    Role = UserRole.Admin,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    EmailConfirmed = true
                };

                var result = await _userManager.CreateAsync(admin, "Admin@123");
                if (!result.Succeeded)
                {
                    throw new Exception($"Failed to create seed admin: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                }
            }

            // Admin 2
            if (!await _context.Admins.AnyAsync(a => a.UserName == "superadmin"))
            {
                var admin2 = new Admin
                {
                    FullName = "Super Admin",
                    Email = "superadmin@salahly.com",
                    PhoneNumber = "01222222222",
                    NationalId = "29801031234567",
                    Age = 40,
                    UserName = "superadmin",
                    Role = UserRole.Admin,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    EmailConfirmed = true
                };

                var result2 = await _userManager.CreateAsync(admin2, "Admin@123");
                if (!result2.Succeeded)
                {
                    throw new Exception($"Failed to create seed superadmin: {string.Join(", ", result2.Errors.Select(e => e.Description))}");
                }
            }
        }

        // ===== Workers (32 total) =====
        private async Task SeedWorkersAsync()
        {
            var workerData = new[]
            {
                // Plumbing (8 workers)
                ("Ahmed Mahmoud", "ahmed.m@salahly.com", "ahmedm", "Plumbing", 150m, 4.9f, "Certified plumber with 10+ years experience in all plumbing services."),
                ("Omar Khaled", "omar.k@salahly.com", "omark", "Plumbing", 120m, 4.7f, "Specialist in water heater installation and bathroom renovations."),
                ("Samy Tawfik", "samy.t@salahly.com", "samyt", "Plumbing", 135m, 4.5f, "Expert in drainage systems and emergency pipe repairs."),
                ("Waleed Farid", "waleed.f@salahly.com", "waleedf", "Plumbing", 110m, 4.2f, "Kitchen plumbing specialist with modern fixture expertise."),
                ("Nabil Saeed", "nabil.s@salahly.com", "nabils", "Plumbing", 145m, 4.8f, "Industrial plumbing and large-scale water system installations."),
                ("Fady Ayman", "fady.a@salahly.com", "fadya", "Plumbing", 100m, 3.9f, "Affordable plumbing services for basic home repairs."),
                ("Hatem Gamal", "hatem.g@salahly.com", "hatemg", "Plumbing", 150m, 4.4f, "Reliable plumber for home repairs."),
                ("Hesham Gamal", "hesham.g@salahly.com", "heshamg", "Plumbing", 130m, 4.8f, "Expert in plumbing."),
                // Electrical (5 workers)
                ("Youssef Samir", "youssef.s@salahly.com", "youssefs", "Electrical", 140m, 4.6f, "Certified electrician for residential and commercial projects."),
                ("Hesham Nour", "hesham.n@salahly.com", "heshamn", "Electrical", 155m, 4.9f, "Smart home automation and advanced electrical systems specialist."),
                ("Bassem Gamal", "bassem.g@salahly.com", "bassemg", "Electrical", 125m, 4.3f, "Reliable electrician for outlet installation and basic wiring."),
                ("Tamer Adel", "tamer.a@salahly.com", "tamera", "Electrical", 160m, 5.0f, "Master electrician with solar panel installation experience."),
                // AC & Cooling (5 workers)
                ("Khaled Youssef", "khaled.y@salahly.com", "khaledy", "AC & Cooling", 180m, 5.0f, "AC installation and maintenance specialist with factory training."),
                ("Hassan Ibrahim", "hassan.i@salahly.com", "hassani", "AC & Cooling", 160m, 4.5f, "Cooling systems expert for split and central AC units."),
                ("Ramy Sherif", "ramy.s@salahly.com", "ramys", "AC & Cooling", 170m, 4.7f, "AC cleaning, gas refill, and compressor repair specialist."),
                ("Wael Mostafa", "wael.m@salahly.com", "waelm", "AC & Cooling", 145m, 4.1f, "Budget-friendly AC maintenance and filter replacement."),
                ("Osama Hazem", "osama.h@salahly.com", "osamah", "AC & Cooling", 190m, 4.8f, "Premium AC installation with energy efficiency consulting."),
                // Carpentry (5 workers)
                ("Tarek Nabil", "tarek.n@salahly.com", "tarekn", "Carpentry", 170m, 4.8f, "Master carpenter for furniture repair and custom woodwork."),
                ("Amr Sayed", "amr.s@salahly.com", "amrs", "Carpentry", 150m, 4.4f, "Experienced in door installation and kitchen cabinets."),
                ("Magdy Fouad", "magdy.f@salahly.com", "magdyf", "Carpentry", 165m, 4.6f, "Custom shelving, wardrobes, and built-in furniture specialist."),
                ("Ashraf Salem", "ashraf.s@salahly.com", "ashrafs", "Carpentry", 140m, 4.0f, "General carpentry, wood polishing, and repairs."),
                ("Sherif Hamdy", "sherif.h@salahly.com", "sherifh", "Carpentry", 185m, 4.9f, "Luxury furniture restoration and antique woodwork."),
                // Painting (5 workers)
                ("Mostafa Amin", "mostafa.a@salahly.com", "mostafaa", "Painting", 110m, 4.7f, "Professional painter for interior and exterior projects."),
                ("Karim Adel", "karim.a@salahly.com", "karima", "Painting", 100m, 4.3f, "Decorative painting and wall texture specialist."),
                ("Ayman Ragab", "ayman.r@salahly.com", "aymanr", "Painting", 95m, 4.1f, "Fast and clean painting for apartments and villas."),
                ("Hany Lotfy", "hany.l@salahly.com", "hanyl", "Painting", 120m, 4.5f, "Epoxy flooring and waterproof coating specialist."),
                ("Ehab Nasr", "ehab.n@salahly.com", "ehabn", "Painting", 130m, 4.8f, "Premium decorative finishes and 3D wall art."),
                // Cleaning (6 workers)
                ("Mahmoud Fathy", "mahmoud.f@salahly.com", "mahmoudf", "Cleaning", 90m, 4.9f, "Deep cleaning and sanitization services for homes and offices."),
                ("Ibrahim Reda", "ibrahim.r@salahly.com", "ibrahimr", "Cleaning", 80m, 4.6f, "Regular maintenance cleaning with eco-friendly products."),
                ("Saad Mansour", "saad.m@salahly.com", "saadm", "Cleaning", 75m, 4.2f, "Affordable daily cleaning for apartments and studios."),
                ("Medhat Ali", "medhat.a@salahly.com", "medhata", "Cleaning", 100m, 4.7f, "Post-construction and renovation cleaning specialist."),
                ("Essam Waheed", "essam.w@salahly.com", "essamw", "Cleaning", 85m, 4.4f, "Carpet and upholstery deep cleaning expert."),
                ("Adel Barakat", "adel.b@salahly.com", "adelb", "Cleaning", 110m, 4.8f, "Comprehensive home cleaning with steam sanitization.")
            };

            foreach (var (name, email, username, categoryName, price, rating, bio) in workerData)
            {
                // Skip if worker already exists (check by username — unique)
                if (await _context.Workers.AnyAsync(w => w.UserName == username))
                    continue;

                // Find the category by name
                var category = await _context.Categories.FirstOrDefaultAsync(c => c.Name == categoryName);
                if (category == null)
                    continue;

                // Generate unique Phone/NationalId from username hash (stable, won't collide)
                var usernameHash = Math.Abs(username.GetHashCode()) % 100000000;

                var worker = new Worker
                {
                    FullName = name,
                    Email = email,
                    PhoneNumber = $"010{usernameHash:D8}",
                    NationalId = $"298010{usernameHash:D8}",
                    Age = 25 + (usernameHash % 15),
                    UserName = username,
                    Role = UserRole.Worker,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    CategoryId = category.CategoryId,
                    ProfilePicture = $"https://ui-avatars.com/api/?name={Uri.EscapeDataString(name)}&background=0f2cbd&color=fff&size=128",
                    IdFrontImage = "placeholder",
                    IdBackImage = "placeholder",
                    Portfolio = bio,
                    ServicePrice = price,
                    AvailabilityStatus = usernameHash % 4 == 0 ? AvailabilityStatus.Busy : AvailabilityStatus.Available,
                    AverageRating = rating,
                    IsValidated = true,
                    EmailConfirmed = true // Seed users have confirmed email
                };

                var result = await _userManager.CreateAsync(worker, "Worker@123");
                if (!result.Succeeded)
                {
                    throw new Exception($"Failed to create seed worker {username}: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                }
            }

            await _context.SaveChangesAsync();
        }
    }
}
