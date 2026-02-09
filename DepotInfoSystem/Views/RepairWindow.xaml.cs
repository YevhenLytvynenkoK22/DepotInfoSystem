using System.Windows;
using DepotInfoSystem.ViewModels;
using DepotInfoSystem.Classes;

namespace DepotInfoSystem.Froms
{
    public partial class RepairWindow : Window
    {
        public RepairWindow(Repair repair = null)
        {
            InitializeComponent();
            var viewModel = new RepairViewModel(repair);
            this.DataContext = viewModel;

            viewModel.RequestClose += (s, e) =>
            {
                this.DialogResult = viewModel.DialogResult;
                this.Close();
            };
        }
    }
}