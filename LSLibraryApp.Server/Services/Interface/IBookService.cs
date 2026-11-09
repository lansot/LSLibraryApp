using LSLibraryApp.Server.Models.Entity;

namespace LSLibraryApp.Server.Services.Interface
{
    public interface IBookService
    {
        Task<List<Book>> GetAllBooks();
        Task<Book?> GetBookById(int bookId);
        Task<Book> CreateBook(Book book);
        Task<Book> UpdateBook(Book book);
        Task DeleteBook(int bookId);
        Task<List<BookReference>> AddBookReferenceToBook(int bookId, int topicId, int level, int? position = null);
        Task<List<BookReference>> DeleteBookReferenceFromBook(int bookReferenceId);
    }
}
