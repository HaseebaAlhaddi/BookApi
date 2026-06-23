using BookApi.DTOs;
using FluentValidation;
namespace BookApi.Validators;
public class UpdateBookDtoValidator : AbstractValidator<UpdateBookDto>
{
    public UpdateBookDtoValidator()
    {
        RuleFor(x => x.Title).NotEmpty();
        RuleFor(x => x.Author).NotEmpty();
        RuleFor(x => x.Price).GreaterThan(0);
    }
}