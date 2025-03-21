using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Deadlines : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reports_ReportTemplates_TemplateId",
                table: "Reports");

            migrationBuilder.DropIndex(
                name: "IX_SubmissionDeadlines_ReportTemplateId",
                table: "SubmissionDeadlines");

            migrationBuilder.DropColumn(
                name: "Deadline",
                table: "SubmissionDeadlines");

            migrationBuilder.DropColumn(
                name: "NotificationSent",
                table: "SubmissionDeadlines");

            migrationBuilder.DropColumn(
                name: "PeriodEnd",
                table: "ReportTemplates");

            migrationBuilder.DropColumn(
                name: "PeriodStart",
                table: "ReportTemplates");

            migrationBuilder.AddColumn<int>(
                name: "DeadlineType",
                table: "SubmissionDeadlines",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "FixedDay",
                table: "SubmissionDeadlines",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DeadlineId",
                table: "ReportTemplates",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SubmissionDeadlines_ReportTemplateId",
                table: "SubmissionDeadlines",
                column: "ReportTemplateId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Reports_ReportTemplates_TemplateId",
                table: "Reports",
                column: "TemplateId",
                principalTable: "ReportTemplates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reports_ReportTemplates_TemplateId",
                table: "Reports");

            migrationBuilder.DropIndex(
                name: "IX_SubmissionDeadlines_ReportTemplateId",
                table: "SubmissionDeadlines");

            migrationBuilder.DropColumn(
                name: "DeadlineType",
                table: "SubmissionDeadlines");

            migrationBuilder.DropColumn(
                name: "FixedDay",
                table: "SubmissionDeadlines");

            migrationBuilder.DropColumn(
                name: "DeadlineId",
                table: "ReportTemplates");

            migrationBuilder.AddColumn<DateTime>(
                name: "Deadline",
                table: "SubmissionDeadlines",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "NotificationSent",
                table: "SubmissionDeadlines",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "PeriodEnd",
                table: "ReportTemplates",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "PeriodStart",
                table: "ReportTemplates",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateIndex(
                name: "IX_SubmissionDeadlines_ReportTemplateId",
                table: "SubmissionDeadlines",
                column: "ReportTemplateId");

            migrationBuilder.AddForeignKey(
                name: "FK_Reports_ReportTemplates_TemplateId",
                table: "Reports",
                column: "TemplateId",
                principalTable: "ReportTemplates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
