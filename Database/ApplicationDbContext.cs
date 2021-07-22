using InventoryApp.Entities;
using System.Data.Entity;

namespace ExceParserEF6.Database
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext() : base("DbConnection") {  }

        public DbSet<Good> Goods { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<CustomerDetails> CustomerDetails { get; set; }
        public DbSet<GoodItem> GoodItems { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderGood> OrderGoods { get; set; }
    }
}
