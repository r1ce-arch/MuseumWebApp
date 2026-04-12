using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace web_application_museum.Migrations
{
    /// <inheritdoc />
    public partial class AddVisitorAndFixTableNames : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Employees_Positions_PositionId",
                table: "Employees");

            migrationBuilder.DropForeignKey(
                name: "FK_Tickets_TourSchedules_TourScheduleId",
                table: "Tickets");

            migrationBuilder.DropForeignKey(
                name: "FK_TourExhibits_Exhibits_ExhibitId",
                table: "TourExhibits");

            migrationBuilder.DropForeignKey(
                name: "FK_TourExhibits_Tours_TourId",
                table: "TourExhibits");

            migrationBuilder.DropForeignKey(
                name: "FK_TourSchedules_Employees_GuideId",
                table: "TourSchedules");

            migrationBuilder.DropForeignKey(
                name: "FK_TourSchedules_Tours_TourId",
                table: "TourSchedules");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Tours",
                table: "Tours");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Tickets",
                table: "Tickets");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Positions",
                table: "Positions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Exhibits",
                table: "Exhibits");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Employees",
                table: "Employees");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TourSchedules",
                table: "TourSchedules");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TourExhibits",
                table: "TourExhibits");

            migrationBuilder.DropColumn(
                name: "VisitorName",
                table: "Tickets");

            migrationBuilder.RenameTable(
                name: "Tours",
                newName: "tours");

            migrationBuilder.RenameTable(
                name: "Tickets",
                newName: "tickets");

            migrationBuilder.RenameTable(
                name: "Positions",
                newName: "positions");

            migrationBuilder.RenameTable(
                name: "Exhibits",
                newName: "exhibits");

            migrationBuilder.RenameTable(
                name: "Employees",
                newName: "employees");

            migrationBuilder.RenameTable(
                name: "TourSchedules",
                newName: "tour_schedules");

            migrationBuilder.RenameTable(
                name: "TourExhibits",
                newName: "tour_exhibits");

            migrationBuilder.RenameIndex(
                name: "IX_Tickets_TourScheduleId",
                table: "tickets",
                newName: "IX_tickets_TourScheduleId");

            migrationBuilder.RenameIndex(
                name: "IX_Employees_PositionId",
                table: "employees",
                newName: "IX_employees_PositionId");

            migrationBuilder.RenameIndex(
                name: "IX_Employees_Login",
                table: "employees",
                newName: "IX_employees_Login");

            migrationBuilder.RenameIndex(
                name: "IX_TourSchedules_TourId",
                table: "tour_schedules",
                newName: "IX_tour_schedules_TourId");

            migrationBuilder.RenameIndex(
                name: "IX_TourSchedules_GuideId",
                table: "tour_schedules",
                newName: "IX_tour_schedules_GuideId");

            migrationBuilder.RenameIndex(
                name: "IX_TourExhibits_ExhibitId",
                table: "tour_exhibits",
                newName: "IX_tour_exhibits_ExhibitId");

            migrationBuilder.AlterColumn<DateTime>(
                name: "SaleDate",
                table: "tickets",
                type: "timestamp without time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AddColumn<int>(
                name: "VisitorId",
                table: "tickets",
                type: "integer",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PhotoPath",
                table: "exhibits",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(300)",
                oldMaxLength: 300,
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "HireDate",
                table: "employees",
                type: "timestamp without time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<DateTime>(
                name: "StartTime",
                table: "tour_schedules",
                type: "timestamp without time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AddPrimaryKey(
                name: "PK_tours",
                table: "tours",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_tickets",
                table: "tickets",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_positions",
                table: "positions",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_exhibits",
                table: "exhibits",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_employees",
                table: "employees",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_tour_schedules",
                table: "tour_schedules",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_tour_exhibits",
                table: "tour_exhibits",
                columns: new[] { "TourId", "ExhibitId" });

            migrationBuilder.CreateTable(
                name: "visitors",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FullName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: false),
                    RegistrationDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_visitors", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_tickets_VisitorId",
                table: "tickets",
                column: "VisitorId");

            migrationBuilder.CreateIndex(
                name: "IX_visitors_Email",
                table: "visitors",
                column: "Email",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_employees_positions_PositionId",
                table: "employees",
                column: "PositionId",
                principalTable: "positions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_tickets_tour_schedules_TourScheduleId",
                table: "tickets",
                column: "TourScheduleId",
                principalTable: "tour_schedules",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_tickets_visitors_VisitorId",
                table: "tickets",
                column: "VisitorId",
                principalTable: "visitors",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_tour_exhibits_exhibits_ExhibitId",
                table: "tour_exhibits",
                column: "ExhibitId",
                principalTable: "exhibits",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_tour_exhibits_tours_TourId",
                table: "tour_exhibits",
                column: "TourId",
                principalTable: "tours",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_tour_schedules_employees_GuideId",
                table: "tour_schedules",
                column: "GuideId",
                principalTable: "employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_tour_schedules_tours_TourId",
                table: "tour_schedules",
                column: "TourId",
                principalTable: "tours",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_employees_positions_PositionId",
                table: "employees");

            migrationBuilder.DropForeignKey(
                name: "FK_tickets_tour_schedules_TourScheduleId",
                table: "tickets");

            migrationBuilder.DropForeignKey(
                name: "FK_tickets_visitors_VisitorId",
                table: "tickets");

            migrationBuilder.DropForeignKey(
                name: "FK_tour_exhibits_exhibits_ExhibitId",
                table: "tour_exhibits");

            migrationBuilder.DropForeignKey(
                name: "FK_tour_exhibits_tours_TourId",
                table: "tour_exhibits");

            migrationBuilder.DropForeignKey(
                name: "FK_tour_schedules_employees_GuideId",
                table: "tour_schedules");

            migrationBuilder.DropForeignKey(
                name: "FK_tour_schedules_tours_TourId",
                table: "tour_schedules");

            migrationBuilder.DropTable(
                name: "visitors");

            migrationBuilder.DropPrimaryKey(
                name: "PK_tours",
                table: "tours");

            migrationBuilder.DropPrimaryKey(
                name: "PK_tickets",
                table: "tickets");

            migrationBuilder.DropIndex(
                name: "IX_tickets_VisitorId",
                table: "tickets");

            migrationBuilder.DropPrimaryKey(
                name: "PK_positions",
                table: "positions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_exhibits",
                table: "exhibits");

            migrationBuilder.DropPrimaryKey(
                name: "PK_employees",
                table: "employees");

            migrationBuilder.DropPrimaryKey(
                name: "PK_tour_schedules",
                table: "tour_schedules");

            migrationBuilder.DropPrimaryKey(
                name: "PK_tour_exhibits",
                table: "tour_exhibits");

            migrationBuilder.DropColumn(
                name: "VisitorId",
                table: "tickets");

            migrationBuilder.RenameTable(
                name: "tours",
                newName: "Tours");

            migrationBuilder.RenameTable(
                name: "tickets",
                newName: "Tickets");

            migrationBuilder.RenameTable(
                name: "positions",
                newName: "Positions");

            migrationBuilder.RenameTable(
                name: "exhibits",
                newName: "Exhibits");

            migrationBuilder.RenameTable(
                name: "employees",
                newName: "Employees");

            migrationBuilder.RenameTable(
                name: "tour_schedules",
                newName: "TourSchedules");

            migrationBuilder.RenameTable(
                name: "tour_exhibits",
                newName: "TourExhibits");

            migrationBuilder.RenameIndex(
                name: "IX_tickets_TourScheduleId",
                table: "Tickets",
                newName: "IX_Tickets_TourScheduleId");

            migrationBuilder.RenameIndex(
                name: "IX_employees_PositionId",
                table: "Employees",
                newName: "IX_Employees_PositionId");

            migrationBuilder.RenameIndex(
                name: "IX_employees_Login",
                table: "Employees",
                newName: "IX_Employees_Login");

            migrationBuilder.RenameIndex(
                name: "IX_tour_schedules_TourId",
                table: "TourSchedules",
                newName: "IX_TourSchedules_TourId");

            migrationBuilder.RenameIndex(
                name: "IX_tour_schedules_GuideId",
                table: "TourSchedules",
                newName: "IX_TourSchedules_GuideId");

            migrationBuilder.RenameIndex(
                name: "IX_tour_exhibits_ExhibitId",
                table: "TourExhibits",
                newName: "IX_TourExhibits_ExhibitId");

            migrationBuilder.AlterColumn<DateTime>(
                name: "SaleDate",
                table: "Tickets",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone");

            migrationBuilder.AddColumn<string>(
                name: "VisitorName",
                table: "Tickets",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PhotoPath",
                table: "Exhibits",
                type: "character varying(300)",
                maxLength: 300,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(255)",
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "HireDate",
                table: "Employees",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone");

            migrationBuilder.AlterColumn<DateTime>(
                name: "StartTime",
                table: "TourSchedules",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Tours",
                table: "Tours",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Tickets",
                table: "Tickets",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Positions",
                table: "Positions",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Exhibits",
                table: "Exhibits",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Employees",
                table: "Employees",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TourSchedules",
                table: "TourSchedules",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TourExhibits",
                table: "TourExhibits",
                columns: new[] { "TourId", "ExhibitId" });

            migrationBuilder.AddForeignKey(
                name: "FK_Employees_Positions_PositionId",
                table: "Employees",
                column: "PositionId",
                principalTable: "Positions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Tickets_TourSchedules_TourScheduleId",
                table: "Tickets",
                column: "TourScheduleId",
                principalTable: "TourSchedules",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TourExhibits_Exhibits_ExhibitId",
                table: "TourExhibits",
                column: "ExhibitId",
                principalTable: "Exhibits",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TourExhibits_Tours_TourId",
                table: "TourExhibits",
                column: "TourId",
                principalTable: "Tours",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TourSchedules_Employees_GuideId",
                table: "TourSchedules",
                column: "GuideId",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TourSchedules_Tours_TourId",
                table: "TourSchedules",
                column: "TourId",
                principalTable: "Tours",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
