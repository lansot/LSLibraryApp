using System.ComponentModel.DataAnnotations;

namespace LSLibraryApp.Server.Models.Entity
{
    public class Book
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Title is required")]
        [MaxLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public ICollection<BookReference> Contents { get; set; } = []; // A book's "Contents" == a book's "References"

        [Required]
        public DateTimeOffset CreatedAt { get; set; }
    }
}
