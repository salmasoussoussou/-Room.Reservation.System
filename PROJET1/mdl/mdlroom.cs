using System.ComponentModel.DataAnnotations;

namespace projet1.mdl
{
    public class mdlroom
    {
        [Required(ErrorMessage = "The Name field is required.")]
        public string Name { get; set; }

        [Range(1, 500, ErrorMessage = "Capacity must be between 1 and 500.")]
        public int Capacity { get; set; }

        public bool IsAvailable { get; set; }
    }

}
