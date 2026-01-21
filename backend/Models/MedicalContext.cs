using Microsoft.EntityFrameworkCore;

namespace MedicalDemo.Models;

public class MedicalContext : DbContext
{
    public MedicalContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<Admins> admins { get; set; }
    public DbSet<Announcements> announcements { get; set; }
    public DbSet<Residents> residents { get; set; }
    public DbSet<Rotations> rotations { get; set; }
    public DbSet<Dates> dates { get; set; }
    public DbSet<Schedules> schedules { get; set; }
    public DbSet<Blackouts> blackouts { get; set; }
    public DbSet<Vacations> vacations { get; set; }
    public DbSet<Invitation> Invitations { get; set; }
    public DbSet<SwapRequest> SwapRequests { get; set; }
    public DbSet<RotationTypes> RotationTypes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Map AnnouncementId as binary(16)
        modelBuilder.Entity<Announcements>()
            .Property(s => s.AnnouncementId)
            .HasColumnType("binary(16)");

        // Map SwapId as binary(16)
        modelBuilder.Entity<SwapRequest>()
            .Property(s => s.SwapId)
            .HasColumnType("binary(16)");

        // Map ScheduleSwapId as binary(16)
        modelBuilder.Entity<SwapRequest>()
            .Property(s => s.ScheduleSwapId)
            .HasColumnType("binary(16)");

        // Create Timestamps for the swap requests on creation
        modelBuilder.Entity<SwapRequest>()
            .Property(b => b.CreatedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        // Create Timestamps for the swap requests on creation/update
        modelBuilder.Entity<SwapRequest>()
            .Property(b => b.UpdatedAt)
            .HasDefaultValueSql(
                "CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP");

        // Map BlackoutId as binary(16)
        modelBuilder.Entity<Blackouts>()
            .Property(s => s.BlackoutId)
            .HasColumnType("binary(16)");

        // Map DateId as binary(16)
        modelBuilder.Entity<Dates>()
            .Property(s => s.DateId)
            .HasColumnType("binary(16)");

        // Map ScheduleId in Dates as binary(16)
        modelBuilder.Entity<Dates>()
            .Property(d => d.ScheduleId)
            .HasColumnType("binary(16)");

        // Map RotationId as binary(16)
        modelBuilder.Entity<Rotations>()
            .Property(s => s.RotationId)
            .HasColumnType("binary(16)");

        modelBuilder.Entity<Rotations>()
            .Property(s => s.RotationTypeId)
            .HasColumnType("binary(16)")
            .HasConversion(
                v => v.ToByteArray(),
                v => new Guid(v)
            )
            .HasDefaultValue(Guid.Parse("0713102b-03c1-4a62-bfd8-4c04369ac7c4"));

        // Map ScheduleId as binary(16)
        modelBuilder.Entity<Schedules>()
            .Property(s => s.ScheduleId)
            .HasColumnType("binary(16)");

        // Map VacationId as binary(16)
        modelBuilder.Entity<Vacations>()
            .Property(v => v.VacationId)
            .HasColumnType("binary(16)");

        modelBuilder.Entity<RotationTypes>()
            .Property(v => v.RotationTypeId)
            .HasColumnType("binary(16)")
            .HasConversion(
                v => v.ToByteArray(),
                v => new Guid(v)
            );

        modelBuilder.Entity<RotationTypes>()
            .Property(e => e.PgyYearFlags)
            .HasConversion<int>()
            .HasColumnType("int")
            .HasDefaultValue(PgyYearFlags.NONE);

        base.OnModelCreating(modelBuilder);

        PopulateRotationTypesTable(modelBuilder);
    }

    private void PopulateRotationTypesTable(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<RotationTypes>().HasData(
            new RotationTypes
            {
                RotationTypeId = Guid.Parse("f5b8c6a3-6417-4b44-bda3-ca5e7bee086d"),
                RotationName = "Unassigned",
                DoesLongCall = true,
                DoesShortCall = true,
                DoesTrainingLongCall = true,
                DoesTrainingShortCall = true,
                IsChiefRotation = false,
                PgyYearFlags = PgyYearFlags.PGY1 | PgyYearFlags.PGY2 | PgyYearFlags.PGY3 | PgyYearFlags.PGY4
            },
            new RotationTypes
            {
                RotationTypeId = Guid.Parse("0713102b-03c1-4a62-bfd8-4c04369ac7c4"),
                RotationName = "Inpatient Psy",
                DoesLongCall = true,
                DoesShortCall = true,
                DoesTrainingLongCall = true,
                DoesTrainingShortCall = true,
                IsChiefRotation = false,
                PgyYearFlags = PgyYearFlags.PGY1 | PgyYearFlags.PGY2 | PgyYearFlags.PGY3 | PgyYearFlags.PGY4
            },
            new RotationTypes
            {
                RotationTypeId = Guid.Parse("f8ab8fea-afff-46dd-aec7-423d791ce8d1"),
                RotationName = "Geriatric",
                DoesLongCall = true,
                DoesShortCall = true,
                DoesTrainingLongCall = true,
                DoesTrainingShortCall = true,
                IsChiefRotation = false,
                PgyYearFlags = PgyYearFlags.PGY1 | PgyYearFlags.PGY2 | PgyYearFlags.PGY3
            },
            new RotationTypes
            {
                RotationTypeId = Guid.Parse("446b12a5-7e18-48fd-8abb-20d39a5aa353"),
                RotationName = "PHPandIOP",
                DoesLongCall = true,
                DoesShortCall = true,
                DoesTrainingLongCall = true,
                DoesTrainingShortCall = true,
                IsChiefRotation = false,
                PgyYearFlags = PgyYearFlags.PGY1 | PgyYearFlags.PGY2 | PgyYearFlags.PGY3
            },
            new RotationTypes
            {
                RotationTypeId = Guid.Parse("a06557d7-1099-42ec-9cee-ed3eec6213ab"),
                RotationName = "Psy Consults",
                DoesLongCall = true,
                DoesShortCall = true,
                DoesTrainingLongCall = true,
                DoesTrainingShortCall = true,
                IsChiefRotation = false,
                PgyYearFlags = PgyYearFlags.PGY1 | PgyYearFlags.PGY2 | PgyYearFlags.PGY3 | PgyYearFlags.PGY4
            },
            new RotationTypes
            {
                RotationTypeId = Guid.Parse("8932166e-dc24-4f3c-88d7-3fb9ee7e161a"),
                RotationName = "Community Psy",
                DoesLongCall = true,
                DoesShortCall = true,
                DoesTrainingLongCall = true,
                DoesTrainingShortCall = true,
                IsChiefRotation = false,
                PgyYearFlags = PgyYearFlags.PGY1 | PgyYearFlags.PGY2 | PgyYearFlags.PGY3 | PgyYearFlags.PGY4
            },
            new RotationTypes
            {
                RotationTypeId = Guid.Parse("89e68fd1-28cc-4d7f-b0fd-20fed8396231"),
                RotationName = "CAP",
                DoesLongCall = true,
                DoesShortCall = true,
                DoesTrainingLongCall = true,
                DoesTrainingShortCall = true,
                IsChiefRotation = false,
                PgyYearFlags = PgyYearFlags.PGY1 | PgyYearFlags.PGY2 | PgyYearFlags.PGY3
            },
            new RotationTypes
            {
                RotationTypeId = Guid.Parse("771627d7-ba9f-4549-898a-4267fd14e43a"),
                RotationName = "Addiction",
                DoesLongCall = true,
                DoesShortCall = true,
                DoesTrainingLongCall = true,
                DoesTrainingShortCall = true,
                IsChiefRotation = false,
                PgyYearFlags = PgyYearFlags.PGY1 | PgyYearFlags.PGY2 | PgyYearFlags.PGY3 | PgyYearFlags.PGY4
            },
            new RotationTypes
            {
                RotationTypeId = Guid.Parse("b544012c-cce0-49d9-b2d6-1f17068db7d2"),
                RotationName = "Forensic",
                DoesLongCall = true,
                DoesShortCall = true,
                DoesTrainingLongCall = true,
                DoesTrainingShortCall = true,
                IsChiefRotation = false,
                PgyYearFlags = PgyYearFlags.PGY1 | PgyYearFlags.PGY2 | PgyYearFlags.PGY3 | PgyYearFlags.PGY4
            },
            new RotationTypes
            {
                RotationTypeId = Guid.Parse("a57c8e92-77fa-410f-be54-9126881c3514"),
                RotationName = "Float",
                DoesLongCall = true,
                DoesShortCall = true,
                DoesTrainingLongCall = true,
                DoesTrainingShortCall = true,
                IsChiefRotation = false,
                PgyYearFlags = PgyYearFlags.PGY1 | PgyYearFlags.PGY2 | PgyYearFlags.PGY3
            },
            new RotationTypes
            {
                RotationTypeId = Guid.Parse("a54ddddb-0245-4582-8736-57e30d8a0c17"),
                RotationName = "Neurology",
                DoesLongCall = false,
                DoesShortCall = true,
                DoesTrainingLongCall = true,
                DoesTrainingShortCall = true,
                IsChiefRotation = false,
                PgyYearFlags = PgyYearFlags.PGY1 | PgyYearFlags.PGY2 | PgyYearFlags.PGY3
            },
            new RotationTypes
            {
                RotationTypeId = Guid.Parse("6e46b558-555a-4404-8155-131dc59fa430"),
                RotationName = "ImOutpatient",
                DoesLongCall = false,
                DoesShortCall = true,
                DoesTrainingLongCall = true,
                DoesTrainingShortCall = true,
                IsChiefRotation = false,
                PgyYearFlags = PgyYearFlags.PGY1 | PgyYearFlags.PGY2 | PgyYearFlags.PGY3
            },
            new RotationTypes
            {
                RotationTypeId = Guid.Parse("10406b7f-1cb7-4d0d-b14e-7983a48c1ce8"),
                RotationName = "ImInpatient",
                DoesLongCall = false,
                DoesShortCall = false,
                DoesTrainingLongCall = false,
                DoesTrainingShortCall = false,
                IsChiefRotation = false,
                PgyYearFlags = PgyYearFlags.PGY1 | PgyYearFlags.PGY2 | PgyYearFlags.PGY3
            },
            new RotationTypes
            {
                RotationTypeId = Guid.Parse("265a80c6-629b-468f-83ea-7465c0f633c3"),
                RotationName = "NightFloat",
                DoesLongCall = true,
                DoesShortCall = true,
                DoesTrainingLongCall = true,
                DoesTrainingShortCall = true,
                IsChiefRotation = false,
                PgyYearFlags = PgyYearFlags.PGY1 | PgyYearFlags.PGY2 | PgyYearFlags.PGY3
            },
            new RotationTypes
            {
                RotationTypeId = Guid.Parse("901c7951-5714-488f-9731-636be1fb6aec"),
                RotationName = "EmergencyMed",
                DoesLongCall = false,
                DoesShortCall = false,
                DoesTrainingLongCall = false,
                DoesTrainingShortCall = false,
                IsChiefRotation = false,
                PgyYearFlags = PgyYearFlags.PGY1 | PgyYearFlags.PGY2 | PgyYearFlags.PGY3
            },
            new RotationTypes
            {
                RotationTypeId = Guid.Parse("e0f5284e-00d5-4cea-ba2e-a3718d5b70c4"),
                RotationName = "Chief",
                DoesLongCall = true,
                DoesShortCall = true,
                DoesTrainingLongCall = true,
                DoesTrainingShortCall = true,
                IsChiefRotation = true,
                PgyYearFlags = PgyYearFlags.PGY4
            },
            new RotationTypes
            {
                RotationTypeId = Guid.Parse("e7f62d10-57da-4749-9c10-0b4ec538a047"),
                RotationName = "TMS",
                DoesLongCall = true,
                DoesShortCall = true,
                DoesTrainingLongCall = true,
                DoesTrainingShortCall = true,
                IsChiefRotation = false,
                PgyYearFlags = PgyYearFlags.PGY4
            },
            new RotationTypes
            {
                RotationTypeId = Guid.Parse("4547b1bf-577c-4ad9-8246-7f085ed957f9"),
                RotationName = "IOP",
                DoesLongCall = true,
                DoesShortCall = true,
                DoesTrainingLongCall = true,
                DoesTrainingShortCall = true,
                IsChiefRotation = false,
                PgyYearFlags = PgyYearFlags.PGY4
            },
            new RotationTypes
            {
                RotationTypeId = Guid.Parse("b0e37c32-7919-4d52-bf20-f272c98a4e83"),
                RotationName = "NFETC",
                DoesLongCall = true,
                DoesShortCall = true,
                DoesTrainingLongCall = true,
                DoesTrainingShortCall = true,
                IsChiefRotation = false,
                PgyYearFlags = PgyYearFlags.PGY4
            },
            new RotationTypes
            {
                RotationTypeId = Guid.Parse("7e43c941-e1c0-4c58-8f41-535e1a1a82db"),
                RotationName = "HPC",
                DoesLongCall = true,
                DoesShortCall = true,
                DoesTrainingLongCall = true,
                DoesTrainingShortCall = true,
                IsChiefRotation = false,
                PgyYearFlags = PgyYearFlags.PGY4
            },
            new RotationTypes
            {
                RotationTypeId = Guid.Parse("984d44d9-2ff1-4919-a895-cc865a9e4872"),
                RotationName = "VA",
                DoesLongCall = true,
                DoesShortCall = true,
                DoesTrainingLongCall = true,
                DoesTrainingShortCall = true,
                IsChiefRotation = false,
                PgyYearFlags = PgyYearFlags.PGY4
            },
            new RotationTypes
            {
                RotationTypeId = Guid.Parse("80f33f29-cfa9-4442-b458-75285ad993c4"),
                RotationName = "CLC",
                DoesLongCall = true,
                DoesShortCall = true,
                DoesTrainingLongCall = true,
                DoesTrainingShortCall = true,
                IsChiefRotation = false,
                PgyYearFlags = PgyYearFlags.PGY4
            },
            new RotationTypes
            {
                RotationTypeId = Guid.Parse("9063f281-7919-4937-b46b-ea04147018ca"),
                RotationName = "Sum",
                DoesLongCall = true,
                DoesShortCall = true,
                DoesTrainingLongCall = true,
                DoesTrainingShortCall = true,
                IsChiefRotation = false,
                PgyYearFlags = PgyYearFlags.PGY4
            }
        );
    }
}