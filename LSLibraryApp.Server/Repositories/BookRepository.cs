using LSLibraryApp.Server.Data;
using LSLibraryApp.Server.Models.Entity;
using LSLibraryApp.Server.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace LSLibraryApp.Server.Repositories
{
    public class BookRepository : IBookRepository
    {
        private readonly LibraryDbContext _context;

        public BookRepository(LibraryDbContext context)
        {
            _context = context;
        }

        public async Task<List<Book>> GetAll()
        {
            return await _context.Books.ToListAsync();
        }

        public async Task<Book?> GetById(int id)
        {
            return await _context.Books
                .Include(b => b.Contents.OrderBy(br => br.Position))
                .ThenInclude(br => br.Topic)
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task<List<Book>> GetByTopicWithTopicReferences(int topicId)
        {
            return await _context.Books
                .Where(b => b.Contents.Any(br => br.TopicId == topicId))
                .Include(b => b.Contents.Where(br => br.TopicId == topicId).OrderBy(br => br.Position))
                .ToListAsync();
        }

        public async Task<Book> Create(Book book)
        {
            book.CreatedAt = DateTimeOffset.UtcNow;

            _context.Books.Add(book);

            await _context.SaveChangesAsync();

            return book;
        }

        public async Task<Book> Update(Book book)
        {
            var dbBook = await _context.Books.FindAsync(book.Id)
                ?? throw new KeyNotFoundException();

            dbBook.Title = book.Title;
            dbBook.Description = book.Description;

            await _context.SaveChangesAsync();

            return dbBook;
        }

        public async Task Delete(int id)
        {
            var book = await _context.Books.FindAsync(id);

            if (book is not null)
            {
                _context.Books.Remove(book);
                await _context.SaveChangesAsync();
            }
        }
    }
}
