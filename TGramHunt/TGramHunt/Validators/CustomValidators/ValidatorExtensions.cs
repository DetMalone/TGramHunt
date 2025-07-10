using FluentValidation;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TGramHunt.Validators.CustomValidators;

namespace TGramHunt.Validators
{
    public static class ValidatorExtensions
    {
        public static IRuleBuilderOptions<T, IList<TElement>> ListMaxCount<T, TElement>(this IRuleBuilder<T, IList<TElement>> ruleBuilder, int num)
            => ruleBuilder
            .Must(list => list is null || list.Count <= num)
            .WithMessage("List contains too many items");

        public static IRuleBuilderOptions<T, IList<TElement>> ListItemsMatches<T, TElement>(this IRuleBuilder<T, IList<TElement>> ruleBuilder, string pattern)
            => ruleBuilder
            .SetValidator(new ListItemRegularExpressionValidator<T, TElement>(pattern))
            .WithMessage("One or more elements do not match pattern");

        public static IRuleBuilderOptions<T, IList<TElement>> ListItemsMaxLength<T, TElement>(this IRuleBuilder<T, IList<TElement>> ruleBuilder, int length)
            => ruleBuilder
            .SetValidator(new ListItemMaxLengthValidator<T, TElement>(length))
            .WithMessage("One or more elements do not match maximum length");

        public static IRuleBuilderOptions<T, IFormFile> FileSmallerThan<T>(this IRuleBuilder<T, IFormFile> ruleBuilder, int max)
            => ruleBuilder
            .SetValidator(new FileSizeValidator<T>(max))
            .WithMessage("File size too large");

        public static IRuleBuilderOptions<T, IFormFile> AllowedExtensions<T>(this IRuleBuilder<T, IFormFile> ruleBuilder, IList<byte[]> signatures)
            => ruleBuilder
            .SetValidator(new AllowedExtensionsValidator<T>(signatures))
            .WithMessage("Unsupported file format");

        public static IRuleBuilderOptions<T, string> EditProfileNameOneWordLength<T>(this IRuleBuilder<T, string> ruleBuilder)
            => ruleBuilder.Must(name =>
            {
                if (string.IsNullOrWhiteSpace(name) ||
                    name.Contains(' '))
                {
                    return true;
                }

                var valLength = name.Trim().Length;
                return valLength >= 3 && valLength <= 255;
            })
            .WithMessage("One word of name should contain more than 2 and less than 256 symbols.");

        public static IRuleBuilderOptions<T, string> EditProfileNameTwoWordLengthFirstPart<T>(this IRuleBuilder<T, string> ruleBuilder)
            => ruleBuilder.Must(name =>
            {
                if (string.IsNullOrWhiteSpace(name) ||
                    !name.Contains(' '))
                {
                    return true;
                }

                name = name.Trim();
                var index = name.IndexOf(' ');
                if (index == -1)
                {
                    return true;
                }

                var firstPart = name[..index].Trim();
                var firstPartLength = firstPart.Length;
                return firstPartLength >= 3 && firstPartLength <= 123;
            })
            .WithMessage("First part of name should contain more than 2 and less than 124 symbols.");

        public static IRuleBuilderOptions<T, string> EditProfileNameTwoWordLengthSecondPart<T>(this IRuleBuilder<T, string> ruleBuilder)
            => ruleBuilder.Must(name =>
            {
                if (string.IsNullOrWhiteSpace(name) ||
                    !name.Contains(' '))
                {
                    return true;
                }

                name = name.Trim();
                var index = name.IndexOf(' ');
                if (index == -1)
                {
                    return true;
                }

                var firstPart = name[index..].Trim();
                var firstPartLength = firstPart.Length;
                return firstPartLength >= 3 && firstPartLength <= 123;
            })
            .WithMessage("Second part of name should contain more than 2 and less than 124 symbols.");

        public static IRuleBuilderOptions<T, string> CustomNameValidation<T>(this IRuleBuilder<T, string> ruleBuilder)
            => ruleBuilder.Must(name =>
            {
                if (string.IsNullOrWhiteSpace(name))
                {
                    return false;
                }

                return Regex.IsMatch(name.Trim(), @"(^[A-Za-z0-9]{3,255}$)|(^[A-Za-z0-9]{3,123} {1}[A-Za-z0-9]{3,123}$)");
            })
            .WithMessage("Name should contain only latin characters and numeric, it can be consist of two or one word.");
    }
}