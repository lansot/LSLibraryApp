using LSLibraryApp.Server.Models.Entity;

namespace LSLibraryApp.Server.Data
{
    public static class DbContextSeeding
    {
        public static async Task SeedTopicsAsync(this LibraryDbContext dbContext)
        {
            // Only seed if the database is empty
            if (dbContext.Topics.Any())
            {
                return;
            }

            var topics = new List<Topic>
            {
                new Topic
                {
                    Title = "Introduction to Author-it",
                    Body = "bahbahbahbahbah. bahbahbah.",
                    CreatedAt = DateTimeOffset.UtcNow,
                    Version = 1,
                    Tags = new List<TopicTag>
                    {
                        new TopicTag { Name = "overview" },
                        new TopicTag { Name = "getting-started" }
                    }
                },
                new Topic
                {
                    Title = "Installing the Desktop Client",
                    Body = "you know you want to. you know you want to!",
                    CreatedAt = DateTimeOffset.UtcNow,
                    Version = 1,
                    Tags = new List<TopicTag>
                    {
                        new TopicTag { Name = "installation" },
                        new TopicTag { Name = "windows" },
                        new TopicTag { Name = "desktop" }
                    }
                },
                new Topic
                {
                    Title = "Connecting to a Library",
                    Body = "having fun isn't hard when you've got a library card!",
                    CreatedAt = DateTimeOffset.UtcNow,
                    Version = 1,
                    Tags = new List<TopicTag>
                    {
                        new TopicTag { Name = "setup" },
                        new TopicTag { Name = "library" },
                        new TopicTag { Name = "configuration" }
                    }
                },
                new Topic
                {
                    Title = "Creating Your First Topic",
                    Body = "the first time changes a man forever!",
                    CreatedAt = DateTimeOffset.UtcNow,
                    Version = 1,
                    Tags = new List<TopicTag>
                    {
                        new TopicTag { Name = "authoring" },
                        new TopicTag { Name = "topics" },
                        new TopicTag { Name = "beginner" }
                    }
                },
                new Topic
                {
                    Title = "Publishing to PDF",
                    Body = "it's alive! it's alive!!!",
                    CreatedAt = DateTimeOffset.UtcNow,
                    Version = 1,
                    Tags = new List<TopicTag>
                    {
                        new TopicTag { Name = "publishing" },
                        new TopicTag { Name = "output" },
                        new TopicTag { Name = "pdf" }
                    }
                }
            };

            dbContext.Topics.AddRange(topics);
            await dbContext.SaveChangesAsync();
        }

        public static async Task SeedBooksAsync(this LibraryDbContext dbContext)
        {
            // Only seed if the database is empty
            if (dbContext.Books.Any())
            {
                return;
            }

            var books = new List<Book>
            {
                new Book
                {
                    Title = "Getting Started Guide",
                    Description = "The Hitchhiker's Guide to the Library",
                    CreatedAt = DateTimeOffset.UtcNow,
                },
                new Book
                {
                    Title = "Publishing Handbook",
                    Description = "it's a book that fits in your hand. wow!",
                    CreatedAt = DateTimeOffset.UtcNow,
                },
                new Book
                {
                    Title = "Administrator Guide",
                    Description = "or: how to strike fear into the hearts of men",
                    CreatedAt = DateTimeOffset.UtcNow,
                }
            };
            dbContext.Books.AddRange(books);
            await dbContext.SaveChangesAsync();

            // Seed the BookReferences
            // it's not great that we're fetching topics and books by title, but for this set of test data, we can let it slide...

            var book1 = dbContext.Books.Single(b => b.Title == "Getting Started Guide");
            var book2 = dbContext.Books.Single(b => b.Title == "Publishing Handbook");
            var book3 = dbContext.Books.Single(b => b.Title == "Administrator Guide");
            var topic1 = dbContext.Topics.Single(t => t.Title == "Introduction to Author-it");
            var topic2 = dbContext.Topics.Single(t => t.Title == "Installing the Desktop Client");
            var topic3 = dbContext.Topics.Single(t => t.Title == "Connecting to a Library");
            var topic4 = dbContext.Topics.Single(t => t.Title == "Creating Your First Topic");
            var topic5 = dbContext.Topics.Single(t => t.Title == "Publishing to PDF");

            var book1References = new List<BookReference>
            {
                new BookReference
                {
                    Book = book1,
                    Topic = topic1,
                    Position = 1,
                    Level = 1
                },
                new BookReference
                {
                    Book = book1,
                    Topic = topic2,
                    Position = 2,
                    Level = 1
                },
                new BookReference
                {
                    Book = book1,
                    Topic = topic3,
                    Position = 3,
                    Level = 2
                },
                new BookReference
                {
                    Book = book1,
                    Topic = topic4,
                    Position = 4,
                    Level = 1
                }
            };

            var book2References = new List<BookReference>
            {
                new BookReference
                {
                    Book = book2,
                    Topic = topic1,
                    Position = 1,
                    Level = 1
                },
                new BookReference
                {
                    Book = book2,
                    Topic = topic5,
                    Position = 2,
                    Level = 1
                },
                new BookReference
                {
                    Book = book2,
                    Topic = topic4,
                    Position = 3,
                    Level = 2
                }
            };

            var book3References = new List<BookReference>
            {
                new BookReference
                {
                    Book = book3,
                    Topic = topic2,
                    Position = 1,
                    Level = 1
                },
                new BookReference
                {
                    Book = book3,
                    Topic = topic3,
                    Position = 2,
                    Level = 2
                },
                new BookReference
                {
                    Book = book3,
                    Topic = topic5,
                    Position = 3,
                    Level = 1
                },
                new BookReference
                {
                    Book = book3,
                    Topic = topic3,
                    Position = 4,
                    Level = 2
                }
            };

            dbContext.BookReferences.AddRange(book1References);
            dbContext.BookReferences.AddRange(book2References);
            dbContext.BookReferences.AddRange(book3References);

            await dbContext.SaveChangesAsync();
        }
    }
}
