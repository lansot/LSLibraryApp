using LSLibraryApp.Server.Models.Entity;

namespace LSLibraryApp.Server.Repositories.Interface
{
    public interface IBookRepository
    {
        Task<List<Book>> GetAll();
        Task<Book?> GetById(int id);
        Task<List<Book>> GetByTopicWithTopicReferences(int topicId);
        Task<Book> Create(Book book);
        Task<Book> Update(Book book);
        Task Delete(int id);
    }
}
