using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace HR_Recruitment.Models;

public partial class RecruitmentSystemContext : DbContext
{
    public RecruitmentSystemContext()
    {
    }

    public RecruitmentSystemContext(DbContextOptions<RecruitmentSystemContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Applicant> Applicants { get; set; }

    public virtual DbSet<ApplicantVacancy> ApplicantVacancies { get; set; }

    public virtual DbSet<Department> Departments { get; set; }

    public virtual DbSet<Employee> Employees { get; set; }

    public virtual DbSet<Interview> Interviews { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<User> Users { get; set; }
    public virtual DbSet<EmailVerificationOTP> EmailVerificationOTPs { get; set; }

    public virtual DbSet<Vacancy> Vacancies { get; set; }

    public virtual DbSet<Complaint> Complaints { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Applicant>(entity =>
        {
            entity.HasKey(e => e.ApplicantId).HasName("PK__Applican__39AE91A8E6D5EA1B");

            entity.HasIndex(e => e.UserId, "UQ__Applican__1788CC4DB8C18BFE").IsUnique();

            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.FullName).HasMaxLength(150);
            entity.Property(e => e.Phone).HasMaxLength(20);
            entity.Property(e => e.Status)
                .HasMaxLength(30)
                .HasDefaultValue("NotInProcess");

            entity.HasOne(d => d.User).WithOne(p => p.Applicant)
                .HasForeignKey<Applicant>(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Applicants_Users");
        });

        modelBuilder.Entity<ApplicantVacancy>(entity =>
        {
            entity.HasKey(e => e.ApplicantVacancyId).HasName("PK__Applican__364E1CB683FD5220");

            entity.HasIndex(e => new { e.ApplicantId, e.VacancyId }, "UQ_ApplicantVacancy").IsUnique();

            entity.Property(e => e.AppliedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Status)
                .HasMaxLength(30)
                .HasDefaultValue("Applied");

            entity.HasOne(d => d.Applicant).WithMany(p => p.ApplicantVacancies)
                .HasForeignKey(d => d.ApplicantId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ApplicantVacancies_Applicants");

            entity.HasOne(d => d.Vacancy).WithMany(p => p.ApplicantVacancies)
                .HasForeignKey(d => d.VacancyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ApplicantVacancies_Vacancies");
        });

        modelBuilder.Entity<Department>(entity =>
        {
            entity.HasKey(e => e.DepartmentId).HasName("PK__Departme__B2079BEDC2D7D8F0");

            entity.HasIndex(e => e.DepartmentName, "UQ__Departme__D949CC34867DBDC7").IsUnique();

            entity.Property(e => e.DepartmentName).HasMaxLength(100);
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.EmployeeId).HasName("PK__Employee__7AD04F11C94449B3");

            entity.HasIndex(e => e.UserId, "UQ__Employee__1788CC4D9456B8AA").IsUnique();

            entity.HasIndex(e => e.EmployeeCode, "UQ__Employee__1F6425483BC13252").IsUnique();

            entity.Property(e => e.EmployeeCode).HasMaxLength(50);
            entity.Property(e => e.FullName).HasMaxLength(150);

            entity.HasOne(d => d.Department).WithMany(p => p.Employees)
                .HasForeignKey(d => d.DepartmentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Employees_Departments");

            entity.HasOne(d => d.User).WithOne(p => p.Employee)
                .HasForeignKey<Employee>(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Employees_Users");
        });

        modelBuilder.Entity<Interview>(entity =>
        {
            entity.HasKey(e => e.InterviewId).HasName("PK__Intervie__C97C585237B4E9C7");

            entity.Property(e => e.Result)
                .HasMaxLength(30)
                .HasDefaultValue("Pending");

            entity.HasOne(d => d.ApplicantVacancy).WithMany(p => p.Interviews)
                .HasForeignKey(d => d.ApplicantVacancyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Interviews_ApplicantVacancies");

            entity.HasOne(d => d.InterviewerEmployee).WithMany(p => p.Interviews)
                .HasForeignKey(d => d.InterviewerEmployeeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Interviews_Employees");
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.NotificationId).HasName("PK__Notifica__20CF2E127EFE8138");

            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Message).HasMaxLength(500);

            entity.HasOne(d => d.User).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Notifications_Users");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__Roles__8AFACE1AC4C1A37E");

            entity.HasIndex(e => e.RoleName, "UQ__Roles__8A2B6160FD7406BF").IsUnique();

            entity.Property(e => e.RoleName).HasMaxLength(50);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CC4CDA80FD45");

            entity.HasIndex(e => e.Email, "UQ__Users__A9D105340C08B810").IsUnique();

            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Email).HasMaxLength(150);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.PasswordHash).HasMaxLength(255);

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Users_Roles");
        });

        modelBuilder.Entity<EmailVerificationOTP>(entity =>
        {
            entity.HasKey(e => e.OTPId);

            entity.Property(e => e.OTPCode)
                .HasMaxLength(10)
                .IsRequired();

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())");

            entity.Property(e => e.ExpiryDate)
                .HasColumnType("datetime");

            entity.HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });




        modelBuilder.Entity<Vacancy>(entity =>
        {
            entity.HasKey(e => e.VacancyId).HasName("PK__Vacancie__6456763F7924A088");

            entity.Property(e => e.CloseDate).HasColumnType("datetime");
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.ImagePath)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Status)
                .HasMaxLength(30)
                .HasDefaultValue("Open");
            entity.Property(e => e.Title).HasMaxLength(150);

            entity.HasOne(d => d.CreatedByEmployee).WithMany(p => p.Vacancies)
                .HasForeignKey(d => d.CreatedByEmployeeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Vacancies_Employees");

            entity.HasOne(d => d.Department).WithMany(p => p.Vacancies)
                .HasForeignKey(d => d.DepartmentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Vacancies_Departments");
        });

        modelBuilder.Entity<Complaint>(entity =>
        {
            entity.HasKey(e => e.ComplaintId)
                  .HasName("PK_Complaints");

            entity.Property(e => e.FullName)
                  .HasMaxLength(150)
                  .IsRequired();

            entity.Property(e => e.Email)
                  .HasMaxLength(150)
                  .IsRequired();

            entity.Property(e => e.Subject)
                  .HasMaxLength(200)
                  .IsRequired();

            entity.Property(e => e.Details)
                  .IsRequired();

            entity.Property(e => e.CreatedAt)
                  .HasDefaultValueSql("(getdate())")
                  .HasColumnType("datetime");
        });


        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
