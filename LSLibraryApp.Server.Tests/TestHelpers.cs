using LSLibraryApp.Server.Data;
using Microsoft.EntityFrameworkCore;

namespace LSLibraryApp.Server.Tests
{
    internal class TestHelpers
    {
        public static LibraryDbContext CreateInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<LibraryDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            return new LibraryDbContext(options);
        }
    }
}
