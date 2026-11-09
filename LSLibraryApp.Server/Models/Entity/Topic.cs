using LSLibraryApp.Server.Validators;
using System.ComponentModel.DataAnnotations;

namespace LSLibraryApp.Server.Models.Entity
{
    public class Topic
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Title is required")]
        [MaxLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
        public string Title { get; set; } = string.Empty;

        public string Body { get; set; } = string.Empty;

        [TopicTagsValidation]
        public ICollection<TopicTag> Tags { get; set; } = [];

        [Required]
        public DateTimeOffset CreatedAt { get; set; }

        public DateTimeOffset? UpdatedAt { get; set; }

        public int Version { get; set; } = 1;

        public ICollection<BookReference> BookReferences { get; set; } = [];
    }
}
