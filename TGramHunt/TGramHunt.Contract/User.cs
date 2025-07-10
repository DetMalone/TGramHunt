using AspNetCore.Identity.MongoDbCore.Models;
using System;

namespace TGramHunt.Contract
{
    public class User : MongoIdentityUser<Guid>
    {
        public User() : base()
        {
        }

        public User(string userName, string email) : base(userName, email)
        {
        }

        public string? Name { get; set; }

        public string? Surname { get; set; }

        public DateTime RegistrationDate { get; set; }

        public string? Picture { get; set; }

        public string? PictureIdx41 { get; set; }

        public string? PictureIdx100 { get; set; }

        public int PictureCache { get; set; } = 1;

        public bool IsClosed { get; set; }

        public long TelegramNativeId { get; set; }

        public string? TelegramUserName { get; set; }
    }
}