using System.ComponentModel.DataAnnotations;

namespace LSLibraryApp.Server.Models.Entity
{
    public class TopicTag
    {
        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;

        public TopicTag() { }

        public TopicTag(string name)
        {
            Name = name;
        }

        public override bool Equals(object? obj)
        {
            return obj is TopicTag tag &&
                   string.Equals(Name, tag.Name, StringComparison.OrdinalIgnoreCase);
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode(StringComparison.OrdinalIgnoreCase);
        }
    }
}
