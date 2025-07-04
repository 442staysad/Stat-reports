﻿// <auto-generated />
using System;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Infrastructure.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20250320094008_Nullablecomment")]
    partial class Nullablecomment
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Core.Entities.Branch", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Adres")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ChiefAccountant")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("GoverningName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("HeadName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("OKPO")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("OKYLP")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Region")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Shortname")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Supervisor")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UNP")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("Branches");
                });

            modelBuilder.Entity("Core.Entities.Report", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("BranchId")
                        .HasColumnType("int");

                    b.Property<string>("Comment")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FilePath")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.Property<int?>("SummaryReportId")
                        .HasColumnType("int");

                    b.Property<int>("TemplateId")
                        .HasColumnType("int");

                    b.Property<DateTime>("UploadDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("UploadedById")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("BranchId");

                    b.HasIndex("SummaryReportId");

                    b.HasIndex("TemplateId");

                    b.HasIndex("UploadedById");

                    b.ToTable("Reports");
                });

            modelBuilder.Entity("Core.Entities.ReportAccess", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("AccessLevel")
                        .HasColumnType("int");

                    b.Property<int>("ReportId")
                        .HasColumnType("int");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("ReportId");

                    b.HasIndex("UserId");

                    b.ToTable("ReportAccess");
                });

            modelBuilder.Entity("Core.Entities.ReportTemplate", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Fields")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime>("PeriodEnd")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("PeriodStart")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("ReportTemplates");
                });

            modelBuilder.Entity("Core.Entities.SubmissionDeadline", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("Deadline")
                        .HasColumnType("datetime2");

                    b.Property<bool>("NotificationSent")
                        .HasColumnType("bit");

                    b.Property<int>("ReportTemplateId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("ReportTemplateId");

                    b.ToTable("SubmissionDeadlines");
                });

            modelBuilder.Entity("Core.Entities.SummaryReport", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("FilePath")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("PeriodEnd")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("PeriodStart")
                        .HasColumnType("datetime2");

                    b.Property<int>("ReportTemplateId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("ReportTemplateId");

                    b.ToTable("SummaryReports");
                });

            modelBuilder.Entity("Core.Entities.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("AccessId")
                        .HasColumnType("int");

                    b.Property<int?>("BranchId")
                        .HasColumnType("int");

                    b.Property<string>("PasswordHash")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Role")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("BranchId");

                    b.HasIndex("UserName")
                        .IsUnique();

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Core.Entities.Report", b =>
                {
                    b.HasOne("Core.Entities.Branch", "Branch")
                        .WithMany("Reports")
                        .HasForeignKey("BranchId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Core.Entities.SummaryReport", null)
                        .WithMany("Reports")
                        .HasForeignKey("SummaryReportId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Core.Entities.ReportTemplate", "Template")
                        .WithMany()
                        .HasForeignKey("TemplateId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Core.Entities.User", "UploadedBy")
                        .WithMany()
                        .HasForeignKey("UploadedById")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Branch");

                    b.Navigation("Template");

                    b.Navigation("UploadedBy");
                });

            modelBuilder.Entity("Core.Entities.ReportAccess", b =>
                {
                    b.HasOne("Core.Entities.Report", "Report")
                        .WithMany("Accesses")
                        .HasForeignKey("ReportId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Core.Entities.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Report");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Core.Entities.SubmissionDeadline", b =>
                {
                    b.HasOne("Core.Entities.ReportTemplate", "Template")
                        .WithMany()
                        .HasForeignKey("ReportTemplateId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Template");
                });

            modelBuilder.Entity("Core.Entities.SummaryReport", b =>
                {
                    b.HasOne("Core.Entities.ReportTemplate", "ReportTemplate")
                        .WithMany()
                        .HasForeignKey("ReportTemplateId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("ReportTemplate");
                });

            modelBuilder.Entity("Core.Entities.User", b =>
                {
                    b.HasOne("Core.Entities.Branch", "Branch")
                        .WithMany("Users")
                        .HasForeignKey("BranchId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.Navigation("Branch");
                });

            modelBuilder.Entity("Core.Entities.Branch", b =>
                {
                    b.Navigation("Reports");

                    b.Navigation("Users");
                });

            modelBuilder.Entity("Core.Entities.Report", b =>
                {
                    b.Navigation("Accesses");
                });

            modelBuilder.Entity("Core.Entities.SummaryReport", b =>
                {
                    b.Navigation("Reports");
                });
#pragma warning restore 612, 618
        }
    }
}
