using System.Windows;
using DepotInfoSystem.ViewModels; // Додайте цей using

namespace DepotInfoSystem.Views
{
    /// <summary>
    /// Логіка взаємодії для LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
            DataContext = new LoginViewModel(this);
        }
    }
}