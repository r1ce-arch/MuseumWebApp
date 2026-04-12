using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace web_application_museum.Migrations
{
    public partial class FixTicketCodeColumnName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Переименовываем ticketcode -> TicketCode если нужно (безопасно)
            migrationBuilder.Sql(@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns
                        WHERE table_name = 'tickets' AND column_name = 'ticketcode'
                    ) THEN
                        ALTER TABLE tickets RENAME COLUMN ticketcode TO ""TicketCode"";
                    END IF;
                END $$;
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Ничего не делаем при откате
        }
    }
}
