using System.Windows;
using DepotInfoSystem.Classes;
using DepotInfoSystem.ViewModels;

namespace DepotInfoSystem.Froms
{
    public partial class WarehouseWindow : Window
    {
        public WarehouseWindow(Warehouse warehouse)
        {
            InitializeComponent();
            var viewModel = new WarehouseViewModel(warehouse);
            this.DataContext = viewModel;
            viewModel.RequestClose += (s, e) =>
            {
                this.DialogResult = viewModel.DialogResult;
                this.Close();
            };
        }
    }
}