using Microsoft.EntityFrameworkCore;

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

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}