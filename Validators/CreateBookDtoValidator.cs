using BookApi.DTOs;
using FluentValidation;
namespace BookApi.Validators;
public class CreateBookDtoValidator : AbstractValidator<CreateBookDto>
{
    public CreateBookDtoValidator()
    {
        RuleFor(x => x.Title) .NotEmpty();

        RuleFor(x => x.Author).NotEmpty();

        RuleFor(x => x.Price).GreaterThan(0);
    }

}