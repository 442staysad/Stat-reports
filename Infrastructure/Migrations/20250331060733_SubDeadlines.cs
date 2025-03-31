using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SubDeadlines : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reports_ReportStatus_ReportStatusId",
                table: "Reports");

            migrationBuilder.DropTable(
                name: "ReportStatus");

            migrationBuilder.DropIndex(
                name: "IX_SubmissionDeadlines_ReportTemplateId",
                table: "SubmissionDeadlines");

            migrationBuilder.DropIndex(
                name: "IX_Reports_ReportStatusId",
                table: "Reports");

            migrationBuilder.AlterColumn<int>(
                name: "ReportTemplateId",
                table: "SubmissionDeadlines",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Comment",
                table: "SubmissionDeadlines",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeadlineDate",
                table: "SubmissionDeadlines",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsClosed",
                table: "SubmissionDeadlines",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Reports",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SubmissionDeadlines_ReportTemplateId",
                table: "SubmissionDeadlines",
                column: "ReportTemplateId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SubmissionDeadlines_ReportTemplateId",
                table: "SubmissionDeadlines");

            migrationBuilder.DropColumn(
                name: "Comment",
                table: "SubmissionDeadlines");

            migrationBuilder.DropColumn(
                name: "DeadlineDate",
                table: "SubmissionDeadlines");

            migrationBuilder.DropColumn(
                name: "IsClosed",
                table: "SubmissionDeadlines");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Reports");

            migrationBuilder.AlterColumn<int>(
                name: "ReportTemplateId",
                table: "SubmissionDeadlines",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateTable(
                name: "ReportStatus",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StatusName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportStatus", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SubmissionDeadlines_ReportTemplateId",
                table: "SubmissionDeadlines",
                column: "ReportTemplateId",
                unique: true,
                filter: "[ReportTemplateId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Reports_ReportStatusId",
                table: "Reports",
                column: "ReportStatusId");

            migrationBuilder.AddForeignKey(
                name: "FK_Reports_ReportStatus_ReportStatusId",
                table: "Reports",
                column: "ReportStatusId",
                principalTable: "ReportStatus",
                principalColumn: "Id");
        }
    }
}
