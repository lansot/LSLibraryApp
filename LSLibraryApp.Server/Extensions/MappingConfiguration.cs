using LSLibraryApp.Server.Models.Entity;
using LSLibraryApp.Server.Models.DTO;
using LSLibraryApp.Server.Models.Service;
using Mapster;

namespace LSLibraryApp.Server.Extensions
{
    public static class MappingConfiguration
    {
        public static void RegisterMappings()
        {
            TypeAdapterConfig<BookReference, BookReferenceDTO>
                .NewConfig()
                .Map(dest => dest.TopicTitle, src => src.Topic.Title);

            TypeAdapterConfig<TopicUsageResult, TopicUsageDTO>
                .NewConfig()
                .Map(dest => dest.TopicId, src => src.Topic.Id)
                .Map(dest => dest.TopicTitle, src => src.Topic.Title)
                .Map(dest => dest.ReferencingBooks, src => src.BooksWithReferences.Adapt<List<BookDTO>>())
                .Map(dest => dest.TotalReferences, src => src.BooksWithReferences.SelectMany(b => b.Contents).Count());
        }
    }
}
