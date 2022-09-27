using MailContainerTest.Abstraction;
using MailContainerTest.Types;

namespace MailContainerTest.Validation
{
    public class SmallParcelContainerValidator : IMailContainerValidator
    {
        public bool CanValidate(MailType type) => type == MailType.SmallParcel;

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
            else if (!container.AllowedMailType.HasFlag(AllowedMailType.SmallParcel))
            {
                return new MakeMailTransferResult
                {
                    Success = false,
                    ErrorMessage = $"Container does not allow mail type {AllowedMailType.SmallParcel}"
                };

            }
            else if (container.Status != MailContainerStatus.Operational)
            {
                return new MakeMailTransferResult
                {
                    Success = false,
                    ErrorMessage = $"Container status is {container.Status} and not {MailContainerStatus.Operational}"
                };
            }

            return new MakeMailTransferResult
            {
                Success = true
            };
        }
    }
}