using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using PlatformService.Models;
using Microsoft.EntityFrameworkCore;

namespace PlatformService.Data
{
  public static class PrebDB
  {
    public static void PrepPopulation(IApplicationBuilder app, bool isProd)
    {
      using (var serviceScope = app.ApplicationServices.CreateScope())
      {
        SeedData(serviceScope.ServiceProvider.GetRequiredService<AppDBContext>(), isProd);
      }
    }

    private static void SeedData(AppDBContext context, bool isProd)
    {
      if (isProd)
      {
        Console.WriteLine("--> Attempting to apply migrations...");
        try
        {
          context.Database.Migrate();
        }
        catch (Exception ex)
        {
          Console.WriteLine($"--> Could not run migrations: {ex.Message}");
          throw;
        }
        // context.Database.Migrate();
      }

      if (!context.Platforms.Any())
      {
        Console.WriteLine("--> Seeding data...");

        context.Platforms.AddRange(
          new Platform() { Name = "DOTNET", Publisher = "MICROSOFT", Cost = "FREE" },
          new Platform() { Name = "SQL SERVER EXPRESS", Publisher = "MICROSOFT", Cost = "FREE" },
          new Platform()
          {
            Name = "KUBERNETES",
            Publisher = "CLOUD NATIVE COMPUTING FOUNDATION",
            Cost = "FREE"
          });

        context.SaveChanges();
      }
      else
      {
        Console.WriteLine("--> We are already have data");
      }
    }
  }
}