using MailContainerTest.Abstraction;
using MailContainerTest.Types;

namespace MailContainerTest.Validation
{
    public class LargeLetterContainerValidator : IMailContainerValidator
    {
        public bool CanValidate(MailType type) => type == MailType.LargeLetter;

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
            else if (!container.AllowedMailType.HasFlag(AllowedMailType.LargeLetter))
            {
                return new MakeMailTransferResult
                {
                    Success = false,
                    ErrorMessage = $"Container does not allow mail type {AllowedMailType.LargeLetter}"
                };
            }
            else if (container.Capacity < numberOfMailItems)
            {
                return new MakeMailTransferResult
                {
                    Success = false,
                    ErrorMessage = $"Container capacity {container.Capacity} is lower than numberOfMailItems {numberOfMailItems}"
                };
            }

            return new MakeMailTransferResult
            {
                Success = true
            };
        }
    }
}