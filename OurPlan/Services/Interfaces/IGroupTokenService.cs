using OurPlan.DTO;
using OurPlan.Helpers;


namespace OurPlan.Services.Interfaces
{
    public interface IGroupTokenService
    {
        public Task<ServiceResult<string>> GenerateToken(int groupId);

        public Task<ServiceResult<bool>> JoinGroupByToken(string token);
    }
    
}

