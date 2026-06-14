using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Conqueco.Data;

namespace Conqueco.Data;

public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();

        optionsBuilder.UseNpgsql(
            "Host=localhost;Port=5433;Database=conqueco;Username=postgres;Password=postgres"
        );

        return new ApplicationDbContext(optionsBuilder.Options);
    }
}