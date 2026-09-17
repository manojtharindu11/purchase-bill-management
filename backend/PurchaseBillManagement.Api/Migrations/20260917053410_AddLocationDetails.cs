using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PurchaseBillManagement.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddLocationDetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Location_Details",
                columns: table => new
                {
                    Location_Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Location_Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Location_Details", x => x.Location_Code);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Location_Details");
        }
    }
}
