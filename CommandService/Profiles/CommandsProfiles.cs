using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CommandService.Dtos;
using CommandService.Models;

namespace CommandService.Profiles
{
  public class CommandsProfiles : Profile
  {
    public CommandsProfiles()
    {
      // Source -> Target
      CreateMap<Platform, PlatformReadDto>();
      CreateMap<CommandCreateDto, Command>();
      CreateMap<Command, CommandReadDto>();
    }
  }
}