using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TGramHunt.AdminPanel.Models;
using TGramHunt.Services.Services.IServices;
using System.ComponentModel.DataAnnotations;

namespace TGramHunt.AdminPanel.Views.Login
{
    public class LoginViewModel
    {
        [Required]
        [EmailAddress(ErrorMessage = "Invalid Email")]
        public string? Email { get; set; }
        [Required]
        [DataType(DataType.Password, ErrorMessage = "Invalid Password")]
        public string? Password { get; set; }
    }
}
