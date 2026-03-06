using System.ComponentModel.DataAnnotations;

namespace Orders_TW.DTOs.Customers
{
    public class CreateCustomerDto
    {
        [Required(ErrorMessage = "El nombre completo es requerido")]
        [StringLength(150, MinimumLength = 2, ErrorMessage = "El nombre debe tener entre 2 y 150 caracteres")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "El email es requerido")]
        [EmailAddress(ErrorMessage = "El formato del email no es válido")]
        public string Email { get; set; } = string.Empty;
    }
}
