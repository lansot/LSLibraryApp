using LSLibraryApp.Server.Models.Entity;
using LSLibraryApp.Server.Validators;
using System.ComponentModel.DataAnnotations;

namespace LSLibraryApp.Server.Tests
{
    public class TopicTagsValidationTests
    {
        private readonly TopicTagsValidationAttribute _validator = new();

        #region TopicTagsValidation Tests

        [Fact]
        public void TopicTagsValidation_WithEmptyTagsList_ReturnsSuccess()
        {
            // Given
            var topic = new Topic { Tags = [] };
            var context = new ValidationContext(topic);

            // When
            var result = _validator.GetValidationResult(topic.Tags, context);

            // Then
            Assert.Equal(ValidationResult.Success, result);
        }

        [Fact]
        public void TopicTagsValidation_WithNullTags_ReturnsSuccess()
        {
            // Given
            var topic = new Topic();
            topic.Tags = null!;
            var context = new ValidationContext(topic);

            // When
            var result = _validator.GetValidationResult(null, context);

            // Then
            Assert.Equal(ValidationResult.Success, result);
        }

        [Fact]
        public void TopicTagsValidation_WithValidSingleTag_ReturnsSuccess()
        {
            // Given
            var tags = new List<TopicTag> { new TopicTag { Name = "csharp" } };
            var topic = new Topic { Tags = tags };
            var context = new ValidationContext(topic);

            // When
            var result = _validator.GetValidationResult(tags, context);

            // Then
            Assert.Equal(ValidationResult.Success, result);
        }

        [Fact]
        public void TopicTagsValidation_WithValidMultipleTags_ReturnsSuccess()
        {
            // Given
            var tags = new List<TopicTag>
            {
                new TopicTag { Name = "csharp" },
                new TopicTag { Name = "dotnet" },
                new TopicTag { Name = "testing" }
            };
            var topic = new Topic { Tags = tags };
            var context = new ValidationContext(topic);

            // When
            var result = _validator.GetValidationResult(tags, context);

            // Then
            Assert.Equal(ValidationResult.Success, result);
        }

        [Fact]
        public void TopicTagsValidation_WithWhitespaceOnlyTag_ReturnsFails()
        {
            // Given
            var tags = new List<TopicTag> { new TopicTag { Name = "   " } };
            var topic = new Topic { Tags = tags };
            var context = new ValidationContext(topic);

            // When
            var result = _validator.GetValidationResult(tags, context);

            // Then
            Assert.NotEqual(ValidationResult.Success, result);
            Assert.Contains("Tags cannot be empty or whitespace only", result!.ErrorMessage!);
        }

        [Fact]
        public void TopicTagsValidation_WithEmptyTag_ReturnsFails()
        {
            // Given
            var tags = new List<TopicTag> { new TopicTag { Name = "" } };
            var topic = new Topic { Tags = tags };
            var context = new ValidationContext(topic);

            // When
            var result = _validator.GetValidationResult(tags, context);

            // Then
            Assert.NotEqual(ValidationResult.Success, result);
            Assert.Contains("Tags cannot be empty or whitespace only", result!.ErrorMessage!);
        }

        [Fact]
        public void TopicTagsValidation_WithDuplicateTags_ReturnsFails()
        {
            // Given
            var tags = new List<TopicTag>
            {
                new TopicTag { Name = "csharp" },
                new TopicTag { Name = "csharp" }
            };
            var topic = new Topic { Tags = tags };
            var context = new ValidationContext(topic);

            // When
            var result = _validator.GetValidationResult(tags, context);

            // Then
            Assert.NotEqual(ValidationResult.Success, result);
            Assert.Contains("Tags must be unique (case-insensitive)", result!.ErrorMessage!);
        }

        [Fact]
        public void TopicTagsValidation_WithDuplicateTagsDifferentCase_ReturnsFails()
        {
            // Given
            var tags = new List<TopicTag>
            {
                new TopicTag { Name = "CSharp" },
                new TopicTag { Name = "csharp" }
            };
            var topic = new Topic { Tags = tags };
            var context = new ValidationContext(topic);

            // When
            var result = _validator.GetValidationResult(tags, context);

            // Then
            Assert.NotEqual(ValidationResult.Success, result);
            Assert.Contains("Tags must be unique (case-insensitive)", result!.ErrorMessage!);
        }

        [Fact]
        public void TopicTagsValidation_WithTagExceedingMaxLength_ReturnsFails()
        {
            // Given
            var longTagName = new string('a', 51);
            var tags = new List<TopicTag> { new TopicTag { Name = longTagName } };
            var topic = new Topic { Tags = tags };
            var context = new ValidationContext(topic);

            // When
            var result = _validator.GetValidationResult(tags, context);

            // Then
            Assert.NotEqual(ValidationResult.Success, result);
            Assert.Contains("Each tag cannot exceed 50 characters", result!.ErrorMessage!);
        }

        [Fact]
        public void TopicTagsValidation_WithTagAtMaxLength_ReturnsSuccess()
        {
            // Given
            var maxLengthTagName = new string('a', 50);
            var tags = new List<TopicTag> { new TopicTag { Name = maxLengthTagName } };
            var topic = new Topic { Tags = tags };
            var context = new ValidationContext(topic);

            // When
            var result = _validator.GetValidationResult(tags, context);

            // Then
            Assert.Equal(ValidationResult.Success, result);
        }

        [Fact]
        public void TopicTagsValidation_WithLeadingAndTrailingWhitespace_TrimsAndReturnsSuccess()
        {
            // Given
            var tags = new List<TopicTag> { new TopicTag { Name = "  csharp  " } };
            var topic = new Topic { Tags = tags };
            var context = new ValidationContext(topic);

            // When
            var result = _validator.GetValidationResult(tags, context);

            // Then
            Assert.Equal(ValidationResult.Success, result);
            Assert.Equal("csharp", tags[0].Name);
        }

        [Fact]
        public void TopicTagsValidation_WithMultipleTagsContainingOneInvalid_ReturnsFails()
        {
            // Given
            var tags = new List<TopicTag>
            {
                new TopicTag { Name = "csharp" },
                new TopicTag { Name = "   " },
                new TopicTag { Name = "dotnet" }
            };
            var topic = new Topic { Tags = tags };
            var context = new ValidationContext(topic);

            // When
            var result = _validator.GetValidationResult(tags, context);

            // Then
            Assert.NotEqual(ValidationResult.Success, result);
            Assert.Contains("Tags cannot be empty or whitespace only", result!.ErrorMessage!);
        }

        #endregion
    }
}
