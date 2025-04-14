using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RoleIdAdded2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_SystemRole_RoleId",
                table: "Users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SystemRole",
                table: "SystemRole");

            migrationBuilder.RenameTable(
                name: "SystemRole",
                newName: "SystemRoles");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SystemRoles",
                table: "SystemRoles",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_SystemRoles_RoleId",
                table: "Users",
                column: "RoleId",
                principalTable: "SystemRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_SystemRoles_RoleId",
                table: "Users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SystemRoles",
                table: "SystemRoles");

            migrationBuilder.RenameTable(
                name: "SystemRoles",
                newName: "SystemRole");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SystemRole",
                table: "SystemRole",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_SystemRole_RoleId",
                table: "Users",
                column: "RoleId",
                principalTable: "SystemRole",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
