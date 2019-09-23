using AutoMapper;
using Monitor.Application.MonitoringChecks.Models;
using Monitor.Infrastructure.Logger;

namespace Monitor.Infrastructure.Mappings
{
    public class ChecksMappingProfile : Profile
    {
        public ChecksMappingProfile()
        {
            CreateMap<CheckSettings, Check>()
                .ForMember(dest => dest.Settings, opts => opts.MapFrom(src => src));

            CreateMap<Check, CheckWebModel>()
                .ForMember(dest => dest.Description, opts => opts.MapFrom(src => src.State.Description))
                .ForMember(dest => dest.Status, opts => opts.MapFrom(src => src.State.Status))
                .ForMember(dest => dest.LastCheckTime, opts => opts.MapFrom(src => src.State.LastCheckTime))
                .ForMember(dest => dest.ExecutionDuration, opts => opts.MapFrom(src => src.State.ExecutionDuration))
                .ForMember(dest => dest.StatusChangeTime, opts => opts.MapFrom(src => src.State.StatusChangeTime))
                .ForMember(dest => dest.Type, opts => opts.MapFrom(src => src.Settings.Type))
                .ForMember(dest => dest.Service, opts => opts.MapFrom(src => src.Settings.Service))
                .ForMember(dest => dest.EnvironmentId, opts => opts.MapFrom(src => src.Settings.EnvironmentId))
                .ForMember(dest => dest.Priority, opts => opts.MapFrom(src => src.Settings.Priority))
                .ForMember(dest => dest.CheckFullDescription, opts => opts.MapFrom(src => src.Settings.CheckFullDescription));

            CreateMap<Check, LogStashCheckRecord>()
                .ForMember(dest => dest.Description, opts => opts.MapFrom(src => src.State.Description))
                .ForMember(dest => dest.Status, opts => opts.MapFrom(src => src.State.Status.ToFriendlyString()))
                .ForMember(dest => dest.LastCheckTime, opts => opts.MapFrom(src => src.State.LastCheckTime))
                .ForMember(dest => dest.ExecutionDuration, opts => opts.MapFrom(src => src.State.ExecutionDuration))
                .ForMember(dest => dest.Service, opts => opts.MapFrom(src => src.Settings.Service))
                .ForMember(dest => dest.Type, opts => opts.MapFrom(src => src.Settings.Type))
                .ForMember(dest => dest.EnvironmentId, opts => opts.MapFrom(src => src.Settings.EnvironmentId));
        }
    }
}
