using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using TaskFlow.Infrastructure.Data;

#nullable disable

namespace TaskFlow.Infrastructure.Migrations;

[DbContext(typeof(TaskFlowDbContext))]
[Migration("20260729000000_AddTaskPriorityAndIndexes")]
public partial class AddTaskPriorityAndIndexes : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "Priority",
            table: "Tasks",
            type: "nvarchar(10)",
            maxLength: 10,
            nullable: false,
            defaultValue: "Medium");

        migrationBuilder.CreateIndex(
            name: "IX_Tasks_DueTime",
            table: "Tasks",
            column: "DueTime");

        migrationBuilder.CreateIndex(
            name: "IX_Tasks_IsCompleted_Priority",
            table: "Tasks",
            columns: new[] { "IsCompleted", "Priority" });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(name: "IX_Tasks_DueTime", table: "Tasks");
        migrationBuilder.DropIndex(name: "IX_Tasks_IsCompleted_Priority", table: "Tasks");
        migrationBuilder.DropColumn(name: "Priority", table: "Tasks");
    }
}
