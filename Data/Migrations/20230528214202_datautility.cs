using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BugTracker.Data.Migrations
{
    /// <inheritdoc />
    public partial class datautility : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BTUserProject_projects_ProjectsId",
                table: "BTUserProject");

            migrationBuilder.DropForeignKey(
                name: "FK_Invites_projects_ProjectId",
                table: "Invites");

            migrationBuilder.DropForeignKey(
                name: "FK_projects_Companies_CompanyId",
                table: "projects");

            migrationBuilder.DropForeignKey(
                name: "FK_projects_ProjectPriorities_ProjectPriorityId",
                table: "projects");

            migrationBuilder.DropForeignKey(
                name: "FK_Tickets_projects_ProjectId",
                table: "Tickets");

            migrationBuilder.DropPrimaryKey(
                name: "PK_projects",
                table: "projects");

            migrationBuilder.RenameTable(
                name: "projects",
                newName: "Projects");

            migrationBuilder.RenameIndex(
                name: "IX_projects_ProjectPriorityId",
                table: "Projects",
                newName: "IX_Projects_ProjectPriorityId");

            migrationBuilder.RenameIndex(
                name: "IX_projects_CompanyId",
                table: "Projects",
                newName: "IX_Projects_CompanyId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Projects",
                table: "Projects",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BTUserProject_Projects_ProjectsId",
                table: "BTUserProject",
                column: "ProjectsId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Invites_Projects_ProjectId",
                table: "Invites",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_Companies_CompanyId",
                table: "Projects",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_ProjectPriorities_ProjectPriorityId",
                table: "Projects",
                column: "ProjectPriorityId",
                principalTable: "ProjectPriorities",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Tickets_Projects_ProjectId",
                table: "Tickets",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BTUserProject_Projects_ProjectsId",
                table: "BTUserProject");

            migrationBuilder.DropForeignKey(
                name: "FK_Invites_Projects_ProjectId",
                table: "Invites");

            migrationBuilder.DropForeignKey(
                name: "FK_Projects_Companies_CompanyId",
                table: "Projects");

            migrationBuilder.DropForeignKey(
                name: "FK_Projects_ProjectPriorities_ProjectPriorityId",
                table: "Projects");

            migrationBuilder.DropForeignKey(
                name: "FK_Tickets_Projects_ProjectId",
                table: "Tickets");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Projects",
                table: "Projects");

            migrationBuilder.RenameTable(
                name: "Projects",
                newName: "projects");

            migrationBuilder.RenameIndex(
                name: "IX_Projects_ProjectPriorityId",
                table: "projects",
                newName: "IX_projects_ProjectPriorityId");

            migrationBuilder.RenameIndex(
                name: "IX_Projects_CompanyId",
                table: "projects",
                newName: "IX_projects_CompanyId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_projects",
                table: "projects",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BTUserProject_projects_ProjectsId",
                table: "BTUserProject",
                column: "ProjectsId",
                principalTable: "projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Invites_projects_ProjectId",
                table: "Invites",
                column: "ProjectId",
                principalTable: "projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_projects_Companies_CompanyId",
                table: "projects",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_projects_ProjectPriorities_ProjectPriorityId",
                table: "projects",
                column: "ProjectPriorityId",
                principalTable: "ProjectPriorities",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Tickets_projects_ProjectId",
                table: "Tickets",
                column: "ProjectId",
                principalTable: "projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
