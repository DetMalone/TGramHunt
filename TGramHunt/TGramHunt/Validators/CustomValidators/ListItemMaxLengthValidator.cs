using FluentValidation;
using FluentValidation.Validators;
using System.Collections.Generic;
using System.Linq;

namespace TGramHunt.Validators.CustomValidators
{
    public class ListItemMaxLengthValidator<T, TCollectionElement> : PropertyValidator<T, IList<TCollectionElement>>
    {
        private readonly int length;

        public ListItemMaxLengthValidator(int length)
        {
            this.length = length;
        }

        public override bool IsValid(ValidationContext<T> context, IList<TCollectionElement> value)
        {
            if (value is null)
                return true;

            if (value is IList<string> list && list.Any(item => string.IsNullOrWhiteSpace(item) || item.Length > length))
                return false;

            return true;
        }

        public override string Name => "ListItemMaxLengthValidator";
    }
}
