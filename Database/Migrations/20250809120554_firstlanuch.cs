using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class firstlanuch : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Application_Offers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(350)", maxLength: 350, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(350)", maxLength: 350, nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NumberOfTasks = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Application_Offers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Application_Result_Statuses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(350)", maxLength: 350, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Application_Result_Statuses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ApplicationTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(350)", maxLength: 350, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Countries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(350)", maxLength: 350, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Countries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Job_Levels",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(350)", maxLength: 350, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Job_Levels", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Job_Types",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(350)", maxLength: 350, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Job_Types", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SubscriptionTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(350)", maxLength: 350, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubscriptionTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Universities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(350)", maxLength: 350, nullable: false),
                    image = table.Column<string>(type: "nvarchar(350)", maxLength: 350, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Universities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(350)", maxLength: 350, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Governorates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<int>(type: "int", nullable: false),
                    CountryId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Governorates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Governorates_Countries_CountryId",
                        column: x => x.CountryId,
                        principalTable: "Countries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Phone_Codes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(350)", maxLength: 350, nullable: false),
                    CountryId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Phone_Codes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Phone_Codes_Countries_CountryId",
                        column: x => x.CountryId,
                        principalTable: "Countries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Companies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(350)", maxLength: 350, nullable: false),
                    ImageURL = table.Column<string>(type: "nvarchar(350)", maxLength: 350, nullable: true),
                    CountryId = table.Column<int>(type: "int", nullable: true),
                    GovernorateId = table.Column<int>(type: "int", nullable: true),
                    Linked_In_Link = table.Column<string>(type: "nvarchar(350)", maxLength: 350, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Companies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Companies_Countries_CountryId",
                        column: x => x.CountryId,
                        principalTable: "Countries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Companies_Governorates_GovernorateId",
                        column: x => x.GovernorateId,
                        principalTable: "Governorates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Persons",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(type: "nvarchar(350)", maxLength: 350, nullable: true),
                    LastName = table.Column<string>(type: "nvarchar(350)", maxLength: 350, nullable: true),
                    ImageURL = table.Column<string>(type: "nvarchar(350)", maxLength: 350, nullable: true),
                    Google_Id = table.Column<string>(type: "nvarchar(350)", maxLength: 350, nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(350)", maxLength: 350, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(350)", maxLength: 350, nullable: false),
                    Password = table.Column<string>(type: "nvarchar(350)", maxLength: 350, nullable: true),
                    CountryId = table.Column<int>(type: "int", nullable: true),
                    GovernorateId = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    UserTypeId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Persons", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Persons_Countries_CountryId",
                        column: x => x.CountryId,
                        principalTable: "Countries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Persons_Governorates_GovernorateId",
                        column: x => x.GovernorateId,
                        principalTable: "Governorates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Persons_UserTypes_UserTypeId",
                        column: x => x.UserTypeId,
                        principalTable: "UserTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Recruiters",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PersonId = table.Column<int>(type: "int", nullable: false),
                    CompanyId = table.Column<int>(type: "int", nullable: true),
                    Linked_In_Link = table.Column<string>(type: "nvarchar(350)", maxLength: 350, nullable: true),
                    LastLoggedInTime = table.Column<DateTime>(type: "datetime2(0)", nullable: true),
                    IsFreelancer = table.Column<bool>(type: "bit", nullable: false),
                    IsProfileComplete = table.Column<bool>(type: "bit", nullable: false),
                    IsInstructor = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    UnviersityId = table.Column<int>(type: "int", nullable: true),
                    UniversityId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Recruiters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Recruiters_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Recruiters_Persons_PersonId",
                        column: x => x.PersonId,
                        principalTable: "Persons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Recruiters_Universities_UniversityId",
                        column: x => x.UniversityId,
                        principalTable: "Universities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Seekers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PersonId = table.Column<int>(type: "int", nullable: false),
                    Job_TypeId = table.Column<int>(type: "int", nullable: true),
                    Job_LevelId = table.Column<int>(type: "int", nullable: true),
                    ResumeLink = table.Column<string>(type: "nvarchar(350)", maxLength: 350, nullable: true),
                    LinkedInLink = table.Column<string>(type: "nvarchar(350)", maxLength: 350, nullable: true),
                    IsProfileCompleted = table.Column<bool>(type: "bit", nullable: false),
                    LastLoggedInTime = table.Column<DateTime>(type: "datetime2(0)", nullable: false, defaultValueSql: "GETDATE()"),
                    IsStudent = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    UniversityId = table.Column<int>(type: "int", nullable: true),
                    UniversityStudentNumber = table.Column<string>(type: "nvarchar(350)", maxLength: 350, nullable: true),
                    StudyingYear = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Seekers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Seekers_Job_Levels_Job_LevelId",
                        column: x => x.Job_LevelId,
                        principalTable: "Job_Levels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Seekers_Job_Types_Job_TypeId",
                        column: x => x.Job_TypeId,
                        principalTable: "Job_Types",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Seekers_Persons_PersonId",
                        column: x => x.PersonId,
                        principalTable: "Persons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Seekers_Universities_UniversityId",
                        column: x => x.UniversityId,
                        principalTable: "Universities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Applications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Subject = table.Column<string>(type: "nvarchar(350)", maxLength: 350, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(2500)", maxLength: 2500, nullable: false),
                    File_Link = table.Column<string>(type: "nvarchar(350)", maxLength: 350, nullable: true),
                    RecruiterId = table.Column<int>(type: "int", nullable: false),
                    Job_LevelId = table.Column<int>(type: "int", nullable: false),
                    Job_TypeId = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    TimeToComplete = table.Column<TimeSpan>(type: "time", nullable: false),
                    DateOfCreation = table.Column<DateTime>(type: "datetime2(0)", nullable: false),
                    NumbersOfApplied = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    AiSummary = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    ApplicationTypeId = table.Column<int>(type: "int", nullable: false),
                    UniversityId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Applications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Applications_ApplicationTypes_ApplicationTypeId",
                        column: x => x.ApplicationTypeId,
                        principalTable: "ApplicationTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Applications_Job_Levels_Job_LevelId",
                        column: x => x.Job_LevelId,
                        principalTable: "Job_Levels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Applications_Job_Types_Job_TypeId",
                        column: x => x.Job_TypeId,
                        principalTable: "Job_Types",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Applications_Recruiters_RecruiterId",
                        column: x => x.RecruiterId,
                        principalTable: "Recruiters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Applications_Universities_UniversityId",
                        column: x => x.UniversityId,
                        principalTable: "Universities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Recruiter_Application_Usages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RecruiterId = table.Column<int>(type: "int", nullable: false),
                    AvailableApplicationsNumber = table.Column<int>(type: "int", nullable: false),
                    TotalApplications = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Recruiter_Application_Usages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Recruiter_Application_Usages_Recruiters_RecruiterId",
                        column: x => x.RecruiterId,
                        principalTable: "Recruiters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Application_Results",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SeekerId = table.Column<int>(type: "int", nullable: false),
                    Application_Result_StatusId = table.Column<int>(type: "int", nullable: false),
                    TakenTimeToComplete = table.Column<TimeSpan>(type: "time", nullable: true),
                    Result = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ApplicationId = table.Column<int>(type: "int", nullable: false),
                    DateAndTimeOfJoined = table.Column<DateTime>(type: "datetime2(0)", nullable: false, defaultValueSql: "GETDATE()"),
                    SolutionFileUri = table.Column<string>(type: "nvarchar(350)", maxLength: 350, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Application_Results", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Application_Results_Application_Result_Statuses_Application_Result_StatusId",
                        column: x => x.Application_Result_StatusId,
                        principalTable: "Application_Result_Statuses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Application_Results_Applications_ApplicationId",
                        column: x => x.ApplicationId,
                        principalTable: "Applications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Application_Results_Seekers_SeekerId",
                        column: x => x.SeekerId,
                        principalTable: "Seekers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Application_Task_Payments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserTypeId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    PaidAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Date = table.Column<DateOnly>(type: "date", nullable: false, defaultValueSql: "CAST(GETDATE() AS DATE)"),
                    Time = table.Column<DateTime>(type: "datetime2(0)", nullable: false, defaultValueSql: "CAST(GETDATE() AS TIME)"),
                    SubscriptionTypeId = table.Column<int>(type: "int", nullable: true),
                    ApplicationId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Application_Task_Payments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Application_Task_Payments_Applications_ApplicationId",
                        column: x => x.ApplicationId,
                        principalTable: "Applications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Application_Task_Payments_SubscriptionTypes_SubscriptionTypeId",
                        column: x => x.SubscriptionTypeId,
                        principalTable: "SubscriptionTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Application_Result_Questions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AnswerCorrectChoiceNum = table.Column<int>(type: "int", nullable: false),
                    Application_ResultId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Application_Result_Questions", x => x.Id);
                    table.CheckConstraint("CK_AnswerCorrectChoiceNum_ValidRange", "[AnswerCorrectChoiceNum] BETWEEN 1 AND 4");
                    table.ForeignKey(
                        name: "FK_Application_Result_Questions_Application_Results_Application_ResultId",
                        column: x => x.Application_ResultId,
                        principalTable: "Application_Results",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "ApplicationTypes",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Job_Appliaction" },
                    { 2, "University_Application" },
                    { 3, "Assesment_Application" }
                });

            migrationBuilder.InsertData(
                table: "Application_Result_Statuses",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Accepted" },
                    { 2, "under-checked" },
                    { 3, "Not-Accepted" },
                    { 4, "pending" },
                    { 5, "banned" }
                });

            migrationBuilder.InsertData(
                table: "Job_Levels",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Junior" },
                    { 2, "Intermediate" },
                    { 3, "Senior" },
                    { 4, "Lead" },
                    { 5, "Principal" }
                });

            migrationBuilder.InsertData(
                table: "UserTypes",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Seeker" },
                    { 2, "Recruiter" },
                    { 3, "Person" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Application_Result_Questions_Application_ResultId",
                table: "Application_Result_Questions",
                column: "Application_ResultId");

            migrationBuilder.CreateIndex(
                name: "IX_Application_Results_Application_Result_StatusId",
                table: "Application_Results",
                column: "Application_Result_StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Application_Results_ApplicationId",
                table: "Application_Results",
                column: "ApplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_Application_Results_SeekerId",
                table: "Application_Results",
                column: "SeekerId");

            migrationBuilder.CreateIndex(
                name: "IX_Application_Task_Payments_ApplicationId",
                table: "Application_Task_Payments",
                column: "ApplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_Application_Task_Payments_SubscriptionTypeId",
                table: "Application_Task_Payments",
                column: "SubscriptionTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Applications_ApplicationTypeId",
                table: "Applications",
                column: "ApplicationTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Applications_Job_LevelId",
                table: "Applications",
                column: "Job_LevelId");

            migrationBuilder.CreateIndex(
                name: "IX_Applications_Job_TypeId",
                table: "Applications",
                column: "Job_TypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Applications_RecruiterId",
                table: "Applications",
                column: "RecruiterId");

            migrationBuilder.CreateIndex(
                name: "IX_Applications_UniversityId",
                table: "Applications",
                column: "UniversityId");

            migrationBuilder.CreateIndex(
                name: "IX_Companies_CountryId",
                table: "Companies",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_Companies_GovernorateId",
                table: "Companies",
                column: "GovernorateId");

            migrationBuilder.CreateIndex(
                name: "IX_Governorates_CountryId",
                table: "Governorates",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_Persons_CountryId",
                table: "Persons",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_Persons_GovernorateId",
                table: "Persons",
                column: "GovernorateId");

            migrationBuilder.CreateIndex(
                name: "IX_Persons_UserTypeId",
                table: "Persons",
                column: "UserTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Phone_Codes_CountryId",
                table: "Phone_Codes",
                column: "CountryId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Recruiter_Application_Usages_RecruiterId",
                table: "Recruiter_Application_Usages",
                column: "RecruiterId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Recruiters_CompanyId",
                table: "Recruiters",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Recruiters_PersonId",
                table: "Recruiters",
                column: "PersonId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Recruiters_UniversityId",
                table: "Recruiters",
                column: "UniversityId");

            migrationBuilder.CreateIndex(
                name: "IX_Seekers_Job_LevelId",
                table: "Seekers",
                column: "Job_LevelId");

            migrationBuilder.CreateIndex(
                name: "IX_Seekers_Job_TypeId",
                table: "Seekers",
                column: "Job_TypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Seekers_PersonId",
                table: "Seekers",
                column: "PersonId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Seekers_UniversityId",
                table: "Seekers",
                column: "UniversityId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Application_Offers");

            migrationBuilder.DropTable(
                name: "Application_Result_Questions");

            migrationBuilder.DropTable(
                name: "Application_Task_Payments");

            migrationBuilder.DropTable(
                name: "Phone_Codes");

            migrationBuilder.DropTable(
                name: "Recruiter_Application_Usages");

            migrationBuilder.DropTable(
                name: "Application_Results");

            migrationBuilder.DropTable(
                name: "SubscriptionTypes");

            migrationBuilder.DropTable(
                name: "Application_Result_Statuses");

            migrationBuilder.DropTable(
                name: "Applications");

            migrationBuilder.DropTable(
                name: "Seekers");

            migrationBuilder.DropTable(
                name: "ApplicationTypes");

            migrationBuilder.DropTable(
                name: "Recruiters");

            migrationBuilder.DropTable(
                name: "Job_Levels");

            migrationBuilder.DropTable(
                name: "Job_Types");

            migrationBuilder.DropTable(
                name: "Companies");

            migrationBuilder.DropTable(
                name: "Persons");

            migrationBuilder.DropTable(
                name: "Universities");

            migrationBuilder.DropTable(
                name: "Governorates");

            migrationBuilder.DropTable(
                name: "UserTypes");

            migrationBuilder.DropTable(
                name: "Countries");
        }
    }
}
