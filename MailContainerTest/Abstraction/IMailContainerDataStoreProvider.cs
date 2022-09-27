namespace MailContainerTest.Abstraction;

public interface IMailContainerDataStoreProvider
{
    bool MatchesType(string containerType);
    IMailContainerDataStore GetMailContainerDataStore();
}
