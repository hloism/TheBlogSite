using TheBlogSite.Client.Models;

namespace TheBlogSite.Client.Services.Interfaces
{
    public interface ITaskerItemService
    {
        //only present to front-end with DTO server will also respond

        Task<TaskerItemDTO> CreateTaskerItemAsync(TaskerItemDTO taskerItemDTO, string userId);
        Task<IEnumerable<TaskerItemDTO>> GetTaskersAsync(string userId);
        Task<TaskerItemDTO?> GetTaskerItemByIdAsync(Guid taskerItemId, string userId);

        Task UpdateTaskerItemAsync(TaskerItemDTO taskerItem, string userId);

        Task DeleteTaskerItemAsync(Guid taskerItemId, string userId);
     
    }
}
