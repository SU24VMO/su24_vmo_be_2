using BusinessObject.Models;
using MaxMind.GeoIP2;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Repository.Implements;
using Repository.Interfaces;
using SU24_VMO_API.Services;
using SU24_VMO_API.Supporters.JWTAuthSupport;
using SU24_VMO_API.Supporters.Middlewares;
using System.Text;
using System.Text.Json.Serialization;
using SU24_VMO_API_2.Services;
using SU24_VMO_API_2.Supporters;

namespace SU24_VMO_API_2
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

#if DEBUG


            // Add services to the container.
            builder.Configuration.AddJsonFile("appsettings.Development.json", optional: false, reloadOnChange: true);
            //builder.Services.AddSingleton(serviceProvider =>
            //{
            //    var env = builder.Environment;
            //    var databasePath = Path.Combine(env.ContentRootPath, "Supporters/AppData", "GeoLite2-City.mmdb");
            //    return new DatabaseReader(databasePath);
            //});
#else 
            // Add services to the container.
            builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
#endif
            builder.Services.AddControllers().AddJsonOptions(x =>
                    x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles)
                .AddOData(option => option.Select().Filter()
                .Count().OrderBy().Expand().SetMaxTop(100).AddRouteComponents("odata", GetEdmModel()));
            builder.Services.AddODataQueryFilter();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });

            builder.Services.AddRouting(options =>
            {
                options.LowercaseUrls = true;
            });
            //add jwt bearer to swagger
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "VMO API", Version = "v1" });
                c.IgnoreObsoleteActions();
                c.IgnoreObsoleteProperties();
                c.DocInclusionPredicate((name, api) => true);
                c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = @"JWT Authorization header using the Bearer scheme. \r\n\r\n
                      Enter 'Bearer' [space] and then your token in the text input below.
                      \r\n\r\nExample: 'Bearer 12345abcdef'",
                    Name = "Authorization",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement()
                  {
                    {
                      new OpenApiSecurityScheme
                      {
                        Reference = new OpenApiReference
                          {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                          },
                          Scheme = "oauth2",
                          Name = "Bearer",
                          In = ParameterLocation.Header,
                        },
                        new List<string>()
                      }
                    });
            });





            //repository
            builder.Services.AddScoped<IAdminRepository, AdminRepository>();
            builder.Services.AddScoped<IAccountRepository, AccountRepository>();
            builder.Services.AddScoped<IAchievementRepository, AchievementRepository>();
            builder.Services.AddScoped<IActivityImageRepository, ActivityImageRepository>();
            builder.Services.AddScoped<IActivityRepository, ActivityRepository>();
            builder.Services.AddScoped<IBankingAccountRepository, BankingAccountRepository>();
            builder.Services.AddScoped<ICreateActivityRequestRepository, CreateActivityRequestRepository>();
            builder.Services.AddScoped<ICreateOrganizationManagerRequestRepository, CreateOrganizationManagerRequestRepository>();
            builder.Services.AddScoped<ICreateOrganizationRequestRepository, CreateOrganizationRequestRepository>();
            builder.Services.AddScoped<ICreatePostRequestRepository, CreatePostRequestRepository>();
            builder.Services.AddScoped<IPostRepository, PostRepository>();
            builder.Services.AddScoped<IOrganizationRepository, OrganizationRepository>();
            builder.Services.AddScoped<IPostRepository, PostRepository>();
            builder.Services.AddScoped<IStatementFileRepository, StatementFileRepository>();
            builder.Services.AddScoped<IMemberRepository, MemberRepository>();
            builder.Services.AddScoped<ICampaignRepository, CampaignRepository>();
            builder.Services.AddScoped<ICampaignTypeRepository, CampaignTypeRepository>();
            builder.Services.AddScoped<IOrganizationManagerRepository, OrganizationManagerRepository>();
            builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();
            builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
            builder.Services.AddScoped<IModeratorRepository, ModeratorRepository>();
            builder.Services.AddScoped<ICreateVolunteerRequestRepository, CreateVolunteerRequestRepository>();
            builder.Services.AddScoped<ICreateCampaignRequestRepository, CreateCampaignRequestRepository>();
            builder.Services.AddScoped<IAccountTokenRepository, AccountTokenRepository>();
            builder.Services.AddScoped<IDonatePhaseRepository, DonatePhaseRepository>();
            builder.Services.AddScoped<IProcessingPhaseRepository, ProcessingPhaseRepository>();
            builder.Services.AddScoped<IStatementPhaseRepository, StatementPhaseRepository>();
            builder.Services.AddScoped<IDBTransactionRepository, DBTransactionRepository>();
            builder.Services.AddScoped<IIPAddressRepository, IPAddressRepository>();
            builder.Services.AddScoped<IProcessingPhaseStatementFileRepository, ProcessingPhaseStatementFileRepository>();
            builder.Services.AddScoped<IActivityStatementFileRepository, ActivityStatementFileRepository>();







            //service
            builder.Services.AddScoped<MemberService, MemberService>();
            builder.Services.AddScoped<OrganizationManagerService, OrganizationManagerService>();
            builder.Services.AddScoped<ModeratorService, ModeratorService>();
            builder.Services.AddScoped<CampaignTypeService, CampaignTypeService>();
            builder.Services.AddScoped<CampaignService, CampaignService>();
            builder.Services.AddScoped<TransactionService, TransactionService>();
            builder.Services.AddScoped<CreateVolunteerRequestService, CreateVolunteerRequestService>();
            builder.Services.AddScoped<CreateCampaignRequestService, CreateCampaignRequestService>();
            builder.Services.AddScoped<JwtTokenSupporter, JwtTokenSupporter>();
            builder.Services.AddScoped<AccountService, AccountService>();
            builder.Services.AddScoped<AccountTokenService, AccountTokenService>();
            builder.Services.AddScoped<AchievementService, AchievementService>();
            builder.Services.AddScoped<ActivityService, ActivityService>();
            builder.Services.AddScoped<ActivityImageService, ActivityImageService>();
            builder.Services.AddScoped<BankingAccountService, BankingAccountService>();
            builder.Services.AddScoped<CreateActivityRequestService, CreateActivityRequestService>();
            builder.Services.AddScoped<CreateOrganizationManagerRequestService, CreateOrganizationManagerRequestService>();
            builder.Services.AddScoped<CreateOrganizationRequestService, CreateOrganizationRequestService>();
            builder.Services.AddScoped<CreatePostRequestService, CreatePostRequestService>();
            builder.Services.AddScoped<DonatePhaseService, DonatePhaseService>();
            builder.Services.AddScoped<NotificationService, NotificationService>();
            builder.Services.AddScoped<OrganizationManagerService, OrganizationManagerService>();
            builder.Services.AddScoped<OrganizationService, OrganizationService>();
            builder.Services.AddScoped<PostService, PostService>();
            builder.Services.AddScoped<ProcessingPhaseService, ProcessingPhaseService>();
            builder.Services.AddScoped<StatementFileService, StatementFileService>();
            builder.Services.AddScoped<StatementPhaseService, StatementPhaseService>();
            builder.Services.AddScoped<DBTransactionService, DBTransactionService>();
            builder.Services.AddScoped<FirebaseService, FirebaseService>();
            builder.Services.AddScoped(typeof(PaginationService<>), typeof(PaginationService<>));
            builder.Services.AddScoped<DonatePhaseService, DonatePhaseService>();
            builder.Services.AddScoped<IPAddressService, IPAddressService>();

            builder.Services.AddHttpContextAccessor();
            builder.Services.AddTransient<IpAddressHelper>();




            builder.Services.AddMvc().AddNewtonsoftJson(options => { options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore; });


            builder.Services.AddCors(options =>
            {
                options.AddDefaultPolicy(builder =>
                {
                    builder.AllowAnyOrigin()
                           .AllowAnyMethod()
                           .AllowAnyHeader();
                });
            });
            builder.Services.AddAuthentication((options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })).AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidAudience = builder.Configuration["Jwt:Audience"],
                    ValidIssuer = builder.Configuration["Jwt:Issuer"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
                };
            });


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "VMO API V1");
                });
            }
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "VMO API V1");
                c.RoutePrefix = string.Empty; // Set the root path for Swagger UI
            });






            //cors
            app.UseCors(options => options.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());

            app.UseAuthentication();
            app.UseHttpsRedirection();
            app.UseODataBatching();
            app.UseRouting();
            app.UseAuthorization();
            app.UseMiddleware<DbTransactionMiddleware>();
            app.MapControllers();

            app.Run();


            static IEdmModel GetEdmModel()
            {
                ODataConventionModelBuilder builder = new ODataConventionModelBuilder();
                builder.EntitySet<Admin>("Admins");
                builder.EntitySet<Member>("Members");


                return builder.GetEdmModel();
            }
        }
    }
}
