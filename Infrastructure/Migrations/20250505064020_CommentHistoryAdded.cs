using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CommentHistoryAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ReportAccess");

            migrationBuilder.DropColumn(
                name: "Comment",
                table: "SubmissionDeadlines");

            migrationBuilder.DropColumn(
                name: "ReportStatusId",
                table: "SubmissionDeadlines");

            migrationBuilder.DropColumn(
                name: "Comment",
                table: "Reports");

            migrationBuilder.AddColumn<bool>(
                name: "IsClosed",
                table: "Reports",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "CommentHistory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReportId = table.Column<int>(type: "int", nullable: true),
                    DeadlineId = table.Column<int>(type: "int", nullable: true),
                    Comment = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    AuthorId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommentHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CommentHistory_Reports_ReportId",
                        column: x => x.ReportId,
                        principalTable: "Reports",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CommentHistory_SubmissionDeadlines_DeadlineId",
                        column: x => x.DeadlineId,
                        principalTable: "SubmissionDeadlines",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CommentHistory_Users_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_CommentHistory_AuthorId",
                table: "CommentHistory",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_CommentHistory_DeadlineId",
                table: "CommentHistory",
                column: "DeadlineId");

            migrationBuilder.CreateIndex(
                name: "IX_CommentHistory_ReportId",
                table: "CommentHistory",
                column: "ReportId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CommentHistory");

            migrationBuilder.DropColumn(
                name: "IsClosed",
                table: "Reports");

            migrationBuilder.AddColumn<string>(
                name: "Comment",
                table: "SubmissionDeadlines",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ReportStatusId",
                table: "SubmissionDeadlines",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Comment",
                table: "Reports",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ReportAccess",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReportId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    AccessLevel = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportAccess", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReportAccess_Reports_ReportId",
                        column: x => x.ReportId,
                        principalTable: "Reports",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ReportAccess_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ReportAccess_ReportId",
                table: "ReportAccess",
                column: "ReportId");

            migrationBuilder.CreateIndex(
                name: "IX_ReportAccess_UserId",
                table: "ReportAccess",
                column: "UserId");
        }
    }
}
