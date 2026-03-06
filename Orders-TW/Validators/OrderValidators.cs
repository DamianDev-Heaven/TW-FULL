using FluentValidation;
using Orders_TW.DTOs.Orders;

namespace Orders_TW.Validators
{
    public class CreateOrderDtoValidator : AbstractValidator<CreateOrderDto>
    {
        public CreateOrderDtoValidator()
        {
            RuleFor(x => x.CustomerId)
                .GreaterThan(0).WithMessage("El CustomerId debe ser válido");

            RuleFor(x => x.OrderItems)
                .NotEmpty().WithMessage("La orden debe contener al menos un producto")
                .Must(items => items != null && items.Any())
                .WithMessage("La orden debe contener al menos un producto");

            RuleForEach(x => x.OrderItems)
                .SetValidator(new CreateOrderItemDtoValidator());
        }
    }

    public class CreateOrderItemDtoValidator : AbstractValidator<CreateOrderItemDto>
    {
        public CreateOrderItemDtoValidator()
        {
            RuleFor(x => x.ProductId)
                .GreaterThan(0).WithMessage("El ProductId debe ser válido");

            RuleFor(x => x.Quantity)
                .GreaterThan(0).WithMessage("La cantidad debe ser al menos 1");
        }
    }
}
