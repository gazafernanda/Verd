using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Verd.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddPlantLastWateredAt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "LastWateredAt",
                table: "Plants",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastWateredAt",
                table: "Plants");
        }
    }
}
