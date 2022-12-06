﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace mark.davison.finance.migrations.sqlite.Migrations
{
    /// <inheritdoc />
    public partial class AddTransactionSourceFlag : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsSource",
                table: "Transactions",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsSource",
                table: "Transactions");
        }
    }
}
