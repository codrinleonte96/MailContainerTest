using MailContainerTest.Types;

namespace MailContainerTest.Abstraction
{
    public interface IMailContainerValidator
    {
        public bool CanValidate(MailType type);
        public MakeMailTransferResult IsValid(MailContainer container, int numberOfMailItems);
    }
}