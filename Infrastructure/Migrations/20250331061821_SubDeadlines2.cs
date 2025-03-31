using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SubDeadlines2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SubmissionDeadlines_ReportTemplateId",
                table: "SubmissionDeadlines");

            migrationBuilder.AddColumn<int>(
                name: "ReportTemplateId1",
                table: "SubmissionDeadlines",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SubmissionDeadlines_ReportTemplateId",
                table: "SubmissionDeadlines",
                column: "ReportTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_SubmissionDeadlines_ReportTemplateId1",
                table: "SubmissionDeadlines",
                column: "ReportTemplateId1",
                unique: true,
                filter: "[ReportTemplateId1] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_SubmissionDeadlines_ReportTemplates_ReportTemplateId1",
                table: "SubmissionDeadlines",
                column: "ReportTemplateId1",
                principalTable: "ReportTemplates",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SubmissionDeadlines_ReportTemplates_ReportTemplateId1",
                table: "SubmissionDeadlines");

            migrationBuilder.DropIndex(
                name: "IX_SubmissionDeadlines_ReportTemplateId",
                table: "SubmissionDeadlines");

            migrationBuilder.DropIndex(
                name: "IX_SubmissionDeadlines_ReportTemplateId1",
                table: "SubmissionDeadlines");

            migrationBuilder.DropColumn(
                name: "ReportTemplateId1",
                table: "SubmissionDeadlines");

            migrationBuilder.CreateIndex(
                name: "IX_SubmissionDeadlines_ReportTemplateId",
                table: "SubmissionDeadlines",
                column: "ReportTemplateId",
                unique: true);
        }
    }
}
