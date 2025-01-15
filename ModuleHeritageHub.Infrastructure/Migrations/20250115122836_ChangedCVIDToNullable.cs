using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ModuleHeritageHub.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ChangedCVIDToNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Pages_PageVersions_CurrentVersionId",
                table: "Pages");

            migrationBuilder.AlterColumn<Guid>(
                name: "CurrentVersionId",
                table: "Pages",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddForeignKey(
                name: "FK_Pages_PageVersions_CurrentVersionId",
                table: "Pages",
                column: "CurrentVersionId",
                principalTable: "PageVersions",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Pages_PageVersions_CurrentVersionId",
                table: "Pages");

            migrationBuilder.AlterColumn<Guid>(
                name: "CurrentVersionId",
                table: "Pages",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Pages_PageVersions_CurrentVersionId",
                table: "Pages",
                column: "CurrentVersionId",
                principalTable: "PageVersions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
