using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConvoSeekBackend.Migrations
{
    /// <inheritdoc />
    public partial class AddEmbeddingsToMessages : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<float[]>(
                name: "Embedding",
                table: "Messages",
                type: "vector(1536)",
                nullable: true
            );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Embedding",
                table: "Messages"
            );
        }
    }
}
