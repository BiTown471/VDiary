using Microsoft.EntityFrameworkCore.Migrations;

namespace VDiary.Migrations
{
    public partial class UpdateCourseModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CourseUser");

            migrationBuilder.AddColumn<int>(
                name: "LectruerId",
                table: "Course",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Course_LectruerId",
                table: "Course",
                column: "LectruerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Course_User_LectruerId",
                table: "Course",
                column: "LectruerId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Course_User_LectruerId",
                table: "Course");

            migrationBuilder.DropIndex(
                name: "IX_Course_LectruerId",
                table: "Course");

            migrationBuilder.DropColumn(
                name: "LectruerId",
                table: "Course");

            migrationBuilder.CreateTable(
                name: "CourseUser",
                columns: table => new
                {
                    CourseListId = table.Column<int>(type: "int", nullable: false),
                    UsersListId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseUser", x => new { x.CourseListId, x.UsersListId });
                    table.ForeignKey(
                        name: "FK_CourseUser_Course_CourseListId",
                        column: x => x.CourseListId,
                        principalTable: "Course",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CourseUser_User_UsersListId",
                        column: x => x.UsersListId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CourseUser_UsersListId",
                table: "CourseUser",
                column: "UsersListId");
        }
    }
}
