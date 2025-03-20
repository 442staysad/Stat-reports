using Microsoft.EntityFrameworkCore;
using Core.Entities;
using System;
using System.Collections.Generic;



    namespace Infrastructure.Data
    {
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Branch> Branches { get; set; }
        public DbSet<ReportTemplate> ReportTemplates { get; set; }
        public DbSet<Report> Reports { get; set; }
        public DbSet<SubmissionDeadline> SubmissionDeadlines { get; set; }
        public DbSet<SummaryReport> SummaryReports { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Уникальные ограничения

            modelBuilder.Entity<User>()
                .HasIndex(u => u.UserName)
                .IsUnique();

            modelBuilder.Entity<Branch>()
                .HasIndex(b => b.Name)
                .IsUnique();

            modelBuilder.Entity<ReportTemplate>()
                .HasIndex(rt => rt.Name)
                .IsUnique();

            // Связи
            modelBuilder.Entity<User>()
                .HasOne(u => u.Branch)
                .WithMany(b => b.Users)
                .HasForeignKey(u => u.BranchId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Report>()
                .HasOne(r => r.Template)
                .WithMany()
                .HasForeignKey(r => r.TemplateId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Report>()
                .HasOne(r => r.Branch)
                .WithMany(b => b.Reports)
                .HasForeignKey(r => r.BranchId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Report>()
                .HasOne(r => r.UploadedBy)
                .WithMany()
                .HasForeignKey(r => r.UploadedById)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<SubmissionDeadline>()
                .HasOne(sd => sd.Template)
                .WithMany()
                .HasForeignKey(sd => sd.ReportTemplateId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<SummaryReport>()
                .HasOne(sr => sr.ReportTemplate)
                .WithMany()
                .HasForeignKey(sr => sr.ReportTemplateId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<SummaryReport>()
                .HasMany(sr => sr.Reports)
                .WithOne()
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
