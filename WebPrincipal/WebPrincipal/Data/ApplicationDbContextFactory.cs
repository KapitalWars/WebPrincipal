using DotNetEnv;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using WebPrincipal.Data;

public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var basePath = Directory.GetCurrentDirectory();

        var envPath = Path.Combine(basePath, ".env");

        if (!File.Exists(envPath))
        {
            basePath = Directory.GetParent(basePath)!.FullName;
            envPath = Path.Combine(basePath, ".env");
        }

        Env.Load(envPath);

        var connectionString =
            $"Host=localhost;" +
            $"Database={Env.GetString("POSTGRES_DB")};" +
            $"Username={Env.GetString("POSTGRES_USER")};" +
            $"Password={Env.GetString("POSTGRES_PASSWORD")}";

        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();

        optionsBuilder.UseNpgsql(connectionString);

        return new ApplicationDbContext(optionsBuilder.Options);
    }
}