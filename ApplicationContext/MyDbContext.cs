using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TestEFCoreIssue.Model;

namespace TestEFCoreIssue.ApplicationContext;

public class MyDbContext : DbContext
{
    public MyDbContext(DbContextOptions<MyDbContext> options)
        : base(options) { }

    public DbSet<MapInfo> MapInfo => Set<MapInfo>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<MapInfo>(info =>
        {
            info.Property(p => p.Tags).HasConversion<List<int>>();
            // It will work after replacing the above code with the commented code.
            //info.PrimitiveCollection(c => c.Tags).ElementType(e => e.HasConversion(typeof(EnumToNumberConverter<MapTag, int>)));
        });


        base.OnModelCreating(modelBuilder);
    }
}
