using App.Entity.DTO;
using App.Entity.DTO.Request;
using App.Entity.DTO.Response;
using App.Entity.DTO.Response.Team;
using App.Entity.DTO.Response.Issue;
using App.Entity.Models;
using AutoMapper;

namespace Main.Mappings
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<UserModel, UserResponseDTO>().ReverseMap();
            CreateMap<TeamModel, TeamResponseDTO>().ReverseMap();
            CreateMap<TeamMemberModel, TeamMemberResponseDTO>().ReverseMap();
            CreateMap<TeamInviteModel, TeamInviteResponseDTO>().ReverseMap();

            // Issue mappings
            CreateMap<ProjectLabelModel, LabelResponseDTO>().ReverseMap();
            CreateMap<IssueSubtaskModel, SubtaskResponseDTO>().ReverseMap();
            
            // Notification mappings
            CreateMap<NotificationModel, App.Entity.DTO.Response.Notification.NotificationResponseDTO>()
                .ForMember(dest => dest.TypeName, opt => opt.MapFrom(src => src.Type.ToString()));
        }
    }
}
