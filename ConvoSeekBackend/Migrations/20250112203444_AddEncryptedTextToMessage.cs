using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConvoSeekBackend.Migrations
{
    /// <inheritdoc />
    public partial class AddEncryptedTextToMessage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EncryptedText",
                table: "Messages",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EncryptedText",
                table: "Messages");
        }
    }
}
