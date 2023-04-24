using Microsoft.EntityFrameworkCore; // UseSqlServer
using Microsoft.Extensions.DependencyInjection; // IServiceCollection
namespace Packt.Shared;
public static class NorthwindContextExtensions
{
    /// <summary>
    /// Adds NorthwindContext to the specified IServiceCollection. Uses the SqlServer database provider.
    /// </summary>
    /// <param name="services"></param>
    /// <param name="connectionString">Set to override the default.</param>
    /// <returns>An IServiceCollection that can be used to add more services.</returns>
  public static IServiceCollection AddNorthwindContext(
    this IServiceCollection services, string connectionString =
      "Server=.;Database=Northwind;"
      + "Trusted_Connection=True;MultipleActiveResultsets=true;")
  {
    services.AddDbContext<NorthwindContext>(options =>
      options.UseSqlServer(connectionString));

    return services;
  }
}
