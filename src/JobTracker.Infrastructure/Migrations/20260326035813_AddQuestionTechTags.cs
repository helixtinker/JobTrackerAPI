using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JobTracker.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddQuestionTechTags : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "QuestionTechTags",
                columns: table => new
                {
                    QuestionTechTagId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    QuestionId = table.Column<int>(type: "int", nullable: false),
                    Tag = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestionTechTags", x => x.QuestionTechTagId);
                    table.ForeignKey(
                        name: "FK_QuestionTechTags_Questions_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "Questions",
                        principalColumn: "QuestionId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_QuestionTechTags_QuestionId",
                table: "QuestionTechTags",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionTechTags_Tag_QuestionId",
                table: "QuestionTechTags",
                columns: new[] { "Tag", "QuestionId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_QuestionTechTags_Tag_QuestionId",
                table: "QuestionTechTags");

            migrationBuilder.DropTable(
                name: "QuestionTechTags");
        }
    }
}
