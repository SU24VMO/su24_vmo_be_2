using BusinessObject.FluentAPIs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Models
{
    public class VMODBContext : DbContext
    {
        public VMODBContext() : base()
        {

        }


        public DbSet<Admin> Admins { get; set; } = default!;
        public DbSet<Account> Accounts { get; set; } = default!;
        public DbSet<Achievement> Achievements { get; set; } = default!;
        public DbSet<Activity> Activities { get; set; } = default!;
        public DbSet<ActivityImage> ActivityImages { get; set; } = default!;
        public DbSet<BankingAccount> BankingAccounts { get; set; } = default!;
        public DbSet<Campaign> Campaigns { get; set; } = default!;
        public DbSet<CampaignType> CampaignTypes { get; set; } = default!;
        public DbSet<CreateCampaignRequest> CreateCampaignRequests { get; set; } = default!;
        public DbSet<CreateVolunteerRequest> CreateVolunteerRequests { get; set; } = default!;
        public DbSet<CreateOrganizationRequest> CreateOrganizationRequests { get; set; } = default!;
        public DbSet<CreateActivityRequest> CreateActivityRequests { get; set; } = default!;
        public DbSet<CreateOrganizationManagerRequest> CreateOrganizationManagerRequests { get; set; } = default!;
        public DbSet<CreatePostRequest> CreatePostRequests { get; set; } = default!;
        public DbSet<DonatePhase> DonatePhases { get; set; } = default!;
        public DbSet<Notification> Notifications { get; set; } = default!;
        public DbSet<Organization> Organizations { get; set; } = default!;
        public DbSet<OrganizationManager> OrganizationManagers { get; set; } = default!;
        public DbSet<Post> Posts { get; set; } = default!;
        public DbSet<ProcessingPhase> ProcessingPhases { get; set; } = default!;
        public DbSet<Moderator> Moderators { get; set; } = default!;
        public DbSet<StatementFile> StatementFiles { get; set; } = default!;
        public DbSet<StatementPhase> StatementPhases { get; set; } = default!;
        public DbSet<Transaction> Transactions { get; set; } = default!;
        public DbSet<Member> Members { get; set; } = default!;
        public DbSet<AccountToken> AccountTokens { get; set; } = default!;
        public DbSet<IPAddress> IPAddresses { get; set; } = default!;
        public DbSet<ProcessingPhaseStatementFile> ProcessingPhaseStatementFiles { get; set; } = default!;




        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .UseLazyLoadingProxies().UseSqlServer(GetConnectionString());
        }

//#if DEBUG
//        private string GetConnectionString()
//        {
//            IConfiguration config = new ConfigurationBuilder()
//                .SetBasePath(Directory.GetCurrentDirectory())
//                .AddJsonFile("appsettings.Development.json", true, true)
//                .Build();
//            return config["ConnectionStrings:DB"]!;
//        }
//#else
        private string GetConnectionString()
        {
            IConfiguration config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", true, true)
                .Build();
            return config["ConnectionStrings:DB"]!;
        }
//#endif


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Notification>()
                .Property(stage => stage.NotificationCategory)
                .HasConversion<int>();

            modelBuilder.Entity<Account>()
                .Property(stage => stage.Role)
                .HasConversion<int>();

            modelBuilder.Entity<Transaction>()
                .Property(stage => stage.TransactionStatus)
                .HasConversion<int>();

            modelBuilder.Entity<Transaction>()
                .Property(stage => stage.TransactionType)
                .HasConversion<int>();
            modelBuilder.Entity<CreateVolunteerRequest>()
                .Property(stage => stage.RoleInClub)
                .HasConversion<int>();
            modelBuilder.Entity<Campaign>()
                .Property(stage => stage.CampaignTier)
                .HasConversion<int>();

            modelBuilder.ApplyConfiguration(new AccountConfiguration());
            modelBuilder.ApplyConfiguration(new AccountTokenConfiguration());
            modelBuilder.ApplyConfiguration(new AchievementConfiguration());
            modelBuilder.ApplyConfiguration(new ActivityConfiguration());
            modelBuilder.ApplyConfiguration(new ActivityImageConfiguration());
            modelBuilder.ApplyConfiguration(new AdminConfiguration());
            modelBuilder.ApplyConfiguration(new BankingAccountConfiguration());
            modelBuilder.ApplyConfiguration(new CampaignConfiguration());
            modelBuilder.ApplyConfiguration(new CampaignTypeConfiguration());
            modelBuilder.ApplyConfiguration(new CreateCampaignRequestConfiguration());
            modelBuilder.ApplyConfiguration(new CreateVolunteerRequestConfiguration());
            modelBuilder.ApplyConfiguration(new CreateOrganizationRequestConfiguration());
            modelBuilder.ApplyConfiguration(new CreateActivityRequestConfiguration());
            modelBuilder.ApplyConfiguration(new CreateOrganizationManagerRequestConfiguration());
            modelBuilder.ApplyConfiguration(new CreatePostRequestConfiguration());
            modelBuilder.ApplyConfiguration(new DonatePhaseConfiguration());
            modelBuilder.ApplyConfiguration(new NotificationConfiguration());
            modelBuilder.ApplyConfiguration(new OrganizationConfiguration());
            modelBuilder.ApplyConfiguration(new OrganizationManagerConfiguration());
            modelBuilder.ApplyConfiguration(new PostConfiguration());
            modelBuilder.ApplyConfiguration(new ProcessingPhaseConfiguration());
            modelBuilder.ApplyConfiguration(new ModeratorConfiguration());
            modelBuilder.ApplyConfiguration(new StatementFileConfiguration());
            modelBuilder.ApplyConfiguration(new StatementPhaseConfiguration());
            modelBuilder.ApplyConfiguration(new TransactionConfiguration());
            modelBuilder.ApplyConfiguration(new MemberConfiguration());
            modelBuilder.ApplyConfiguration(new IPAddressConfiguration());
            modelBuilder.ApplyConfiguration(new ProcessingPhaseStatementFileConfiguration());


        }

    }
}
