using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommandService.Models;

namespace CommandService.SyncDataServices.Grpc
{
  public interface IPlatformDataClient
  {
    IEnumerable<Platform> ReturnAllPlatforms();
  }
}