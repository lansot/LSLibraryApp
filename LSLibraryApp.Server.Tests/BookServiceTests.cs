using LSLibraryApp.Server.Models.Entity;
using LSLibraryApp.Server.Repositories.Interface;
using LSLibraryApp.Server.Services;
using LSLibraryApp.Server.Services.Interface;
using Moq;

namespace LSLibraryApp.Server.Tests
{
    public class BookServiceTests
    {
        private readonly IBookService _bookService;
        private readonly Mock<IBookRepository> _mockBookRepository;
        private readonly Mock<IBookReferenceRepository> _mockBookReferenceRepository;

        public BookServiceTests()
        {
            _mockBookRepository = new Mock<IBookRepository>();
            _mockBookReferenceRepository = new Mock<IBookReferenceRepository>();
            _bookService = new BookService(
                _mockBookRepository.Object,
                _mockBookReferenceRepository.Object);
        }

        [Fact]
        public async Task AddBookReferenceToBook_GivenLevelTooHigh_ThrowsArgumentException()
        {
            // Given
            _mockBookReferenceRepository
                .Setup(m => m.GetByBook(It.IsAny<int>()))
                .Returns(Task.FromResult(new List<BookReference>()
                {
                    new BookReference()
                    {
                        Position = 1,
                        Level = 1
                    },
                    new BookReference()
                    {
                        Position = 2,
                        Level = 2
                    },
                    new BookReference()
                    {
                        Position = 3,
                        Level = 1
                    },
                }));

            _mockBookReferenceRepository
                .Setup(m => m.Upsert(It.IsAny<IEnumerable<BookReference>>()))
                .Returns(Task.FromResult(new List<BookReference>()));

            // When/Then
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => _bookService.AddBookReferenceToBook(1, 1, level: 3, position: null)); // add to end
            Assert.Equal("The added entry must not have a level that is more than one deeper than the previous entry.", exception.Message);
        }

        [Fact]
        public async Task AddBookReferenceToBook_UpdatesPositions()
        {
            // Given
            _mockBookReferenceRepository
                .Setup(m => m.GetByBook(It.IsAny<int>()))
                .Returns(Task.FromResult(new List<BookReference>()
                {
                    new BookReference()
                    {
                        Id = 1,
                        Position = 1,
                        Level = 1
                    },
                    new BookReference()
                    {
                        Id = 2,
                        Position = 2,
                        Level = 2
                    },
                    new BookReference()
                    {
                        Id = 3,
                        Position = 3,
                        Level = 1
                    },
                }));

            _mockBookReferenceRepository
                .Setup(m => m.Upsert(It.IsAny<IEnumerable<BookReference>>()))
                .Returns<List<BookReference>>(x => Task.FromResult(x));

            // When
            var result = await _bookService.AddBookReferenceToBook(bookId: 50, topicId: 50, level: 1, position: 2);

            // Then
            Assert.Collection(result, 
                r =>
                {
                    Assert.Equal(1, r.Id);
                    Assert.Equal(1, r.Position);
                },
                r => // the newly-added
                {
                    Assert.Equal(50, r.BookId);
                    Assert.Equal(2, r.Position);
                },
                r =>
                {
                    Assert.Equal(2, r.Id);
                    Assert.Equal(3, r.Position);
                },
                r =>
                {
                    Assert.Equal(3, r.Id);
                    Assert.Equal(4, r.Position);
                });
        }

        [Fact]
        public async Task DeleteBookReferenceFromBook_UpdatesLevelsAndPositions()
        {
            // Given
            var bookRefToDelete = new BookReference()
            {
                Id = 2,
                Position = 2,
                Level = 2
            };

            _mockBookReferenceRepository
                .Setup(m => m.GetById(It.IsAny<int>()))
                .Returns(Task.FromResult((BookReference?)bookRefToDelete));

            _mockBookReferenceRepository
                .Setup(m => m.GetByBook(It.IsAny<int>()))
                .Returns(Task.FromResult(new List<BookReference>()
                {
                    new BookReference()
                    {
                        Id = 1,
                        Position = 1,
                        Level = 1
                    },
                    bookRefToDelete,
                    new BookReference()
                    {
                        Id = 3,
                        Position = 3,
                        Level = 3
                    },
                    new BookReference()
                    {
                        Id = 4,
                        Position = 4,
                        Level = 4
                    },
                    new BookReference()
                    {
                        Id = 5,
                        Position = 5,
                        Level = 3
                    },
                }));

            _mockBookReferenceRepository
                .Setup(m => m.Delete(It.IsAny<int>()))
                .Returns(Task.FromResult(true));

            _mockBookReferenceRepository
                .Setup(m => m.Upsert(It.IsAny<IEnumerable<BookReference>>()))
                .Returns<List<BookReference>>(x => Task.FromResult(x));

            // When
            var result = await _bookService.DeleteBookReferenceFromBook(2);

            // Then
            Assert.Collection(result,
                r =>
                {
                    Assert.Equal(1, r.Id);
                    Assert.Equal(1, r.Position);
                    Assert.Equal(1, r.Level);
                },
                r =>
                {
                    Assert.Equal(3, r.Id);
                    Assert.Equal(2, r.Position);
                    Assert.Equal(2, r.Level);
                },
                r =>
                {
                    Assert.Equal(4, r.Id);
                    Assert.Equal(3, r.Position);
                    Assert.Equal(3, r.Level);
                },
                r =>
                {
                    Assert.Equal(5, r.Id);
                    Assert.Equal(4, r.Position);
                    Assert.Equal(3, r.Level);
                });
        }
    }
}
