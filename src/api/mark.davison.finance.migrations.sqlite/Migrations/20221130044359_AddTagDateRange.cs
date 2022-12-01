using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace mark.davison.finance.migrations.sqlite.Migrations
{
    /// <inheritdoc />
    public partial class AddTagDateRange : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "MaxDate",
                table: "Tag",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "MinDate",
                table: "Tag",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MaxDate",
                table: "Tag");

            migrationBuilder.DropColumn(
                name: "MinDate",
                table: "Tag");
        }
    }
}
