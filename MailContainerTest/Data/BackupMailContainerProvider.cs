using MailContainerTest.Abstraction;

namespace MailContainerTest.Data
{
    public class BackupMailContainerProvider : IMailContainerDataStoreProvider
    {
        public bool MatchesType(string containerType) => containerType == "Backup";

        public IMailContainerDataStore GetMailContainerDataStore() => new BackupMailContainerDataStore();
    }
}
