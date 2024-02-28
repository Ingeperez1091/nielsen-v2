using System.ComponentModel.DataAnnotations;

namespace NielsenChannelsReporting.Application.Models
{
    public class AwsMailParameters : IValidatableObject
    {
        public string? Sender { get; set; }

        public List<string>? ToAddresses { get; set; }

        public string? Subject { get; set; }

        public string? Body { get; set; }

        public bool IsHtml { get; set; }

        public List<string> CcAddresses { get; set; }

        public List<string> BccAdressess { get; set; }

        public AwsMailParameters()
        {
            IsHtml = false;
            CcAddresses = new List<string>();
            BccAdressess = new List<string>();
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (ToAddresses == null || ToAddresses.Count == 0)
            {
                yield return new ValidationResult("ToAddresses list must contain at least 1 element.", new[] { nameof(ToAddresses) });
            }
            if (string.IsNullOrEmpty(Sender) || string.IsNullOrWhiteSpace(Sender))
            {
                yield return new ValidationResult("Sender must be a valid email address.", new[] { nameof(Sender) });
            }
            if (string.IsNullOrEmpty(Subject) || string.IsNullOrWhiteSpace(Subject))
            {
                yield return new ValidationResult("Subject must not be null or empty.", new[] { nameof(Subject) });
            }
        }

        public void Validate()
        {
            ValidationContext context = new(this, serviceProvider: null, items: null);
            var validationResults = new List<ValidationResult>();

            bool isValid = Validator.TryValidateObject(this, context, validationResults, validateAllProperties: true);

            if (!isValid)
            {
                var details = string.Join("-", validationResults.Select(x => x.ErrorMessage).ToArray());
                throw new ValidationException($"Validation errors: {details}");
            }
        }
    }
}
