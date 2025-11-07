using OurPlan.DTO;
using OurPlan.Helpers;


namespace OurPlan.Services.Interfaces
{
    public interface IGroupService
    {
        public Task<ServiceResult<GroupModel>> GetGroupsForCurrentUser();
        public Task<ServiceResult<List<GroupModel>>> GetAllGroups();

        public Task<ServiceResult<GroupModel>> CreateGroup(GroupModel model);

        public Task<ServiceResult<GroupModel>> UpdateGroup(GroupModel model);

        public Task<ServiceResult<GroupModel>> DeleteGroup(int groupId);
    }
}
