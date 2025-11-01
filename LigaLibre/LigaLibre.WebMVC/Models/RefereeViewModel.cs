using System.ComponentModel.DataAnnotations;

namespace LigaLibre.WebMVC.Models
{
    public class RefereeViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es requerido")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder 100 caracteres")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "El apellido es requerido")]
        [StringLength(100, ErrorMessage = "El apellido no puede exceder 100 caracteres")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "La categoría es requerida")]
        public int Category { get; set; }

        [Required(ErrorMessage = "El número de licencia es requerido")]
        [StringLength(50)]
        public string LicenseNumber { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;
    }
}
