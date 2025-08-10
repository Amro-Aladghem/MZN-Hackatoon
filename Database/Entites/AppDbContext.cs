using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database.Entites
{
    public class AppDbContext :DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    optionsBuilder.UseSqlServer("");
        //}
        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            configurationBuilder.Properties<string>().HaveMaxLength(350);

            configurationBuilder.Properties<DateTime>().HaveColumnType("datetime2(0)");
        }

        public DbSet<Application_Result_Status> Application_Result_Statuses { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<Governorate> Governorates { get; set; }   
        public DbSet<Application> Applications { get; set; }
        public DbSet<Application_Result> Application_Results { get; set; }
        public DbSet<Job_Level> Job_Levels { get; set; }
        public DbSet<Job_Type> Job_Types { get; set; }
        public DbSet<Person> Persons { get; set; }
        public DbSet<Phone_Code> Phone_Codes { get; set; }
        public DbSet<Recruiter> Recruiters { get; set; }
        public DbSet<Seeker> Seekers { get; set; }
        public DbSet<UserType> UserTypes { get; set; }
        public DbSet<SubscriptionType> SubscriptionTypes { get; set; }
        public DbSet<Recruiter_Application_Usage> Recruiter_Application_Usages { get; set; }
        public DbSet<Application_Task_Payment> Application_Task_Payments { get; set; }
        public DbSet<Application_Offer> Application_Offers { get; set; }
        public DbSet<Application_Result_Question> Application_Result_Questions { get; set; }
        public DbSet<University> Universities { get; set; }
        public DbSet<ApplicationType> ApplicationTypes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Application_Result>().HasOne(JR => JR.application_Result_Status)
                                                         .WithMany(S => S.Application_Results);

            modelBuilder.Entity<Application>().HasMany(J => J.Application_Results)
                                                  .WithOne(JR => JR.Application);


            modelBuilder.Entity<Application>().HasOne(JA => JA.Job_Type)
                                                  .WithMany(JT => JT.Job_Applications);


            modelBuilder.Entity<Seeker>().HasMany(S => S.Application_Results)
                                         .WithOne(JR => JR.Seeker);


            modelBuilder.Entity<Job_Level>().HasMany(JL => JL.Job_Applications)
                                            .WithOne(JA => JA.Job_Level);


            modelBuilder.Entity<UserType>().HasData(
               new UserType() { Id = 1, Name = "Seeker" },
               new UserType() { Id = 2, Name = "Recruiter" },
               new UserType() { Id=3,Name="Person"}
            );

            modelBuilder.Entity<Application_Task_Payment>().Property(A => A.Date)
                                                           .HasDefaultValueSql("CAST(GETDATE() AS DATE)");

            modelBuilder.Entity<Application_Task_Payment>().Property(A => A.Time)
                                                          .HasDefaultValueSql("CAST(GETDATE() AS TIME)");

            modelBuilder.Entity<Seeker>().Property(S => S.LastLoggedInTime).HasDefaultValueSql("GETDATE()");


            modelBuilder.Entity<Job_Level>().HasData(
                new { Id = 1, Name = "Junior" },
                new { Id = 2, Name = "Intermediate" },
                new { Id = 3, Name = "Senior" },
                new { Id = 4, Name = "Lead" },
                new { Id = 5, Name = "Principal" }
            );

            modelBuilder.Entity<Application_Result_Status>().HasData(
                new { Id = 1, Name = "Accepted" },
                new { Id = 2, Name = "under-checked" },
                new { Id = 3, Name = "Not-Accepted" },
                new { Id = 4, Name = "pending" },
                new { Id = 5, Name = "banned" }
            );

            modelBuilder.Entity<Application>().Property(P => P.IsActive).HasDefaultValue(true);
            modelBuilder.Entity<Application>().Property(P => P.NumbersOfApplied).HasDefaultValue(0);

            modelBuilder.Entity<Application>().Property(P => P.Description).HasMaxLength(2500);

            modelBuilder.Entity<Application>().Property(P => P.AiSummary).HasMaxLength(4000);

            modelBuilder.Entity<Application_Result_Question>()
                        .ToTable(t => t.HasCheckConstraint("CK_AnswerCorrectChoiceNum_ValidRange", "[AnswerCorrectChoiceNum] BETWEEN 1 AND 4"));

            modelBuilder.Entity<Application_Result>()
                        .Property(P => P.TakenTimeToComplete)
                        .IsRequired(false);

            modelBuilder.Entity<Application_Result>().Property(P => P.DateAndTimeOfJoined)
                                                         .HasDefaultValueSql("GETDATE()");


            modelBuilder.Entity<ApplicationType>().HasData(
                new ApplicationType() { Id = 1,Name="Job_Appliaction" },
                new ApplicationType() { Id=2,Name="University_Application"},
                new ApplicationType() { Id=3,Name="Assesment_Application"}
            );

            modelBuilder.Entity<Recruiter>().Property(p => p.IsInstructor).HasDefaultValue(false);
            modelBuilder.Entity<Seeker>().Property(p=>p.IsStudent).HasDefaultValue(false);

            modelBuilder.Entity<University>().HasData(
                new University() { Id = 1, Name = "Hashemit University" }
            );

            modelBuilder.Entity<Job_Type>().HasData(
                new Job_Type() { Id = 1, Name = "Frontend Developer" },
                new Job_Type() { Id = 2, Name = "Back-end Developer" },
                new Job_Type() { Id = 3, Name = "Full-stack Developer" },
                new Job_Type() { Id = 4, Name = "Mobile Developer" },
                new Job_Type() { Id = 5, Name = "DevOps Engineer" },
                new Job_Type() { Id = 6, Name = "Quality Assurance (QA) Engineer" },
                new Job_Type() { Id = 7, Name = "Data Scientist" },
                new Job_Type() { Id = 8, Name = "Data Analyst" },
                new Job_Type() { Id = 9, Name = "Database Administrator (DBA)" },
                new Job_Type() { Id = 10, Name = "Network Administrator" },
                new Job_Type() { Id = 11, Name = "System Administrator (SysAdmin)" },
                new Job_Type() { Id = 12, Name = "Cybersecurity Analyst" },
                new Job_Type() { Id = 13, Name = "Cloud Engineer" },
                new Job_Type() { Id = 14, Name = "AI/ML Engineer" },
                new Job_Type() { Id = 15, Name = "Business Analyst (BA)" },
                new Job_Type() { Id = 16, Name = "IT Project Manager" },
                new Job_Type() { Id = 17, Name = "UI/UX Designer" },
                new Job_Type() { Id = 18, Name = "Technical Support Specialist" },
                new Job_Type() { Id = 19, Name = "IT Support Specialist" },
                new Job_Type() { Id = 20, Name = "Solutions Architect" },
                new Job_Type() { Id = 21, Name = "Technical Writer" },
                new Job_Type() { Id = 22, Name = "Student" }
            );


            modelBuilder.Entity<Country>().HasData(
                new Country() { Id = 1, Name = "jordan" }
                );


            foreach (var foreignKey in modelBuilder.Model
                                           .GetEntityTypes()
                                           .SelectMany(e => e.GetForeignKeys()))
            {
                foreignKey.DeleteBehavior = DeleteBehavior.Restrict;
            }
        }
    }
}
