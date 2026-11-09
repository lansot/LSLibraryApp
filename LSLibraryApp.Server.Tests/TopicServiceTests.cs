using LSLibraryApp.Server.Repositories.Interface;
using LSLibraryApp.Server.Services;
using LSLibraryApp.Server.Services.Interface;
using Moq;

namespace LSLibraryApp.Server.Tests
{
    public class TopicServiceTests
    {
        private readonly ITopicService _topicService;
        private readonly Mock<ITopicRepository> _mockTopicRepository;
        private readonly Mock<IBookRepository> _mockBookRepository;

        public TopicServiceTests()
        {
            _mockTopicRepository = new Mock<ITopicRepository>();
            _mockBookRepository = new Mock<IBookRepository>();
            _topicService = new TopicService(
                _mockTopicRepository.Object,
                _mockBookRepository.Object);
        }

        // TODO tests... when there's more business logic to test in the service?
    }
}
