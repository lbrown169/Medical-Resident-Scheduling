using MedicalDemo.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace MedicalDemo.Models.Entities
{
    public partial class MedicalContext : DbContext
    {
        public MedicalContext()
        {
        }

        public MedicalContext(DbContextOptions<MedicalContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Admin> Admins { get; set; } = null!;
        public virtual DbSet<Announcement> Announcements { get; set; } = null!;
        public virtual DbSet<Blackout> Blackouts { get; set; } = null!;
        public virtual DbSet<Date> Dates { get; set; } = null!;
        public virtual DbSet<Invitation> Invitations { get; set; } = null!;
        public virtual DbSet<Resident> Residents { get; set; } = null!;
        public virtual DbSet<Rotation> Rotations { get; set; } = null!;
        public virtual DbSet<Schedule> Schedules { get; set; } = null!;
        public virtual DbSet<SwapRequest> SwapRequests { get; set; } = null!;
        public virtual DbSet<Vacation> Vacations { get; set; } = null!;
        public virtual DbSet<RotationType> RotationTypes { get; set; } = null!;
        public virtual DbSet<RotationPrefRequest> RotationPrefRequests { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // EF Tooling
            modelBuilder.UseCollation("utf8mb4_0900_ai_ci")
                .HasCharSet("utf8mb4");

            modelBuilder.Entity<Admin>(entity =>
            {
                entity.ToTable("admins");

                entity.HasIndex(e => e.AdminId, "admin_id_UNIQUE")
                    .IsUnique();

                entity.Property(e => e.AdminId)
                    .HasMaxLength(15)
                    .HasColumnName("admin_id");

                entity.Property(e => e.Email)
                    .HasMaxLength(45)
                    .HasColumnName("email");

                entity.Property(e => e.FirstName)
                    .HasMaxLength(45)
                    .HasColumnName("first_name");

                entity.Property(e => e.LastName)
                    .HasMaxLength(45)
                    .HasColumnName("last_name");

                entity.Property(e => e.Password)
                    .HasMaxLength(100)
                    .HasColumnName("password");

                entity.Property(e => e.PhoneNum)
                    .HasMaxLength(15)
                    .HasColumnName("phone_num");
            });

            modelBuilder.Entity<Announcement>(entity =>
            {
                entity.ToTable("announcements");

                entity.HasIndex(e => e.AnnouncementId, "announcement_id_UNIQUE")
                    .IsUnique();

                entity.HasIndex(e => e.AuthorId, "author_id_idx");

                entity.Property(e => e.AnnouncementId)
                    .HasColumnName("announcement_id")
                    .HasColumnType("binary(16)");

                entity.Property(e => e.AuthorId)
                    .HasMaxLength(45)
                    .HasColumnName("author_id");

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("created_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.Message)
                    .HasMaxLength(250)
                    .HasColumnName("message");

                entity.HasOne(d => d.Author)
                    .WithMany(p => p.Announcements)
                    .HasForeignKey(d => d.AuthorId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("author_id");
            });

            modelBuilder.Entity<Blackout>(entity =>
            {
                entity.ToTable("blackouts");

                entity.HasIndex(e => e.BlackoutId, "blackout_id_UNIQUE")
                    .IsUnique();

                entity.HasIndex(e => e.ResidentId, "resident_id_blackouts_idx");

                entity.Property(e => e.BlackoutId)
                    .HasColumnType("binary(16)")
                    .HasColumnName("blackout_id");

                entity.Property(e => e.Date).HasColumnName("date");

                entity.Property(e => e.ResidentId)
                    .HasMaxLength(15)
                    .HasColumnName("resident_id");

                entity.HasOne(d => d.Resident)
                    .WithMany(p => p.Blackouts)
                    .HasForeignKey(d => d.ResidentId)
                    .HasConstraintName("resident_id_blackouts");
            });

            modelBuilder.Entity<Date>(entity =>
            {
                entity.ToTable("dates");

                entity.HasIndex(e => e.ResidentId, "resident_id_dates_idx");

                entity.HasIndex(e => e.ScheduleId, "schedule_id");

                entity.HasIndex(e => e.DateId, "schedule_id_UNIQUE")
                    .IsUnique();

                entity.Property(e => e.DateId)
                    .HasColumnType("binary(16)")
                    .HasColumnName("date_id");

                entity.Property(e => e.CallType)
                    .HasColumnName("call_type");

                entity.Property(e => e.ShiftDate).HasColumnName("date");

                entity.Property(e => e.Hours).HasColumnName("hours");

                entity.Property(e => e.ResidentId)
                    .HasMaxLength(15)
                    .HasColumnName("resident_id");

                entity.Property(e => e.ScheduleId)
                    .HasColumnType("binary(16)")
                    .HasColumnName("schedule_id");

                entity.HasOne(d => d.Resident)
                    .WithMany(p => p.Dates)
                    .HasForeignKey(d => d.ResidentId)
                    .HasConstraintName("resident_id_dates");

                entity.HasOne(d => d.Schedule)
                    .WithMany(p => p.Dates)
                    .HasForeignKey(d => d.ScheduleId)
                    .HasConstraintName("schedule_id");
            });

            modelBuilder.Entity<Invitation>(entity =>
            {
                entity.HasKey(e => e.Token)
                    .HasName("PRIMARY");

                entity.ToTable("invitations");

                entity.Property(e => e.Token).HasColumnName("token");

                entity.Property(e => e.Expires)
                    .HasColumnType("datetime")
                    .HasColumnName("expires");

                entity.Property(e => e.ResidentId)
                    .HasMaxLength(255)
                    .HasColumnName("resident_id");

                entity.Property(e => e.Used)
                    .HasColumnName("used")
                    .HasDefaultValue(false);
            });

            modelBuilder.Entity<Resident>(entity =>
            {
                entity.ToTable("residents");

                entity.HasIndex(e => e.Email, "email_UNIQUE")
                    .IsUnique();

                entity.HasIndex(e => e.ResidentId, "resident_id_UNIQUE")
                    .IsUnique();

                entity.Property(e => e.ResidentId)
                    .HasMaxLength(15)
                    .HasColumnName("resident_id");

                entity.Property(e => e.BiYearlyHours).HasColumnName("bi_yearly_hours");

                entity.Property(e => e.Email)
                    .HasMaxLength(45)
                    .HasColumnName("email");

                entity.Property(e => e.FirstName)
                    .HasMaxLength(45)
                    .HasColumnName("first_name");

                entity.Property(e => e.GraduateYr)
                    .HasColumnName("graduate_yr")
                    .HasDefaultValueSql("'1'");

                entity.Property(e => e.HospitalRoleProfile).HasColumnName("hospital_role_profile");

                entity.Property(e => e.LastName)
                    .HasMaxLength(45)
                    .HasColumnName("last_name");

                entity.Property(e => e.Password)
                    .HasMaxLength(100)
                    .HasColumnName("password")
                    .HasDefaultValueSql("''");

                entity.Property(e => e.PhoneNum)
                    .HasMaxLength(15)
                    .HasColumnName("phone_num")
                    .HasDefaultValueSql("''");

                entity.Property(e => e.TotalHours).HasColumnName("total_hours");

                entity.Property(e => e.WeeklyHours).HasColumnName("weekly_hours");
            });

            modelBuilder.Entity<Rotation>(entity =>
            {
                entity.ToTable("rotations");

                entity.HasIndex(e => e.ResidentId, "resident_id_rotation_idx");

                entity.HasIndex(e => e.RotationId, "rotation_id_UNIQUE")
                    .IsUnique();

                entity.Property(e => e.RotationId)
                    .HasColumnType("binary(16)")
                    .HasColumnName("rotation_id");

                entity.Property(e => e.Month)
                    .HasMaxLength(45)
                    .HasColumnName("month");

                entity.Property(e => e.ResidentId)
                    .HasMaxLength(15)
                    .HasColumnName("resident_id");

                entity.Property(e => e.Rotation1)
                    .HasMaxLength(45)
                    .HasColumnName("rotation");

                entity.HasOne(d => d.Resident)
                    .WithMany(p => p.Rotations)
                    .HasForeignKey(d => d.ResidentId)
                    .HasConstraintName("resident_id_rotation");
            });

            modelBuilder.Entity<Schedule>(entity =>
            {
                entity.ToTable("schedules");

                entity.HasIndex(e => e.ScheduleId, "schedule_id_UNIQUE")
                    .IsUnique();

                entity.Property(e => e.ScheduleId)
                    .HasColumnType("binary(16)")
                    .HasColumnName("schedule_id");

                entity.Property(e => e.Status)
                    .HasColumnType("int")
                    .HasColumnName("status");
            });

            modelBuilder.Entity<SwapRequest>(entity =>
            {
                entity.HasKey(e => e.SwapRequestId)
                    .HasName("PRIMARY");

                entity.ToTable("swap_requests");

                entity.HasIndex(e => e.SwapRequestId, "idswap_requests_UNIQUE")
                    .IsUnique();

                entity.HasIndex(e => e.RequesteeId, "requestee_id_idx");

                entity.HasIndex(e => e.RequesterId, "requester_id_idx");

                entity.HasIndex(e => e.ScheduleId, "schedule_id_idx");

                entity.Property(e => e.SwapRequestId)
                    .HasColumnType("binary(16)")
                    .HasColumnName("idswap_requests");

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("created_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.Details)
                    .HasMaxLength(150)
                    .HasColumnName("details");

                entity.Property(e => e.RequesteeDate).HasColumnName("requestee_date");

                entity.Property(e => e.RequesteeId)
                    .HasMaxLength(15)
                    .HasColumnName("requestee_id");

                entity.Property(e => e.RequesterDate).HasColumnName("requester_date");

                entity.Property(e => e.RequesterId)
                    .HasMaxLength(15)
                    .HasColumnName("requester_id");

                entity.Property(e => e.ScheduleId)
                    .HasColumnType("binary(16)")
                    .HasColumnName("schedule_swap_id");

                entity.Property(e => e.Status)
                    .HasColumnType("int")
                    .HasColumnName("status");

                entity.Property(e => e.UpdatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("updated_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.HasOne(d => d.Requestee)
                    .WithMany(p => p.SwapRequestRequestees)
                    .HasForeignKey(d => d.RequesteeId)
                    .HasConstraintName("requestee_id");

                entity.HasOne(d => d.Requester)
                    .WithMany(p => p.SwapRequestRequesters)
                    .HasForeignKey(d => d.RequesterId)
                    .HasConstraintName("requester_id");

                entity.HasOne(d => d.ScheduleSwap)
                    .WithMany(p => p.SwapRequests)
                    .HasForeignKey(d => d.ScheduleId)
                    .HasConstraintName("schedule_swap_id");
            });

            modelBuilder.Entity<Vacation>(entity =>
            {
                entity.ToTable("vacations");

                entity.HasIndex(e => e.ResidentId, "idx_resident_id");

                entity.Property(e => e.VacationId)
                    .HasColumnType("binary(16)")
                    .HasColumnName("vacation_id");

                entity.Property(e => e.Date).HasColumnName("date");

                entity.Property(e => e.Details)
                    .HasMaxLength(255)
                    .HasColumnName("details");

                entity.Property(e => e.GroupId)
                    .HasMaxLength(36)
                    .HasColumnName("groupId");

                entity.Property(e => e.HalfDay)
                    .HasMaxLength(1)
                    .HasColumnName("half_day")
                    .IsFixedLength();

                entity.Property(e => e.Reason)
                    .HasMaxLength(45)
                    .HasColumnName("reason");

                entity.Property(e => e.ResidentId)
                    .HasMaxLength(15)
                    .HasColumnName("resident_id");

                entity.Property(e => e.Status)
                    .HasMaxLength(45)
                    .HasColumnName("status")
                    .HasDefaultValueSql("'Pending'");

                entity.HasOne(v => v.Resident)
                    .WithMany(p => p.Vacations)
                    .HasForeignKey(v => v.ResidentId)
                    .HasConstraintName("resident_id_vacations");
            });

            modelBuilder.Entity<RotationType>()
                .Property(v => v.RotationTypeId)
                .HasColumnType("binary(16)")
                .HasConversion(
                    v => v.ToByteArray(),
                    v => new Guid(v)
                );

            modelBuilder.Entity<RotationType>()
                .Property(e => e.PgyYearFlags)
                .HasConversion<int>()
                .HasColumnType("int")
                .HasDefaultValue(PgyYearFlags.NONE);

            modelBuilder.Entity<RotationPrefRequest>()
                .Property(v => v.RotationPrefRequestId)
                .HasColumnType("binary(16)")
                .HasConversion(
                    v => v.ToByteArray(),
                    v => new Guid(v)
                );

            modelBuilder.Entity<RotationPrefRequest>(entity =>
            {
                entity.Property(v => v.RotationPrefRequestId)
                    .HasColumnType("binary(16)")
                    .HasConversion(
                        v => v.ToByteArray(),
                        v => new Guid(v)
                    );

                entity.Property(v => v.FirstPriorityId)
                    .HasColumnType("binary(16)")
                    .HasConversion(
                        v => v.ToByteArray(),
                        v => new Guid(v)
                    );

                entity.Property(v => v.SecondPriorityId)
                   .HasColumnType("binary(16)")
                   .HasConversion(
                       v => v.ToByteArray(),
                       v => new Guid(v)
                   );

                entity.Property(v => v.ThirdPriorityId)
                   .HasColumnType("binary(16)")
                   .HasConversion(
                       v => v.ToByteArray(),
                       v => new Guid(v)
                   );

                entity.Property(v => v.FourthPriorityId)
                   .HasColumnType("binary(16)")
                   .HasConversion(
                       v => v.ToByteArray(),
                       v => new Guid(v)
                   );

                ValueConverter<Guid?, byte[]?> nullableGuidToBytesConverter =
                    new(
                        v => v.HasValue ? v.Value.ToByteArray() : null,
                        v => v == null ? null : new Guid(v)
                    );

                entity.Property(v => v.FifthPriorityId)
                   .HasColumnType("binary(16)")
                   .HasConversion(nullableGuidToBytesConverter);

                entity.Property(v => v.SixthPriorityId)
                   .HasColumnType("binary(16)")
                   .HasConversion(nullableGuidToBytesConverter);

                entity.Property(v => v.SeventhPriorityId)
                    .HasColumnType("binary(16)")
                    .HasConversion(nullableGuidToBytesConverter);

                entity.Property(v => v.EighthPriorityId)
                    .HasColumnType("binary(16)")
                    .HasConversion(nullableGuidToBytesConverter);

                entity.Property(v => v.FirstAlternativeId)
                    .HasColumnType("binary(16)")
                    .HasConversion(nullableGuidToBytesConverter);

                entity.Property(v => v.SecondAlternativeId)
                    .HasColumnType("binary(16)")
                    .HasConversion(nullableGuidToBytesConverter);

                entity.Property(v => v.ThirdAlternativeId)
                    .HasColumnType("binary(16)")
                    .HasConversion(nullableGuidToBytesConverter);

                entity.Property(v => v.FirstAvoidId)
                    .HasColumnType("binary(16)")
                    .HasConversion(nullableGuidToBytesConverter);

                entity.Property(v => v.SecondAvoidId)
                    .HasColumnType("binary(16)")
                    .HasConversion(nullableGuidToBytesConverter);

                entity.Property(v => v.ThirdAvoidId)
                    .HasColumnType("binary(16)")
                    .HasConversion(nullableGuidToBytesConverter);
            });


            OnModelCreatingPartial(modelBuilder);

            PopulateRotationTypeTable(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);

        private static void PopulateRotationTypeTable(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RotationType>().HasData(
                new RotationType
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
                new RotationType
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
                new RotationType
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
                new RotationType
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
                new RotationType
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
                new RotationType
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
                new RotationType
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
                new RotationType
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
                new RotationType
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
                new RotationType
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
                new RotationType
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
                new RotationType
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
                new RotationType
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
                new RotationType
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
                new RotationType
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
                new RotationType
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
                new RotationType
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
                new RotationType
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
                new RotationType
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
                new RotationType
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
                new RotationType
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
                new RotationType
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
                new RotationType
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
}