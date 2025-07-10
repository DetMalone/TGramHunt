using AspNetCore.Identity.MongoDbCore.Models;
using System;

namespace TGramHunt.Contract
{
    public class Role : MongoIdentityRole<Guid>
    {
        public Role() : base()
        {
        }

        public Role(string roleName) : base(roleName)
        {
        }
    }
}