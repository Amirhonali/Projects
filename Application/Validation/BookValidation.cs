using Domain.Entities;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Validation
{
    public class BookValidation:AbstractValidator<Book>
    {
        public BookValidation()
        {

            RuleFor(book => book.Name).NotEmpty()
                .MaximumLength(100)
                .MinimumLength(2)
                .NotNull()
                .WithMessage("Book cannot be null or empty!");

            RuleFor(book => book.PublishDate).NotEmpty()
                .NotNull()
                .Must(x => x < DateOnly.FromDateTime(DateTime.Now))
                .WithMessage("Date cannot be upper today");   

        }
    }
}
