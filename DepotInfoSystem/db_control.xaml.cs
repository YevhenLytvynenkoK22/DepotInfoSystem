using System;
using System.Collections.Generic;
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
    /// Логика взаимодействия для db_control.xaml
    /// </summary>
    public partial class db_control : Page
    {
        MainWindow mv;
        ApplicationContext db = new ApplicationContext();
        string[] permissionStrings;
        public db_control(MainWindow mainWindow)
        {
            InitializeComponent();
            mv = mainWindow;
            list.Items.Refresh();
            permissionStrings = db.Roles
                   .Where(r => r.id == mv.user.RoleID)
                   .Select(r => r.Permissions)
                   .FirstOrDefault()?.Split(',') ?? Array.Empty<string>();
        }
        public void RefreshBTNs()
        {
            if (HasReadPermission(mv.currentTable))
            {
                btn_add.IsEnabled = false;
                btn_edit.IsEnabled = false;
                btn_delete.IsEnabled = false;
            }
            else
            {
                btn_add.IsEnabled = true;
                btn_edit.IsEnabled = true;
                btn_delete.IsEnabled = true;
            }
        }
        bool HasReadPermission(string permission)
        {
            foreach (string permissionString in permissionStrings)
            {
                var parts = permissionString.Split(':');
                if (parts.Length == 2 && parts[0].Equals(permission, StringComparison.OrdinalIgnoreCase) && parts[1].Equals("Read", StringComparison.OrdinalIgnoreCase))
                {
                    btn_add.IsEnabled = false;
                    return true;
                }

            }
            return false;
        }
        private void btn_add_Click(object sender, RoutedEventArgs e)
        {

            switch (mv.currentTable)
            {
                case "Trains":
                    TrainWindow trainWin = new TrainWindow(new Train());

                    if (trainWin.ShowDialog() == true)
                    {
                        mv.db.Trains.Add(trainWin._train);
                        mv.db.SaveChanges();
                    }
                    break;
                case "Employee":
                    EmployeeWindow employeeWin = new EmployeeWindow(new Employee());
                    if (employeeWin.ShowDialog() == true)
                    {
                        mv.db.Employees.Add(employeeWin._employee);
                        mv.db.SaveChanges();
                    }
                    break;
                case "Warehouse":
                    WarehouseWindow warehouseWin = new WarehouseWindow(new Warehouse());
                    if (warehouseWin.ShowDialog() == true)
                    {
                        mv.db.Warehouses.Add(warehouseWin._warehouse);
                        mv.db.SaveChanges();
                    }
                    break;
                case "Roles":
                    RoleWindow roleWin = new RoleWindow(new Role());
                    if (roleWin.ShowDialog() == true)
                    {
                        mv.db.Roles.Add(roleWin._role);
                        mv.db.SaveChanges();
                    }
                    break;
                case "Users":
                    UserWindow userWin = new UserWindow(new User());
                    if (userWin.ShowDialog() == true)
                    {
                        mv.db.Users.Add(userWin._user);
                        mv.db.SaveChanges();
                    }
                    break;
                case "Repairs":
                    RepairWindow repairWin = new RepairWindow(new Repair());
                    if (repairWin.ShowDialog() == true)
                    {
                        mv.db.Repairs.Add(repairWin._repair);
                        mv.db.SaveChanges();
                    }
                    break;

            }
        }
        private void btn_edit_Click(object sender, RoutedEventArgs e)
        {
            switch (mv.currentTable)
            {
                case "Trains":
                    if (list.SelectedItem != null)
                    {
                        Train train = list.SelectedItem as Train;
                        TrainWindow win = new TrainWindow(train);
                        if (win.ShowDialog() == true)
                        {
                            mv.db.SaveChanges();
                        }
                    }
                    else
                    {
                        MessageBox.Show("Оберіть потяг для редагування", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    break;
                case "Employee":
                    if (list.SelectedItem != null)
                    {
                        Employee employee = list.SelectedItem as Employee;
                        EmployeeWindow win = new EmployeeWindow(employee);
                        if (win.ShowDialog() == true)
                        {
                            mv.db.SaveChanges();
                        }
                    }
                    else
                    {
                        MessageBox.Show("Оберіть працівника для редагування", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    break;
                case "Warehouse":
                    if (list.SelectedItem != null)
                    {
                        Warehouse warehouse = list.SelectedItem as Warehouse;
                        WarehouseWindow win = new WarehouseWindow(warehouse);
                        if (win.ShowDialog() == true)
                        {
                            mv.db.SaveChanges();
                        }
                    }
                    else
                    {
                        MessageBox.Show("Оберіть запчастини для редагування", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    break;
                case "Roles":
                    if (list.SelectedItem != null)
                    {
                        Role role = list.SelectedItem as Role;
                        RoleWindow win = new RoleWindow(role);
                        if (win.ShowDialog() == true)
                        {
                            mv.db.SaveChanges();
                        }
                    }
                    else
                    {
                        MessageBox.Show("Оберіть роль для редагування", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    break;
                case "Users":
                    if (list.SelectedItem != null)
                    {
                        User user = list.SelectedItem as User;
                        UserWindow win = new UserWindow(user);
                        if (win.ShowDialog() == true)
                        {
                            mv.db.SaveChanges();
                        }
                    }
                    else
                    {
                        MessageBox.Show("Оберіть користувача для редагування", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    break;
                case "Repairs":
                    if (list.SelectedItem != null)
                    {
                        Repair repair = list.SelectedItem as Repair;
                        RepairWindow win = new RepairWindow(repair);
                        if (win.ShowDialog() == true)
                        {
                            mv.db.SaveChanges();
                        }
                    }
                    else
                    {
                        MessageBox.Show("Оберіть ремонт для редагування", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    break;
            }

        }
        private void btn_delete_Click(object sender, RoutedEventArgs e)
        {
            switch (mv.currentTable)
            {
                case "Employee":
                    if (list.SelectedItem != null)
                    {
                        Employee employee = list.SelectedItem as Employee;
                        if (MessageBox.Show($"Ви впевнені, що хочете видалити {employee.FullName}?", "Підтвердження", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                        {

                            if (!string.IsNullOrEmpty(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, employee.Photo)) && !employee.Photo.Contains("pack://application:,,,/Assets/profile_image.jpg"))
                            {
                                try
                                {
                                    string fullPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, employee.Photo);
                                    if (File.Exists(fullPath))
                                    {
                                        File.Delete(fullPath);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show($"Не вдалося видалити фото: {ex.Message}",
                                                  "Помилка",
                                                  MessageBoxButton.OK,
                                                  MessageBoxImage.Warning);
                                }
                            }
                            mv.db.Employees.Remove(employee);
                            mv.db.SaveChanges();
                        }
                    }
                    else
                    {
                        MessageBox.Show("Оберіть працівника для видалення", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    break;
                case "Trains":
                    if (list.SelectedItem != null)
                    {
                        Train train = list.SelectedItem as Train;
                        if (MessageBox.Show($"Ви впевнені, що хочете видалити потяг {train.Number}?", "Підтвердження", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                        {
                            if (!string.IsNullOrEmpty(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, train.Photo)) && !train.Photo.Contains("pack://application:,,,/Assets/train_image.jpg"))
                            {
                                try
                                {
                                    string fullPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, train.Photo);
                                    if (File.Exists(fullPath))
                                    {
                                        File.Delete(fullPath);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show($"Не вдалося видалити фото: {ex.Message}",
                                                  "Помилка",
                                                  MessageBoxButton.OK,
                                                  MessageBoxImage.Warning);
                                }
                            }
                            mv.db.Trains.Remove(train);
                            mv.db.SaveChanges();
                        }
                    }
                    else
                    {
                        MessageBox.Show("Оберіть потяг для видалення", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    break;
                case "Warehouse":
                    if (list.SelectedItem != null)
                    {
                        Warehouse warehouse = list.SelectedItem as Warehouse;
                        if (MessageBox.Show($"Ви впевнені, що хочете видалити запчастину {warehouse.Name}?", "Підтвердження", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                        {
                            mv.db.Warehouses.Remove(warehouse);
                            mv.db.SaveChanges();
                        }
                    }
                    else
                    {
                        MessageBox.Show("Оберіть запчастину для видалення", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    break;
                case "Roles":
                    if (list.SelectedItem != null)
                    {
                        Role role = list.SelectedItem as Role;
                        if (MessageBox.Show($"Ви впевнені, що хочете видалити роль {role.Name}?", "Підтвердження", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                        {
                            mv.db.Roles.Remove(role);
                            mv.db.SaveChanges();
                        }
                    }
                    else
                    {
                        MessageBox.Show("Оберіть роль для видалення", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    break;
                case "Users":
                    if (list.SelectedItem != null)
                    {
                        User user = list.SelectedItem as User;
                        if (MessageBox.Show($"Ви впевнені, що хочете видалити користувача {user.Login}?", "Підтвердження", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                        {
                            mv.db.Users.Remove(user);
                            mv.db.SaveChanges();
                        }

                    }
                    else
                    {
                        MessageBox.Show("Оберіть користувача для видалення", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    break;
                case "Repairs":
                    if (list.SelectedItem != null)
                    {
                        Repair repair = list.SelectedItem as Repair;
                        if (MessageBox.Show($"Ви впевнені, що хочете видалити ремонт {repair.TrainID}?", "Підтвердження", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                        {
                            mv.db.Repairs.Remove(repair);
                            mv.db.SaveChanges();
                        }
                    }
                    else
                    {
                        MessageBox.Show("Оберіть ремонт для видалення", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    break;
            }
        }
    }
}
