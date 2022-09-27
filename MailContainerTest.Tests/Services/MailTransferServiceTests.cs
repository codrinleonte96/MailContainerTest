using FluentAssertions;
using MailContainerTest.Abstraction;
using MailContainerTest.Services;
using MailContainerTest.Types;
using MailContainerTest.Validation;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace MailContainerTest.Tests.Services
{
    public class MailTransferServiceTests
    {
        private readonly Mock<IConfigurationService> _configurationMock;
        private readonly Mock<ILogger<MailTransferService>> _loggerMock;
        private readonly Mock<IMailContainerDataStoreProvider> _mailContainerDataStoreProviderMock;

        private readonly IEnumerable<IMailContainerDataStoreProvider> _mailContainerDataStoreProviders;
        private readonly IEnumerable<IMailContainerValidator> _mailContainerValidators;

        private readonly MailTransferService _sut;
        public MailTransferServiceTests()
        {
            _configurationMock = new Mock<IConfigurationService>();
            _loggerMock = new Mock<ILogger<MailTransferService>>();

            _mailContainerDataStoreProviderMock = new Mock<IMailContainerDataStoreProvider>();
            _mailContainerDataStoreProviders = new List<IMailContainerDataStoreProvider>
            {
               _mailContainerDataStoreProviderMock.Object
            };
            _mailContainerValidators = new List<IMailContainerValidator> { new StandardLetterContainerValidator(),
                new SmallParcelContainerValidator(), new LargeLetterContainerValidator() };

            _sut = new MailTransferService(_configurationMock.Object, _loggerMock.Object, _mailContainerDataStoreProviders, _mailContainerValidators);
        }

        [Fact]
        public void MakeMailTransfer_When_ContainerIsBackup_AndMailType_IsStandardLetter_TransfersEmailSuccesfully()
        {
            //Arrange
            var request = new MakeMailTransferRequest
            {
                MailType = MailType.StandardLetter,
                SourceMailContainerNumber = "1",
                DestinationMailContainerNumber = "2",
                NumberOfMailItems = 3
            };
            _configurationMock.Setup(c => c.GetDataStoreType()).Returns("Backup");

            Mock<IMailContainerDataStore> dataStoreMock = new Mock<IMailContainerDataStore>();
            dataStoreMock.Setup(d => d.GetMailContainer(It.IsAny<string>()))
                .Returns(new MailContainer { AllowedMailType = AllowedMailType.StandardLetter });

            _mailContainerDataStoreProviderMock.Setup(p => p.MatchesType("Backup")).
                Returns(true);

            _mailContainerDataStoreProviderMock.Setup(p => p.GetMailContainerDataStore()).
                Returns(dataStoreMock.Object);

            //Act
            var result = _sut.MakeMailTransfer(request);

            //Assert
            result.Success.Should().BeTrue();
        }

        [Fact]
        public void MakeMailTransfer_When_ContainerIsNotBackup_AndMailType_IsLargeLetter_InvalidatesTransfer_DueToCapacity()
        {
            //Arrange
            var request = new MakeMailTransferRequest
            {
                MailType = MailType.LargeLetter,
                SourceMailContainerNumber = "1",
                DestinationMailContainerNumber = "2",
                NumberOfMailItems = 3
            };
            _configurationMock.Setup(c => c.GetDataStoreType()).Returns("NotBackup");

            Mock<IMailContainerDataStore> dataStoreMock = new Mock<IMailContainerDataStore>();
            dataStoreMock.Setup(d => d.GetMailContainer(It.IsAny<string>()))
                .Returns(new MailContainer { AllowedMailType = AllowedMailType.LargeLetter });

            _mailContainerDataStoreProviderMock.Setup(p => p.MatchesType("NotBackup")).
                Returns(true);

            _mailContainerDataStoreProviderMock.Setup(p => p.GetMailContainerDataStore()).
                Returns(dataStoreMock.Object);

            //Act
            var result = _sut.MakeMailTransfer(request);

            //Assert
            result.Success.Should().BeFalse();
            result.ErrorMessage.Should().Be($"Container capacity 0 is lower than numberOfMailItems 3");
        }

        [Fact]
        public void MakeMailTransfer_When_ContainerIsNotBackup_AndMailType_IsSmallParcel_InvalidatesTransfer_DueToDestinationContainerStatus()
        {
            //Arrange
            var request = new MakeMailTransferRequest
            {
                MailType = MailType.SmallParcel,
                SourceMailContainerNumber = "1",
                DestinationMailContainerNumber = "2",
                NumberOfMailItems = 3
            };
            _configurationMock.Setup(c => c.GetDataStoreType()).Returns("NotBackup");

            Mock<IMailContainerDataStore> dataStoreMock = new Mock<IMailContainerDataStore>();

            dataStoreMock.Setup(d => d.GetMailContainer(request.SourceMailContainerNumber))
              .Returns(new MailContainer { AllowedMailType = AllowedMailType.SmallParcel, Status = MailContainerStatus.Operational });
            dataStoreMock.Setup(d => d.GetMailContainer(request.DestinationMailContainerNumber))
                .Returns(new MailContainer { AllowedMailType = AllowedMailType.SmallParcel, Status = MailContainerStatus.OutOfService });

            _mailContainerDataStoreProviderMock.Setup(p => p.MatchesType("NotBackup")).
                Returns(true);

            _mailContainerDataStoreProviderMock.Setup(p => p.GetMailContainerDataStore()).
                Returns(dataStoreMock.Object);

            //Act
            var result = _sut.MakeMailTransfer(request);

            //Assert
            result.Success.Should().BeFalse();
            result.ErrorMessage.Should().Be($"Container status is OutOfService and not Operational");
        }
    }
}
