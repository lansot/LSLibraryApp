using LSLibraryApp.Server.Models.Entity;
using LSLibraryApp.Server.Models.Service;

namespace LSLibraryApp.Server.Services.Interface
{
    public interface ITopicService
    {
        Task<List<Topic>> GetAllTopics();
        Task<Topic?> GetTopicById(int id);
        Task<Topic> CreateTopic(Topic topic);
        Task<Topic> UpdateTopic(Topic topic);
        Task<TopicUsageResult> GetTopicUsageInformation(int topicId);
    }
}
