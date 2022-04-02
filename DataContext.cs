using Microsoft.EntityFrameworkCore;

namespace AsyncEntity;

public class DataContext : DbContext
{
    public DataContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<Supplier> Supplier { get; set; }
    public DbSet<Customer> Customer { get; set; }
}