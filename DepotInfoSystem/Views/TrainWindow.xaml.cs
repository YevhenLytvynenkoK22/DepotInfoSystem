using System.Windows;
using DepotInfoSystem.Classes;
using DepotInfoSystem.ViewModels;

namespace DepotInfoSystem.Froms
{
    public partial class TrainWindow : Window
    {
        public TrainWindow(Train train)
        {
            InitializeComponent();
            var viewModel = new TrainViewModel(train);
            this.DataContext = viewModel;

            viewModel.RequestClose += (s, e) =>
            {
                this.DialogResult = viewModel.DialogResult;
                this.Close();
            };
        }
    }
}