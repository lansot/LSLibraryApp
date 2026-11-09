namespace LSLibraryApp.Server.Models.DTO
{
    public class TopicUsageDTO
    {
        public int TopicId { get; set; }
        public string TopicTitle { get; set; } = string.Empty;
        public List<BookDTO> ReferencingBooks { get; set; } = [];
        public int TotalReferences { get; set; }
        public int BookCount => ReferencingBooks.Count;
    }
}
