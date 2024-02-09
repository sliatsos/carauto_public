using CarAuto.Protos.Customer;
using FluentValidation;

namespace CarAuto.UI.Validators
{
    public class EmailValidator : AbstractValidator<string>
    {
        public EmailValidator()
        {
            RuleFor(e => e)
                .EmailAddress();
        }

        private IEnumerable<string> ValidateValue(string arg)
        {
            var result = Validate(arg);
            if (result.IsValid)
            {
                return Array.Empty<string>();
            }
            return result.Errors.Select(e => e.ErrorMessage);
        }

        public Func<string, IEnumerable<string>> Validation => ValidateValue;
    }
}
