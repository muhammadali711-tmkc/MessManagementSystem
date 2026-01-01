using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace MessManagementSystem.Models;

public partial class MessDbContext : DbContext
{
    public MessDbContext()
    {
    }

    public MessDbContext(DbContextOptions<MessDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Attendance> Attendances { get; set; }

    public virtual DbSet<Bill> Bills { get; set; }

    public virtual DbSet<DailyMenu> DailyMenus { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<SystemConfig> SystemConfigs { get; set; }

    public virtual DbSet<User> Users { get; set; }

  
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Attendance>(entity =>
        {
            entity.HasKey(e => e.AttendanceId).HasName("PK__Attendan__8B69261C183E4A6B");

            entity.ToTable("Attendance");

            entity.Property(e => e.IsPresent).HasDefaultValue(false);
            entity.Property(e => e.RecordedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.User).WithMany(p => p.Attendances)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Attendanc__UserI__4222D4EF");
        });

        modelBuilder.Entity<Bill>(entity =>
        {
            entity.HasKey(e => e.BillId).HasName("PK__Bills__11F2FC6A25756FA5");

            entity.Property(e => e.FoodAmount)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 2)");
            entity.Property(e => e.GeneratedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.IsPaid).HasDefaultValue(false);
            entity.Property(e => e.PreviousArrears)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TotalAmount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.WaterAmount)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.User).WithMany(p => p.Bills)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Bills__UserId__46E78A0C");
        });

        modelBuilder.Entity<DailyMenu>(entity =>
        {
            entity.HasKey(e => e.MenuId).HasName("PK__DailyMen__C99ED230FB55ADB9");

            entity.Property(e => e.BreakfastItem).HasMaxLength(100);
            entity.Property(e => e.DailyRate).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.DinnerItem).HasMaxLength(100);
            entity.Property(e => e.LunchItem).HasMaxLength(100);
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.PaymentId).HasName("PK__Payments__9B556A38FCFF4410");

            entity.Property(e => e.AmountPaid).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.PaymentDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.TransactionToken).HasMaxLength(100);

            entity.HasOne(d => d.Bill).WithMany(p => p.Payments)
                .HasForeignKey(d => d.BillId)
                .HasConstraintName("FK__Payments__BillId__4F7CD00D");

            entity.HasOne(d => d.User).WithMany(p => p.Payments)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Payments__UserId__4E88ABD4");
        });

        modelBuilder.Entity<SystemConfig>(entity =>
        {
            entity.HasKey(e => e.ConfigId).HasName("PK__SystemCo__C3BC335CC99F3322");

            entity.ToTable("SystemConfig");

            entity.HasIndex(e => e.KeyName, "UQ__SystemCo__F0A2A33771D9541F").IsUnique();

            entity.Property(e => e.Description).HasMaxLength(200);
            entity.Property(e => e.KeyName).HasMaxLength(50);
            entity.Property(e => e.Value).HasColumnType("decimal(18, 2)");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CC4C9D9ADD0B");

            entity.HasIndex(e => e.Email, "UQ__Users__A9D10534661288F1").IsUnique();

            entity.Property(e => e.CurrentBalance)
                .HasDefaultValue(0.00m)
                .HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.FullName).HasMaxLength(100);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.IsFirstLogin).HasDefaultValue(true);
            entity.Property(e => e.Password).HasMaxLength(255);
            entity.Property(e => e.Role).HasMaxLength(20);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
