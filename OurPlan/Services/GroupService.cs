using AutoMapper;
using Microsoft.EntityFrameworkCore;
using OurPlan.Data;
using OurPlan.DTO;
using OurPlan.Entity;
using OurPlan.Helpers;
using OurPlan.Services.Interfaces;


namespace OurPlan.Services
{
    public class GroupService : IGroupService
    {
        public readonly ApplicationDbContext _context;
        public readonly IUserService _userService;
        public readonly IMapper _mapper;
        private IGroupService _groupServiceImplementation;

        public GroupService(ApplicationDbContext context, IUserService userService, IMapper mapper)
        {
            _context = context;
            _userService = userService;
            _mapper = mapper;
        }

        public async Task<ServiceResult<GroupModel>> CreateGroup(GroupModel model)
        {
            var result = new ServiceResult<GroupModel>();

            try
            {
                var currentUser = _userService.GetCurrentUser();
                if (currentUser == null)
                {
                    result.ValidationMessage.Add("User not authenticated");
                    return result;
                }

                model.CreatedByUserId = currentUser.Id;
                var entity = _mapper.Map<Group>(model);
                _context.Groups.Add(entity);
                await _context.SaveChangesAsync();

                var userGroup = new UserGroup
                {
                    UserId = currentUser.Id,
                    GroupId = entity.Id
                };

                _context.UserGroups.Add(userGroup);
                await _context.SaveChangesAsync();
                model.Id = entity.Id;

                result.Result = model;
            }

            catch (Exception ex)
            {
                result.ValidationMessage.Add($"An error occurred while creating the event: {ex.Message}");
                
            }

            return result;
        }

        public async Task<ServiceResult<GroupModel>> DeleteGroup(int groupId)
        {
            
        }
        
        public async Task<ServiceResult<GroupModel>> UpdateGroup(GroupModel model)
        {
            
        }

        public async Task<ServiceResult<List<GroupModel>>> GetAllGroups()
        {
            
        }
        
        
        
        
        
    }
}