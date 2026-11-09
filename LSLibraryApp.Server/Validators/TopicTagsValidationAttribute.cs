using LSLibraryApp.Server.Models.Entity;
using System.ComponentModel.DataAnnotations;

namespace LSLibraryApp.Server.Validators
{
    [AttributeUsage(AttributeTargets.Property)]
    public class TopicTagsValidationAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is not ICollection<TopicTag> tags || tags.Count == 0)
                return ValidationResult.Success; // Nothing to validate!

            var uniqueTags = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (var tag in tags)
            {
                tag.Name = tag.Name.Trim();

                if (string.IsNullOrWhiteSpace(tag.Name))
                    return new ValidationResult("Tags cannot be empty or whitespace only");

                if (!uniqueTags.Add(tag.Name))
                    return new ValidationResult("Tags must be unique (case-insensitive)");

                if (tag.Name.Length > 50)
                    return new ValidationResult("Each tag cannot exceed 50 characters");
            }

            return ValidationResult.Success;
        }
    }
}
