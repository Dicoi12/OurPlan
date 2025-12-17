using OurPlan.DTO;
using OurPlan.Helpers;

namespace OurPlan.Services.Interfaces
{
    public interface IGroupTaskService
    {
    
        public Task<ServiceResult<GroupModel>> CreateGroupTask(GroupTaskModel groupTask);
        public Task<ServiceResult<List<GroupTaskModel>>> GetAllGroupTasks(int groupId);
        public Task<ServiceResult<GroupTaskModel>> GetGroupTask(int groupId, int taskId);
        public Task<ServiceResult<List<GroupTaskModel>>> GetTasksCurrentDay(int groupId);
        
        public Task<ServiceResult<GroupTaskModel>> UpdateGroupTask(int groupId,int taskId, GroupTaskModel groupTask);
        public Task<ServiceResult<bool>> DeleteGroupTask(int groupId, int taskId);
        
        
    }
}