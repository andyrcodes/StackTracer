﻿using Microsoft.EntityFrameworkCore.Migrations;

namespace StackTracer.Data.Migrations
{
    public partial class _003_PMId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ProjectManagerId",
                table: "Projects",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProjectManagerId",
                table: "Projects");
        }
    }
}
