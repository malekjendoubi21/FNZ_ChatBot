using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FNZ_ChatBot.Migrations
{
    /// <inheritdoc />
    public partial class AddKnowledgeBaseAndConversations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ConversationId",
                table: "ConversationHistory",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Conversations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastActivity = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Conversations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Conversations_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "KnowledgeBase",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Question = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Response = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KnowledgeBase", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ConversationHistory_ConversationId",
                table: "ConversationHistory",
                column: "ConversationId");

            migrationBuilder.CreateIndex(
                name: "IX_Conversations_LastActivity",
                table: "Conversations",
                column: "LastActivity");

            migrationBuilder.CreateIndex(
                name: "IX_Conversations_UserId",
                table: "Conversations",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_KnowledgeBase_IsActive",
                table: "KnowledgeBase",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_KnowledgeBase_Question",
                table: "KnowledgeBase",
                column: "Question");

            migrationBuilder.AddForeignKey(
                name: "FK_ConversationHistory_Conversations_ConversationId",
                table: "ConversationHistory",
                column: "ConversationId",
                principalTable: "Conversations",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ConversationHistory_Conversations_ConversationId",
                table: "ConversationHistory");

            migrationBuilder.DropTable(
                name: "Conversations");

            migrationBuilder.DropTable(
                name: "KnowledgeBase");

            migrationBuilder.DropIndex(
                name: "IX_ConversationHistory_ConversationId",
                table: "ConversationHistory");

            migrationBuilder.DropColumn(
                name: "ConversationId",
                table: "ConversationHistory");
        }
    }
}
