using AutoMapper;
using Microsoft.EntityFrameworkCore;
using OurPlan.Data;
using OurPlan.DTO;
using OurPlan.Entity;
using OurPlan.Helpers;
using OurPlan.Services.Interfaces;

namespace OurPlan.Services
{
    public class GroupTokenService : IGroupTokenService
    {
        private readonly IMapper _mapper;
        private readonly ApplicationDbContext _context;

        public GroupTokenService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
            
        }

        public async Task<ServiceResult<GroupTokenModel>> GenerateToken(int groupId)
        {
            var result = new ServiceResult<GroupTokenModel>();

            try
            {
                var group = await _context.Groups.FirstOrDefaultAsync(e => e.Id == groupId);
                if (group == null)
                {
                    result.ValidationMessage.Add("Group not found");
                    return result;
                }

                var token = Convert.ToBase64String(Guid.NewGuid().ToByteArray())
                    .Replace("=", "")
                    .Replace("+", "")
                    .Replace("/", "");

                var entity = new GroupToken
                {
                    GroupId = groupId,
                    Token = token,
                    ExpiryDate = DateTime.UtcNow.AddMinutes(30)
                };

                _context.GroupTokens.Add(entity);
                await _context.SaveChangesAsync();

                result.Result = _mapper.Map<GroupTokenModel>(entity);



            }
            catch (Exception e)
            {
                result.ValidationMessage.Add($"An error occurred while generating token: {e.Message}");
            }

            return result;
        }
        
        
        
    }
}