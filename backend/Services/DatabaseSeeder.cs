using MedicalDemo.Enums;
using MedicalDemo.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace MedicalDemo.Services;

public class DatabaseSeeder
{
    private readonly ILogger<DatabaseSeeder> _logger;

    public DatabaseSeeder(ILogger<DatabaseSeeder> logger)
    {
        _logger = logger;
    }

    public async Task Seed(MedicalContext context)
    {
        // Make sure DB is created
        await context.Database.EnsureCreatedAsync();

        if (!await context.Admins.AnyAsync())
        {
            _logger.LogInformation("Seeding Admins");
            context.Admins.AddRange(
                new Admin
                {
                    AdminId = "0001",
                    FirstName = "Tom",
                    LastName = "Hanks",
                    Email = "tom.hanks@example.com",
                    Password
                        = "$2b$12$KqebvwNbxyuEg1OIOSjYZuMdXEDtEcbnsYwB9MHmSQgVsr.kHPWa.",
                    PhoneNum = "201-555-0101"
                },
                new Admin
                {
                    AdminId = "0002",
                    FirstName = "Meryl",
                    LastName = "Streep",
                    Email = "meryl.streep@example.com",
                    Password
                        = "$2b$12$KqebvwNbxyuEg1OIOSjYZuMdXEDtEcbnsYwB9MHmSQgVsr.kHPWa.",
                    PhoneNum = "201-555-0102"
                },
                new Admin
                {
                    AdminId = "0003",
                    FirstName = "Denzel",
                    LastName = "Washington",
                    Email = "denzel.washington@example.com",
                    Password
                        = "$2b$12$KqebvwNbxyuEg1OIOSjYZuMdXEDtEcbnsYwB9MHmSQgVsr.kHPWa.",
                    PhoneNum = "201-555-0103"
                },
                new Admin
                {
                    AdminId = "0004",
                    FirstName = "Scarlett",
                    LastName = "Johansson",
                    Email = "scarlett.johansson@example.com",
                    Password = "$2b$12$KqebvwNbxyuEg1OIOSjYZuMdXEDtEcbnsYwB9MHmSQgVsr.kHPWa.",
                    PhoneNum = "201-555-0104"
                },
                new Admin
                {
                    AdminId = "0005",
                    FirstName = "Faculty",
                    LastName = "User",
                    Email = "faculty@hcahealthcare.com",
                    Password = "$2b$12$KqebvwNbxyuEg1OIOSjYZuMdXEDtEcbnsYwB9MHmSQgVsr.kHPWa.",
                    PhoneNum = "201-555-0105",
                    Role = AdminRole.Faculty
                }
            );
        }

        List<Resident> residents =
        [
            new()
            {
                ResidentId = "AAB1001",
                FirstName = "Cillian",
                LastName = "Murphy",
                GraduateYr = 0,
                Email = "cillian.murphy@example.com",
                Password = "$2b$12$KqebvwNbxyuEg1OIOSjYZuMdXEDtEcbnsYwB9MHmSQgVsr.kHPWa.",
                PhoneNum = "201-555-0201",
                WeeklyHours = 0,
                TotalHours = 0,
                BiYearlyHours = 0
            },
            new()
            {
                ResidentId = "AAB1002",
                FirstName = "Natalie",
                LastName = "Portman",
                GraduateYr = 0,
                Email = "natalie.portman@example.com",
                Password = "$2b$12$KqebvwNbxyuEg1OIOSjYZuMdXEDtEcbnsYwB9MHmSQgVsr.kHPWa.",
                PhoneNum = "201-555-0202",
                WeeklyHours = 0,
                TotalHours = 0,
                BiYearlyHours = 0
            },
            new()
            {
                ResidentId = "AAB1003",
                FirstName = "Russell",
                LastName = "Crowe",
                GraduateYr = 0,
                Email = "russell.crowe@example.com",
                Password = "$2b$12$KqebvwNbxyuEg1OIOSjYZuMdXEDtEcbnsYwB9MHmSQgVsr.kHPWa.",
                PhoneNum = "201-555-0203",
                WeeklyHours = 0,
                TotalHours = 0,
                BiYearlyHours = 0
            },
            new()
            {
                ResidentId = "AAB1004",
                FirstName = "Charlize",
                LastName = "Theron",
                GraduateYr = 0,
                Email = "charlize.theron@example.com",
                Password = "$2b$12$KqebvwNbxyuEg1OIOSjYZuMdXEDtEcbnsYwB9MHmSQgVsr.kHPWa.",
                PhoneNum = "201-555-0204",
                WeeklyHours = 0,
                TotalHours = 0,
                BiYearlyHours = 0
            },
            new()
            {
                ResidentId = "AAB1005",
                FirstName = "Javier",
                LastName = "Bardem",
                GraduateYr = 0,
                Email = "javier.bardem@example.com",
                Password = "$2b$12$KqebvwNbxyuEg1OIOSjYZuMdXEDtEcbnsYwB9MHmSQgVsr.kHPWa.",
                PhoneNum = "201-555-0205",
                WeeklyHours = 0,
                TotalHours = 0,
                BiYearlyHours = 0
            },
            new()
            {
                ResidentId = "AAB1006",
                FirstName = "Viola",
                LastName = "Davis",
                GraduateYr = 0,
                Email = "viola.davis@example.com",
                Password = "$2b$12$KqebvwNbxyuEg1OIOSjYZuMdXEDtEcbnsYwB9MHmSQgVsr.kHPWa.",
                PhoneNum = "201-555-0206",
                WeeklyHours = 0,
                TotalHours = 0,
                BiYearlyHours = 0
            },
            new()
            {
                ResidentId = "AAB1007",
                FirstName = "Idris",
                LastName = "Elba",
                GraduateYr = 0,
                Email = "idris.elba@example.com",
                Password = "$2b$12$KqebvwNbxyuEg1OIOSjYZuMdXEDtEcbnsYwB9MHmSQgVsr.kHPWa.",
                PhoneNum = "201-555-0207",
                WeeklyHours = 0,
                TotalHours = 0,
                BiYearlyHours = 0
            },
            new()
            {
                ResidentId = "AAB1008",
                FirstName = "Lupita",
                LastName = "Nyong'o",
                GraduateYr = 0,
                Email = "lupita.nyongo@example.com",
                Password = "$2b$12$KqebvwNbxyuEg1OIOSjYZuMdXEDtEcbnsYwB9MHmSQgVsr.kHPWa.",
                PhoneNum = "201-555-0208",
                WeeklyHours = 0,
                TotalHours = 0,
                BiYearlyHours = 0
            },
            new()
            {
                ResidentId = "BEA3374",
                FirstName = "Brad",
                LastName = "Pitt",
                GraduateYr = 2,
                Email = "brad.pitt@example.com",
                Password
                    = "$2b$12$KqebvwNbxyuEg1OIOSjYZuMdXEDtEcbnsYwB9MHmSQgVsr.kHPWa.",
                PhoneNum = "201-555-0105",
                WeeklyHours = 0,
                TotalHours = 69,
                BiYearlyHours = 39
            },
            new()
            {
                ResidentId = "COH3276",
                FirstName = "Angelina",
                LastName = "Jolie",
                GraduateYr = 1,
                Email = "angelina.jolie@example.com",
                Password
                    = "$2b$12$KqebvwNbxyuEg1OIOSjYZuMdXEDtEcbnsYwB9MHmSQgVsr.kHPWa.",
                PhoneNum = "201-555-0106",
                WeeklyHours = 0,
                TotalHours = 27,
                BiYearlyHours = 27
            },
            new()
            {
                ResidentId = "CTE3965",
                FirstName = "Leonardo",
                LastName = "DiCaprio",
                GraduateYr = 2,
                Email = "leonardo.dicaprio@example.com",
                Password
                    = "$2b$12$KqebvwNbxyuEg1OIOSjYZuMdXEDtEcbnsYwB9MHmSQgVsr.kHPWa.",
                PhoneNum = "201-555-0107",
                WeeklyHours = 0,
                TotalHours = 57,
                BiYearlyHours = 21
            },
            new()
            {
                ResidentId = "FEU3416",
                FirstName = "Robert",
                LastName = "Downey",
                GraduateYr = 1,
                Email = "robert.downey@example.com",
                Password
                    = "$2b$12$KqebvwNbxyuEg1OIOSjYZuMdXEDtEcbnsYwB9MHmSQgVsr.kHPWa.",
                PhoneNum = "201-555-0109",
                WeeklyHours = 0,
                TotalHours = 21,
                BiYearlyHours = 9
            },
            new()
            {
                ResidentId = "FVO3464",
                FirstName = "Chris",
                LastName = "Evans",
                GraduateYr = 2,
                Email = "chris.evans@example.com",
                Password
                    = "$2b$12$KqebvwNbxyuEg1OIOSjYZuMdXEDtEcbnsYwB9MHmSQgVsr.kHPWa.",
                PhoneNum = "201-555-0110",
                WeeklyHours = 0,
                TotalHours = 63,
                BiYearlyHours = 45
            },
            new()
            {
                ResidentId = "FXI2766",
                FirstName = "Chris",
                LastName = "Hemsworth",
                GraduateYr = 1,
                Email = "chris.hemsworth@example.com",
                Password
                    = "$2b$12$KqebvwNbxyuEg1OIOSjYZuMdXEDtEcbnsYwB9MHmSQgVsr.kHPWa.",
                PhoneNum = "201-555-0111",
                WeeklyHours = 0,
                TotalHours = 24,
                BiYearlyHours = 24
            },
            new()
            {
                ResidentId = "GKU3319",
                FirstName = "Ryan",
                LastName = "Gosling",
                GraduateYr = 1,
                Email = "ryan.gosling@example.com",
                Password
                    = "$2b$12$KqebvwNbxyuEg1OIOSjYZuMdXEDtEcbnsYwB9MHmSQgVsr.kHPWa.",
                PhoneNum = "201-555-0113",
                WeeklyHours = 0,
                TotalHours = 33,
                BiYearlyHours = 9
            },
            new()
            {
                ResidentId = "GMO4083",
                FirstName = "Jennifer",
                LastName = "Lawrence",
                GraduateYr = 3,
                Email = "jennifer.lawrence@example.com",
                Password
                    = "$2b$12$KqebvwNbxyuEg1OIOSjYZuMdXEDtEcbnsYwB9MHmSQgVsr.kHPWa.",
                PhoneNum = "201-555-0114",
                WeeklyHours = 0,
                TotalHours = 9,
                BiYearlyHours = 9,
                ChiefType = ChiefType.Clinic
            },
            new()
            {
                ResidentId = "HKU2780",
                FirstName = "Morgan",
                LastName = "Freeman",
                GraduateYr = 1,
                Email = "morgan.freeman@example.com",
                Password
                    = "$2b$12$KqebvwNbxyuEg1OIOSjYZuMdXEDtEcbnsYwB9MHmSQgVsr.kHPWa.",
                PhoneNum = "201-555-0115",
                WeeklyHours = 0,
                TotalHours = 30,
                BiYearlyHours = 9
            },
            new()
            {
                ResidentId = "HQU5921",
                FirstName = "Cate",
                LastName = "Blanchett",
                GraduateYr = 3,
                Email = "cate.blanchett@example.com",
                Password
                    = "$2b$12$KqebvwNbxyuEg1OIOSjYZuMdXEDtEcbnsYwB9MHmSQgVsr.kHPWa.",
                PhoneNum = "201-555-0116",
                WeeklyHours = 0,
                TotalHours = 9,
                BiYearlyHours = 9
            },
            new()
            {
                ResidentId = "IDP3419",
                FirstName = "Joaquin",
                LastName = "Phoenix",
                GraduateYr = 2,
                Email = "joaquin.phoenix@example.com",
                Password
                    = "$2b$12$KqebvwNbxyuEg1OIOSjYZuMdXEDtEcbnsYwB9MHmSQgVsr.kHPWa.",
                PhoneNum = "201-555-0117",
                WeeklyHours = 0,
                TotalHours = 75,
                BiYearlyHours = 21
            },
            new()
            {
                ResidentId = "JCI5092",
                FirstName = "Samuel",
                LastName = "Jackson",
                GraduateYr = 3,
                Email = "samuel.jackson@example.com",
                Password
                    = "$2b$12$KqebvwNbxyuEg1OIOSjYZuMdXEDtEcbnsYwB9MHmSQgVsr.kHPWa.",
                PhoneNum = "201-555-0118",
                WeeklyHours = 0,
                TotalHours = 9,
                BiYearlyHours = 9
            },
            new()
            {
                ResidentId = "JXU4079",
                FirstName = "Keira",
                LastName = "Knightley",
                GraduateYr = 2,
                Email = "keira.knightley@example.com",
                Password
                    = "$2b$12$KqebvwNbxyuEg1OIOSjYZuMdXEDtEcbnsYwB9MHmSQgVsr.kHPWa.",
                PhoneNum = "201-555-0119",
                WeeklyHours = 0,
                TotalHours = 54,
                BiYearlyHours = 21
            },
            new()
            {
                ResidentId = "KOS3940",
                FirstName = "Hugh",
                LastName = "Jackman",
                GraduateYr = 1,
                Email = "hugh.jackman@example.com",
                Password
                    = "$2b$12$KqebvwNbxyuEg1OIOSjYZuMdXEDtEcbnsYwB9MHmSQgVsr.kHPWa.",
                PhoneNum = "201-555-0120",
                WeeklyHours = 0,
                TotalHours = 39,
                BiYearlyHours = 39
            },
            new()
            {
                ResidentId = "LLU6249",
                FirstName = "Christian",
                LastName = "Bale",
                GraduateYr = 2,
                Email = "christian.bale@example.com",
                Password
                    = "$2b$12$KqebvwNbxyuEg1OIOSjYZuMdXEDtEcbnsYwB9MHmSQgVsr.kHPWa.",
                PhoneNum = "201-555-0121",
                WeeklyHours = 0,
                TotalHours = 78,
                BiYearlyHours = 18
            },
            new()
            {
                ResidentId = "LZU4568",
                FirstName = "Anne",
                LastName = "Hathaway",
                GraduateYr = 3,
                Email = "anne.hathaway@example.com",
                Password
                    = "$2b$12$KqebvwNbxyuEg1OIOSjYZuMdXEDtEcbnsYwB9MHmSQgVsr.kHPWa.",
                PhoneNum = "201-555-0122",
                WeeklyHours = 0,
                TotalHours = 9,
                BiYearlyHours = 9
            },
            new()
            {
                ResidentId = "MGE3752",
                FirstName = "Will",
                LastName = "Smith",
                GraduateYr = 1,
                Email = "will.smith@example.com",
                Password
                    = "$2b$12$KqebvwNbxyuEg1OIOSjYZuMdXEDtEcbnsYwB9MHmSQgVsr.kHPWa.",
                PhoneNum = "201-555-0123",
                WeeklyHours = 0,
                TotalHours = 45,
                BiYearlyHours = 9
            },
            new()
            {
                ResidentId = "MPE3472",
                FirstName = "Zoe",
                LastName = "Saldana",
                GraduateYr = 2,
                Email = "zoe.saldana@example.com",
                Password
                    = "$2b$12$KqebvwNbxyuEg1OIOSjYZuMdXEDtEcbnsYwB9MHmSQgVsr.kHPWa.",
                PhoneNum = "201-555-0124",
                WeeklyHours = 0,
                TotalHours = 60,
                BiYearlyHours = 21
            },
            new()
            {
                ResidentId = "MPE3473",
                FirstName = "Matt",
                LastName = "Damon",
                GraduateYr = 2,
                Email = "matt.damon@example.com",
                Password
                    = "$2b$12$KqebvwNbxyuEg1OIOSjYZuMdXEDtEcbnsYwB9MHmSQgVsr.kHPWa.",
                PhoneNum = "201-555-0125",
                WeeklyHours = 0,
                TotalHours = 51,
                BiYearlyHours = 27
            },
            new()
            {
                ResidentId = "RCU4642",
                FirstName = "Amy",
                LastName = "Adams",
                GraduateYr = 3,
                Email = "amy.adams@example.com",
                Password
                    = "$2b$12$KqebvwNbxyuEg1OIOSjYZuMdXEDtEcbnsYwB9MHmSQgVsr.kHPWa.",
                PhoneNum = "201-555-0126",
                WeeklyHours = 0,
                TotalHours = 9,
                BiYearlyHours = 9,
                ChiefType = ChiefType.Admin
            },
            new()
            {
                ResidentId = "RRO4170",
                FirstName = "Mark",
                LastName = "Ruffalo",
                GraduateYr = 3,
                Email = "mark.ruffalo@example.com",
                Password
                    = "$2b$12$KqebvwNbxyuEg1OIOSjYZuMdXEDtEcbnsYwB9MHmSQgVsr.kHPWa.",
                PhoneNum = "201-555-0127",
                WeeklyHours = 0,
                TotalHours = 9,
                BiYearlyHours = 9
            },
            new()
            {
                ResidentId = "RUZ2717",
                FirstName = "Benedict",
                LastName = "Cumberbatch",
                GraduateYr = 1,
                Email = "benedict.cumberbatch@example.com",
                Password
                    = "$2b$12$KqebvwNbxyuEg1OIOSjYZuMdXEDtEcbnsYwB9MHmSQgVsr.kHPWa.",
                PhoneNum = "201-555-0128",
                WeeklyHours = 0,
                TotalHours = 45,
                BiYearlyHours = 45
            },
            new()
            {
                ResidentId = "VKD7295",
                FirstName = "Jackie",
                LastName = "Chan",
                GraduateYr = 3,
                Email = "jackie.chan@example.com",
                Password
                    = "$2b$12$KqebvwNbxyuEg1OIOSjYZuMdXEDtEcbnsYwB9MHmSQgVsr.kHPWa.",
                PhoneNum = "201-555-0128",
                WeeklyHours = 0,
                TotalHours = 87,
                BiYearlyHours = 67,
                ChiefType = ChiefType.Education
            },
            new()
            {
                ResidentId = "KMD4264",
                FirstName = "Barack",
                LastName = "Obama",
                GraduateYr = 3,
                Email = "barack.obama@example.com",
                Password
                    = "$2b$12$KqebvwNbxyuEg1OIOSjYZuMdXEDtEcbnsYwB9MHmSQgVsr.kHPWa.",
                PhoneNum = "201-555-0128",
                WeeklyHours = 0,
                TotalHours = 11,
                BiYearlyHours = 4,
                ChiefType = ChiefType.Clinic
            }
        ];

        if (!await context.Residents.AnyAsync())
        {
            _logger.LogInformation("Seeding Residents");
            context.Residents.AddRange(residents);

            await context.SaveChangesAsync();
        }

        if (!await context.Vacations.AnyAsync())
        {
            _logger.LogInformation("Seeding Vacations");
            List<Vacation> vacations = [];

            DateOnly start = new(2025, 1, 1);
            DateOnly end = new(2028, 1, 1);

            while (start <= end)
            {
                for (int i = 0; i < 3; i++)
                {
                    vacations.Add(new Vacation
                    {
                        Date = start,
                        Details = "Autogenerated",
                        Reason = "Autogenerated",
                        GroupId = Guid.NewGuid().ToString(),
                        HalfDay = Random.Shared.Next(0, 3) switch
                        {
                            0 => (PartOfDay.Morning | PartOfDay.Afternoon).DbChar,
                            1 => PartOfDay.Morning.DbChar,
                            2 => PartOfDay.Afternoon.DbChar
                        },
                        ResidentId = residents[Random.Shared.Next(0, residents.Count)].ResidentId,
                        Status = "Approved",
                        VacationId = Guid.NewGuid()
                    });
                }

                start = start.AddDays(1);
            }

            await context.Vacations.AddRangeAsync(vacations);
            await context.SaveChangesAsync();
        }
    }
}