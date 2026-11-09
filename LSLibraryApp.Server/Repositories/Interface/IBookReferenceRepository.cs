using LSLibraryApp.Server.Models.Entity;

namespace LSLibraryApp.Server.Repositories.Interface
{
    public interface IBookReferenceRepository
    {
        Task<List<BookReference>> GetByBook(int bookId);
        Task<List<BookReference>> GetByReferencedTopic(int topicId);
        Task<BookReference?> GetById(int bookReferenceId);
        Task<List<BookReference>> Upsert(params IEnumerable<BookReference> bookReferences);
        Task Delete(int bookReferenceId);
    }
}
