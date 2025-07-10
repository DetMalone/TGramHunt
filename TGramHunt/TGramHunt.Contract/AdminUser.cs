using AspNetCore.Identity.MongoDbCore.Models;
using System;

namespace TGramHunt.Contract
{
    public class AdminUser : MongoIdentityUser<Guid>
    {
        public AdminUser() : base() 
        {
        }

        public AdminUser(string userName, string email) : base(userName, email)
        {
        }

        public DateTime RegistrationDate { get; set; }
        public DateTime BlockedDate { get; set; }
    }
}
