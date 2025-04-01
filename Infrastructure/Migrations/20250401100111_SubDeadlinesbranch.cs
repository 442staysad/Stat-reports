using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SubDeadlinesbranch : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BranchId",
                table: "SubmissionDeadlines",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SubmissionDeadlines_BranchId",
                table: "SubmissionDeadlines",
                column: "BranchId");

            migrationBuilder.AddForeignKey(
                name: "FK_SubmissionDeadlines_Branches_BranchId",
                table: "SubmissionDeadlines",
                column: "BranchId",
                principalTable: "Branches",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SubmissionDeadlines_Branches_BranchId",
                table: "SubmissionDeadlines");

            migrationBuilder.DropIndex(
                name: "IX_SubmissionDeadlines_BranchId",
                table: "SubmissionDeadlines");

            migrationBuilder.DropColumn(
                name: "BranchId",
                table: "SubmissionDeadlines");
        }
    }
}
