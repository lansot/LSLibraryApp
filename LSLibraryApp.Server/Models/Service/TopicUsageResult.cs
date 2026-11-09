using LSLibraryApp.Server.Models.Entity;

namespace LSLibraryApp.Server.Models.Service
{
    public class TopicUsageResult
    {
        public required Topic Topic { get; set; }
		// The intention is to only include BookContents that reference the Topic,
        // not every content in the Book.
		public required List<Book> BooksWithReferences { get; set; } 
    }
}
