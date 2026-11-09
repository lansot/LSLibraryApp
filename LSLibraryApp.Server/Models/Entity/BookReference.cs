namespace LSLibraryApp.Server.Models.Entity
{
    public class BookReference
    {
        public int Id { get; set; }
        public int BookId { get; set; }
        public Book Book { get; set; } = null!;

        public int TopicId { get; set; }
        public Topic Topic { get; set; } = null!;

        public int Level { get; set; } // AKA "Indent Depth"

        public int Position { get; set; } // BookReferences within a Book are ordered by this!!
    }
}
