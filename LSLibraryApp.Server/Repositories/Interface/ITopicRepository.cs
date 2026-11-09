using LSLibraryApp.Server.Models.Entity;

namespace LSLibraryApp.Server.Repositories.Interface
{
    public interface ITopicRepository
    {
        Task<List<Topic>> GetAll();
        Task<Topic?> GetById(int id);
        Task<Topic> Create(Topic topic);
        Task<Topic> Update(Topic topic);
    }
}
