using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
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
        public readonly ICurrentUserService _currentUserService;
        public readonly IUserService _userService;
        public readonly IMapper _mapper;

        public GroupService(ApplicationDbContext context, ICurrentUserService currentUserService, IUserService userService, IMapper mapper)
        {
            _context = context;
            _userService = userService;
            _mapper = mapper;
            _currentUserService = currentUserService;
        }

        public async Task<ServiceResult<GroupModel>> CreateGroup(GroupModel model)
        {
            var result = new ServiceResult<GroupModel>();

            try
            {
                var currentUser = _currentUserService.UserId;
                if (currentUser == null)
                {
                    result.ValidationMessage.Add("User not authenticated");
                    return result;
                }

                model.CreatedByUserId = (int)currentUser;
                if (_context.Groups.Any(x => x.CreatedByUserId == currentUser))
                {
                    throw new Exception("This user already has a group.");
                }
                var entity = _mapper.Map<Group>(model);
                _context.Groups.Add(entity);
                await _context.SaveChangesAsync();

                var userGroup = new UserGroup
                {
                    UserId = model.CreatedByUserId,
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
            var result = new ServiceResult<GroupModel>();

            var entity = _context.Groups.FirstOrDefault(e => e.Id == groupId);

            if (entity == null)
            {
                result.ValidationMessage.Add("Group not found");
                return result;
            }

            _context.Groups.Remove(entity);
            await _context.SaveChangesAsync();

            return result;


        }


        public async Task<ServiceResult<GroupModel>> UpdateGroup(GroupModel model)
        {

            var result = new ServiceResult<GroupModel>();
            var entity = _context.Groups.FirstOrDefault(e => e.Id == model.Id);

            if (entity == null)
            {
                result.ValidationMessage.Add("Group not found");
                return result;
            }

            try
            {
                entity.Name = model.Name;
                entity.CreatedByUserId = model.CreatedByUserId;
                entity.UserGroups = model.UserGropus;

                _context.Groups.Update(entity);
                await _context.SaveChangesAsync();
                result.Result = model;

            }
            catch (Exception e)
            {

                result.ValidationMessage.Add($"An error occurred while updating the group: {e.Message}");
            }

            return result;


        }

        public async Task<ServiceResult<GroupModel>> GetGroupsForCurrentUser()
        {
            var result = new ServiceResult<GroupModel>();

            try
            {
                var currentuser = _currentUserService.UserId;
                if (currentuser == null)
                {
                    result.ValidationMessage.Add("User not authenticated");
                    return result;
                }

                var groups = await _context.UserGroups.Include(x => x.Group).Where(x => x.UserId == currentuser)
                    .Select(x => x.Group)
                    .FirstOrDefaultAsync();// doar pentru primul
                result.Result = _mapper.Map<GroupModel>(groups);

            }
            catch (Exception e)
            {
                result.ValidationMessage.Add($"An error occurred while retrieving the groups: {e.Message}");

            }

            return result;
        }

        public async Task<ServiceResult<List<GroupModel>>> GetAllGroups()
        {
            var result = new ServiceResult<List<GroupModel>>();

            try
            {
                var groups = await _context.Groups.Include(x => x.UserGroups)
                    .ThenInclude(x => x.User)
                    .ToListAsync();

                result.Result = _mapper.Map<List<GroupModel>>(groups);
            }
            catch (Exception ex)
            {
                result.ValidationMessage.Add($"An error occurred while retrieving the groups: {ex.Message}");

            }

            return result;
        }





    }
}