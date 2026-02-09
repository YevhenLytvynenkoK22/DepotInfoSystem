using System.Windows;
using DepotInfoSystem.ViewModels;
using DepotInfoSystem.Classes;

namespace DepotInfoSystem.Views
{
    public partial class EmployeeWindow : Window
    {
        public EmployeeWindow(Employee employee)
        {
            InitializeComponent();
            DataContext = new EmployeeViewModel(employee, this);
        }
    }
}