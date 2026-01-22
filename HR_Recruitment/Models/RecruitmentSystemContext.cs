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

    public virtual DbSet<Vacancy> Vacancies { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=LAB8-203;Database=RecruitmentSystem;;User Id=sa;Password=aptech;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Applicant>(entity =>
        {
            entity.HasKey(e => e.ApplicantId).HasName("PK__Applican__39AE91A8651CA644");

            entity.HasIndex(e => e.UserId, "UQ__Applican__1788CC4DB5BA7BB8").IsUnique();

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
            entity.HasKey(e => e.ApplicantVacancyId).HasName("PK__Applican__364E1CB66E6EDACB");

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
            entity.HasKey(e => e.DepartmentId).HasName("PK__Departme__B2079BED7C520C49");

            entity.HasIndex(e => e.DepartmentName, "UQ__Departme__D949CC3499D04071").IsUnique();

            entity.Property(e => e.DepartmentName).HasMaxLength(100);
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.EmployeeId).HasName("PK__Employee__7AD04F1189AAF9AF");

            entity.HasIndex(e => e.UserId, "UQ__Employee__1788CC4D40596B04").IsUnique();

            entity.HasIndex(e => e.EmployeeCode, "UQ__Employee__1F642548CBE4DD0F").IsUnique();

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
            entity.HasKey(e => e.InterviewId).HasName("PK__Intervie__C97C5852BBF68045");

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
            entity.HasKey(e => e.NotificationId).HasName("PK__Notifica__20CF2E1210E1CDCA");

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
            entity.HasKey(e => e.RoleId).HasName("PK__Roles__8AFACE1A72183372");

            entity.HasIndex(e => e.RoleName, "UQ__Roles__8A2B616090A06534").IsUnique();

            entity.Property(e => e.RoleName).HasMaxLength(50);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CC4C4E863172");

            entity.HasIndex(e => e.Email, "UQ__Users__A9D10534F145FF14").IsUnique();

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

        modelBuilder.Entity<Vacancy>(entity =>
        {
            entity.HasKey(e => e.VacancyId).HasName("PK__Vacancie__6456763F67B5C048");

            entity.Property(e => e.CloseDate).HasColumnType("datetime");
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
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

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
