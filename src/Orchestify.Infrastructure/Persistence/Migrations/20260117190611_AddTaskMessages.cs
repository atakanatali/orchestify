using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Orchestify.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddTaskMessages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Tables Settings and TaskMessages already exist in the database.
            // This migration is emptied to synchronize the EF state without attempting to recreate them.
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Nothing to revert as the Up was empty.
        }
    }
}
