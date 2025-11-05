using Microsoft.EntityFrameworkCore;
using PlatformService.Models;

namespace PlatformService.Data
{
  public class AppDBContext : DbContext
  {
    public AppDBContext(DbContextOptions<AppDBContext> opt) : base(opt)
    {

    }

    public DbSet<Platform> Platforms { get; set; }

  }
}