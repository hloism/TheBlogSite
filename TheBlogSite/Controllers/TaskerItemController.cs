using TheBlogSite.Client.Models;
using TheBlogSite.Client.Services.Interfaces;
using TheBlogSite.Components.Account;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace TheBlogSite.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TaskerItemController : ControllerBase
    {
        private readonly ITaskerItemService _taskerItemService;

        //always get UserId with authorize  
        private string _userId => User.GetUserId()!;

        //constructor = from a  class bring me an object 
        public TaskerItemController(ITaskerItemService taskerItemService)
        {
            _taskerItemService = taskerItemService;
        }


        //method must be DTO for front-end condensed set of properties
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TaskerItemDTO>>> GetTaskerItemsAsync()
        {
            var taskerItems = await _taskerItemService.GetTaskersAsync(_userId);
            return Ok(taskerItems);
        }



        [HttpPost]
        public async Task<ActionResult<TaskerItemDTO>> PostTaskerItemAsync([FromBody] TaskerItemDTO taskerItem)
        {
            TaskerItemDTO createdTaskerItem = await _taskerItemService.CreateTaskerItemAsync(taskerItem, _userId);

            return Ok(createdTaskerItem);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<TaskerItemDTO>> GetTaskerItemByIdAsync([FromRoute] Guid id)
        {
            TaskerItemDTO? taskerItem = await _taskerItemService.GetTaskerItemByIdAsync(id, _userId);

            if (taskerItem == null)
            { 
                return NotFound();
            }

            return Ok(taskerItem);

        }
        //edit of data 
        [HttpPut("{id:guid}")]
        public async Task<ActionResult> UpdateTaskerItemsAsync([FromRoute] Guid id, [FromBody] TaskerItemDTO taskerItem)
        {
            if (id != taskerItem.Id)
            { 
                return BadRequest();
            }

            if (await _taskerItemService.GetTaskerItemByIdAsync(id, _userId) == null)
            {
                NotFound();
            }

            await _taskerItemService.UpdateTaskerItemAsync(taskerItem, _userId);
            return NoContent();
        
        }

        [HttpDelete("{id:guid}")]
        public async Task<ActionResult> DeleteTaskerItemAsync([FromRoute] Guid id)
        { 
            await _taskerItemService.DeleteTaskerItemAsync(id, _userId);
            return NoContent();
        
        }


    }
}
