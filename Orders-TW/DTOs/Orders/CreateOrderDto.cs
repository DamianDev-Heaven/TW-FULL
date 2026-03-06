using System.ComponentModel.DataAnnotations;

namespace Orders_TW.DTOs.Orders
{
    public class CreateOrderDto
    {
        [Required(ErrorMessage = "El CustomerId es requerido")]
        [Range(1, int.MaxValue, ErrorMessage = "El CustomerId debe ser válido")]
        public int CustomerId { get; set; }

        [Required(ErrorMessage = "Debe incluir al menos un item")]
        [MinLength(1, ErrorMessage = "Debe incluir al menos un item")]
        public List<CreateOrderItemDto> OrderItems { get; set; } = new();
    }

    public class CreateOrderItemDto
    {
        [Required(ErrorMessage = "El ProductId es requerido")]
        [Range(1, int.MaxValue, ErrorMessage = "El ProductId debe ser válido")]
        public int ProductId { get; set; }

        [Required(ErrorMessage = "La cantidad es requerida")]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser al menos 1")]
        public int Quantity { get; set; }
    }
}
