using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TicketHub.Migrations
{
    /// <inheritdoc />
    public partial class AddOrganizerToEvent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Only add the new column to existing Event table
            migrationBuilder.AddColumn<int>(
                name: "OrganizerID",
                table: "Event",
                type: "int",
                nullable: true);

            // Create index for the new column
            migrationBuilder.CreateIndex(
                name: "IX_Event_OrganizerID",
                table: "Event",
                column: "OrganizerID");

            // Add foreign key constraint
            migrationBuilder.AddForeignKey(
                name: "FK_Event_Member_OrganizerID",
                table: "Event",
                column: "OrganizerID",
                principalTable: "Member",
                principalColumn: "MemberID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Event_Member_OrganizerID",
                table: "Event");

            migrationBuilder.DropIndex(
                name: "IX_Event_OrganizerID",
                table: "Event");

            migrationBuilder.DropColumn(
                name: "OrganizerID",
                table: "Event");
        }
    }
}
