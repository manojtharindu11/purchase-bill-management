using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PurchaseBillManagement.Api.Migrations
{
    /// <inheritdoc />
    public partial class RemoveBillLevelBatchLocation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BatchLocationName",
                table: "Purchase_Bills");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BatchLocationName",
                table: "Purchase_Bills",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");
        }
    }
}
