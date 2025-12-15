using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CommandService.Dtos;
using CommandService.Models;
using PlatformService;

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
      CreateMap<PlatformPublishedDto, Platform>()
        .ForMember(dest => dest.ExternalID, opt => opt.MapFrom(src => src.Id));
      CreateMap<GrpcPlatformModel, Platform>()
        .ForMember(dest => dest.ExternalID, opt => opt.MapFrom(src => src.PlatformId))
        .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
        .ForMember(dest => dest.Commands, opt => opt.Ignore());
    }
  }
}