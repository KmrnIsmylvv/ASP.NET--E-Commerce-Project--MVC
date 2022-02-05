using Microsoft.EntityFrameworkCore.Migrations;

namespace MVC__E_Commerce_Project.Migrations
{
    public partial class addSubscribeColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsSubscribe",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsSubscribe",
                table: "AspNetUsers");
        }
    }
}
