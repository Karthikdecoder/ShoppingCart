using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShoppingCartAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddTable1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CategoryMasterTable",
                columns: table => new
                {
                    CategoryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CategoryName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<int>(type: "int", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoryMasterTable", x => x.CategoryId);
                });

            migrationBuilder.CreateTable(
                name: "RoleMasterTable",
                columns: table => new
                {
                    RoleId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<int>(type: "int", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleMasterTable", x => x.RoleId);
                });

            migrationBuilder.CreateTable(
                name: "RegistrationTable",
                columns: table => new
                {
                    RegistrationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Gender = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateOfBirth = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ContactNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PostalCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CategoryId = table.Column<int>(type: "int", nullable: true),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<int>(type: "int", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegistrationTable", x => x.RegistrationId);
                    table.ForeignKey(
                        name: "FK_RegistrationTable_CategoryMasterTable_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "CategoryMasterTable",
                        principalColumn: "CategoryId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserTable",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RegistrationId = table.Column<int>(type: "int", nullable: true),
                    RoleId = table.Column<int>(type: "int", nullable: true),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<int>(type: "int", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserTable", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_UserTable_RegistrationTable_RegistrationId",
                        column: x => x.RegistrationId,
                        principalTable: "RegistrationTable",
                        principalColumn: "RegistrationId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserTable_RoleMasterTable_RoleId",
                        column: x => x.RoleId,
                        principalTable: "RoleMasterTable",
                        principalColumn: "RoleId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RegistrationTable_CategoryId",
                table: "RegistrationTable",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_UserTable_RegistrationId",
                table: "UserTable",
                column: "RegistrationId");

            migrationBuilder.CreateIndex(
                name: "IX_UserTable_RoleId",
                table: "UserTable",
                column: "RoleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserTable");

            migrationBuilder.DropTable(
                name: "RegistrationTable");

            migrationBuilder.DropTable(
                name: "RoleMasterTable");

            migrationBuilder.DropTable(
                name: "CategoryMasterTable");
        }
    }
}
