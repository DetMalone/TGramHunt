using FluentValidation;
using FluentValidation.Validators;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace TGramHunt.Validators.CustomValidators
{
    public class ListItemRegularExpressionValidator<T, TCollectionElement> : PropertyValidator<T, IList<TCollectionElement>>
    {
        private readonly string pattern;

        public ListItemRegularExpressionValidator(string pattern)
        {
            this.pattern = pattern;
        }

        public override bool IsValid(ValidationContext<T> context, IList<TCollectionElement> value)
        {
            if (value is null)
                return true;

            if (value is IList<string> list && list.Any(item => string.IsNullOrWhiteSpace(item) || !Regex.IsMatch(item, pattern)))
                return false;

            return true;
        }

        public override string Name => "ListItemRegExpValidator";
    }
}
