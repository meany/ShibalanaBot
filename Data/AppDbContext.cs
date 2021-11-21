using dm.Shibalana.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace dm.Shibalana.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<AddressInfo> AddressInfos { get; set; }
        public DbSet<Price> Prices { get; set; }
        public DbSet<Request> Requests { get; set; }
        public DbSet<Stat> Stats { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AddressInfo>()
                .HasIndex(x => x.IsPartOfTeam);

            modelBuilder.Entity<AddressInfo>().HasData(new AddressInfo
            {
                AddressInfoId = 1,
                Address = "2vQBVYD6fn1Z4iA2JWo3qY1tMBanspkHY4TfLe51hj9b",
                IsPartOfTeam = true,
                Label = "Team"
            });
            modelBuilder.Entity<AddressInfo>().HasData(new AddressInfo
            {
                AddressInfoId = 2,
                Address = "Ewg558ARXCoEtCHeGmicUznRWvbae6eczGUFS1tkPBX8",
                IsPartOfTeam = true,
                Label = "Staking"
            });
            modelBuilder.Entity<AddressInfo>().HasData(new AddressInfo
            {
                AddressInfoId = 3,
                Address = "A8GFDkvqg6WLGTsXksiabD3oKeB2aQXtNoG98R9Hr5QG",
                IsPartOfTeam = true,
                Label = "Marketing"
            });
            modelBuilder.Entity<AddressInfo>().HasData(new AddressInfo
            {
                AddressInfoId = 4,
                Address = "9huAyo2PytpiqqNDvvd5tb2nQNXpjE6xKS81M4BdeVES",
                IsPartOfTeam = true,
                Label = "Liquidity"
            });

            modelBuilder.Entity<Price>()
                .HasIndex(x => x.Group);
            modelBuilder.Entity<Price>()
                .HasIndex(x => x.Date);
            modelBuilder.Entity<Request>()
                .HasIndex(x => x.Date);
            modelBuilder.Entity<Request>()
                .HasIndex(x => new { x.Response, x.Command });
            modelBuilder.Entity<Stat>()
                .HasIndex(x => x.Date);
        }
    }

    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<AppDbContext>();
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("Config.Data.json", optional: true, reloadOnChange: true)
                .AddJsonFile("Config.Data.Local.json", optional: true, reloadOnChange: true)
                .Build();

            builder.UseSqlServer(configuration.GetConnectionString("Database"));
            return new AppDbContext(builder.Options);
        }
    }
}