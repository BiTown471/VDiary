using Microsoft.EntityFrameworkCore.Migrations;

namespace VDiary.Migrations
{
    public partial class NewConfigRelationship : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CourseUser_Course_CourseId",
                table: "CourseUser");

            migrationBuilder.DropForeignKey(
                name: "FK_CourseUser_User_UserId",
                table: "CourseUser");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CourseUser",
                table: "CourseUser");

            migrationBuilder.DropIndex(
                name: "IX_CourseUser_UserId",
                table: "CourseUser");

            migrationBuilder.DropColumn(
                name: "CourseId",
                table: "CourseUser");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "CourseUser",
                newName: "UsersListId");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "CourseUser",
                newName: "CourseListId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CourseUser",
                table: "CourseUser",
                columns: new[] { "CourseListId", "UsersListId" });

            migrationBuilder.CreateIndex(
                name: "IX_CourseUser_UsersListId",
                table: "CourseUser",
                column: "UsersListId");

            migrationBuilder.AddForeignKey(
                name: "FK_CourseUser_Course_CourseListId",
                table: "CourseUser",
                column: "CourseListId",
                principalTable: "Course",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CourseUser_User_UsersListId",
                table: "CourseUser",
                column: "UsersListId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CourseUser_Course_CourseListId",
                table: "CourseUser");

            migrationBuilder.DropForeignKey(
                name: "FK_CourseUser_User_UsersListId",
                table: "CourseUser");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CourseUser",
                table: "CourseUser");

            migrationBuilder.DropIndex(
                name: "IX_CourseUser_UsersListId",
                table: "CourseUser");

            migrationBuilder.RenameColumn(
                name: "UsersListId",
                table: "CourseUser",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "CourseListId",
                table: "CourseUser",
                newName: "UserId");

            migrationBuilder.AddColumn<int>(
                name: "CourseId",
                table: "CourseUser",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_CourseUser",
                table: "CourseUser",
                columns: new[] { "CourseId", "UserId" });

            migrationBuilder.CreateIndex(
                name: "IX_CourseUser_UserId",
                table: "CourseUser",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_CourseUser_Course_CourseId",
                table: "CourseUser",
                column: "CourseId",
                principalTable: "Course",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CourseUser_User_UserId",
                table: "CourseUser",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
