using AutoMapper;
using DomainEntities = NielsenChannelsReporting.Application.Models;
using ContextEntities = NielsenChannelsReporting.Infrastructure.Persistence.Models;
using NielsenChannelsReporting.Application.Models;
using Newtonsoft.Json;

namespace NielsenChannelsReporting.Infrastructure.Persistence.Mappers
{
    internal class EntitiesMapper : Profile
    {
        public EntitiesMapper()
        {
            CreateMap<ContextEntities.ChannelSet, DomainEntities.ChannelSet>()
                .ForMember(dest => dest.Channels, 
                    opt => opt.MapFrom(src => JsonConvert.DeserializeObject<List<Channel>>(src.Channels)))
            .ReverseMap();
            CreateMap<ContextEntities.NielsenChannelsReportLog, DomainEntities.NielsenChannelsReportLog>().ReverseMap();
        }
    }
}
