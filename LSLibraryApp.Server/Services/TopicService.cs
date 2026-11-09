using LSLibraryApp.Server.Models.Entity;
using LSLibraryApp.Server.Models.Service;
using LSLibraryApp.Server.Repositories.Interface;
using LSLibraryApp.Server.Services.Interface;

namespace LSLibraryApp.Server.Services
{
    public class TopicService : ITopicService
    {
        private readonly ITopicRepository _topicRepository;
        private readonly IBookRepository _bookRepository;

        public TopicService(
            ITopicRepository topicRepository,
            IBookRepository bookRepository)
        {
            _topicRepository = topicRepository;
            _bookRepository = bookRepository;
        }

        public async Task<List<Topic>> GetAllTopics()
        {
            return await _topicRepository.GetAll();
        }

        public async Task<Topic?> GetTopicById(int id)
        {
            return await _topicRepository.GetById(id);
        }

        public async Task<Topic> CreateTopic(Topic topic)
        {
            return await _topicRepository.Create(topic);
        }

        public async Task<Topic> UpdateTopic(Topic topic)
        {
            return await _topicRepository.Update(topic);
        }

        public async Task<TopicUsageResult> GetTopicUsageInformation(int topicId)
        {
            var topic = await _topicRepository.GetById(topicId);

            if (topic is null)
            {
                throw new ArgumentException($"Topic with ID {topicId} not found.");
            }

            var books = await _bookRepository.GetByTopicWithTopicReferences(topicId);

            return new TopicUsageResult
            {
                Topic = topic,
                BooksWithReferences = books,
            };
        }
    }
}
