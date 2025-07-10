using FluentValidation;
using FluentValidation.Validators;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TGramHunt.Validators.CustomValidators
{
    public class AllowedExtensionsValidator<T> : PropertyValidator<T, IFormFile>
    {
        private readonly IList<byte[]> signatures;

        public AllowedExtensionsValidator(IList<byte[]> signatures)
        {
            this.signatures = signatures;
        }

        public override bool IsValid(ValidationContext<T> context, IFormFile image)
        {
            if (image is null)
                return false;

            var reader = new BinaryReader(image.OpenReadStream());
            var headerBytes = reader.ReadBytes((int)image.Length);

            var res = signatures.Any(signature =>
                headerBytes.Take(signature.Length).SequenceEqual(signature));

            return res;
        }

        public override string Name => "AllowedExtensionsValidator";
    }
}
