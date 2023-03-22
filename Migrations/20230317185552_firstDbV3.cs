using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace webapiV2.Migrations
{
    /// <inheritdoc />
    public partial class firstDbV3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RefreshToken_accounts_Accountid",
                table: "RefreshToken");

            migrationBuilder.DropPrimaryKey(
                name: "PK_accounts",
                table: "accounts");

            migrationBuilder.RenameTable(
                name: "accounts",
                newName: "Accounts");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Accounts",
                table: "Accounts",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_RefreshToken_Accounts_Accountid",
                table: "RefreshToken",
                column: "Accountid",
                principalTable: "Accounts",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RefreshToken_Accounts_Accountid",
                table: "RefreshToken");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Accounts",
                table: "Accounts");

            migrationBuilder.RenameTable(
                name: "Accounts",
                newName: "accounts");

            migrationBuilder.AddPrimaryKey(
                name: "PK_accounts",
                table: "accounts",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_RefreshToken_accounts_Accountid",
                table: "RefreshToken",
                column: "Accountid",
                principalTable: "accounts",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
