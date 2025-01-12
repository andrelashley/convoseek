using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConvoSeekBackend.Migrations
{
    /// <inheritdoc />
    public partial class DropEmbeddingColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Embedding",
                table: "Messages");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Embedding",
                table: "Messages",
                type: "text",
                nullable: true);
        }
    }
}
