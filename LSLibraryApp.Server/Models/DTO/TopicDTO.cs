using LSLibraryApp.Server.Models.Entity;
using LSLibraryApp.Server.Validators;
using System.ComponentModel.DataAnnotations;

namespace LSLibraryApp.Server.Models.DTO
{
    public class TopicDTO
    {
        public int? Id { get; set; }

        [Required(ErrorMessage = "Title is required")]
        [MaxLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
        public string Title { get; set; } = string.Empty;

        public string Body { get; set; } = string.Empty;

        [TopicTagsValidation]
        public List<TopicTag> Tags { get; set; } = [];

        public DateTimeOffset? UpdatedAt { get; set; }

        public int Version { get; set; } = 1;
    }
}