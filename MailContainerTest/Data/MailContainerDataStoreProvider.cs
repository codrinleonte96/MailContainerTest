using MailContainerTest.Abstraction;

namespace MailContainerTest.Data
{
    public class MailContainerDataStoreProvider : IMailContainerDataStoreProvider
    {
        public bool MatchesType(string containerType) => containerType != "Backup";

        public IMailContainerDataStore GetMailContainerDataStore() => new MailContainerDataStore();
    }
}
