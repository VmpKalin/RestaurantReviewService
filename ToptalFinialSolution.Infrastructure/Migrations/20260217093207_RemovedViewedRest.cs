using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ToptalFinialSolution.Migrations
{
    /// <inheritdoc />
    public partial class RemovedViewedRest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ViewedRestaurant");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ViewedRestaurant",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RestaurantId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    ViewedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ViewedRestaurant", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ViewedRestaurant_Restaurants_RestaurantId",
                        column: x => x.RestaurantId,
                        principalTable: "Restaurants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ViewedRestaurant_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ViewedRestaurant_RestaurantId",
                table: "ViewedRestaurant",
                column: "RestaurantId");

            migrationBuilder.CreateIndex(
                name: "IX_ViewedRestaurant_UserId",
                table: "ViewedRestaurant",
                column: "UserId");
        }
    }
}
