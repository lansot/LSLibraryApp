using LSLibraryApp.Server.Data;
using LSLibraryApp.Server.Models.Entity;
using LSLibraryApp.Server.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace LSLibraryApp.Server.Repositories
{
    public class TopicRepository : ITopicRepository
    {
        private readonly LibraryDbContext _context;

        public TopicRepository(LibraryDbContext context)
        {
            _context = context;
        }

        public async Task<List<Topic>> GetAll()
        {
            return await _context.Topics.ToListAsync();
        }

        public async Task<Topic?> GetById(int id)
        {
            return await _context.Topics.FindAsync(id);
        }

        public async Task<Topic> Create(Topic topic)
        {
            topic.CreatedAt = DateTimeOffset.UtcNow;

            _context.Topics.Add(topic);

            await _context.SaveChangesAsync();

            return topic;
        }

        public async Task<Topic> Update(Topic incomingTopic)
        {
            var dbTopic = await _context.Topics.FindAsync(incomingTopic.Id)
                ?? throw new KeyNotFoundException();

            // Set the topic version number to the incoming version number.
            // If there's a version number mismatch, EF will throw a DbUpdateConcurrencyException when saving changes,
            // since we set .IsConcurrencyToken() in the LibraryDbContext.
            _context.Entry(dbTopic).Property(t => t.Version).OriginalValue = incomingTopic.Version;

            // Update properties
            dbTopic.Title = incomingTopic.Title;
            dbTopic.Body = incomingTopic.Body;
            dbTopic.Tags = incomingTopic.Tags;
            dbTopic.UpdatedAt = DateTimeOffset.UtcNow;
            dbTopic.Version = checked(dbTopic.Version + 1); // Makes it throw if it somehow overflows

            await _context.SaveChangesAsync();

            return dbTopic;
        }
    }
}
