using FluentValidation.Validators;
using Microsoft.AspNetCore.Http;

namespace TGramHunt.Validators.CustomValidators
{
    public class FileSizeValidator<T> : PropertyValidator<T, IFormFile>
    {
        private readonly int max;

        public FileSizeValidator(int max) => this.max = max;

        public override bool IsValid(FluentValidation.ValidationContext<T> context, IFormFile value)
        {
            if (value is null || value.Length <= max)
            {
                return true;
            }

            return false;
        }

        public override string Name => "FileSizeValidator";
    }
}
