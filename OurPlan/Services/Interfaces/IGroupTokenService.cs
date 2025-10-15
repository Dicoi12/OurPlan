using OurPlan.DTO;
using OurPlan.Helpers;


namespace OurPlan.Services.Interfaces
{
    public interface IGroupTokenService
    {
        public Task<ServiceResult<GroupTokenModel>> GenerateToken(int groupId);
        public Task<ServiceResult<bool>> CheckUser(int groupId);

        public Task<ServiceResult<bool>> JoinGroupByToken(string token);
    }
    
}

