using TheBlogSite.Models;

namespace TheBlogSite.Services.Interfaces
{
    //name of method and what it will do, code not needed
    public interface ITaskerItemRepository
    {
        Task<IEnumerable<TaskerItem>> GetTaskerItemsAsync(string userId);

        Task<TaskerItem> CreateaTaskerItemAsync(TaskerItem taskerItem);
        
        Task<TaskerItem?> GetTaskerItemByIdAsync(Guid taskerItemId, string userId);

        Task UpdateTaskerItemAsync(TaskerItem taskerItem);

        Task DeleteTaskerItemAsync(Guid taskerItemId, string userId);
      
    }
}
