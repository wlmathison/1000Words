using Microsoft.EntityFrameworkCore.Migrations;

namespace _1000Words.Migrations
{
    public partial class removeIsFavorite : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsFavorite",
                table: "Photos");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "00000000-ffff-ffff-ffff-ffffffffffff",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "81bbf2a9-c8cf-462f-9116-508b82e54fc2", "AQAAAAEAACcQAAAAENz7lSEZEozBCTAsx5VuKRE+jpT+ox4GvhDTvRet41sk377+WLHugXodSlThbHczBw==" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsFavorite",
                table: "Photos",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "00000000-ffff-ffff-ffff-ffffffffffff",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "a2cf6e39-1041-436e-a4a6-9b059e14c4dd", "AQAAAAEAACcQAAAAED3Q58bugikF8SjnE38RictnBsrUz8Xmzmpg7wAWuZ979WCbkL/Re59eNf0HTeu7cA==" });
        }
    }
}
