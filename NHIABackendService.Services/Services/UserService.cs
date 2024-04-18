using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NHIABackendService.Core.DataAccess.EfCore.Context;
using NHIABackendService.Core.DataAccess.EfCore.UnitOfWork;
using NHIABackendService.Core.Pagination;
using NHIABackendService.Core.ViewModels;
using NHIABackendService.Entities;
using NHIABackendService.Services.Interface;
using NHIABackendService.Services.Model;

namespace NHIABackendService.Services.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;

        private readonly UserManager<User> _userManager;
        private readonly ApplicationDbContext _context;

        public UserService(IUnitOfWork unitOfWork, UserManager<User> userManager, ApplicationDbContext context)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _context = context;
        }

        public async Task<User> FindUserById(Guid? userId)
        {
            if (userId == null || userId == Guid.Empty)
                return null;

            var user = await _userManager.FindByIdAsync(userId.ToString());

            return user;
        }

        public User FindUserByName(string firstName, string lastName)
        {
            if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName))
                return null;

            return _userManager.Users.FirstOrDefault(x => x.FirstName == firstName && x.LastName == lastName);
        }

        public async Task<ResultModel<PagedList<UserVM>>> SearchUsers(SearchVM model)
        {
            var nameToSearch = model.Filters.FirstOrDefault()?.Keyword.ToLower().Trim();
            var query = _userManager.Users.Where(x => x.FullName.ToLower().Contains(nameToSearch));

            var users = await query.ToPagedListAsync(model.PageIndex, model.PageSize);
            var userVMs = users.Select(x => (UserVM)x).ToList();

            var data = new PagedList<UserVM>(userVMs, model.PageIndex, model.PageSize, users.TotalItemCount);

            return new ResultModel<PagedList<UserVM>> { Data = data, Message = $"Found {users.Count} Users." };
        }

        public async Task<ResultModel<List<UserVM>>> GetAllUsers()
        {
            ResultModel<List<UserVM>> resultModel = new ResultModel<List<UserVM>>();

            try
            {
                var users = await _userManager.Users.ToListAsync();
                resultModel.Data = users.Select(x => (UserVM)x).ToList();
                resultModel.Message = $"Found {users.Count} Users.";
            }
            catch (Exception error)
            {
                resultModel.AddError(error.Message);
                resultModel.AddError(error.StackTrace);
            }

            return resultModel;
        }

        public async Task<ResultModel<UserVM>> CreateUser(CreateUserVM model)
        {
            var result = new ResultModel<UserVM>();

            try
            {
                User alreadyExists = await _userManager.FindByEmailAsync(model.Email);
                if (alreadyExists != null)
                {
                    result.AddError("User with this credentials already exist.");
                    return result;
                }

                //var passwordValidation = new System.Text.RegularExpressions.Regex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,15}$");
                var passwordValidation = new System.Text.RegularExpressions.Regex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,15}$");
                if (!passwordValidation.IsMatch(model.Password))
                {
                    result.AddError("Your password must contain at least 1 uppercase, 1 lowercase, 1 number and a special character");
                    return result;
                }

                var user = new User
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Email = model.Email,
                    UserName = model.Email,
                    PhoneNumber = model.PhoneNumber,
                    UserType = model.UserType,
                    FullName = model.LastName + " " + model.FirstName,
                };

                var userResult = await _userManager.CreateAsync(user, model.Password);

                if (!userResult.Succeeded)
                {
                    result.AddError(string.Join(';', userResult.Errors.Select(x => x.Description)));
                    return result;
                }

                result.Data = user;
            }
            catch (Exception error)
            {
                result.AddError(error.Message);
                result.AddError(error.StackTrace);
            }

            return result;
        }

        public async Task<ResultModel<UserVM>> UpdateUser(UpdateUserVM model)
        {
            var result = new ResultModel<UserVM>();
            var user = await _userManager.FindByIdAsync(model.UserId.ToString());

            if (user == null)
            {
                result.AddError("User not found");
                return result;
            }

            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.PhoneNumber = model.PhoneNumber;

            await _userManager.UpdateAsync(user);

            result.Data = user;
            return result;
        }

        public async Task<ResultModel<bool>> DeleteUser(Guid userId)
        {
            var result = new ResultModel<bool>();
            var user = await _userManager.FindByIdAsync(userId.ToString());

            if (user == null)
            {
                result.AddError("User not found");
                return result;
            }

            user.IsDeleted = true;

            await _userManager.UpdateAsync(user);

            result.Data = true;
            return result;
        }
    }
}
