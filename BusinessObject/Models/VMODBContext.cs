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


        public virtual DbSet<Admin> Admins { get; set; } = default!;
        public virtual DbSet<Account> Accounts { get; set; } = default!;
        public virtual DbSet<Achievement> Achievements { get; set; } = default!;
        public virtual DbSet<Activity> Activities { get; set; } = default!;
        public virtual DbSet<ActivityImage> ActivityImages { get; set; } = default!;
        public virtual DbSet<ActivityStatementFile> ActivityStatementFiles { get; set; } = default!;
        public virtual DbSet<BankingAccount> BankingAccounts { get; set; } = default!;
        public virtual DbSet<Campaign> Campaigns { get; set; } = default!;
        public virtual DbSet<CampaignType> CampaignTypes { get; set; } = default!;
        public virtual DbSet<CreateCampaignRequest> CreateCampaignRequests { get; set; } = default!;
        public virtual DbSet<CreateVolunteerRequest> CreateVolunteerRequests { get; set; } = default!;
        public virtual DbSet<CreateOrganizationRequest> CreateOrganizationRequests { get; set; } = default!;
        public virtual DbSet<CreateActivityRequest> CreateActivityRequests { get; set; } = default!;
        public virtual DbSet<CreateOrganizationManagerRequest> CreateOrganizationManagerRequests { get; set; } = default!;
        public virtual DbSet<CreatePostRequest> CreatePostRequests { get; set; } = default!;
        public virtual DbSet<DonatePhase> DonatePhases { get; set; } = default!;
        public virtual DbSet<Notification> Notifications { get; set; } = default!;
        public virtual DbSet<Organization> Organizations { get; set; } = default!;
        public virtual DbSet<OrganizationManager> OrganizationManagers { get; set; } = default!;
        public virtual DbSet<Post> Posts { get; set; } = default!;
        public virtual DbSet<ProcessingPhase> ProcessingPhases { get; set; } = default!;
        public virtual DbSet<Moderator> Moderators { get; set; } = default!;
        public virtual DbSet<StatementFile> StatementFiles { get; set; } = default!;
        public virtual DbSet<StatementPhase> StatementPhases { get; set; } = default!;
        public virtual DbSet<Transaction> Transactions { get; set; } = default!;
        public virtual DbSet<Member> Members { get; set; } = default!;
        public virtual DbSet<AccountToken> AccountTokens { get; set; } = default!;
        public virtual DbSet<IPAddress> IPAddresses { get; set; } = default!;
        public virtual DbSet<ProcessingPhaseStatementFile> ProcessingPhaseStatementFiles { get; set; } = default!;




        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .UseLazyLoadingProxies(false).UseSqlServer(GetConnectionString());
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
            modelBuilder.ApplyConfiguration(new ActivityStatementFileConfiguration());
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
