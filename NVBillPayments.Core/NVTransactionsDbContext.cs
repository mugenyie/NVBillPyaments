using Microsoft.EntityFrameworkCore;
using NVBillPayments.Core.Models;
using NVBillPayments.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace NVBillPayments.Core
{
    public class NVTransactionsDbContext : DbContext
    {
        public NVTransactionsDbContext()
        {

        }

        public NVTransactionsDbContext(DbContextOptions<NVTransactionsDbContext> options) : base(options)
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(ConfigurationConstants.DBCONNECTION);
        }

        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<TransactionLog> TransactionLogs { get; set; }
        public DbSet<Category> ProductCategories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ServiceProvider> ServiceProviders { get; set; }
        public DbSet<CashBackPolicy> CashBackPolicies { get; set; }
    }
}
