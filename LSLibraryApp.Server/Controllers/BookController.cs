using LSLibraryApp.Server.Models.Entity;
using LSLibraryApp.Server.Models.DTO;
using LSLibraryApp.Server.Services.Interface;
using Mapster;
using Microsoft.AspNetCore.Mvc;

namespace LSLibraryApp.Server.Controllers
{
    [ApiController]
    [Route("books")]
    public class BookController : ControllerBase
    {
        private readonly IBookService _bookService;

        public BookController(IBookService bookService)
        {
            _bookService = bookService;
        }

        [HttpGet]
        public async Task<ActionResult<List<BookDTO>>> GetAllBooks()
        {
            var books = await _bookService.GetAllBooks();
            var bookDtos = books.Adapt<List<BookDTO>>();
            return Ok(bookDtos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<BookDTO>> GetBookById(int id)
        {
            var book = await _bookService.GetBookById(id);

            if (book == null)
            {
                return NotFound();
            }

            return Ok(book.Adapt<BookDTO>());
        }

        [HttpPost]
        public async Task<ActionResult<BookDTO>> CreateBook(BookDTO bookDto)
        {
            try
            {
                var book = bookDto.Adapt<Book>();
                var createdBook = await _bookService.CreateBook(book);
                return CreatedAtAction(nameof(GetBookById), new { id = createdBook.Id }, createdBook.Adapt<BookDTO>());
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("{bookId}/references")]
        public async Task<ActionResult<List<BookReferenceDTO>>> AddBookReference(int bookId, BookReferenceDTO bookReferenceDTO)
        {
            if (bookReferenceDTO.BookId != bookId) // just make sure they're the same—why would they be different!?
            {
                return BadRequest(new { error = "ID mismatch" });
            }

            try
            {
                var updatedBookReferences = await _bookService
                    .AddBookReferenceToBook(bookReferenceDTO.BookId, bookReferenceDTO.TopicId, bookReferenceDTO.Level, bookReferenceDTO.Position);
                return CreatedAtAction(nameof(GetBookById), new { id = bookReferenceDTO.BookId }, updatedBookReferences.Adapt<List<BookReferenceDTO>>());

            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpDelete("{bookId}/references/{bookReferenceId}")]
        public async Task<ActionResult<List<BookReferenceDTO>>> DeleteBookReference(int _, int bookReferenceId)
        {
            // We don't acutally use the bookId from the endpoint, since bookReferenceId always refers to the one reference...
            // We COULD check that the bookReferenceId actually belongs to the bookId, but that seems like overkill for now?
            // This'd be something to figure out later.
            try
            {
                var updatedBookReferences = await _bookService.DeleteBookReferenceFromBook(bookReferenceId);
                return Ok(updatedBookReferences.Adapt<List<BookReferenceDTO>>());
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}
