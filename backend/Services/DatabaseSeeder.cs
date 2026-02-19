using MedicalDemo.Models;
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
                    AdminId = "004",
                    FirstName = "Scarlett",
                    LastName = "Johansson",
                    Email = "scarlett.johansson@example.com",
                    Password
                        = "$2b$12$KqebvwNbxyuEg1OIOSjYZuMdXEDtEcbnsYwB9MHmSQgVsr.kHPWa.",
                    PhoneNum = "201-555-0104"
                }
            );
        }

        List<Resident> residents = [
            new Resident
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
                BiYearlyHours = 39,
                HospitalRoleProfile = 8
            },
            new Resident
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
                BiYearlyHours = 27,
                HospitalRoleProfile = 0
            },
            new Resident
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
                BiYearlyHours = 21,
                HospitalRoleProfile = 13
            },
            new Resident
            {
                ResidentId = "EIC4231",
                FirstName = "Natalie",
                LastName = "Portman",
                GraduateYr = 3,
                Email = "natalie.portman@example.com",
                Password
                    = "$2b$12$KqebvwNbxyuEg1OIOSjYZuMdXEDtEcbnsYwB9MHmSQgVsr.kHPWa.",
                PhoneNum = "201-555-0108",
                WeeklyHours = 0,
                TotalHours = 9,
                BiYearlyHours = 9,
                HospitalRoleProfile = null
            },
            new Resident
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
                BiYearlyHours = 9,
                HospitalRoleProfile = 5
            },
            new Resident
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
                BiYearlyHours = 45,
                HospitalRoleProfile = 9
            },
            new Resident
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
                BiYearlyHours = 24,
                HospitalRoleProfile = 2
            },
            new Resident
            {
                ResidentId = "GEV4598",
                FirstName = "Emma",
                LastName = "Stone",
                GraduateYr = 3,
                Email = "emma.stone@example.com",
                Password
                    = "$2b$12$KqebvwNbxyuEg1OIOSjYZuMdXEDtEcbnsYwB9MHmSQgVsr.kHPWa.",
                PhoneNum = "201-555-0112",
                WeeklyHours = 0,
                TotalHours = 9,
                BiYearlyHours = 9,
                HospitalRoleProfile = null
            },
            new Resident
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
                BiYearlyHours = 9,
                HospitalRoleProfile = 6
            },
            new Resident
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
                HospitalRoleProfile = null
            },
            new Resident
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
                BiYearlyHours = 9,
                HospitalRoleProfile = 4
            },
            new Resident
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
                BiYearlyHours = 9,
                HospitalRoleProfile = null
            },
            new Resident
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
                BiYearlyHours = 21,
                HospitalRoleProfile = 11
            },
            new Resident
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
                BiYearlyHours = 9,
                HospitalRoleProfile = null
            },
            new Resident
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
                BiYearlyHours = 21,
                HospitalRoleProfile = 12
            },
            new Resident
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
                BiYearlyHours = 39,
                HospitalRoleProfile = 3
            },
            new Resident
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
                BiYearlyHours = 18,
                HospitalRoleProfile = 10
            },
            new Resident
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
                BiYearlyHours = 9,
                HospitalRoleProfile = null
            },
            new Resident
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
                BiYearlyHours = 9,
                HospitalRoleProfile = 7
            },
            new Resident
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
                BiYearlyHours = 21,
                HospitalRoleProfile = 15
            },
            new Resident
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
                BiYearlyHours = 27,
                HospitalRoleProfile = 14
            },
            new Resident
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
                HospitalRoleProfile = null
            },
            new Resident
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
                BiYearlyHours = 9,
                HospitalRoleProfile = null
            },
            new Resident
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
                BiYearlyHours = 45,
                HospitalRoleProfile = 1
            },
            new Resident
            {
                ResidentId = "OFS2842",
                FirstName = "Dwayne",
                LastName = "Johnson",
                GraduateYr = 3,
                Email = "dwayne.johnson@example.com",
                Password
                    = "$2b$12$KqebvwNbxyuEg1OIOSjYZuMdXEDtEcbnsYwB9MHmSQgVsr.kHPWa.",
                PhoneNum = "201-555-0128",
                WeeklyHours = 0,
                TotalHours = 43,
                BiYearlyHours = 23,
                HospitalRoleProfile = null
            },
            new Resident
            {
                ResidentId = "LKD1234",
                FirstName = "Tom",
                LastName = "Cruise",
                GraduateYr = 3,
                Email = "tom.cruise@example.com",
                Password
                    = "$2b$12$KqebvwNbxyuEg1OIOSjYZuMdXEDtEcbnsYwB9MHmSQgVsr.kHPWa.",
                PhoneNum = "201-555-0128",
                WeeklyHours = 0,
                TotalHours = 9,
                BiYearlyHours = 9,
                HospitalRoleProfile = null
            },
            new Resident
            {
                ResidentId = "AKC1234",
                FirstName = "Justin",
                LastName = "Bieber",
                GraduateYr = 3,
                Email = "justin.bieber@example.com",
                Password
                    = "$2b$12$KqebvwNbxyuEg1OIOSjYZuMdXEDtEcbnsYwB9MHmSQgVsr.kHPWa.",
                PhoneNum = "201-555-0128",
                WeeklyHours = 0,
                TotalHours = 17,
                BiYearlyHours = 14,
                HospitalRoleProfile = null
            },
            new Resident
            {
                ResidentId = "POW8237",
                FirstName = "Tom",
                LastName = "Holland",
                GraduateYr = 3,
                Email = "tom.holland@example.com",
                Password
                    = "$2b$12$KqebvwNbxyuEg1OIOSjYZuMdXEDtEcbnsYwB9MHmSQgVsr.kHPWa.",
                PhoneNum = "201-555-0128",
                WeeklyHours = 0,
                TotalHours = 78,
                BiYearlyHours = 51,
                HospitalRoleProfile = null
            },
            new Resident
            {
                ResidentId = "KJD8273",
                FirstName = "Pedro",
                LastName = "Pascal",
                GraduateYr = 3,
                Email = "pedro.pascal@example.com",
                Password
                    = "$2b$12$KqebvwNbxyuEg1OIOSjYZuMdXEDtEcbnsYwB9MHmSQgVsr.kHPWa.",
                PhoneNum = "201-555-0128",
                WeeklyHours = 0,
                TotalHours = 41,
                BiYearlyHours = 39,
                HospitalRoleProfile = null
            },
            new Resident
            {
                ResidentId = "MWU2643",
                FirstName = "Margot",
                LastName = "Robbie",
                GraduateYr = 3,
                Email = "margot.robbie@example.com",
                Password
                    = "$2b$12$KqebvwNbxyuEg1OIOSjYZuMdXEDtEcbnsYwB9MHmSQgVsr.kHPWa.",
                PhoneNum = "201-555-0128",
                WeeklyHours = 0,
                TotalHours = 10,
                BiYearlyHours = 4,
                HospitalRoleProfile = null
            },
            new Resident
            {
                ResidentId = "KSK8232",
                FirstName = "Chris",
                LastName = "Rock",
                GraduateYr = 3,
                Email = "chris.rock@example.com",
                Password
                    = "$2b$12$KqebvwNbxyuEg1OIOSjYZuMdXEDtEcbnsYwB9MHmSQgVsr.kHPWa.",
                PhoneNum = "201-555-0128",
                WeeklyHours = 0,
                TotalHours = 23,
                BiYearlyHours = 18,
                HospitalRoleProfile = null
            },
            new Resident
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
                HospitalRoleProfile = null
            },
            new Resident
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
                HospitalRoleProfile = null
            },
            new Resident
            {
                ResidentId = "ZJT8738",
                FirstName = "Jason",
                LastName = "Momoa",
                GraduateYr = 3,
                Email = "json.momoa@example.com",
                Password
                    = "$2b$12$KqebvwNbxyuEg1OIOSjYZuMdXEDtEcbnsYwB9MHmSQgVsr.kHPWa.",
                PhoneNum = "201-555-0128",
                WeeklyHours = 0,
                TotalHours = 20,
                BiYearlyHours = 12,
                HospitalRoleProfile = null
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
                vacations.Add(new Vacation
                {
                    Date = start,
                    Details = "Autogenerated",
                    Reason = "Autogenerated",
                    GroupId = Guid.NewGuid().ToString(),
                    HalfDay = Random.Shared.Next(0, 3) switch
                    {
                        0 => null,
                        1 => "A",
                        2 => "P",
                    },
                    ResidentId = residents[Random.Shared.Next(0, residents.Count)].ResidentId,
                    Status = "Approved",
                    VacationId = Guid.NewGuid(),
                });
                start = start.AddDays(1);
            }

            await context.Vacations.AddRangeAsync(vacations);
            await context.SaveChangesAsync();
        }
    }
}