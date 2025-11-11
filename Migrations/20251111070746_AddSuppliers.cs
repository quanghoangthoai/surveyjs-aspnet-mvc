using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace surveyjsaspnetmvc.Migrations
{
    /// <inheritdoc />
    public partial class AddSuppliers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsSupplierEvaluation",
                table: "Surveys",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<int>(
                name: "SupplierId",
                table: "SurveyResponses",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Suppliers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    SurveyId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Suppliers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Suppliers_Surveys_SurveyId",
                        column: x => x.SurveyId,
                        principalTable: "Surveys",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SurveyResponses_SupplierId",
                table: "SurveyResponses",
                column: "SupplierId");

            migrationBuilder.CreateIndex(
                name: "IX_Suppliers_DisplayOrder",
                table: "Suppliers",
                column: "DisplayOrder");

            migrationBuilder.CreateIndex(
                name: "IX_Suppliers_SurveyId",
                table: "Suppliers",
                column: "SurveyId",
                unique: true,
                filter: "[SurveyId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_SurveyResponses_Suppliers_SupplierId",
                table: "SurveyResponses",
                column: "SupplierId",
                principalTable: "Suppliers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SurveyResponses_Suppliers_SupplierId",
                table: "SurveyResponses");

            migrationBuilder.DropTable(
                name: "Suppliers");

            migrationBuilder.DropIndex(
                name: "IX_SurveyResponses_SupplierId",
                table: "SurveyResponses");

            migrationBuilder.DropColumn(
                name: "IsSupplierEvaluation",
                table: "Surveys");

            migrationBuilder.DropColumn(
                name: "SupplierId",
                table: "SurveyResponses");
        }
    }
}
