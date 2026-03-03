using Lynqo_AdminWPF.Services;
using Lynqo_AdminWPF.ViewModels;
using System.Windows;

namespace Lynqo_AdminWPF.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow(ApiClient apiClient)
        {
            InitializeComponent();
            DataContext = new MainViewModel(apiClient);
        }
    }
}
