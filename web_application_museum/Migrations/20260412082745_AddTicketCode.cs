using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace web_application_museum.Migrations
{
    public partial class AddTicketCode : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Добавляем колонку только если её ещё нет
            migrationBuilder.Sql(@"
                ALTER TABLE tickets
                ADD COLUMN IF NOT EXISTS ""TicketCode"" character varying(32) NOT NULL DEFAULT '';
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TicketCode",
                table: "tickets");
        }
    }
}
