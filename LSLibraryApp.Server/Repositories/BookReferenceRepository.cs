using LSLibraryApp.Server.Data;
using LSLibraryApp.Server.Models.Entity;
using LSLibraryApp.Server.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace LSLibraryApp.Server.Repositories
{
    public class BookReferenceRepository : IBookReferenceRepository
    {
        private readonly LibraryDbContext _context;

        public BookReferenceRepository(LibraryDbContext context)
        {
            _context = context;
        }

        public async Task<List<BookReference>> GetByBook(int bookId)
        {
            return await _context.BookReferences
                .Where(br => br.BookId == bookId)
                .OrderBy(br => br.Position)
                .ToListAsync();
        }

        public async Task<List<BookReference>> GetByReferencedTopic(int topicId)
        {
            return await _context.BookReferences
                .Where(br => br.TopicId == topicId)
                .OrderBy(br => br.BookId)
                .ThenBy(br => br.Position)
                .ToListAsync();
        }

        public async Task<BookReference?> GetById(int bookReferenceId)
        {
            return await _context.BookReferences.FindAsync(bookReferenceId);
        }

        public async Task<BookReference> Create(BookReference bookReference)
        {
            _context.BookReferences.Add(bookReference);

            await _context.SaveChangesAsync();

            return bookReference;
        }

        public async Task<List<BookReference>> Upsert(params IEnumerable<BookReference> bookReferences)
        {
            foreach (var toUpsert in bookReferences)
            {
                if (toUpsert.Id != 0)
                {
                    var dbBookReference = await _context.BookReferences.FindAsync(toUpsert.Id)
                        ?? throw new KeyNotFoundException();

                    dbBookReference.Level = toUpsert.Level;
                    dbBookReference.Position = toUpsert.Position;
                }
                else
                {
                    _context.BookReferences.Add(toUpsert);
                }
            }

            await _context.SaveChangesAsync();

            return await _context.BookReferences
                .Include(br => br.Topic)
                .Where(br => bookReferences.Select(br => br.Id).Contains(br.Id))
                .OrderBy(br => br.Position)
                .ToListAsync();
        }

        public async Task Delete(int bookReferenceId)
        {
            var bookReference = await _context.BookReferences.FindAsync(bookReferenceId);

            if (bookReference is not null)
            {
                _context.BookReferences.Remove(bookReference);
                await _context.SaveChangesAsync();
            }
        }
    }
}
