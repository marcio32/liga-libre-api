using System.ComponentModel.DataAnnotations;

namespace LigaLibre.WebMVC.Models
{
    public class PlayerViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es requerido")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder 100 caracteres")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "El apellido es requerido")]
        [StringLength(100, ErrorMessage = "El apellido no puede exceder 100 caracteres")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "La posición es requerida")]
        [StringLength(50)]
        public string Position { get; set; } = string.Empty;

        [Required(ErrorMessage = "La nacionalidad es requerida")]
        [StringLength(100)]
        public string Nationality { get; set; } = string.Empty;

        [Required(ErrorMessage = "La edad es requerida")]
        [Range(15, 50, ErrorMessage = "La edad debe estar entre 15 y 50 años")]
        public int Age { get; set; }

        [Required(ErrorMessage = "El número de camiseta es requerido")]
        [Range(1, 99, ErrorMessage = "El número debe estar entre 1 y 99")]
        public int JerseyNumber { get; set; }

        [Required(ErrorMessage = "La altura es requerida")]
        [Range(1.40, 2.20, ErrorMessage = "La altura debe estar entre 1.40 y 2.20 metros")]
        public decimal Height { get; set; }

        [Required(ErrorMessage = "El peso es requerido")]
        [Range(50, 120, ErrorMessage = "El peso debe estar entre 50 y 120 kg")]
        public decimal Weight { get; set; }

        [Required(ErrorMessage = "El club es requerido")]
        public int ClubId { get; set; }

        [Required(ErrorMessage = "La fecha de nacimiento es requerida")]
        public DateTime DateOfBirth { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
