using LSLibraryApp.Server.Models.Entity;
using LSLibraryApp.Server.Repositories;
using Microsoft.EntityFrameworkCore;

namespace LSLibraryApp.Server.Tests
{
    public class TopicRepositoryTests
    {
        #region GetAll Tests

        [Fact]
        public async Task GetAll_WithEmptyDatabase_ReturnsEmptyList()
        {
            // Given
            using var context = TestHelpers.CreateInMemoryContext();
            var repository = new TopicRepository(context);

            // When
            var result = await repository.GetAll();

            // Then
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetAll_WithMultipleTopics_ReturnsAllTopics()
        {
            // Given
            using var context = TestHelpers.CreateInMemoryContext();
            var topic1 = new Topic { Title = "Topic 1", Body = "Body 1", Tags = [] };
            var topic2 = new Topic { Title = "Topic 2", Body = "Body 2", Tags = [] };

            context.Topics.AddRange(topic1, topic2);
            await context.SaveChangesAsync();

            var repository = new TopicRepository(context);

            // When
            var result = await repository.GetAll();

            // Then
            Assert.Equal(2, result.Count);
            Assert.Contains(result, t => t.Title == "Topic 1");
            Assert.Contains(result, t => t.Title == "Topic 2");
        }

        #endregion

        #region GetById Tests

        [Fact]
        public async Task GetById_WithValidId_ReturnsTopic()
        {
            // Given
            using var context = TestHelpers.CreateInMemoryContext();
            var topic = new Topic { Title = "Test Topic", Body = "Test Body", Tags = [] };
            context.Topics.Add(topic);
            await context.SaveChangesAsync();

            var repository = new TopicRepository(context);

            // When
            var result = await repository.GetById(topic.Id);

            // Then
            Assert.NotNull(result);
            Assert.Equal("Test Topic", result.Title);
            Assert.Equal("Test Body", result.Body);
        }

        [Fact]
        public async Task GetById_WithInvalidId_ReturnsNull()
        {
            // Given
            using var context = TestHelpers.CreateInMemoryContext();
            var repository = new TopicRepository(context);

            // When
            var result = await repository.GetById(999);

            // Then
            Assert.Null(result);
        }

        #endregion

        #region Create Tests

        [Fact]
        public async Task Create_WithValidTopic_SavesAndReturnsTopicWithId()
        {
            // Given
            using var context = TestHelpers.CreateInMemoryContext();
            var repository = new TopicRepository(context);
            var topic = new Topic { Title = "New Topic", Body = "New Body", Tags = [] };

            // When
            var result = await repository.Create(topic);

            // Then
            Assert.NotEqual(0, result.Id);
            Assert.Equal("New Topic", result.Title);
            Assert.NotEqual(default, result.CreatedAt);
            Assert.True(result.CreatedAt <= DateTimeOffset.UtcNow);
        }

        [Fact]
        public async Task Create_SetsCreatedAtToUtcNow()
        {
            // Given
            using var context = TestHelpers.CreateInMemoryContext();
            var repository = new TopicRepository(context);
            var topic = new Topic { Title = "Timestamped Topic", Body = "Body", Tags = [] };
            var beforeCreate = DateTimeOffset.UtcNow;

            // When
            var result = await repository.Create(topic);
            var afterCreate = DateTimeOffset.UtcNow;

            // Then
            Assert.True(result.CreatedAt >= beforeCreate && result.CreatedAt <= afterCreate);
        }

        [Fact]
        public async Task Create_WithTags_PersistsTags()
        {
            // Given
            using var context = TestHelpers.CreateInMemoryContext();
            var repository = new TopicRepository(context);
            var topic = new Topic
            {
                Title = "Topic with Tags",
                Body = "Body",
                Tags = [new TopicTag("tag1"), new TopicTag("tag2")]
            };

            // When
            var result = await repository.Create(topic);

            // Then
            Assert.Equal(2, result.Tags.Count);
            Assert.Contains(result.Tags, t => t.Name == "tag1");
            Assert.Contains(result.Tags, t => t.Name == "tag2");
        }

        [Fact]
        public async Task Create_TopicIsPersisted_InDatabase()
        {
            // Given
            using var context = TestHelpers.CreateInMemoryContext();
            var repository = new TopicRepository(context);
            var topic = new Topic { Title = "Persistent Topic", Body = "Body", Tags = [] };

            // When
            var created = await repository.Create(topic);

            // Then - verify in database
            var retrieved = await context.Topics.FindAsync(created.Id);
            Assert.NotNull(retrieved);
            Assert.Equal("Persistent Topic", retrieved.Title);
        }

        #endregion

        #region Update Tests

        [Fact]
        public async Task Update_WithValidTopic_UpdatesProperties()
        {
            // Given
            using var context = TestHelpers.CreateInMemoryContext();
            var topic = new Topic { Title = "Original Title", Body = "Original Body", Tags = [] };
            context.Topics.Add(topic);
            await context.SaveChangesAsync();

            var repository = new TopicRepository(context);
            topic.Title = "Updated Title";
            topic.Body = "Updated Body";

            // When
            var result = await repository.Update(topic);

            // Then
            Assert.Equal("Updated Title", result.Title);
            Assert.Equal("Updated Body", result.Body);
        }

        [Fact]
        public async Task Update_IncrementsVersion()
        {
            // Given
            using var context = TestHelpers.CreateInMemoryContext();
            var topic = new Topic { Title = "Topic", Body = "Body", Tags = [] };
            context.Topics.Add(topic);
            await context.SaveChangesAsync();

            var initialVersion = topic.Version;
            var repository = new TopicRepository(context);

            // When
            var result = await repository.Update(topic);

            // Then
            Assert.Equal(initialVersion + 1, result.Version);
        }

        [Fact]
        public async Task Update_SetsUpdatedAt()
        {
            // Given
            using var context = TestHelpers.CreateInMemoryContext();
            var topic = new Topic { Title = "Topic", Body = "Body", Tags = [] };
            context.Topics.Add(topic);
            await context.SaveChangesAsync();

            var beforeUpdate = DateTimeOffset.UtcNow;
            var repository = new TopicRepository(context);

            // When
            var result = await repository.Update(topic);
            var afterUpdate = DateTimeOffset.UtcNow;

            // Then
            Assert.NotNull(result.UpdatedAt);
            Assert.True(result.UpdatedAt >= beforeUpdate && result.UpdatedAt <= afterUpdate);
        }

        [Fact]
        public async Task Update_WithNonexistentId_ThrowsKeyNotFoundException()
        {
            // Given
            using var context = TestHelpers.CreateInMemoryContext();
            var repository = new TopicRepository(context);
            var topic = new Topic { Id = 999, Title = "Nonexistent", Body = "Body", Tags = [] };

            // When & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => repository.Update(topic));
        }

        [Fact]
        public async Task Update_WithVersionMismatch_ThrowsDbUpdateConcurrencyException()
        {
            // Given
            using var context = TestHelpers.CreateInMemoryContext();
            var topic = new Topic { Title = "Topic", Body = "Body", Tags = [], Version = 3 };
            context.Topics.Add(topic);
            await context.SaveChangesAsync();

            var repository = new TopicRepository(context);
            topic.Version = 2; // Wrong version

            // When & Assert
            await Assert.ThrowsAsync<DbUpdateConcurrencyException>(() => repository.Update(topic));
        }

        [Fact]
        public async Task Update_WithValidVersion_SucceedsAndPersists()
        {
            // Given
            using var context = TestHelpers.CreateInMemoryContext();
            var topic = new Topic { Title = "Original", Body = "Body", Tags = [] };
            context.Topics.Add(topic);
            await context.SaveChangesAsync();

            var repository = new TopicRepository(context);
            topic.Title = "Modified";

            // When
            var result = await repository.Update(topic);

            // Then - verify persistence
            var retrieved = await context.Topics.FindAsync(result.Id);
            Assert.NotNull(retrieved);
            Assert.Equal("Modified", retrieved.Title);
            Assert.Equal(2, retrieved.Version);
        }

        #endregion
    }
}
