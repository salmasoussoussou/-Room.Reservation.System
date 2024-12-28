using System.ComponentModel.DataAnnotations;

namespace projet1.DTO
{
    public class dtoLogin
    //pour l'authentification des utilisateurs.
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
