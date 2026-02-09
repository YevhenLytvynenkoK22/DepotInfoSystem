using System.Windows;
using DepotInfoSystem.Classes;
using DepotInfoSystem.ViewModels;

namespace DepotInfoSystem.Froms
{
    public partial class UserWindow : Window
    {
        public UserWindow(User user)
        {
            InitializeComponent();
            var viewModel = new UserViewModel(user);
            this.DataContext = viewModel;

            viewModel.RequestClose += (s, e) =>
            {
                this.DialogResult = viewModel.DialogResult;
                this.Close();
            };
        }
    }
}