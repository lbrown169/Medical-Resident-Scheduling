using System;
using System.Linq;
using MedicalDemo.Models;


namespace MedicalDemo.Services
{
    public static class DatabaseSeeder
    {
        public static void Seed(MedicalContext context)
        {
            // Make sure DB is created
            context.Database.EnsureCreated();
        }
    }
}