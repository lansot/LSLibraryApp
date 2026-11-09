using LSLibraryApp.Server.Models.Entity;
using LSLibraryApp.Server.Repositories.Interface;
using LSLibraryApp.Server.Services.Interface;

namespace LSLibraryApp.Server.Services
{
    public class BookService : IBookService
    {
        private readonly IBookRepository _bookRepository;
        private readonly IBookReferenceRepository _bookReferenceRepository;

        public BookService(
            IBookRepository bookRepository,
            IBookReferenceRepository bookReferenceRepository)
        {
            _bookRepository = bookRepository;
            _bookReferenceRepository = bookReferenceRepository;
        }

        public async Task<List<Book>> GetAllBooks()
        {
            return await _bookRepository.GetAll();
        }

        public async Task<Book?> GetBookById(int bookId)
        {
            return await _bookRepository.GetById(bookId);
        }
        public async Task<Book> CreateBook(Book book)
        {
            return await _bookRepository.Create(book);
        }
        public async Task<Book> UpdateBook(Book book)
        {
            return await _bookRepository.Update(book);
        }

        public async Task DeleteBook(int bookId)
        {
            await _bookRepository.Delete(bookId);
        }

        public async Task<List<BookReference>> AddBookReferenceToBook(int bookId, int topicId, int level, int? position = null)
        {
            // 1. Calculate positioning
            var bookReferences = (await _bookReferenceRepository.GetByBook(bookId))
                .OrderBy(br => br.Position)
                .ToList();

            var bookReferenceToAdd = new BookReference
            {
                BookId = bookId,
                TopicId = topicId,
                Level = level
            };

            if (position.HasValue)
                bookReferences.Insert(position.Value - 1, bookReferenceToAdd); // zero-index lololol
            else
                bookReferences.Add(bookReferenceToAdd);

            UpdateBookReferencePositions(bookReferences);

            // 2. Validate levels
            var levelsValid = AreBookReferenceLevelsValid(bookReferences);

            if (!levelsValid)
                throw new ArgumentException("The added entry must not have a level that is more than one deeper than the previous entry.");

            // 3. Save
            return await _bookReferenceRepository.Upsert(bookReferences);
        }

        public async Task<List<BookReference>> DeleteBookReferenceFromBook(int bookReferenceId)
        {
            var toDelete = await _bookReferenceRepository.GetById(bookReferenceId);

            if (toDelete == null)
                throw new KeyNotFoundException($"BookReference with ID {bookReferenceId} not found.");

            // Get all the book's BookReferences. We need to recalculate and save their positions (and levels if necessary).
            var bookReferences = (await _bookReferenceRepository.GetByBook(toDelete.BookId))
                .OrderBy(br => br.Position)
                .ToList();

            var toDeleteIndex = bookReferences.FindIndex(br => br.Id == bookReferenceId);

            // Get this out of the way first
            bookReferences.RemoveAt(toDeleteIndex);
            await _bookReferenceRepository.Delete(bookReferenceId);

            // Every BookReference right after the deleted one with a consecutively higher level must have its level decreased by 1.
            // Start at the index of the deleted item i.e. the first item after what's deleted
            for (int i = toDeleteIndex; i < bookReferences.Count; i++)
            {
                int maxAllowed = i == 0 
                    ? 1 // If it's now the first-position item, it must be level 1
                    : bookReferences[i - 1].Level + 1; // Else, it must be at most one deeper than the previous item

                if (bookReferences[i].Level <= maxAllowed)
                    break; // Finish traversal once we reach an item that is equal or lower in level

                bookReferences[i].Level = maxAllowed;
            }

            // Recalculate positions and save the updated list
            UpdateBookReferencePositions(bookReferences);

            return await _bookReferenceRepository.Upsert(bookReferences);
        }

        /// <summary>
        /// Given a list of BookReferences, set each BookReference.Position to its index in the list
        /// </summary>
        /// <param name="bookReferences"></param>
        private void UpdateBookReferencePositions(List<BookReference> bookReferences)
        {
            for (int i = 0; i < bookReferences.Count; i++)
            {
                bookReferences[i].Position = i + 1; // one-index lololol
            }
        }

        /// <summary>
        /// Given a list of BookReferences belonging to a Book, returns whether:
        /// 1. The first BookReference is at level 1, AND
        /// 2. An entry's level is at most one higher (deeper) than the entry before it.
        /// </summary>
        /// <param name="bookReferences"></param>
        /// <returns></returns>
        private bool AreBookReferenceLevelsValid(IReadOnlyList<BookReference> bookReferences)
        {
            if (bookReferences.Count == 0)
                return true;

            var orderedReferences = bookReferences
                .OrderBy(br => br.Position)
                .ToList();

            if (orderedReferences[0].Level != 1)
                return false;

            for (int i = 1; i < orderedReferences.Count; i++)
            {
                if (orderedReferences[i].Level > orderedReferences[i - 1].Level + 1)
                    return false;
            }

            return true;
        }
    }
}
