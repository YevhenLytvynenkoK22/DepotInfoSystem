using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using DepotInfoSystem.Classes;
using DepotInfoSystem.Froms;
namespace DepotInfoSystem
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public ApplicationContext db = new ApplicationContext();
        public string currentTable = "Employee";
        public User user;
        public MainWindow(User user)
        {
            InitializeComponent();
            this.user = user;
            string imagesFolder = (AppDomain.CurrentDomain.BaseDirectory + "/Images");
            string employeePhoto = imagesFolder + "/Employee";
            string trainsPhoto = imagesFolder + "/Trains";
            if (!Directory.Exists(imagesFolder))
            {
                Directory.CreateDirectory(imagesFolder);
            }
            if (!Directory.Exists(employeePhoto))
            {
                Directory.CreateDirectory(employeePhoto);
            }
            if (!Directory.Exists(trainsPhoto))
            {
                Directory.CreateDirectory(trainsPhoto);
            }
            MainFrame.Navigate(new db_control(this));
            if (user.RoleID != 1)
            {
                foreach (Button btn in sp_nav.Children)
                {
                    btn.Background = Brushes.Transparent;
                    btn.Foreground = Brushes.Black;
                    btn.Visibility = Visibility.Collapsed;
                }
                string[] permissionStrings = db.Roles
                    .Where(r => r.id == user.RoleID)
                    .Select(r => r.Permissions)
                    .FirstOrDefault()?.Split(',') ?? Array.Empty<string>();
                foreach (var permissionString in permissionStrings)
                {
                    var parts = permissionString.Split(':');
                    if (parts.Length == 2)
                    {
                        var resource = parts[0];  
                        var access = parts[1];    
                        if (!access.Equals("Deny", StringComparison.OrdinalIgnoreCase))
                        {
                           sp_nav.Children
                                .OfType<Button>()
                                .Where(b => b.Tag.ToString().Equals(resource, StringComparison.OrdinalIgnoreCase))
                                .ToList()
                                .ForEach(b => 
                                {
                                    b.Visibility = Visibility.Visible;
                                });
                        }
                    }
                }

            }
        }
        private void Button_Table_Type_Click(object sender, RoutedEventArgs e)
        {
            foreach (Button btn in sp_nav.Children)
            {
                btn.Background = Brushes.Transparent;
                btn.Foreground = Brushes.Black;
            }
            currentTable = (sender as Button).Tag.ToString();
            (sender as Button).Background = Brushes.LightGray;
            var dbControl = MainFrame.Content as db_control;
            dbControl.RefreshBTNs();
            switch (currentTable)
            {
                case "Trains":
                    db.Trains.Load();
                    dbControl.list.ItemsSource = db.Trains.Local.ToBindingList();
                    dbControl.list.ItemTemplate = (DataTemplate)FindResource("TrainTemplate");
                    break;
                case "Employee":
                    db.Employees.Load();
                    dbControl.list.ItemsSource = db.Employees.Local.ToBindingList();
                    dbControl.list.ItemTemplate = (DataTemplate)FindResource("EmployeeTemplate");
                    break;
                case "Warehouse":
                    db.Warehouses.Load(); 
                    dbControl.list.ItemsSource = db.Warehouses.Local.ToBindingList();
                    dbControl.list.ItemTemplate = (DataTemplate)FindResource("WarehouseTemplate");
                    break;
                case "Roles":
                    db.Roles.Load();
                    dbControl.list.ItemsSource = db.Roles.Local.ToBindingList();
                    dbControl.list.ItemTemplate = (DataTemplate)FindResource("RoleTemplate");
                    break;
                case "Users":
                    db.Users.Load();
                    dbControl.list.ItemsSource = db.Users.Local.ToBindingList();
                    dbControl.list.ItemTemplate = (DataTemplate)FindResource("UserTemplate");
                    break;
                case "Repairs":
                    db.Repairs.Load();
                    dbControl.list.ItemsSource = db.Repairs.Local.ToBindingList();
                    dbControl.list.ItemTemplate = (DataTemplate)FindResource("RepairTemplate");
                    break;
            }
        }
    
    }
}
