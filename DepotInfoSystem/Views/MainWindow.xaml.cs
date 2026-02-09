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

            dbControl.dataGrid.Columns.Clear();

            dbControl.LoadDataForTable(currentTable);

            dbControl.RefreshBTNs();

            switch (currentTable)
            {
                case "Trains":
                    dbControl.list.ItemTemplate = (DataTemplate)FindResource("TrainTemplate");
                    dbControl.dataGrid.Columns.Add(new DataGridTextColumn { Header = "ID", Binding = new Binding("id") });
                    dbControl.dataGrid.Columns.Add(new DataGridTextColumn { Header = "Model", Binding = new Binding("Model") });
                    dbControl.dataGrid.Columns.Add(new DataGridTextColumn { Header = "Number", Binding = new Binding("Number") });
                    dbControl.dataGrid.Columns.Add(new DataGridTextColumn { Header = "Manufacture Year", Binding = new Binding("ManufactureYear") });
                    dbControl.dataGrid.Columns.Add(new DataGridTextColumn { Header = "Mileage", Binding = new Binding("Mileage") });
                    dbControl.dataGrid.Columns.Add(new DataGridTextColumn { Header = "Status", Binding = new Binding("CurrentStatus") });
                    break;

                case "Employee":
                    dbControl.list.ItemTemplate = (DataTemplate)FindResource("EmployeeTemplate");
                    dbControl.dataGrid.Columns.Add(new DataGridTextColumn { Header = "ID", Binding = new Binding("id") });
                    dbControl.dataGrid.Columns.Add(new DataGridTextColumn { Header = "Surname", Binding = new Binding("Surname") });
                    dbControl.dataGrid.Columns.Add(new DataGridTextColumn { Header = "Name", Binding = new Binding("Name") });
                    dbControl.dataGrid.Columns.Add(new DataGridTextColumn { Header = "Patronymic", Binding = new Binding("Patronymic") });
                    dbControl.dataGrid.Columns.Add(new DataGridTextColumn { Header = "Speciality", Binding = new Binding("Speciality") });
                    break;

                case "Warehouse":
                    dbControl.list.ItemTemplate = (DataTemplate)FindResource("WarehouseTemplate");
                    dbControl.dataGrid.Columns.Add(new DataGridTextColumn { Header = "ID", Binding = new Binding("id") });
                    dbControl.dataGrid.Columns.Add(new DataGridTextColumn { Header = "Name", Binding = new Binding("Name") });
                    dbControl.dataGrid.Columns.Add(new DataGridTextColumn { Header = "Quantity", Binding = new Binding("Count") });
                    dbControl.dataGrid.Columns.Add(new DataGridTextColumn { Header = "Price", Binding = new Binding("Price") });
                    dbControl.dataGrid.Columns.Add(new DataGridTextColumn { Header = "Number", Binding = new Binding("Number") });
                    break;

                case "Roles":
                    dbControl.list.ItemTemplate = (DataTemplate)FindResource("RoleTemplate");
                    dbControl.dataGrid.Columns.Add(new DataGridTextColumn { Header = "ID", Binding = new Binding("id") });
                    dbControl.dataGrid.Columns.Add(new DataGridTextColumn { Header = "Role Name", Binding = new Binding("Name") });
                    dbControl.dataGrid.Columns.Add(new DataGridTextColumn { Header = "Description", Binding = new Binding("Description") });
                    dbControl.dataGrid.Columns.Add(new DataGridTextColumn { Header = "Permissions", Binding = new Binding("Permissions") });
                    break;

                case "Users":
                    dbControl.list.ItemTemplate = (DataTemplate)FindResource("UserTemplate");
                    dbControl.dataGrid.Columns.Add(new DataGridTextColumn { Header = "ID", Binding = new Binding("id") });
                    dbControl.dataGrid.Columns.Add(new DataGridTextColumn { Header = "Login", Binding = new Binding("Login") });
                    dbControl.dataGrid.Columns.Add(new DataGridTextColumn { Header = "Password", Binding = new Binding("Password") });
                    dbControl.dataGrid.Columns.Add(new DataGridTextColumn { Header = "Role", Binding = new Binding("Role.Name") });
                    break;

                case "Repairs":
                    dbControl.list.ItemTemplate = (DataTemplate)FindResource("RepairTemplate");
                    dbControl.dataGrid.Columns.Add(new DataGridTextColumn { Header = "ID", Binding = new Binding("id") });
                    dbControl.dataGrid.Columns.Add(new DataGridTextColumn { Header = "Train", Binding = new Binding("Train.Number") });
                    dbControl.dataGrid.Columns.Add(new DataGridTextColumn { Header = "Date", Binding = new Binding("Date") });
                    dbControl.dataGrid.Columns.Add(new DataGridTextColumn { Header = "Repair Type", Binding = new Binding("Type.Name") });
                    dbControl.dataGrid.Columns.Add(new DataGridTextColumn { Header = "Brigade", Binding = new Binding("Brigada.Employee.FullName") });
                    dbControl.dataGrid.Columns.Add(new DataGridTextColumn { Header = "Description", Binding = new Binding("Description") });
                    dbControl.dataGrid.Columns.Add(new DataGridTextColumn { Header = "User", Binding = new Binding("User.Login") });
                    dbControl.dataGrid.Columns.Add(new DataGridTextColumn { Header = "Status", Binding = new Binding("Status") });
                    break;

                case "Brigades":
                    dbControl.list.ItemTemplate = (DataTemplate)FindResource("BrigadeTemplate");
                    dbControl.dataGrid.Columns.Add(new DataGridTextColumn { Header = "ID", Binding = new Binding("id") });
                    dbControl.dataGrid.Columns.Add(new DataGridTextColumn { Header = "Employee ID", Binding = new Binding("EmployeeID") });
                    dbControl.dataGrid.Columns.Add(new DataGridTextColumn { Header = "Employee Name", Binding = new Binding("Employee.FullName") });
                    dbControl.dataGrid.Columns.Add(new DataGridTextColumn { Header = "Repair ID", Binding = new Binding("RepairID") });
                    dbControl.dataGrid.Columns.Add(new DataGridTextColumn { Header = "Repair Description", Binding = new Binding("Repair.Description") });
                    break;
                case "Spares":
                    dbControl.list.ItemTemplate = (DataTemplate)FindResource("SpareTemplate");
                    dbControl.dataGrid.Columns.Add(new DataGridTextColumn { Header = "ID", Binding = new Binding("id") });
                    dbControl.dataGrid.Columns.Add(new DataGridTextColumn { Header = "Part ID", Binding = new Binding("SpareID") });
                    dbControl.dataGrid.Columns.Add(new DataGridTextColumn { Header = "Repair ID", Binding = new Binding("RemontID") });
                    dbControl.dataGrid.Columns.Add(new DataGridTextColumn { Header = "Quantity", Binding = new Binding("Count") });
                    break;
            }
        }
    }
}