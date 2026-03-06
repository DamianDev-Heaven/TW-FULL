using FluentValidation;
using Orders_TW.DTOs.Auth;

namespace Orders_TW.Validators
{
    public class LoginRequestValidator : AbstractValidator<LoginRequest>
    {
        public LoginRequestValidator()
        {
            RuleFor(x => x.Username)
                .NotEmpty().WithMessage("El nombre de usuario es requerido")
                .Length(3, 50).WithMessage("El usuario debe tener entre 3 y 50 caracteres");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("La contraseña es requerida")
                .MinimumLength(4).WithMessage("La contraseña debe tener al menos 4 caracteres");
        }
    }
}
