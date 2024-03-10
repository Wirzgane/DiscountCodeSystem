using DiscountCodeSystem.Worker.Domain;
using Microsoft.EntityFrameworkCore;

namespace DiscountCodeSystem.Worker.Infrastructure;
public class DiscountCodeDbContext : DbContext
{
    public DiscountCodeDbContext(DbContextOptions<DiscountCodeDbContext> options)
    : base(options)
    {
    }

    public DbSet<DiscountCode> DiscountCodes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DiscountCode>()
            .HasKey(dc => dc.Code); // Set the primary key to the Code field
    }
}
