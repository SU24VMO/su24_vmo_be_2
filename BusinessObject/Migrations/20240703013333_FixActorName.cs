using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BusinessObject.Migrations
{
    public partial class FixActorName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BankingAccount_User_UserID",
                table: "BankingAccount");

            migrationBuilder.DropForeignKey(
                name: "FK_CreateActivityRequest_RequestManager_ApprovedBy",
                table: "CreateActivityRequest");

            migrationBuilder.DropForeignKey(
                name: "FK_CreateActivityRequest_User_UserID",
                table: "CreateActivityRequest");

            migrationBuilder.DropForeignKey(
                name: "FK_CreateCampaignRequest_RequestManager_ApprovedBy",
                table: "CreateCampaignRequest");

            migrationBuilder.DropForeignKey(
                name: "FK_CreateCampaignRequest_User_CreateByUser",
                table: "CreateCampaignRequest");

            migrationBuilder.DropForeignKey(
                name: "FK_CreateOrganizationManagerRequest_RequestManager_ApprovedBy",
                table: "CreateOrganizationManagerRequest");

            migrationBuilder.DropForeignKey(
                name: "FK_CreateOrganizationRequest_RequestManager_ApprovedBy",
                table: "CreateOrganizationRequest");

            migrationBuilder.DropForeignKey(
                name: "FK_CreatePostRequest_RequestManager_ApprovedBy",
                table: "CreatePostRequest");

            migrationBuilder.DropForeignKey(
                name: "FK_CreatePostRequest_User_CreateByUser",
                table: "CreatePostRequest");

            migrationBuilder.DropTable(
                name: "CreateMemberRequest");

            migrationBuilder.DropTable(
                name: "RequestManager");

            migrationBuilder.DropTable(
                name: "User");

            migrationBuilder.RenameColumn(
                name: "CreateByUser",
                table: "CreatePostRequest",
                newName: "CreateByMember");

            migrationBuilder.RenameIndex(
                name: "IX_CreatePostRequest_CreateByUser",
                table: "CreatePostRequest",
                newName: "IX_CreatePostRequest_CreateByMember");

            migrationBuilder.RenameColumn(
                name: "CreateByUser",
                table: "CreateCampaignRequest",
                newName: "CreateByMember");

            migrationBuilder.RenameIndex(
                name: "IX_CreateCampaignRequest_CreateByUser",
                table: "CreateCampaignRequest",
                newName: "IX_CreateCampaignRequest_CreateByMember");

            migrationBuilder.RenameColumn(
                name: "UserID",
                table: "CreateActivityRequest",
                newName: "MemberID");

            migrationBuilder.RenameColumn(
                name: "CreateByUser",
                table: "CreateActivityRequest",
                newName: "CreateByMember");

            migrationBuilder.RenameIndex(
                name: "IX_CreateActivityRequest_UserID",
                table: "CreateActivityRequest",
                newName: "IX_CreateActivityRequest_MemberID");

            migrationBuilder.RenameColumn(
                name: "UserID",
                table: "BankingAccount",
                newName: "MemberID");

            migrationBuilder.RenameIndex(
                name: "IX_BankingAccount_UserID",
                table: "BankingAccount",
                newName: "IX_BankingAccount_MemberID");

            migrationBuilder.CreateTable(
                name: "Member",
                columns: table => new
                {
                    MemberID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AccountID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Gender = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BirthDay = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FacebookUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    YoutubeUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TiktokUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsVerified = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Member", x => x.MemberID);
                    table.ForeignKey(
                        name: "FK_Member_Account_AccountID",
                        column: x => x.AccountID,
                        principalTable: "Account",
                        principalColumn: "AccountID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Moderator",
                columns: table => new
                {
                    ModeratorID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AccountID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Moderator", x => x.ModeratorID);
                    table.ForeignKey(
                        name: "FK_Moderator_Account_AccountID",
                        column: x => x.AccountID,
                        principalTable: "Account",
                        principalColumn: "AccountID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CreateVolunteerRequest",
                columns: table => new
                {
                    CreateVolunteerRequestID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MemberID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MemberName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Birthday = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SocialMediaLink = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MemberAddress = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RoleInClub = table.Column<int>(type: "int", nullable: true),
                    ClubName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DetailDescriptionLink = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AchievementLink = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreateBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ApprovedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ApprovedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsAcceptTermOfUse = table.Column<bool>(type: "bit", nullable: false),
                    IsApproved = table.Column<bool>(type: "bit", nullable: false),
                    IsRejected = table.Column<bool>(type: "bit", nullable: false),
                    IsPending = table.Column<bool>(type: "bit", nullable: false),
                    IsLocked = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CreateVolunteerRequest", x => x.CreateVolunteerRequestID);
                    table.ForeignKey(
                        name: "FK_CreateVolunteerRequest_Member_CreateBy",
                        column: x => x.CreateBy,
                        principalTable: "Member",
                        principalColumn: "MemberID");
                    table.ForeignKey(
                        name: "FK_CreateVolunteerRequest_Moderator_ApprovedBy",
                        column: x => x.ApprovedBy,
                        principalTable: "Moderator",
                        principalColumn: "ModeratorID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_CreateVolunteerRequest_ApprovedBy",
                table: "CreateVolunteerRequest",
                column: "ApprovedBy");

            migrationBuilder.CreateIndex(
                name: "IX_CreateVolunteerRequest_CreateBy",
                table: "CreateVolunteerRequest",
                column: "CreateBy");

            migrationBuilder.CreateIndex(
                name: "IX_Member_AccountID",
                table: "Member",
                column: "AccountID");

            migrationBuilder.CreateIndex(
                name: "IX_Moderator_AccountID",
                table: "Moderator",
                column: "AccountID");

            migrationBuilder.AddForeignKey(
                name: "FK_BankingAccount_Member_MemberID",
                table: "BankingAccount",
                column: "MemberID",
                principalTable: "Member",
                principalColumn: "MemberID");

            migrationBuilder.AddForeignKey(
                name: "FK_CreateActivityRequest_Member_MemberID",
                table: "CreateActivityRequest",
                column: "MemberID",
                principalTable: "Member",
                principalColumn: "MemberID");

            migrationBuilder.AddForeignKey(
                name: "FK_CreateActivityRequest_Moderator_ApprovedBy",
                table: "CreateActivityRequest",
                column: "ApprovedBy",
                principalTable: "Moderator",
                principalColumn: "ModeratorID");

            migrationBuilder.AddForeignKey(
                name: "FK_CreateCampaignRequest_Member_CreateByMember",
                table: "CreateCampaignRequest",
                column: "CreateByMember",
                principalTable: "Member",
                principalColumn: "MemberID");

            migrationBuilder.AddForeignKey(
                name: "FK_CreateCampaignRequest_Moderator_ApprovedBy",
                table: "CreateCampaignRequest",
                column: "ApprovedBy",
                principalTable: "Moderator",
                principalColumn: "ModeratorID");

            migrationBuilder.AddForeignKey(
                name: "FK_CreateOrganizationManagerRequest_Moderator_ApprovedBy",
                table: "CreateOrganizationManagerRequest",
                column: "ApprovedBy",
                principalTable: "Moderator",
                principalColumn: "ModeratorID");

            migrationBuilder.AddForeignKey(
                name: "FK_CreateOrganizationRequest_Moderator_ApprovedBy",
                table: "CreateOrganizationRequest",
                column: "ApprovedBy",
                principalTable: "Moderator",
                principalColumn: "ModeratorID");

            migrationBuilder.AddForeignKey(
                name: "FK_CreatePostRequest_Member_CreateByMember",
                table: "CreatePostRequest",
                column: "CreateByMember",
                principalTable: "Member",
                principalColumn: "MemberID");

            migrationBuilder.AddForeignKey(
                name: "FK_CreatePostRequest_Moderator_ApprovedBy",
                table: "CreatePostRequest",
                column: "ApprovedBy",
                principalTable: "Moderator",
                principalColumn: "ModeratorID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BankingAccount_Member_MemberID",
                table: "BankingAccount");

            migrationBuilder.DropForeignKey(
                name: "FK_CreateActivityRequest_Member_MemberID",
                table: "CreateActivityRequest");

            migrationBuilder.DropForeignKey(
                name: "FK_CreateActivityRequest_Moderator_ApprovedBy",
                table: "CreateActivityRequest");

            migrationBuilder.DropForeignKey(
                name: "FK_CreateCampaignRequest_Member_CreateByMember",
                table: "CreateCampaignRequest");

            migrationBuilder.DropForeignKey(
                name: "FK_CreateCampaignRequest_Moderator_ApprovedBy",
                table: "CreateCampaignRequest");

            migrationBuilder.DropForeignKey(
                name: "FK_CreateOrganizationManagerRequest_Moderator_ApprovedBy",
                table: "CreateOrganizationManagerRequest");

            migrationBuilder.DropForeignKey(
                name: "FK_CreateOrganizationRequest_Moderator_ApprovedBy",
                table: "CreateOrganizationRequest");

            migrationBuilder.DropForeignKey(
                name: "FK_CreatePostRequest_Member_CreateByMember",
                table: "CreatePostRequest");

            migrationBuilder.DropForeignKey(
                name: "FK_CreatePostRequest_Moderator_ApprovedBy",
                table: "CreatePostRequest");

            migrationBuilder.DropTable(
                name: "CreateVolunteerRequest");

            migrationBuilder.DropTable(
                name: "Member");

            migrationBuilder.DropTable(
                name: "Moderator");

            migrationBuilder.RenameColumn(
                name: "CreateByMember",
                table: "CreatePostRequest",
                newName: "CreateByUser");

            migrationBuilder.RenameIndex(
                name: "IX_CreatePostRequest_CreateByMember",
                table: "CreatePostRequest",
                newName: "IX_CreatePostRequest_CreateByUser");

            migrationBuilder.RenameColumn(
                name: "CreateByMember",
                table: "CreateCampaignRequest",
                newName: "CreateByUser");

            migrationBuilder.RenameIndex(
                name: "IX_CreateCampaignRequest_CreateByMember",
                table: "CreateCampaignRequest",
                newName: "IX_CreateCampaignRequest_CreateByUser");

            migrationBuilder.RenameColumn(
                name: "MemberID",
                table: "CreateActivityRequest",
                newName: "UserID");

            migrationBuilder.RenameColumn(
                name: "CreateByMember",
                table: "CreateActivityRequest",
                newName: "CreateByUser");

            migrationBuilder.RenameIndex(
                name: "IX_CreateActivityRequest_MemberID",
                table: "CreateActivityRequest",
                newName: "IX_CreateActivityRequest_UserID");

            migrationBuilder.RenameColumn(
                name: "MemberID",
                table: "BankingAccount",
                newName: "UserID");

            migrationBuilder.RenameIndex(
                name: "IX_BankingAccount_MemberID",
                table: "BankingAccount",
                newName: "IX_BankingAccount_UserID");

            migrationBuilder.CreateTable(
                name: "RequestManager",
                columns: table => new
                {
                    RequestManagerID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AccountID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequestManager", x => x.RequestManagerID);
                    table.ForeignKey(
                        name: "FK_RequestManager_Account_AccountID",
                        column: x => x.AccountID,
                        principalTable: "Account",
                        principalColumn: "AccountID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    UserID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AccountID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BirthDay = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FacebookUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Gender = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsVerified = table.Column<bool>(type: "bit", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TiktokUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    YoutubeUrl = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.UserID);
                    table.ForeignKey(
                        name: "FK_User_Account_AccountID",
                        column: x => x.AccountID,
                        principalTable: "Account",
                        principalColumn: "AccountID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CreateMemberRequest",
                columns: table => new
                {
                    CreateMemberRequestID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ApprovedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreateBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AchievementLink = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ApprovedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Birthday = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ClubName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DetailDescriptionLink = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsAcceptTermOfUse = table.Column<bool>(type: "bit", nullable: false),
                    IsApproved = table.Column<bool>(type: "bit", nullable: false),
                    IsLocked = table.Column<bool>(type: "bit", nullable: false),
                    IsPending = table.Column<bool>(type: "bit", nullable: false),
                    IsRejected = table.Column<bool>(type: "bit", nullable: false),
                    MemberAddress = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MemberName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RoleInClub = table.Column<int>(type: "int", nullable: true),
                    SocialMediaLink = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UserID = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CreateMemberRequest", x => x.CreateMemberRequestID);
                    table.ForeignKey(
                        name: "FK_CreateMemberRequest_RequestManager_ApprovedBy",
                        column: x => x.ApprovedBy,
                        principalTable: "RequestManager",
                        principalColumn: "RequestManagerID");
                    table.ForeignKey(
                        name: "FK_CreateMemberRequest_User_CreateBy",
                        column: x => x.CreateBy,
                        principalTable: "User",
                        principalColumn: "UserID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_CreateMemberRequest_ApprovedBy",
                table: "CreateMemberRequest",
                column: "ApprovedBy");

            migrationBuilder.CreateIndex(
                name: "IX_CreateMemberRequest_CreateBy",
                table: "CreateMemberRequest",
                column: "CreateBy");

            migrationBuilder.CreateIndex(
                name: "IX_RequestManager_AccountID",
                table: "RequestManager",
                column: "AccountID");

            migrationBuilder.CreateIndex(
                name: "IX_User_AccountID",
                table: "User",
                column: "AccountID");

            migrationBuilder.AddForeignKey(
                name: "FK_BankingAccount_User_UserID",
                table: "BankingAccount",
                column: "UserID",
                principalTable: "User",
                principalColumn: "UserID");

            migrationBuilder.AddForeignKey(
                name: "FK_CreateActivityRequest_RequestManager_ApprovedBy",
                table: "CreateActivityRequest",
                column: "ApprovedBy",
                principalTable: "RequestManager",
                principalColumn: "RequestManagerID");

            migrationBuilder.AddForeignKey(
                name: "FK_CreateActivityRequest_User_UserID",
                table: "CreateActivityRequest",
                column: "UserID",
                principalTable: "User",
                principalColumn: "UserID");

            migrationBuilder.AddForeignKey(
                name: "FK_CreateCampaignRequest_RequestManager_ApprovedBy",
                table: "CreateCampaignRequest",
                column: "ApprovedBy",
                principalTable: "RequestManager",
                principalColumn: "RequestManagerID");

            migrationBuilder.AddForeignKey(
                name: "FK_CreateCampaignRequest_User_CreateByUser",
                table: "CreateCampaignRequest",
                column: "CreateByUser",
                principalTable: "User",
                principalColumn: "UserID");

            migrationBuilder.AddForeignKey(
                name: "FK_CreateOrganizationManagerRequest_RequestManager_ApprovedBy",
                table: "CreateOrganizationManagerRequest",
                column: "ApprovedBy",
                principalTable: "RequestManager",
                principalColumn: "RequestManagerID");

            migrationBuilder.AddForeignKey(
                name: "FK_CreateOrganizationRequest_RequestManager_ApprovedBy",
                table: "CreateOrganizationRequest",
                column: "ApprovedBy",
                principalTable: "RequestManager",
                principalColumn: "RequestManagerID");

            migrationBuilder.AddForeignKey(
                name: "FK_CreatePostRequest_RequestManager_ApprovedBy",
                table: "CreatePostRequest",
                column: "ApprovedBy",
                principalTable: "RequestManager",
                principalColumn: "RequestManagerID");

            migrationBuilder.AddForeignKey(
                name: "FK_CreatePostRequest_User_CreateByUser",
                table: "CreatePostRequest",
                column: "CreateByUser",
                principalTable: "User",
                principalColumn: "UserID");
        }
    }
}
