using MailContainerTest.Abstraction;
using MailContainerTest.Types;
using Microsoft.Extensions.Logging;

namespace MailContainerTest.Services
{
    public partial class MailTransferService : IMailTransferService
    {
        private readonly IConfigurationService _configuration;
        private readonly ILogger<MailTransferService> _logger;
        private readonly IEnumerable<IMailContainerDataStoreProvider> _mailContainerProviders;
        private readonly IEnumerable<IMailContainerValidator> _mailContainerValidators;

        public MailTransferService(IConfigurationService configuration, ILogger<MailTransferService> logger, IEnumerable<IMailContainerDataStoreProvider> mailContainerProviders,
            IEnumerable<IMailContainerValidator> mailContainerValidators)
        {
            _configuration = configuration;
            _logger = logger;
            _mailContainerProviders = mailContainerProviders;
            _mailContainerValidators = mailContainerValidators;
        }

        public MakeMailTransferResult MakeMailTransfer(MakeMailTransferRequest request)
        {
            string dataStoreType = _configuration.GetDataStoreType();

            var mailContainerProviders = _mailContainerProviders.SingleOrDefault(s => s.MatchesType(dataStoreType));
            if (mailContainerProviders == null)
            {
               return GetFailedResult($"Could not find a mailContainerProviders to match type: {dataStoreType}");
            }

            IMailContainerDataStore containerDataStore = mailContainerProviders.GetMailContainerDataStore();
            var sourceMailContainer = containerDataStore.GetMailContainer(request.SourceMailContainerNumber);
            var destinationMailContainer = containerDataStore.GetMailContainer(request.DestinationMailContainerNumber);

            var mailContainerValidator = _mailContainerValidators.SingleOrDefault(v => v.CanValidate(request.MailType));
            if (mailContainerValidator == null)
            {
                return GetFailedResult($"Could not find a mailContainerValidator to match type: {request.MailType}");
            }

            var sourceContainerValidationResult = mailContainerValidator.IsValid(sourceMailContainer, request.NumberOfMailItems);
            var destinationContainerValidationResult = mailContainerValidator.IsValid(destinationMailContainer, request.NumberOfMailItems);

            if(!sourceContainerValidationResult.Success)
            {
                return sourceContainerValidationResult;
            }
            if (!destinationContainerValidationResult.Success)
            {
                return destinationContainerValidationResult;
            }

            sourceMailContainer.Capacity -= request.NumberOfMailItems;
            containerDataStore.UpdateMailContainer(sourceMailContainer);

            destinationMailContainer.Capacity += request.NumberOfMailItems;
            containerDataStore.UpdateMailContainer(destinationMailContainer);

            return new MakeMailTransferResult
            {
                Success = true
            };
        }

        private MakeMailTransferResult GetFailedResult(string message)
        {
            _logger.LogCritical(message);

            return new MakeMailTransferResult
            {
                Success = false,
                ErrorMessage = message
            };
        }
    }
}
