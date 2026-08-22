using AutoMapper;
using back_end.DTOs.Folder.Requests;
using back_end.DTOs.Folder.Responses;
using back_end.DTOs.Topic.Responses;
using back_end.Models;

namespace back_end.Profiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<CreateFolderRequest, Folder>();
            CreateMap<Folder, FolderResponse>();
            CreateMap<UpdateFolderRequest, Folder>();
            CreateMap<CreateFolderRequest, Topic>();
            CreateMap<UpdateFolderRequest, Topic>();
            CreateMap<Topic, TopicResponse>();
        }
    }
}
