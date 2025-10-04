using System;
using System.Linq;
using MedicalDemo.Data.Models;

namespace MedicalDemo.Services
{
    public static class DatabaseSeeder
    {
        public static void Seed(MedicalContext context)
        {
            // Make sure DB is created
            context.Database.EnsureCreated();

            // ✅ Seed Admin
            if (!context.admins.Any())
            {
                var admin = new Admins
                {
                    admin_id = Guid.NewGuid().ToString("N"),  // required since PK is string
                    email = "admin@demo.com",
                    password = BCrypt.Net.BCrypt.HashPassword("Password123!"),
                    first_name = "System",
                    last_name = "Admin",
                    phone_num = "0000000000"
                };

                context.admins.Add(admin);
            }

            // ✅ Seed Resident
            if (!context.residents.Any())
            {
                var resident = new Residents
                {
                    resident_id = Guid.NewGuid().ToString("N"),  // required since PK is string
                    email = "resident1@demo.com",
                    password = BCrypt.Net.BCrypt.HashPassword("Password123!"),
                    first_name = "John",
                    last_name = "Doe",
                    graduate_yr = 2026,
                    phone_num = "1111111111",
                    weekly_hours = 0,
                    total_hours = 0,
                    bi_yearly_hours = 0
                };

                context.residents.Add(resident);
            }

            // Save all changes
            context.SaveChanges();
        }
    }
}
