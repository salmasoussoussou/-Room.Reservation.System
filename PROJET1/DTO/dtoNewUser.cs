using System.ComponentModel.DataAnnotations;

namespace projet1.DTO
{
    public class dtoNewUser
    {

        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
    }
}

