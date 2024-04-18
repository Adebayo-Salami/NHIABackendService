using Microsoft.Graph.Models.Security;
using NHIABackendService.Core.Pagination;
using NHIABackendService.Core.ViewModels;
using NHIABackendService.Entities;
using NHIABackendService.Services.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NHIABackendService.Services.Interface
{
    public interface IUserService
    {
        Task<User> FindUserById(Guid? userId);
        User FindUserByName(string firstName, string lastName);
        Task<ResultModel<PagedList<UserVM>>> SearchUsers(SearchVM model);
        Task<ResultModel<UserVM>> CreateUser(CreateUserVM model);
        Task<ResultModel<UserVM>> UpdateUser(UpdateUserVM model);
        Task<ResultModel<bool>> DeleteUser(Guid userId);
        Task<ResultModel<List<UserVM>>> GetAllUsers();
    }
}
