using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskFlow.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Activities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Category = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Priority = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false, defaultValue: "Normal"),
                    IsCompleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Activities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tasks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ActivityId = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    IsCompleted = table.Column<bool>(type: "bit", nullable: false),
                    DueTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tasks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tasks_Activities_ActivityId",
                        column: x => x.ActivityId,
                        principalTable: "Activities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_ActivityId",
                table: "Tasks",
                column: "ActivityId");

            // Seed: Activities
            migrationBuilder.InsertData(
                table: "Activities",
                columns: new[] { "Id", "Title", "Description", "Date", "Category", "Priority", "IsCompleted", "CreatedAt" },
                values: new object[,]
                {
                    { 1, "Estudiar Arquitectura de Software", "Repasar patrones GoF y ADRs", new DateTime(2026, 6, 24, 0, 0, 0, DateTimeKind.Utc), "Estudio", "High", false, new DateTime(2026, 6, 24, 0, 0, 0, DateTimeKind.Utc) },
                    { 2, "Entregar proyecto TaskFlow", "Subir avances al repositorio", new DateTime(2026, 6, 24, 0, 0, 0, DateTimeKind.Utc), "Universidad", "High", false, new DateTime(2026, 6, 24, 0, 0, 0, DateTimeKind.Utc) },
                    { 3, "Hacer ejercicio", "30 min cardio", new DateTime(2026, 6, 24, 0, 0, 0, DateTimeKind.Utc), "Salud", "Normal", false, new DateTime(2026, 6, 24, 0, 0, 0, DateTimeKind.Utc) }
                });

            // Seed: Tasks
            migrationBuilder.InsertData(
                table: "Tasks",
                columns: new[] { "Id", "ActivityId", "Title", "Description", "IsCompleted", "CreatedAt" },
                values: new object[,]
                {
                    { 1, 1, "Leer sobre Strategy Pattern", "Capítulo 5 del libro GoF", true, new DateTime(2026, 6, 24, 0, 0, 0, DateTimeKind.Utc) },
                    { 2, 1, "Implementar Builder en el proyecto", "ActivityBuilder y TaskBuilder", false, new DateTime(2026, 6, 24, 0, 0, 0, DateTimeKind.Utc) },
                    { 3, 2, "Hacer commit de los patrones", "Push a rama api", false, new DateTime(2026, 6, 24, 0, 0, 0, DateTimeKind.Utc) }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "Tasks");
            migrationBuilder.DropTable(name: "Activities");
        }
    }
}
