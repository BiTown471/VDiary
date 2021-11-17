using Microsoft.EntityFrameworkCore.Migrations;

namespace VDiary.Migrations
{
    public partial class UpdateCourseModel2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Course_User_LectruerId",
                table: "Course");

            migrationBuilder.RenameColumn(
                name: "LectruerId",
                table: "Course",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Course_LectruerId",
                table: "Course",
                newName: "IX_Course_UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Course_User_UserId",
                table: "Course",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Course_User_UserId",
                table: "Course");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Course",
                newName: "LectruerId");

            migrationBuilder.RenameIndex(
                name: "IX_Course_UserId",
                table: "Course",
                newName: "IX_Course_LectruerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Course_User_LectruerId",
                table: "Course",
                column: "LectruerId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
