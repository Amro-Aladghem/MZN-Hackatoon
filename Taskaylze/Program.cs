using CloudinaryDotNet;
using Database.Entites;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Services.CompanyServices;
using Services.Coordinators;
using Services.ExternalServices;
using Services.ExternalServices.UploadServices;
using Services.ApplicactionServices;
using Services.JobApplicationOfferService;
using Services.JobApplicationResultServices;
using Services.PersonServices;
using Services.RecruiterServices;
using Services.RecruiterServices.Interfaces;
using Services.SecurityServices;
using Services.SeekerServices;
using System.Security.Claims;
using System.Text;
using Taskalayze;
using Taskalayze.MiddleWares;
using Mscc.GenerativeAI;
//using Services.ExternalServices.AIServices;
//using Services.ExternalServices.AIServices.Interfaces;


namespace Taskaylze
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddScoped<PersonService>();
            builder.Services.AddScoped<RecruiterService>();
            builder.Services.AddScoped<ISecurityService,PasswordServices>();
            builder.Services.AddScoped<TokenService>();
            builder.Services.AddScoped<IUploadService,CloudinaryImageUploadService>();
            builder.Services.AddScoped<IUploadService, BlobStorageUploadService>();
            builder.Services.AddScoped<AppliactionService>();
            builder.Services.AddScoped<IRecruiterApplicationUsageService, RecruiterApplicationUsageService>();
            builder.Services.AddScoped<JobAppliactionOfferService>();
            builder.Services.AddScoped<ApplicationPostingCoordinatorService>();
            builder.Services.AddScoped<JobApplicationResultService>();
            builder.Services.AddScoped<CompanyService>();
            builder.Services.AddScoped<RecruiterCompanyCoordinatorService>();
            builder.Services.AddScoped<WhatsAppLinkService>();
            builder.Services.AddScoped<SeekerService>();
            //builder.Services.AddScoped<IAiApplicationSummery,ApplicationGemma3SummaryService>();
            builder.Services.AddScoped<JobApplicationResultQuestionsService>();
            builder.Services.AddScoped<JobApplicationSumbitCoordinatorService>();
            //builder.Services.AddScoped<IAiApplicationSumbit, ApplicationSumbitWithGeminiFlashLiteService>();
            builder.Services.AddScoped<RecruiterLaunshCoordinatorService>();
            builder.Services.AddScoped<IApplicationManibulate, ApplicationManibulationService>();


            //builder.Services.AddSingleton<GoogleAI>(sp => new GoogleAI(apiKey: builder.Configuration.GetSection("geminiApi").Value!));
            //builder.Services.AddSingleton<GenerationConfig>(gc => new GenerationConfig() { ResponseMimeType = "application/json" });


            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Configuration.AddEnvironmentVariables();


            builder.Services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration["SQL_CONNECTION_STRING"]);
            });


            var jwtOption = builder.Configuration.GetSection("JWT").Get<JwtOptions>();

            builder.Services.AddSingleton(jwtOption);


            builder.Services.AddAuthentication()
                   .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
                   {
                       options.SaveToken = true;
                       options.TokenValidationParameters = new TokenValidationParameters()
                       {
                           ValidateIssuer = true,
                           ValidIssuer = jwtOption!.Issuer,
                           ValidateAudience =true,
                           ValidAudience=jwtOption!.Audience,
                           ValidateIssuerSigningKey=true,
                           IssuerSigningKey= new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOption.SigningKey))
                       };
                   });

            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("RequiresPersonRole", policy =>
                {
                    policy.RequireRole("person");
                    policy.RequireClaim(ClaimTypes.NameIdentifier);
                });

                options.AddPolicy("RequiresSeekerRole", policy =>
                {
                    policy.RequireRole("seeker");
                    policy.RequireClaim(ClaimTypes.NameIdentifier);
                                                                    
                });

                options.AddPolicy("RequiresRecruiterRole", policy =>
                {
                    policy.RequireRole("recruiter");
                    policy.RequireClaim(ClaimTypes.NameIdentifier); 
                });
            });


            builder.Services.AddSingleton(c =>
            {
                var cloudinary = new Cloudinary(new Account(
                    builder.Configuration["CLOUDINARY_NAME"],
                    builder.Configuration["CLOUDINARY_API"],
                    builder.Configuration["CLOUDINARY_SECRET_API"]
                ));

                return cloudinary;
            });


            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll",
                    policy =>
                    {
                        policy.WithOrigins("https://taskalyze.com",
                            "https://5b89b9835354.ngrok-free.app",
                            "https://bb0d7032bb22.ngrok-free.app",
                            "https://f417c60afb37.ngrok-free.app")
                              .AllowAnyMethod()
                              .AllowAnyHeader()
                              .AllowCredentials();
                    });
            });


            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseMiddleware<HandleGlobalExeptionMiddleware>();
            
            app.UseHttpsRedirection();

            app.UseMiddleware<JwtFromCookieMiddleware>();
            app.UseAuthentication();
            app.UseAuthorization();

            app.Use(async (context, next) =>
            {
                await next.Invoke();
            });

            app.UseCors("AllowAll");
            app.MapControllers();

            app.Run();
        }
    }
}
