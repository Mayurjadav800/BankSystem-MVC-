using BankSystem_MVC_.Models;
using Microsoft.EntityFrameworkCore;

namespace BankSystem_MVC_.Data
{
    public class BankDbContext:DbContext
    {
        public BankDbContext(DbContextOptions<BankDbContext>options):base(options)
        {
            
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<Account>()
            .HasIndex(a => a.AccountNumber)
            .IsUnique();


            modelBuilder.Entity<Account>()
               .HasMany(a => a.Deposites)
               .WithOne(d => d.Account)
               .HasForeignKey(d => d.AccountId);

            modelBuilder.Entity<Account>()
                .HasMany(a => a.Transfers)
                .WithOne(t => t.Account)
                .HasForeignKey(t => t.AccountId);

            modelBuilder.Entity<Account>()
                .HasMany(a => a.Withdraws)
                .WithOne(w => w.Account)
                .HasForeignKey(w => w.AccountId);

            modelBuilder.Entity<Account>()
                .HasMany(a => a.Otps)
                .WithOne(o => o.Account)
                .HasForeignKey(o => o.AccountId);

            base.OnModelCreating(modelBuilder);

        }
        public DbSet<Account> Account { get; set; }
        public DbSet<Deposite>Deposite { get; set; }
        public DbSet<Transfer>Transfer { get; set; }
        public DbSet<Withdraw> Withdraw { get; set; }
        public DbSet<BankSystem_MVC_.Models.Otp> Otp { get; set; } = default!;
    }
}
