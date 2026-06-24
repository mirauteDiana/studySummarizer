using AutoMapper;
using StudySummarizer.Application.DTOs;
using StudySummarizer.Domain.Entities;

namespace StudySummarizer.Application.Mappings;

public class DocumentMappingProfile : Profile
{
    public DocumentMappingProfile()
    {
        CreateMap<Document, DocumentResponse>();
        CreateMap<DocumentResponse, UserDocumentResponse>()
            .ForMember(dest => dest.DocumentId, opt => opt.MapFrom(src => src.Id));
    }
}
