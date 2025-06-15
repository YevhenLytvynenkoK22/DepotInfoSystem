using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using DepotInfoSystem.Classes;

namespace DepotInfoSystem.Froms
{
    /// <summary>
    /// Логика взаимодействия для WarehouseWindow.xaml
    /// </summary>
    public partial class WarehouseWindow : Window
    {
        public Warehouse _warehouse { get; set; }

        public WarehouseWindow(Warehouse warehouse)
        {
            InitializeComponent();
            _warehouse = warehouse;
            this.DataContext = _warehouse;
        }

        private void btn_save_Click(object sender, RoutedEventArgs e)
        {
            
            this.DialogResult = true;
            this.Close();
        }

        private void btn_cancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}
