using NHIABackendService.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#nullable disable

namespace NHIABackendService.Core.ViewModels
{
    public class UserVM
    {
        public string Id { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public Entities.Enums.UserType UserType { get; set; }


        public static implicit operator UserVM(User model)
        {
            return model == null
                ? null
                : new UserVM
                {
                    FullName = model.LastName + " " + model.FirstName,
                    Email = model.Email,
                    PhoneNumber = model.PhoneNumber,
                    Id = model.Id.ToString(),
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    UserType = model.UserType
                };
        }
    }
}
