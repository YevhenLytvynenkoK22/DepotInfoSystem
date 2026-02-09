using System.Windows;
using DepotInfoSystem.Classes;
using DepotInfoSystem.ViewModels;

namespace DepotInfoSystem.Froms
{
    public partial class RoleWindow : Window
    {
        public RoleWindow(Role role)
        {
            InitializeComponent();
            var viewModel = new RoleViewModel(role);
            this.DataContext = viewModel;

            viewModel.RequestClose += (s, e) =>
            {
                this.DialogResult = viewModel.DialogResult;
                this.Close();
            };
        }
    }
}