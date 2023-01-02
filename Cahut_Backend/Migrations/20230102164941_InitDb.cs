using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CahutBackend.Migrations
{
    /// <inheritdoc />
    public partial class InitDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EmailSender",
                columns: table => new
                {
                    usr = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    pwd = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EmailSended = table.Column<int>(type: "int", nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmailSender", x => x.usr);
                });

            migrationBuilder.CreateTable(
                name: "Role",
                columns: table => new
                {
                    RoleId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Role", x => x.RoleId);
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Avatar = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AccountStatus = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    RefreshToken = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ResetPasswordString = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RefreshTokenExpiredTime = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValue: new DateTime(2023, 1, 2, 16, 49, 41, 359, DateTimeKind.Utc).AddTicks(7869))
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "Group",
                columns: table => new
                {
                    GroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OwnerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NumOfMems = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    JoinGrString = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HasPresentationPresenting = table.Column<bool>(type: "bit", nullable: false),
                    GroupName = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PresentationId = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Group", x => x.GroupId);
                    table.ForeignKey(
                        name: "FK_Group_User_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "User",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Presentation",
                columns: table => new
                {
                    PresentationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OwnerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PresentationName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PresentationType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsBeingPresented = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Presentation", x => x.PresentationId);
                    table.ForeignKey(
                        name: "FK_Presentation_User_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "User",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GroupDetail",
                columns: table => new
                {
                    GroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MemberId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoleId = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    JoinedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupDetail", x => new { x.GroupId, x.MemberId });
                    table.ForeignKey(
                        name: "FK_GroupDetail_Group_GroupId",
                        column: x => x.GroupId,
                        principalTable: "Group",
                        principalColumn: "GroupId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GroupDetail_Role_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Role",
                        principalColumn: "RoleId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Chat",
                columns: table => new
                {
                    ChatId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PresentationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NumOfMessage = table.Column<int>(type: "int", nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Chat", x => x.ChatId);
                    table.ForeignKey(
                        name: "FK_Chat_Presentation_PresentationId",
                        column: x => x.PresentationId,
                        principalTable: "Presentation",
                        principalColumn: "PresentationId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HeadingSlide",
                columns: table => new
                {
                    SlideId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    SlideOrder = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    SlideType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsCurrent = table.Column<int>(type: "int", nullable: false),
                    PresentationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    HeadingContent = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SubHeadingContent = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HeadingSlide", x => x.SlideId);
                    table.ForeignKey(
                        name: "FK_HeadingSlide_Presentation_PresentationId",
                        column: x => x.PresentationId,
                        principalTable: "Presentation",
                        principalColumn: "PresentationId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MultipleChoiceSlide",
                columns: table => new
                {
                    SlideId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    SlideOrder = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    SlideType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsCurrent = table.Column<int>(type: "int", nullable: false),
                    PresentationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NumOfMultipleChoiceQuestion = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MultipleChoiceSlide", x => x.SlideId);
                    table.ForeignKey(
                        name: "FK_MultipleChoiceSlide_Presentation_PresentationId",
                        column: x => x.PresentationId,
                        principalTable: "Presentation",
                        principalColumn: "PresentationId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ParagraphSlide",
                columns: table => new
                {
                    SlideId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    SlideOrder = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    SlideType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsCurrent = table.Column<int>(type: "int", nullable: false),
                    PresentationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ParagraphContent = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HeadingContent = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ParagraphSlide", x => x.SlideId);
                    table.ForeignKey(
                        name: "FK_ParagraphSlide_Presentation_PresentationId",
                        column: x => x.PresentationId,
                        principalTable: "Presentation",
                        principalColumn: "PresentationId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PresentationDetail",
                columns: table => new
                {
                    PresentationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ColaboratorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PresentationDetail", x => new { x.PresentationId, x.ColaboratorId });
                    table.ForeignKey(
                        name: "FK_PresentationDetail_Presentation_PresentationId",
                        column: x => x.PresentationId,
                        principalTable: "Presentation",
                        principalColumn: "PresentationId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PresentationQuestion",
                columns: table => new
                {
                    QuestionId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    QuestionType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PresentationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NumUpVote = table.Column<int>(type: "int", nullable: false),
                    isAnswered = table.Column<bool>(type: "bit", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PresentationQuestion", x => x.QuestionId);
                    table.ForeignKey(
                        name: "FK_PresentationQuestion_Presentation_PresentationId",
                        column: x => x.PresentationId,
                        principalTable: "Presentation",
                        principalColumn: "PresentationId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ChatMessage",
                columns: table => new
                {
                    ChatId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TimeSend = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SenderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SenderName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MsgContent = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatMessage", x => new { x.ChatId, x.TimeSend });
                    table.ForeignKey(
                        name: "FK_ChatMessage_Chat_ChatId",
                        column: x => x.ChatId,
                        principalTable: "Chat",
                        principalColumn: "ChatId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MultipleChoiceQuestion",
                columns: table => new
                {
                    QuestionId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    QuestionType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SlideId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RightAnswer = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MultipleChoiceQuestion", x => x.QuestionId);
                    table.ForeignKey(
                        name: "FK_MultipleChoiceQuestion_MultipleChoiceSlide_SlideId",
                        column: x => x.SlideId,
                        principalTable: "MultipleChoiceSlide",
                        principalColumn: "SlideId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserUpvoteQuestion",
                columns: table => new
                {
                    QuestionId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PresentationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TimeUpVote = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserUpvoteQuestion", x => new { x.UserId, x.PresentationId, x.QuestionId });
                    table.ForeignKey(
                        name: "FK_UserUpvoteQuestion_Group_GroupId",
                        column: x => x.GroupId,
                        principalTable: "Group",
                        principalColumn: "GroupId");
                    table.ForeignKey(
                        name: "FK_UserUpvoteQuestion_PresentationQuestion_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "PresentationQuestion",
                        principalColumn: "QuestionId");
                    table.ForeignKey(
                        name: "FK_UserUpvoteQuestion_Presentation_PresentationId",
                        column: x => x.PresentationId,
                        principalTable: "Presentation",
                        principalColumn: "PresentationId");
                    table.ForeignKey(
                        name: "FK_UserUpvoteQuestion_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "UserId");
                });

            migrationBuilder.CreateTable(
                name: "MultipleChoiceOption",
                columns: table => new
                {
                    OptionId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    QuestionId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    OptionContent = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NumSelected = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MultipleChoiceOption", x => x.OptionId);
                    table.ForeignKey(
                        name: "FK_MultipleChoiceOption_MultipleChoiceQuestion_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "MultipleChoiceQuestion",
                        principalColumn: "QuestionId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserSubmitChoice",
                columns: table => new
                {
                    QuestionId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    OptionId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SubmitTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSubmitChoice", x => new { x.UserId, x.OptionId, x.QuestionId });
                    table.ForeignKey(
                        name: "FK_UserSubmitChoice_Group_GroupId",
                        column: x => x.GroupId,
                        principalTable: "Group",
                        principalColumn: "GroupId");
                    table.ForeignKey(
                        name: "FK_UserSubmitChoice_MultipleChoiceOption_OptionId",
                        column: x => x.OptionId,
                        principalTable: "MultipleChoiceOption",
                        principalColumn: "OptionId");
                    table.ForeignKey(
                        name: "FK_UserSubmitChoice_MultipleChoiceQuestion_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "MultipleChoiceQuestion",
                        principalColumn: "QuestionId");
                    table.ForeignKey(
                        name: "FK_UserSubmitChoice_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "UserId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Chat_PresentationId",
                table: "Chat",
                column: "PresentationId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ChatMessage_ChatId",
                table: "ChatMessage",
                column: "ChatId");

            migrationBuilder.CreateIndex(
                name: "IX_ChatMessage_TimeSend",
                table: "ChatMessage",
                column: "TimeSend");

            migrationBuilder.CreateIndex(
                name: "IX_Group_GroupName",
                table: "Group",
                column: "GroupName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Group_OwnerId",
                table: "Group",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_GroupDetail_GroupId",
                table: "GroupDetail",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_GroupDetail_MemberId",
                table: "GroupDetail",
                column: "MemberId");

            migrationBuilder.CreateIndex(
                name: "IX_GroupDetail_RoleId",
                table: "GroupDetail",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_HeadingSlide_PresentationId",
                table: "HeadingSlide",
                column: "PresentationId");

            migrationBuilder.CreateIndex(
                name: "IX_MultipleChoiceOption_QuestionId",
                table: "MultipleChoiceOption",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_MultipleChoiceQuestion_SlideId",
                table: "MultipleChoiceQuestion",
                column: "SlideId");

            migrationBuilder.CreateIndex(
                name: "IX_MultipleChoiceSlide_PresentationId",
                table: "MultipleChoiceSlide",
                column: "PresentationId");

            migrationBuilder.CreateIndex(
                name: "IX_ParagraphSlide_PresentationId",
                table: "ParagraphSlide",
                column: "PresentationId");

            migrationBuilder.CreateIndex(
                name: "IX_Presentation_OwnerId",
                table: "Presentation",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_PresentationDetail_PresentationId",
                table: "PresentationDetail",
                column: "PresentationId");

            migrationBuilder.CreateIndex(
                name: "IX_PresentationQuestion_PresentationId",
                table: "PresentationQuestion",
                column: "PresentationId");

            migrationBuilder.CreateIndex(
                name: "IX_User_Email",
                table: "User",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserSubmitChoice_GroupId",
                table: "UserSubmitChoice",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_UserSubmitChoice_OptionId",
                table: "UserSubmitChoice",
                column: "OptionId");

            migrationBuilder.CreateIndex(
                name: "IX_UserSubmitChoice_QuestionId",
                table: "UserSubmitChoice",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_UserSubmitChoice_UserId",
                table: "UserSubmitChoice",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserUpvoteQuestion_GroupId",
                table: "UserUpvoteQuestion",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_UserUpvoteQuestion_PresentationId",
                table: "UserUpvoteQuestion",
                column: "PresentationId");

            migrationBuilder.CreateIndex(
                name: "IX_UserUpvoteQuestion_QuestionId",
                table: "UserUpvoteQuestion",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_UserUpvoteQuestion_UserId",
                table: "UserUpvoteQuestion",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChatMessage");

            migrationBuilder.DropTable(
                name: "EmailSender");

            migrationBuilder.DropTable(
                name: "GroupDetail");

            migrationBuilder.DropTable(
                name: "HeadingSlide");

            migrationBuilder.DropTable(
                name: "ParagraphSlide");

            migrationBuilder.DropTable(
                name: "PresentationDetail");

            migrationBuilder.DropTable(
                name: "UserSubmitChoice");

            migrationBuilder.DropTable(
                name: "UserUpvoteQuestion");

            migrationBuilder.DropTable(
                name: "Chat");

            migrationBuilder.DropTable(
                name: "Role");

            migrationBuilder.DropTable(
                name: "MultipleChoiceOption");

            migrationBuilder.DropTable(
                name: "Group");

            migrationBuilder.DropTable(
                name: "PresentationQuestion");

            migrationBuilder.DropTable(
                name: "MultipleChoiceQuestion");

            migrationBuilder.DropTable(
                name: "MultipleChoiceSlide");

            migrationBuilder.DropTable(
                name: "Presentation");

            migrationBuilder.DropTable(
                name: "User");
        }
    }
}
