namespace LSLibraryApp.Server.Models.DTO
{
    public class BookReferenceDTO
    {
        public int Id { get; set; }
        public int BookId { get; set; }
        public int TopicId { get; set; }
        public string TopicTitle { get; set; } = string.Empty;
        public int Level { get; set; } // AKA "Indent Depth"
        public int? Position { get; set; } // BookReferences within a Book are ordered by this!!
    }
}
