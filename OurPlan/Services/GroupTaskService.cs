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

    public class GroupTaskService : IGroupTaskService
    {

        public readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
       

        public GroupTaskService(ApplicationDbContext context, IMapper mapper, IUserService userService,
            IGroupService groupService)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<ServiceResult<List<GroupTaskModel>>> GetAllGroupTasks(int groupId)
        {
            var result = new ServiceResult<List<GroupTaskModel>>();

            try
            {

                var tasks = await _context.GroupTasks
                    .Where(x => x.GroupId == groupId)
                    .ToListAsync();

                result.Result = _mapper.Map<List<GroupTaskModel>>(tasks);
            }
            catch (Exception e)
            {
                result.ValidationMessage.Add($"An error occurred while retrieving the tasks: {e.Message}");
            }
            
            return result;
        }

        public async Task<ServiceResult<GroupTaskModel>> GetGroupTask(int groupId, int taskId)
        {
            var result = new ServiceResult<GroupTaskModel>();

            try
            {

                var task = await _context.GroupTasks
                    .FirstOrDefaultAsync(e => e.Id == taskId  && e.GroupId == groupId);

                if (task == null)
                {
                    result.ValidationMessage.Add("Nu exista acest task");
                    return result;
                }
                
                result.Result = _mapper.Map<GroupTaskModel>(task);
                
                

            }
            catch (Exception e)
            {
                result.ValidationMessage.Add($"An error occurred while retrieving the task: {e.Message}");
            }
            
            return result;
        }

        public async Task<ServiceResult<GroupTaskModel>> UpdateGroupTask(GroupTaskModel groupTask)
        {
            var result = new ServiceResult<GroupTaskModel>();

            var exist = await _context.GroupTasks.FirstOrDefaultAsync(e => e.Id == groupTask.Id);
            
            if (exist == null)
            {
                result.ValidationMessage.Add("Task not found");
                return result;
            }

            try
            {
                exist.Title = groupTask.Title;
                exist.Description = groupTask.Description;
                exist.StartDate = groupTask.StartDate;
                exist.DueDate = groupTask.DueDate;
                exist.Priority = groupTask.Priority;
                _context.GroupTasks.Update(exist);
                await _context.SaveChangesAsync();
                result.Result = _mapper.Map<GroupTaskModel>(exist);

            }
            catch (Exception e)
            {
                result.ValidationMessage.Add($"An error occurred while updating the task: {e.Message}");
            }

            return result;
        }


        public async Task<ServiceResult<List<GroupTaskModel>>> GetTasksForCurrentDay(int groupId)
        {
            var result = new ServiceResult<List<GroupTaskModel>>();

            try
            {
                var today = DateTime.Today;
                
                var task = await _context.GroupTasks
                    .Where(e => e.GroupId == groupId && e.StartDate.Date == today)
                    .ToListAsync();
                
                result.Result = _mapper.Map<List<GroupTaskModel>>(task);

            }
            catch (Exception e)
            {
                result.ValidationMessage.Add($"An error occurred while fetching tasks: {e.Message}");
                
            }
            
            return result;
            
        }

        
        public async Task<ServiceResult<List<GroupTaskModel>>> GetValideTasks(int groupId)
        {
            var result = new ServiceResult<List<GroupTaskModel>>();

            try
            {
                var today = DateTime.Today;
                
                
                var task = await _context.GroupTasks
                    .Where(e => e.GroupId == groupId )
                    .Where(t => t.DueDate == null || t.DueDate> today)
                    .OrderBy(t => t.DueDate)
                    .ToListAsync();
                
                result.Result = _mapper.Map<List<GroupTaskModel>>(task);

            }
            catch (Exception e)
            {
                result.ValidationMessage.Add($"An error occurred while fetching valid tasks: {e.Message}");
                
            }
            
            return result;
            
        }

        public async Task<ServiceResult<bool>> DeleteGroupTask(int groupId, int taskId)
        {
            var result = new ServiceResult<bool>();

            try
            {
                var task = await _context.GroupTasks.FirstOrDefaultAsync(e => e.Id == taskId && e.GroupId == groupId);

                if (task == null)
                {
                    result.ValidationMessage.Add("Task not found");
                    return result;
                }

                _context.GroupTasks.Remove(task);

                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                result.ValidationMessage.Add($"An error occurred while deleting the task: {e.Message}");
                
            }

            return result;
            
        }

        public async Task<ServiceResult<GroupTaskModel>> CreateGroupTask(GroupTaskModel groupTask)
        {
            var result = new ServiceResult<GroupTaskModel>();
            
            var groupExists = await _context.Groups.AnyAsync(g => g.Id == groupTask.GroupId);
    
            if (!groupExists)
            {
                result.ValidationMessage.Add("Group not found");
                return result;
            }

            try
            {
                var newTask = new GroupTask();
                
                newTask.Title = groupTask.Title;
                newTask.Description = groupTask.Description;
                newTask.StartDate = groupTask.StartDate;
                newTask.DueDate = groupTask.DueDate;
                newTask.Priority = groupTask.Priority;
                
                newTask.GroupId = groupTask.GroupId;
                newTask.CreatedByUserId = groupTask.CreatedByUserId;
                
                
                await _context.GroupTasks.AddAsync(newTask);
                await _context.SaveChangesAsync();
                result.Result = _mapper.Map<GroupTaskModel>(newTask);

            }
            catch (Exception e)
            {
                result.ValidationMessage.Add($"An error occurred while creating the task: {e.Message}");
            }
            
            return result;
            
        }

    
    }



}

