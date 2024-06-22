using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BusinessObject.Migrations
{
    /// <inheritdoc />
    public partial class Initialize : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Account",
                columns: table => new
                {
                    AccountID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    HashPassword = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    SaltPassword = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Username = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Avatar = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Role = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActived = table.Column<bool>(type: "bit", nullable: false),
                    IsBlocked = table.Column<bool>(type: "bit", nullable: false),
                    ModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Account", x => x.AccountID);
                });

            migrationBuilder.CreateTable(
                name: "CampaignType",
                columns: table => new
                {
                    CampaignTypeID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreateAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsValid = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CampaignType", x => x.CampaignTypeID);
                });

            migrationBuilder.CreateTable(
                name: "Post",
                columns: table => new
                {
                    PostID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Cover = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Image = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreateAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Post", x => x.PostID);
                });

            migrationBuilder.CreateTable(
                name: "AccountToken",
                columns: table => new
                {
                    AccountTokenId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AccountID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AccessToken = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ExpiredDateAccessToken = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CodeRefreshToken = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RefreshToken = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ExpiredDateRefreshToken = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountToken", x => x.AccountTokenId);
                    table.ForeignKey(
                        name: "FK_AccountToken_Account_AccountID",
                        column: x => x.AccountID,
                        principalTable: "Account",
                        principalColumn: "AccountID");
                });

            migrationBuilder.CreateTable(
                name: "Admin",
                columns: table => new
                {
                    AdminID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AccountID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Admin", x => x.AdminID);
                    table.ForeignKey(
                        name: "FK_Admin_Account_AccountID",
                        column: x => x.AccountID,
                        principalTable: "Account",
                        principalColumn: "AccountID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Notification",
                columns: table => new
                {
                    NotificationID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AccountID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NotificationCategory = table.Column<int>(type: "int", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsSeen = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notification", x => x.NotificationID);
                    table.ForeignKey(
                        name: "FK_Notification_Account_AccountID",
                        column: x => x.AccountID,
                        principalTable: "Account",
                        principalColumn: "AccountID");
                });

            migrationBuilder.CreateTable(
                name: "OrganizationManager",
                columns: table => new
                {
                    OrganizationManagerID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AccountID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Gender = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BirthDay = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FacebookUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    YoutubeUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TiktokUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsVerified = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganizationManager", x => x.OrganizationManagerID);
                    table.ForeignKey(
                        name: "FK_OrganizationManager_Account_AccountID",
                        column: x => x.AccountID,
                        principalTable: "Account",
                        principalColumn: "AccountID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RequestManager",
                columns: table => new
                {
                    RequestManagerID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AccountID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false)
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
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Gender = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BirthDay = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FacebookUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    YoutubeUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TiktokUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsVerified = table.Column<bool>(type: "bit", nullable: false)
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
                name: "Organization",
                columns: table => new
                {
                    OrganizationID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrganizationManagerID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Logo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Website = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Tax = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Location = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FoundingDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    OperatingLicense = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsModify = table.Column<bool>(type: "bit", nullable: false),
                    Category = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Organization", x => x.OrganizationID);
                    table.ForeignKey(
                        name: "FK_Organization_OrganizationManager_OrganizationManagerID",
                        column: x => x.OrganizationManagerID,
                        principalTable: "OrganizationManager",
                        principalColumn: "OrganizationManagerID");
                });

            migrationBuilder.CreateTable(
                name: "CreateOrganizationManagerRequest",
                columns: table => new
                {
                    CreateOrganizationManagerRequestID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrganizationManagerID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CitizenIdentification = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PersonalTaxCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreateBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ApprovedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsAcceptTermOfUse = table.Column<bool>(type: "bit", nullable: false),
                    IsApproved = table.Column<bool>(type: "bit", nullable: false),
                    IsRejected = table.Column<bool>(type: "bit", nullable: false),
                    IsPending = table.Column<bool>(type: "bit", nullable: false),
                    IsLocked = table.Column<bool>(type: "bit", nullable: false),
                    RequestManagerID = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CreateOrganizationManagerRequest", x => x.CreateOrganizationManagerRequestID);
                    table.ForeignKey(
                        name: "FK_CreateOrganizationManagerRequest_OrganizationManager_OrganizationManagerID",
                        column: x => x.OrganizationManagerID,
                        principalTable: "OrganizationManager",
                        principalColumn: "OrganizationManagerID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CreateOrganizationManagerRequest_RequestManager_RequestManagerID",
                        column: x => x.RequestManagerID,
                        principalTable: "RequestManager",
                        principalColumn: "RequestManagerID");
                });

            migrationBuilder.CreateTable(
                name: "BankingAccount",
                columns: table => new
                {
                    BankingAccountID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BankingName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AccountNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AccountName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    QRCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsAvailable = table.Column<bool>(type: "bit", nullable: false),
                    OrganizationManagerID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UserID = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BankingAccount", x => x.BankingAccountID);
                    table.ForeignKey(
                        name: "FK_BankingAccount_Account_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Account",
                        principalColumn: "AccountID");
                    table.ForeignKey(
                        name: "FK_BankingAccount_OrganizationManager_OrganizationManagerID",
                        column: x => x.OrganizationManagerID,
                        principalTable: "OrganizationManager",
                        principalColumn: "OrganizationManagerID");
                    table.ForeignKey(
                        name: "FK_BankingAccount_User_UserID",
                        column: x => x.UserID,
                        principalTable: "User",
                        principalColumn: "UserID");
                });

            migrationBuilder.CreateTable(
                name: "CreateMemberRequest",
                columns: table => new
                {
                    CreateMemberRequestID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
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
                    IsAcceptTermOfUse = table.Column<bool>(type: "bit", nullable: false),
                    IsApproved = table.Column<bool>(type: "bit", nullable: false),
                    IsRejected = table.Column<bool>(type: "bit", nullable: false),
                    IsPending = table.Column<bool>(type: "bit", nullable: false),
                    IsLocked = table.Column<bool>(type: "bit", nullable: false)
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

            migrationBuilder.CreateTable(
                name: "CreatePostRequest",
                columns: table => new
                {
                    CreatePostRequestID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PostID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreateByUser = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreateByOM = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ApprovedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ApprovedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsApproved = table.Column<bool>(type: "bit", nullable: false),
                    IsRejected = table.Column<bool>(type: "bit", nullable: false),
                    IsPending = table.Column<bool>(type: "bit", nullable: false),
                    IsLocked = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CreatePostRequest", x => x.CreatePostRequestID);
                    table.ForeignKey(
                        name: "FK_CreatePostRequest_OrganizationManager_CreateByOM",
                        column: x => x.CreateByOM,
                        principalTable: "OrganizationManager",
                        principalColumn: "OrganizationManagerID");
                    table.ForeignKey(
                        name: "FK_CreatePostRequest_Post_PostID",
                        column: x => x.PostID,
                        principalTable: "Post",
                        principalColumn: "PostID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CreatePostRequest_RequestManager_ApprovedBy",
                        column: x => x.ApprovedBy,
                        principalTable: "RequestManager",
                        principalColumn: "RequestManagerID");
                    table.ForeignKey(
                        name: "FK_CreatePostRequest_User_CreateByUser",
                        column: x => x.CreateByUser,
                        principalTable: "User",
                        principalColumn: "UserID");
                });

            migrationBuilder.CreateTable(
                name: "Achievement",
                columns: table => new
                {
                    AchievementID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrganizationID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Link = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Achievement", x => x.AchievementID);
                    table.ForeignKey(
                        name: "FK_Achievement_Organization_OrganizationID",
                        column: x => x.OrganizationID,
                        principalTable: "Organization",
                        principalColumn: "OrganizationID");
                });

            migrationBuilder.CreateTable(
                name: "Campaign",
                columns: table => new
                {
                    CampaignID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrganizationID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CampaignTypeID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Image = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpectedEndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ActualEndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TargetAmount = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ApplicationConfirmForm = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsTransparent = table.Column<bool>(type: "bit", nullable: false),
                    CreateAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsModify = table.Column<bool>(type: "bit", nullable: false),
                    IsComplete = table.Column<bool>(type: "bit", nullable: false),
                    CanBeDonated = table.Column<bool>(type: "bit", nullable: false),
                    CheckTransparentDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Campaign", x => x.CampaignID);
                    table.ForeignKey(
                        name: "FK_Campaign_CampaignType_CampaignTypeID",
                        column: x => x.CampaignTypeID,
                        principalTable: "CampaignType",
                        principalColumn: "CampaignTypeID");
                    table.ForeignKey(
                        name: "FK_Campaign_Organization_OrganizationID",
                        column: x => x.OrganizationID,
                        principalTable: "Organization",
                        principalColumn: "OrganizationID");
                });

            migrationBuilder.CreateTable(
                name: "CreateOrganizationRequest",
                columns: table => new
                {
                    CreateOrganizationRequestID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrganizationID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrganizationName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OrganizationManagerEmail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OrganizationManagerTaxCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FoundingDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SocialMediaLink = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AreaOfActivity = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PlanInformation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AchievementLink = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreateBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ApprovedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsApproved = table.Column<bool>(type: "bit", nullable: false),
                    IsRejected = table.Column<bool>(type: "bit", nullable: false),
                    IsPending = table.Column<bool>(type: "bit", nullable: false),
                    IsLocked = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CreateOrganizationRequest", x => x.CreateOrganizationRequestID);
                    table.ForeignKey(
                        name: "FK_CreateOrganizationRequest_OrganizationManager_CreateBy",
                        column: x => x.CreateBy,
                        principalTable: "OrganizationManager",
                        principalColumn: "OrganizationManagerID");
                    table.ForeignKey(
                        name: "FK_CreateOrganizationRequest_Organization_OrganizationID",
                        column: x => x.OrganizationID,
                        principalTable: "Organization",
                        principalColumn: "OrganizationID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CreateOrganizationRequest_RequestManager_ApprovedBy",
                        column: x => x.ApprovedBy,
                        principalTable: "RequestManager",
                        principalColumn: "RequestManagerID");
                });

            migrationBuilder.CreateTable(
                name: "CreateCampaignRequest",
                columns: table => new
                {
                    CreateCampaignRequestID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CampaignID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreateByUser = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreateByOM = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ApprovedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsApproved = table.Column<bool>(type: "bit", nullable: false),
                    IsRejected = table.Column<bool>(type: "bit", nullable: false),
                    IsPending = table.Column<bool>(type: "bit", nullable: false),
                    IsLocked = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CreateCampaignRequest", x => x.CreateCampaignRequestID);
                    table.ForeignKey(
                        name: "FK_CreateCampaignRequest_Campaign_CampaignID",
                        column: x => x.CampaignID,
                        principalTable: "Campaign",
                        principalColumn: "CampaignID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CreateCampaignRequest_OrganizationManager_CreateByOM",
                        column: x => x.CreateByOM,
                        principalTable: "OrganizationManager",
                        principalColumn: "OrganizationManagerID");
                    table.ForeignKey(
                        name: "FK_CreateCampaignRequest_RequestManager_ApprovedBy",
                        column: x => x.ApprovedBy,
                        principalTable: "RequestManager",
                        principalColumn: "RequestManagerID");
                    table.ForeignKey(
                        name: "FK_CreateCampaignRequest_User_CreateByUser",
                        column: x => x.CreateByUser,
                        principalTable: "User",
                        principalColumn: "UserID");
                });

            migrationBuilder.CreateTable(
                name: "DonatePhase",
                columns: table => new
                {
                    DonatePhaseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CampaignId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CurrentMoney = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Percent = table.Column<double>(type: "float", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsProcessing = table.Column<bool>(type: "bit", nullable: false),
                    IsEnd = table.Column<bool>(type: "bit", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DonatePhase", x => x.DonatePhaseId);
                    table.ForeignKey(
                        name: "FK_DonatePhase_Campaign_CampaignId",
                        column: x => x.CampaignId,
                        principalTable: "Campaign",
                        principalColumn: "CampaignID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProcessingPhase",
                columns: table => new
                {
                    ProcessingPhaseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CampaignId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsProcessing = table.Column<bool>(type: "bit", nullable: false),
                    IsEnd = table.Column<bool>(type: "bit", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProcessingPhase", x => x.ProcessingPhaseId);
                    table.ForeignKey(
                        name: "FK_ProcessingPhase_Campaign_CampaignId",
                        column: x => x.CampaignId,
                        principalTable: "Campaign",
                        principalColumn: "CampaignID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StatementPhase",
                columns: table => new
                {
                    StatementPhaseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CampaignId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsProcessing = table.Column<bool>(type: "bit", nullable: false),
                    IsEnd = table.Column<bool>(type: "bit", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StatementPhase", x => x.StatementPhaseId);
                    table.ForeignKey(
                        name: "FK_StatementPhase_Campaign_CampaignId",
                        column: x => x.CampaignId,
                        principalTable: "Campaign",
                        principalColumn: "CampaignID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Transaction",
                columns: table => new
                {
                    TransactionID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CampaignID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TransactionType = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<float>(type: "real", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PayerName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsIncognito = table.Column<bool>(type: "bit", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    OrderId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BankingAccountID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    TransactionStatus = table.Column<int>(type: "int", nullable: false),
                    TransactionQRImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transaction", x => x.TransactionID);
                    table.ForeignKey(
                        name: "FK_Transaction_Account_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Account",
                        principalColumn: "AccountID");
                    table.ForeignKey(
                        name: "FK_Transaction_BankingAccount_BankingAccountID",
                        column: x => x.BankingAccountID,
                        principalTable: "BankingAccount",
                        principalColumn: "BankingAccountID");
                    table.ForeignKey(
                        name: "FK_Transaction_Campaign_CampaignID",
                        column: x => x.CampaignID,
                        principalTable: "Campaign",
                        principalColumn: "CampaignID");
                });

            migrationBuilder.CreateTable(
                name: "Activity",
                columns: table => new
                {
                    ActivityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProcessingPhaseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Activity", x => x.ActivityId);
                    table.ForeignKey(
                        name: "FK_Activity_ProcessingPhase_ProcessingPhaseId",
                        column: x => x.ProcessingPhaseId,
                        principalTable: "ProcessingPhase",
                        principalColumn: "ProcessingPhaseId");
                });

            migrationBuilder.CreateTable(
                name: "StatementFile",
                columns: table => new
                {
                    StatementFileId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StatementPhaseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Link = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StatementFile", x => x.StatementFileId);
                    table.ForeignKey(
                        name: "FK_StatementFile_StatementPhase_StatementPhaseId",
                        column: x => x.StatementPhaseId,
                        principalTable: "StatementPhase",
                        principalColumn: "StatementPhaseId");
                });

            migrationBuilder.CreateTable(
                name: "ActivityImage",
                columns: table => new
                {
                    ActivityImageId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ActivityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Link = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActivityImage", x => x.ActivityImageId);
                    table.ForeignKey(
                        name: "FK_ActivityImage_Activity_ActivityId",
                        column: x => x.ActivityId,
                        principalTable: "Activity",
                        principalColumn: "ActivityId");
                });

            migrationBuilder.CreateTable(
                name: "CreateActivityRequest",
                columns: table => new
                {
                    CreateActivityRequestID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ActivityID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreateByOM = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreateByUser = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ApprovedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsApproved = table.Column<bool>(type: "bit", nullable: false),
                    IsRejected = table.Column<bool>(type: "bit", nullable: false),
                    IsPending = table.Column<bool>(type: "bit", nullable: false),
                    IsLocked = table.Column<bool>(type: "bit", nullable: false),
                    OrganizationManagerID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UserID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RequestManagerID = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CreateActivityRequest", x => x.CreateActivityRequestID);
                    table.ForeignKey(
                        name: "FK_CreateActivityRequest_Activity_ActivityID",
                        column: x => x.ActivityID,
                        principalTable: "Activity",
                        principalColumn: "ActivityId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CreateActivityRequest_OrganizationManager_OrganizationManagerID",
                        column: x => x.OrganizationManagerID,
                        principalTable: "OrganizationManager",
                        principalColumn: "OrganizationManagerID");
                    table.ForeignKey(
                        name: "FK_CreateActivityRequest_RequestManager_RequestManagerID",
                        column: x => x.RequestManagerID,
                        principalTable: "RequestManager",
                        principalColumn: "RequestManagerID");
                    table.ForeignKey(
                        name: "FK_CreateActivityRequest_User_UserID",
                        column: x => x.UserID,
                        principalTable: "User",
                        principalColumn: "UserID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Account_Email",
                table: "Account",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Account_Username",
                table: "Account",
                column: "Username",
                unique: true,
                filter: "[Username] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AccountToken_AccountID",
                table: "AccountToken",
                column: "AccountID");

            migrationBuilder.CreateIndex(
                name: "IX_Achievement_OrganizationID",
                table: "Achievement",
                column: "OrganizationID");

            migrationBuilder.CreateIndex(
                name: "IX_Activity_ProcessingPhaseId",
                table: "Activity",
                column: "ProcessingPhaseId");

            migrationBuilder.CreateIndex(
                name: "IX_ActivityImage_ActivityId",
                table: "ActivityImage",
                column: "ActivityId");

            migrationBuilder.CreateIndex(
                name: "IX_Admin_AccountID",
                table: "Admin",
                column: "AccountID");

            migrationBuilder.CreateIndex(
                name: "IX_BankingAccount_AccountId",
                table: "BankingAccount",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_BankingAccount_OrganizationManagerID",
                table: "BankingAccount",
                column: "OrganizationManagerID");

            migrationBuilder.CreateIndex(
                name: "IX_BankingAccount_UserID",
                table: "BankingAccount",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_Campaign_CampaignTypeID",
                table: "Campaign",
                column: "CampaignTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_Campaign_OrganizationID",
                table: "Campaign",
                column: "OrganizationID");

            migrationBuilder.CreateIndex(
                name: "IX_CreateActivityRequest_ActivityID",
                table: "CreateActivityRequest",
                column: "ActivityID");

            migrationBuilder.CreateIndex(
                name: "IX_CreateActivityRequest_OrganizationManagerID",
                table: "CreateActivityRequest",
                column: "OrganizationManagerID");

            migrationBuilder.CreateIndex(
                name: "IX_CreateActivityRequest_RequestManagerID",
                table: "CreateActivityRequest",
                column: "RequestManagerID");

            migrationBuilder.CreateIndex(
                name: "IX_CreateActivityRequest_UserID",
                table: "CreateActivityRequest",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_CreateCampaignRequest_ApprovedBy",
                table: "CreateCampaignRequest",
                column: "ApprovedBy");

            migrationBuilder.CreateIndex(
                name: "IX_CreateCampaignRequest_CampaignID",
                table: "CreateCampaignRequest",
                column: "CampaignID");

            migrationBuilder.CreateIndex(
                name: "IX_CreateCampaignRequest_CreateByOM",
                table: "CreateCampaignRequest",
                column: "CreateByOM");

            migrationBuilder.CreateIndex(
                name: "IX_CreateCampaignRequest_CreateByUser",
                table: "CreateCampaignRequest",
                column: "CreateByUser");

            migrationBuilder.CreateIndex(
                name: "IX_CreateMemberRequest_ApprovedBy",
                table: "CreateMemberRequest",
                column: "ApprovedBy");

            migrationBuilder.CreateIndex(
                name: "IX_CreateMemberRequest_CreateBy",
                table: "CreateMemberRequest",
                column: "CreateBy");

            migrationBuilder.CreateIndex(
                name: "IX_CreateOrganizationManagerRequest_OrganizationManagerID",
                table: "CreateOrganizationManagerRequest",
                column: "OrganizationManagerID");

            migrationBuilder.CreateIndex(
                name: "IX_CreateOrganizationManagerRequest_RequestManagerID",
                table: "CreateOrganizationManagerRequest",
                column: "RequestManagerID");

            migrationBuilder.CreateIndex(
                name: "IX_CreateOrganizationRequest_ApprovedBy",
                table: "CreateOrganizationRequest",
                column: "ApprovedBy");

            migrationBuilder.CreateIndex(
                name: "IX_CreateOrganizationRequest_CreateBy",
                table: "CreateOrganizationRequest",
                column: "CreateBy");

            migrationBuilder.CreateIndex(
                name: "IX_CreateOrganizationRequest_OrganizationID",
                table: "CreateOrganizationRequest",
                column: "OrganizationID");

            migrationBuilder.CreateIndex(
                name: "IX_CreatePostRequest_ApprovedBy",
                table: "CreatePostRequest",
                column: "ApprovedBy");

            migrationBuilder.CreateIndex(
                name: "IX_CreatePostRequest_CreateByOM",
                table: "CreatePostRequest",
                column: "CreateByOM");

            migrationBuilder.CreateIndex(
                name: "IX_CreatePostRequest_CreateByUser",
                table: "CreatePostRequest",
                column: "CreateByUser");

            migrationBuilder.CreateIndex(
                name: "IX_CreatePostRequest_PostID",
                table: "CreatePostRequest",
                column: "PostID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DonatePhase_CampaignId",
                table: "DonatePhase",
                column: "CampaignId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Notification_AccountID",
                table: "Notification",
                column: "AccountID");

            migrationBuilder.CreateIndex(
                name: "IX_Organization_OrganizationManagerID",
                table: "Organization",
                column: "OrganizationManagerID");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationManager_AccountID",
                table: "OrganizationManager",
                column: "AccountID");

            migrationBuilder.CreateIndex(
                name: "IX_ProcessingPhase_CampaignId",
                table: "ProcessingPhase",
                column: "CampaignId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RequestManager_AccountID",
                table: "RequestManager",
                column: "AccountID");

            migrationBuilder.CreateIndex(
                name: "IX_StatementFile_StatementPhaseId",
                table: "StatementFile",
                column: "StatementPhaseId");

            migrationBuilder.CreateIndex(
                name: "IX_StatementPhase_CampaignId",
                table: "StatementPhase",
                column: "CampaignId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Transaction_AccountId",
                table: "Transaction",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Transaction_BankingAccountID",
                table: "Transaction",
                column: "BankingAccountID");

            migrationBuilder.CreateIndex(
                name: "IX_Transaction_CampaignID",
                table: "Transaction",
                column: "CampaignID");

            migrationBuilder.CreateIndex(
                name: "IX_User_AccountID",
                table: "User",
                column: "AccountID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccountToken");

            migrationBuilder.DropTable(
                name: "Achievement");

            migrationBuilder.DropTable(
                name: "ActivityImage");

            migrationBuilder.DropTable(
                name: "Admin");

            migrationBuilder.DropTable(
                name: "CreateActivityRequest");

            migrationBuilder.DropTable(
                name: "CreateCampaignRequest");

            migrationBuilder.DropTable(
                name: "CreateMemberRequest");

            migrationBuilder.DropTable(
                name: "CreateOrganizationManagerRequest");

            migrationBuilder.DropTable(
                name: "CreateOrganizationRequest");

            migrationBuilder.DropTable(
                name: "CreatePostRequest");

            migrationBuilder.DropTable(
                name: "DonatePhase");

            migrationBuilder.DropTable(
                name: "Notification");

            migrationBuilder.DropTable(
                name: "StatementFile");

            migrationBuilder.DropTable(
                name: "Transaction");

            migrationBuilder.DropTable(
                name: "Activity");

            migrationBuilder.DropTable(
                name: "Post");

            migrationBuilder.DropTable(
                name: "RequestManager");

            migrationBuilder.DropTable(
                name: "StatementPhase");

            migrationBuilder.DropTable(
                name: "BankingAccount");

            migrationBuilder.DropTable(
                name: "ProcessingPhase");

            migrationBuilder.DropTable(
                name: "User");

            migrationBuilder.DropTable(
                name: "Campaign");

            migrationBuilder.DropTable(
                name: "CampaignType");

            migrationBuilder.DropTable(
                name: "Organization");

            migrationBuilder.DropTable(
                name: "OrganizationManager");

            migrationBuilder.DropTable(
                name: "Account");
        }
    }
}
