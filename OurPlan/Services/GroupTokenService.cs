using System.Security.Cryptography;
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
        public readonly ICurrentUserService _currentUserService;

        public GroupTokenService(ApplicationDbContext context, IMapper mapper, ICurrentUserService currentUserService)
        {
            _context = context;
            _mapper = mapper;
            _currentUserService = currentUserService;
            
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
                
                var checkUserInGroup = await CheckUser(groupId);
                if (!checkUserInGroup.Result)
                {
                    result.ValidationMessage.Add("You are not a member of this group and cannot generate a token.");
                    result.ValidationMessage.AddRange(checkUserInGroup.ValidationMessage);
                    return result;
                }

                await RemoveValidTokens(groupId);

                var token = GenerateTokenString(8);
                    

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

     



        public async Task<ServiceResult<bool>> CheckUser(int groupId)
        {
            var result = new ServiceResult<bool>();

            try
            {
                var currentUserId = _currentUserService.UserId;
                if (currentUserId == null)
                {
                    result.ValidationMessage.Add("User not found");
                    result.Result = false;
                    return result;
                }

                bool InGroup = await _context.UserGroups
                    .AnyAsync(e => e.GroupId == groupId && e.UserId == currentUserId);
                
                if(!InGroup) 
                    result.ValidationMessage.Add("User does not belong to this group");
                result.Result = InGroup;
            }
            
            catch (Exception e)
            {
                result.ValidationMessage.Add($"An error occurred while checking user: {e.Message}");
                result.Result = false;
                
            }
            return result;
        }


        public async Task<ServiceResult<bool>> JoinGroupByToken(string token)
        {
            var result = new ServiceResult<bool>();

            try
            {
                var currentUserId = _currentUserService.UserId;
                if (currentUserId == null)
                {
                    result.ValidationMessage.Add("User not found");
                    result.Result = false;
                    return result;
                }
                
                var groupToken = await _context.GroupTokens
                    .FirstOrDefaultAsync(x => x.Token == token);

                if (groupToken == null)
                {
                    result.ValidationMessage.Add("Invalid token");
                    result.Result = false;
                    return result;
                }

                if (groupToken.ExpiryDate < DateTime.UtcNow)
                {
                    result.ValidationMessage.Add("Token expired");
                    result.Result = false;
                    return result;
                }
                
                var group = await _context.Groups.FirstOrDefaultAsync(x => x.Id == groupToken.GroupId);
                if (group == null)
                {
                    result.ValidationMessage.Add("Group not found");
                    result.Result = false;
                    return result;
                }
                
                var member = await _context.UserGroups
                    .AnyAsync(x => x.GroupId == group.Id && x.UserId == currentUserId);
                if (member)
                {
                    result.ValidationMessage.Add("User already in this this group");
                    result.Result = false;
                    return result;
                }

                var userGroup = new UserGroup
                {
                    GroupId = group.Id,
                    UserId = (int)currentUserId
                };
                
                _context.UserGroups.Add(userGroup);
                await _context.SaveChangesAsync();
                
                result.Result = true;
            }
            catch (Exception e)
            {
                result.ValidationMessage.Add($"An error occurred while joining group: {e.Message}");
                result.Result = false;
            }
            return result;
        }

        private async Task RemoveValidTokens(int groupId)
        {
            var date = DateTime.UtcNow;
            
            var existingGroup = await _context.GroupTokens
                .Where(x => x.GroupId == groupId && x.ExpiryDate < date)
                .ToListAsync();

            if (!existingGroup.Any())
            {
                _context.GroupTokens.RemoveRange(existingGroup);
                await _context.SaveChangesAsync();
            }
        }
        
        private static string GenerateTokenString(int length )
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var bytes = new byte[length];
            using var random = RandomNumberGenerator.Create();
            random.GetBytes(bytes);
            return new string(bytes.Select(x => chars[x % chars.Length]).ToArray());
        }

    }
    
}