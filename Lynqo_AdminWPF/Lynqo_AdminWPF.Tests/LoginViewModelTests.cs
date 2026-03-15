using Xunit;
using FluentAssertions;
using Moq;
using Lynqo_AdminWPF.ViewModels;
using Lynqo_AdminWPF.Services;

namespace Lynqo_AdminWPF.Tests
{
    public class LoginViewModelTests
    {
        [Fact]
        public void UsernameProperty_WhenChanged_ShouldRaisePropertyChangedEvent()
        {
            // Arrange
            // Készítünk egy "kamu" API klienst a teszthez. Kell neki egy kamu URL a konstruktorba.
            var mockApi = new Mock<ApiClient>("http://teszt.com");
            var viewModel = new LoginViewModel(mockApi.Object);

            // A FluentAssertions csodafegyvere: Figyeljük, hogy a ViewModel küld-e jelet a felületnek!
            using var monitor = viewModel.Monitor();

            // Act
            viewModel.Username = "admin";

            // Assert
            // Ellenőrizzük, hogy a ViewModel szólt-e a WPF ablaknak, hogy megváltozott a Username
            monitor.Should().RaisePropertyChangeFor(vm => vm.Username);
        }

        [Fact]
        public void LoginCommand_WhenIsBusyIsTrue_ShouldBeDisabled()
        {
            // Arrange
            var mockApi = new Mock<ApiClient>("http://teszt.com");
            var viewModel = new LoginViewModel(mockApi.Object);

            // Act: Beállítjuk, hogy épp "tölt" a rendszer (pl. pörög a karika)
            viewModel.IsBusy = true;

            // Megkérdezzük a gombot, hogy ilyenkor meg lehet-e nyomni
            bool canExecute = viewModel.LoginCommand.CanExecute(null);

            // Assert
            canExecute.Should().BeFalse("mert ha a rendszer dolgozik (IsBusy), a Login gombnak inaktívnak kell lennie");
        }

        [Fact]
        public void LoginCommand_WhenIsBusyIsFalse_ShouldBeEnabled()
        {
            // Arrange
            var mockApi = new Mock<ApiClient>("http://teszt.com");
            var viewModel = new LoginViewModel(mockApi.Object);

            // Act: A rendszer nem dolgozik
            viewModel.IsBusy = false;

            bool canExecute = viewModel.LoginCommand.CanExecute(null);

            // Assert
            canExecute.Should().BeTrue("mert ha a rendszer nem dolgozik, a Login gombot meg lehet nyomni");
        }
    }
}