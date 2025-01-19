using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Fitness_Center.Models;

public partial class ModelContext : DbContext
{
    public ModelContext()
    {
    }

    public ModelContext(DbContextOptions<ModelContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Bankaccount> Bankaccounts { get; set; }

    public virtual DbSet<Member> Members { get; set; }

    public virtual DbSet<Membershipplan> Membershipplans { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<Report> Reports { get; set; }

    public virtual DbSet<Staticpage> Staticpages { get; set; }

    public virtual DbSet<Subscription> Subscriptions { get; set; }

    public virtual DbSet<Testimonial> Testimonials { get; set; }

    public virtual DbSet<Trainer> Trainers { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<Workout> Workouts { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseOracle("USER ID=C##BATOOL3;PASSWORD=Test3210;DATA SOURCE=localhost:1521");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .HasDefaultSchema("C##BATOOL3")
            .UseCollation("USING_NLS_COMP");

        modelBuilder.Entity<Bankaccount>(entity =>
        {
            entity.HasKey(e => e.BankAccountId).HasName("SYS_C008589");

            entity.ToTable("BANKACCOUNTS");

            entity.HasIndex(e => e.CardNumber, "SYS_C008590").IsUnique();

            entity.Property(e => e.BankAccountId)
                .ValueGeneratedOnAdd()
                .HasColumnType("NUMBER(38)")
                .HasColumnName("BANK_ACCOUNT_ID");
            entity.Property(e => e.Balance)
                .HasColumnType("NUMBER(10,2)")
                .HasColumnName("BALANCE");
            entity.Property(e => e.CardHolderName)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("CARD_HOLDER_NAME");
            entity.Property(e => e.CardNumber)
                .HasMaxLength(16)
                .IsUnicode(false)
                .HasColumnName("CARD_NUMBER");
            entity.Property(e => e.Cvv)
                .HasMaxLength(3)
                .IsUnicode(false)
                .HasColumnName("CVV");
            entity.Property(e => e.ExpiryDate)
                .HasColumnType("DATE")
                .HasColumnName("EXPIRY_DATE");
            entity.Property(e => e.MemberId)
                .HasColumnType("NUMBER(38)")
                .HasColumnName("MEMBER_ID");

            entity.HasOne(d => d.Member).WithMany(p => p.Bankaccounts)
                .HasForeignKey(d => d.MemberId)
                .HasConstraintName("SYS_C008591");
        });

        modelBuilder.Entity<Member>(entity =>
        {
            entity.HasKey(e => e.MemberId).HasName("SYS_C008550");

            entity.ToTable("MEMBERS");

            entity.HasIndex(e => e.UserId, "SYS_C008551").IsUnique();

            entity.Property(e => e.MemberId)
                .ValueGeneratedOnAdd()
                .HasColumnType("NUMBER(38)")
                .HasColumnName("MEMBER_ID");
            entity.Property(e => e.Address)
                .HasColumnType("CLOB")
                .HasColumnName("ADDRESS");
            entity.Property(e => e.ContactNumber)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("CONTACT_NUMBER");
            entity.Property(e => e.DateOfBirth)
                .HasColumnType("DATE")
                .HasColumnName("DATE_OF_BIRTH");
            entity.Property(e => e.FullName)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("FULL_NAME");
            entity.Property(e => e.ProfilePicture)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("PROFILE_PICTURE");
            entity.Property(e => e.SubscriptionId)
                .HasColumnType("NUMBER(38)")
                .HasColumnName("SUBSCRIPTION_ID");
            entity.Property(e => e.UserId)
                .HasColumnType("NUMBER(38)")
                .HasColumnName("USER_ID");

            entity.HasOne(d => d.Subscription).WithMany(p => p.Members)
                .HasForeignKey(d => d.SubscriptionId)
                .HasConstraintName("SYS_C008553");

            entity.HasOne(d => d.User).WithOne(p => p.Member)
                .HasForeignKey<Member>(d => d.UserId)
                .HasConstraintName("SYS_C008552");
        });

        modelBuilder.Entity<Membershipplan>(entity =>
        {
            entity.HasKey(e => e.PlanId).HasName("SYS_C008527");

            entity.ToTable("MEMBERSHIPPLANS");

            entity.Property(e => e.PlanId)
                .ValueGeneratedOnAdd()
                .HasColumnType("NUMBER(38)")
                .HasColumnName("PLAN_ID");
            entity.Property(e => e.Description)
                .HasColumnType("CLOB")
                .HasColumnName("DESCRIPTION");
            entity.Property(e => e.DurationMonths)
                .HasColumnType("NUMBER(38)")
                .HasColumnName("DURATION_MONTHS");
            entity.Property(e => e.PlanName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("PLAN_NAME");
            entity.Property(e => e.Price)
                .HasColumnType("NUMBER(10,2)")
                .HasColumnName("PRICE");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.PaymentId).HasName("SYS_C008573");

            entity.ToTable("PAYMENTS");

            entity.Property(e => e.PaymentId)
                .ValueGeneratedOnAdd()
                .HasColumnType("NUMBER(38)")
                .HasColumnName("PAYMENT_ID");
            entity.Property(e => e.Amount)
                .HasColumnType("NUMBER(10,2)")
                .HasColumnName("AMOUNT");
            entity.Property(e => e.PaymentMethod)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("PAYMENT_METHOD");
            entity.Property(e => e.SubscriptionId)
                .HasColumnType("NUMBER(38)")
                .HasColumnName("SUBSCRIPTION_ID");
            entity.Property(e => e.TransactionDate)
                .HasPrecision(6)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("TRANSACTION_DATE");

            entity.HasOne(d => d.Subscription).WithMany(p => p.Payments)
                .HasForeignKey(d => d.SubscriptionId)
                .HasConstraintName("SYS_C008574");
        });

        modelBuilder.Entity<Report>(entity =>
        {
            entity.HasKey(e => e.ReportId).HasName("SYS_C008532");

            entity.ToTable("REPORTS");

            entity.Property(e => e.ReportId)
                .ValueGeneratedOnAdd()
                .HasColumnType("NUMBER(38)")
                .HasColumnName("REPORT_ID");
            entity.Property(e => e.Content)
                .HasColumnType("CLOB")
                .HasColumnName("CONTENT");
            entity.Property(e => e.CreatedAt)
                .HasPrecision(6)
                .HasDefaultValueSql("CURRENT_TIMESTAMP\n")
                .HasColumnName("CREATED_AT");
            entity.Property(e => e.ReportType)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("REPORT_TYPE");
        });

        modelBuilder.Entity<Staticpage>(entity =>
        {
            entity.HasKey(e => e.PageId).HasName("SYS_C008536");

            entity.ToTable("STATICPAGES");

            entity.HasIndex(e => e.PageName, "SYS_C008537").IsUnique();

            entity.Property(e => e.PageId)
                .ValueGeneratedOnAdd()
                .HasColumnType("NUMBER(38)")
                .HasColumnName("PAGE_ID");
            entity.Property(e => e.Content)
                .HasColumnType("CLOB")
                .HasColumnName("CONTENT");
            entity.Property(e => e.PageName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("PAGE_NAME");
            entity.Property(e => e.UpdatedAt)
                .HasPrecision(6)
                .HasDefaultValueSql("CURRENT_TIMESTAMP\n")
                .HasColumnName("UPDATED_AT");
        });

        modelBuilder.Entity<Subscription>(entity =>
        {
            entity.HasKey(e => e.SubscriptionId).HasName("SYS_C008545");

            entity.ToTable("SUBSCRIPTIONS");

            entity.Property(e => e.SubscriptionId)
                .ValueGeneratedOnAdd()
                .HasColumnType("NUMBER(38)")
                .HasColumnName("SUBSCRIPTION_ID");
            entity.Property(e => e.EndDate)
                .HasColumnType("DATE")
                .HasColumnName("END_DATE");
            entity.Property(e => e.InvoicePdf)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("INVOICE_PDF");
            entity.Property(e => e.MemberId)
                .HasColumnType("NUMBER(38)")
                .HasColumnName("MEMBER_ID");
            entity.Property(e => e.PaymentDate)
                .HasPrecision(6)
                .HasColumnName("PAYMENT_DATE");
            entity.Property(e => e.PaymentStatus)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("PAYMENT_STATUS");
            entity.Property(e => e.PlanId)
                .HasColumnType("NUMBER(38)")
                .HasColumnName("PLAN_ID");
            entity.Property(e => e.StartDate)
                .HasColumnType("DATE")
                .HasColumnName("START_DATE");

            entity.HasOne(d => d.Member).WithMany(p => p.Subscriptions)
                .HasForeignKey(d => d.MemberId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SUBSCRIPTIONS_MEMBER_ID");

            entity.HasOne(d => d.Plan).WithMany(p => p.Subscriptions)
                .HasForeignKey(d => d.PlanId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("SYS_C008546");
        });

        modelBuilder.Entity<Testimonial>(entity =>
        {
            entity.HasKey(e => e.TestimonialId).HasName("SYS_C008566");

            entity.ToTable("TESTIMONIALS");

            entity.Property(e => e.TestimonialId)
                .ValueGeneratedOnAdd()
                .HasColumnType("NUMBER(38)")
                .HasColumnName("TESTIMONIAL_ID");
            entity.Property(e => e.Content)
                .HasColumnType("CLOB")
                .HasColumnName("CONTENT");
            entity.Property(e => e.CreatedAt)
                .HasPrecision(6)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("CREATED_AT");
            entity.Property(e => e.MemberId)
                .HasColumnType("NUMBER(38)")
                .HasColumnName("MEMBER_ID");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("STATUS");

            entity.HasOne(d => d.Member).WithMany(p => p.Testimonials)
                .HasForeignKey(d => d.MemberId)
                .HasConstraintName("SYS_C008567");
        });

        modelBuilder.Entity<Trainer>(entity =>
        {
            entity.HasKey(e => e.TrainerId).HasName("SYS_C008558");

            entity.ToTable("TRAINERS");

            entity.HasIndex(e => e.UserId, "SYS_C008559").IsUnique();

            entity.Property(e => e.TrainerId)
                .ValueGeneratedOnAdd()
                .HasColumnType("NUMBER(38)")
                .HasColumnName("TRAINER_ID");
            entity.Property(e => e.ContactNumber)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("CONTACT_NUMBER");
            entity.Property(e => e.ExperienceYears)
                .HasColumnType("NUMBER(38)")
                .HasColumnName("EXPERIENCE_YEARS");
            entity.Property(e => e.FullName)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("FULL_NAME");
            entity.Property(e => e.ProfilePicture)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("PROFILE_PICTURE");
            entity.Property(e => e.Specialization)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("SPECIALIZATION");
            entity.Property(e => e.UserId)
                .HasColumnType("NUMBER(38)")
                .HasColumnName("USER_ID");

            entity.HasOne(d => d.User).WithOne(p => p.Trainer)
                .HasForeignKey<Trainer>(d => d.UserId)
                .HasConstraintName("SYS_C008560");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("SYS_C008520");

            entity.ToTable("USERS");

            entity.HasIndex(e => e.Username, "SYS_C008521").IsUnique();

            entity.HasIndex(e => e.Email, "SYS_C008522").IsUnique();

            entity.Property(e => e.UserId)
                .ValueGeneratedOnAdd()
                .HasColumnType("NUMBER(38)")
                .HasColumnName("USER_ID");
            entity.Property(e => e.CreatedAt)
                .HasPrecision(6)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("CREATED_AT");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("EMAIL");
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("PASSWORD");
            entity.Property(e => e.Role)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("ROLE");
            entity.Property(e => e.UpdatedAt)
                .HasPrecision(6)
                .HasDefaultValueSql("CURRENT_TIMESTAMP\n")
                .HasColumnName("UPDATED_AT");
            entity.Property(e => e.Username)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("USERNAME");
        });

        modelBuilder.Entity<Workout>(entity =>
        {
            entity.HasKey(e => e.WorkoutId).HasName("SYS_C008579");

            entity.ToTable("WORKOUTS");

            entity.Property(e => e.WorkoutId)
                .ValueGeneratedOnAdd()
                .HasColumnType("NUMBER(38)")
                .HasColumnName("WORKOUT_ID");
            entity.Property(e => e.Description)
                .HasColumnType("CLOB")
                .HasColumnName("DESCRIPTION");
            entity.Property(e => e.EndDate)
                .HasColumnType("DATE")
                .HasColumnName("END_DATE");
            entity.Property(e => e.MemberId)
                .HasColumnType("NUMBER(38)")
                .HasColumnName("MEMBER_ID");
            entity.Property(e => e.PlanName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("PLAN_NAME");
            entity.Property(e => e.StartDate)
                .HasColumnType("DATE")
                .HasColumnName("START_DATE");
            entity.Property(e => e.TrainerId)
                .HasColumnType("NUMBER(38)")
                .HasColumnName("TRAINER_ID");

            entity.HasOne(d => d.Member).WithMany(p => p.Workouts)
                .HasForeignKey(d => d.MemberId)
                .HasConstraintName("SYS_C008581");

            entity.HasOne(d => d.Trainer).WithMany(p => p.Workouts)
                .HasForeignKey(d => d.TrainerId)
                .HasConstraintName("SYS_C008580");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
