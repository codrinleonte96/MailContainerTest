using MailContainerTest.Abstraction;
using MailContainerTest.Types;

namespace MailContainerTest.Validation
{
    public class StandardLetterContainerValidator : IMailContainerValidator
    {
        public bool CanValidate(MailType type) => type == MailType.StandardLetter;

        public MakeMailTransferResult IsValid(MailContainer container, int numberOfMailItems)
        {
            if (container == null)
            {
                return new MakeMailTransferResult
                {
                    Success = false,
                    ErrorMessage = "Container is null"
                };
            }
            else if (!container.AllowedMailType.HasFlag(AllowedMailType.StandardLetter))
            {
                return new MakeMailTransferResult
                {
                    Success = false,
                    ErrorMessage = $"Container does not allow mail type {AllowedMailType.StandardLetter}"
                };
            }

            return new MakeMailTransferResult
            {
                Success = true
            };
        }
    }
}