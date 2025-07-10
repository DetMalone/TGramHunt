using FluentValidation;
using System.Collections.Generic;
using TGramHunt.ViewModels;

namespace TGramHunt.Validators
{
    public class CreateProductViewModelValidator : AbstractValidator<CreateProductViewModel>
    {
        public CreateProductViewModelValidator()
        {
            var signatures = new List<byte[]>()
            {
                new byte[] { 0xFF, 0xD8, 0xFF, 0xE0 },
                new byte[] { 0xFF, 0xD8, 0xFF, 0xE1 },
                new byte[] { 0xFF, 0xD8, 0xFF, 0xE2 }, //jpg
                new byte[] { 0x89, 0x50, 0x4E, 0x47 } // png
            };

            RuleFor(product => product.Tag)
                .NotNull()
                .NotEmpty()
                .Length(6, 33)
                .Matches("^@[a-zA-Z]([a-zA-Z0-9][_]?)+[a-zA-Z0-9]$");

            RuleFor(product => product.Name)
                .NotNull()
                .NotEmpty()
                .MaximumLength(255);

            RuleFor(product => product.Description)
                .NotNull()
                .NotEmpty()
                .MaximumLength(2000);

            RuleFor(product => product.Category)
                .IsInEnum();

            RuleFor(product => product.Makers)
                .ListItemsMatches("^[a-zA-Z][a-zA-Z’ -]+$")
                .ListMaxCount(6)
                .ListItemsMaxLength(100);

            RuleFor(product => product.Cover)
                .FileSmallerThan(1024 * 1024 * 10)
                .AllowedExtensions(signatures)
                .NotNull();
        }
    }
}