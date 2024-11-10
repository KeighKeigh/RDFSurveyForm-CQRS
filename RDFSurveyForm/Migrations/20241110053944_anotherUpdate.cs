using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RDFSurveyForm.Migrations
{
    /// <inheritdoc />
    public partial class anotherUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Customer_CRole_RoleId",
                table: "Customer");

            migrationBuilder.DropForeignKey(
                name: "FK_Customer_Department_DepartmentId",
                table: "Customer");

            migrationBuilder.DropForeignKey(
                name: "FK_Customer_Groups_GroupsId",
                table: "Customer");

            migrationBuilder.DropForeignKey(
                name: "FK_Customer_Subunits_SubunitId",
                table: "Customer");

            migrationBuilder.DropForeignKey(
                name: "FK_Customer_Units_UnitId",
                table: "Customer");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Customer",
                table: "Customer");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CRole",
                table: "CRole");

            migrationBuilder.RenameTable(
                name: "Customer",
                newName: "Users");

            migrationBuilder.RenameTable(
                name: "CRole",
                newName: "Roles");

            migrationBuilder.RenameIndex(
                name: "IX_Customer_UnitId",
                table: "Users",
                newName: "IX_Users_UnitId");

            migrationBuilder.RenameIndex(
                name: "IX_Customer_SubunitId",
                table: "Users",
                newName: "IX_Users_SubunitId");

            migrationBuilder.RenameIndex(
                name: "IX_Customer_RoleId",
                table: "Users",
                newName: "IX_Users_RoleId");

            migrationBuilder.RenameIndex(
                name: "IX_Customer_GroupsId",
                table: "Users",
                newName: "IX_Users_GroupsId");

            migrationBuilder.RenameIndex(
                name: "IX_Customer_DepartmentId",
                table: "Users",
                newName: "IX_Users_DepartmentId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Users",
                table: "Users",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Roles",
                table: "Roles",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Department_DepartmentId",
                table: "Users",
                column: "DepartmentId",
                principalTable: "Department",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Groups_GroupsId",
                table: "Users",
                column: "GroupsId",
                principalTable: "Groups",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Roles_RoleId",
                table: "Users",
                column: "RoleId",
                principalTable: "Roles",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Subunits_SubunitId",
                table: "Users",
                column: "SubunitId",
                principalTable: "Subunits",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Units_UnitId",
                table: "Users",
                column: "UnitId",
                principalTable: "Units",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Department_DepartmentId",
                table: "Users");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Groups_GroupsId",
                table: "Users");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Roles_RoleId",
                table: "Users");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Subunits_SubunitId",
                table: "Users");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Units_UnitId",
                table: "Users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Users",
                table: "Users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Roles",
                table: "Roles");

            migrationBuilder.RenameTable(
                name: "Users",
                newName: "Customer");

            migrationBuilder.RenameTable(
                name: "Roles",
                newName: "CRole");

            migrationBuilder.RenameIndex(
                name: "IX_Users_UnitId",
                table: "Customer",
                newName: "IX_Customer_UnitId");

            migrationBuilder.RenameIndex(
                name: "IX_Users_SubunitId",
                table: "Customer",
                newName: "IX_Customer_SubunitId");

            migrationBuilder.RenameIndex(
                name: "IX_Users_RoleId",
                table: "Customer",
                newName: "IX_Customer_RoleId");

            migrationBuilder.RenameIndex(
                name: "IX_Users_GroupsId",
                table: "Customer",
                newName: "IX_Customer_GroupsId");

            migrationBuilder.RenameIndex(
                name: "IX_Users_DepartmentId",
                table: "Customer",
                newName: "IX_Customer_DepartmentId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Customer",
                table: "Customer",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CRole",
                table: "CRole",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Customer_CRole_RoleId",
                table: "Customer",
                column: "RoleId",
                principalTable: "CRole",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Customer_Department_DepartmentId",
                table: "Customer",
                column: "DepartmentId",
                principalTable: "Department",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Customer_Groups_GroupsId",
                table: "Customer",
                column: "GroupsId",
                principalTable: "Groups",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Customer_Subunits_SubunitId",
                table: "Customer",
                column: "SubunitId",
                principalTable: "Subunits",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Customer_Units_UnitId",
                table: "Customer",
                column: "UnitId",
                principalTable: "Units",
                principalColumn: "Id");
        }
    }
}
