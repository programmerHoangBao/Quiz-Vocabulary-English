using AutoMapper;
using back_end.DTOs.Folder.Requests;
using back_end.DTOs.Folder.Responses;
using back_end.DTOs.Projections;
using back_end.DTOs.Topic.Requests;
using back_end.DTOs.Topic.Responses;
using back_end.DTOs.Vocabolury.Requests;
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
            CreateMap<CreateTopicRequest, Topic>();
            CreateMap<UpdateTopicRequest, Topic>();
            CreateMap<Topic, TopicResponse>();
            CreateMap<CreateVocaboluryRequest, Topic>();
            CreateMap<UpdateVocaboluryRequest, Topic>();
            CreateMap<Topic, TopicResponse>();
            CreateMap<User, LoginUserProjection>();
        }
    }
}
