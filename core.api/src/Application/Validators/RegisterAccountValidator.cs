using System.ComponentModel.DataAnnotations;
using FluentValidation;
using Domain.ApiContracts.AccountRegistration;
using Infrastructure.Repository.Interfaces;
using Infrastructure.Services.Interfaces;

namespace Application.Validators;

public class RegisterAccountValidator : AbstractValidator<AccountRegistrationRequest>
{
    public RegisterAccountValidator(IApplicationUserRepository userRepository, ICryptoService cryptoService)
    {
        var userRepository1 = userRepository;
        var cryptoService1 = cryptoService;
        
        RuleFor(user => user.Email)
            .NotEmpty().WithMessage("Email is required.")
            .Matches(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$")
            .WithMessage("Invalid email format.");
        RuleFor(user => user.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters long.")
            .Matches(@"[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
            .Matches(@"[a-z]").WithMessage("Password must contain at least one lowercase letter.")
            .Matches(@"\d").WithMessage("Password must contain at least one digit.")
            .Matches(@"[\W_]").WithMessage("Password must contain at least one special character.");
        RuleFor(x => x.FirstName).NotEmpty().WithMessage("First name is required");
        RuleFor(x => x.LastName).NotEmpty().WithMessage("Last name is required");
        RuleFor(x => x.Password).Equal(x => x.PasswordConfirmation).WithMessage("Passwords don't match");

        RuleFor(user => user.Email).MustAsync(async (email, cancellation) =>
        {
            string emailHash = cryptoService1.HashValue(email);
            var user = await userRepository1.GetUserByEmail(emailHash);

            return user == null;
        }).WithMessage("An account with that email address already exists.");
    }
}