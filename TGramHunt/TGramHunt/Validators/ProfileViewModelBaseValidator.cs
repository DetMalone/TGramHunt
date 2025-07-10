using FluentValidation;
using TGramHunt.Contract.ViewModels.EditProfile;

namespace TGramHunt.Validators
{
    public class ProfileViewModelBaseValidator : AbstractValidator<ProfileViewModelBase>
    {
        public ProfileViewModelBaseValidator()
        {
            RuleFor(profile => profile.Name)
                .Cascade(CascadeMode.Stop)
                .NotNull()
                .WithMessage("Enter Name.")
                .NotEmpty()
                .WithMessage("Enter Name.")
                .EditProfileNameOneWordLength()
                .EditProfileNameTwoWordLengthFirstPart()
                .EditProfileNameTwoWordLengthSecondPart()
                .CustomNameValidation();
        }
    }
}