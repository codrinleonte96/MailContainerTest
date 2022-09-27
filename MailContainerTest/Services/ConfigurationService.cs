using MailContainerTest.Abstraction;
using System.Configuration;

namespace MailContainerTest.Services
{
    public class ConfigurationService : IConfigurationService
    {
        public string GetDataStoreType() => ConfigurationManager.AppSettings["DataStoreType"];
    }
}
