using DepotInfoSystem.Classes;
using DepotInfoSystem.Froms;
using DepotInfoSystem.ViewModels;
using DepotInfoSystem.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace DepotInfoSystem
{
    public partial class db_control : Page
    {
        MainWindow mv = null;
        ApplicationContext db = new ApplicationContext();
        string[] permissionStrings;
        bool isInitialized = false;

        public db_control(MainWindow mainWindow)
        {
            InitializeComponent();
            mv = mainWindow;
            list.Items.Refresh();
            permissionStrings = db.Roles
                   .Where(r => r.id == mv.user.RoleID)
                   .Select(r => r.Permissions)
                   .FirstOrDefault()?.Split(',') ?? Array.Empty<string>();
            list.Visibility = Visibility.Visible;
            isInitialized = true;

            LoadComboBoxData();
            var repairs = db.Repairs
                .Include(r => r.Train)
                .Include(r => r.Type)
                .Include(r => r.User)
                .ToList();
        }

        private void LoadComboBoxData()
        {
            var specialities = db.Employees.Select(e => e.Speciality).Distinct().ToList();
            filter_EmployeeSpeciality.Items.Clear();
            filter_EmployeeSpeciality.Items.Add(new ComboBoxItem { Content = "All" });
            foreach (var speciality in specialities)
            {
                if (!string.IsNullOrEmpty(speciality))
                    filter_EmployeeSpeciality.Items.Add(new ComboBoxItem { Content = speciality });
            }
            filter_EmployeeSpeciality.SelectedIndex = 0;

            var trainNumbers = db.Trains.Select(t => t.Number).Distinct().ToList();
            filter_RepairTrainNumber.Items.Clear();
            filter_RepairTrainNumber.Items.Add(new ComboBoxItem { Content = "All" });
            foreach (var number in trainNumbers)
            {
                if (!string.IsNullOrEmpty(number))
                    filter_RepairTrainNumber.Items.Add(new ComboBoxItem { Content = number });
            }
            filter_RepairTrainNumber.SelectedIndex = 0;

            var repairTypes = db.RepairTypes.Select(rt => rt.Name).Distinct().ToList();
            filter_RepairType.Items.Clear();
            filter_RepairType.Items.Add(new ComboBoxItem { Content = "All" });
            foreach (var typeName in repairTypes)
            {
                if (!string.IsNullOrEmpty(typeName))
                    filter_RepairType.Items.Add(new ComboBoxItem { Content = typeName });
            }
            filter_RepairType.SelectedIndex = 0;

            var userLogins = db.Users.Select(u => u.Login).Distinct().ToList();
            filter_RepairUser.Items.Clear();
            filter_RepairUser.Items.Add(new ComboBoxItem { Content = "All" });
            foreach (var login in userLogins)
            {
                if (!string.IsNullOrEmpty(login))
                    filter_RepairUser.Items.Add(new ComboBoxItem { Content = login });
            }
            filter_RepairUser.SelectedIndex = 0;

            var roles = db.Roles.Select(r => r.Name).Distinct().ToList();
            filter_UserRole.Items.Clear();
            filter_UserRole.Items.Add(new ComboBoxItem { Content = "All" });
            foreach (var roleName in roles)
            {
                if (!string.IsNullOrEmpty(roleName))
                    filter_UserRole.Items.Add(new ComboBoxItem { Content = roleName });
            }
            filter_UserRole.SelectedIndex = 0;

            PopulateRolePermissionsFilter();
        }

        private void PopulateRolePermissionsFilter()
        {
            filter_RolePermissionsList.Items.Clear();

            List<string> tableNames = new List<string> { "Trains", "Employee", "Repairs", "Warehouse", "Roles", "Users", "Brigades" };
            List<string> accessTypes = new List<string> { "Read", "Write", "Delete" };

            Dictionary<string, string> displayNames = new Dictionary<string, string>
            {
                {"Trains", "Trains"},
                {"Employee", "Employees"},
                {"Repairs", "Repairs"},
                {"Warehouse", "Warehouse"},
                {"Roles", "Roles"},
                {"Users", "Users"},
                {"Brigades", "Brigades"},
                {"Read", "Read"},
                {"Write", "Write"},
                {"Delete", "Delete"}
            };

            foreach (string tableName in tableNames)
            {
                foreach (string accessType in accessTypes)
                {
                    string permissionString = $"{tableName}:{accessType}";

                    string displayTableName = displayNames.ContainsKey(tableName) ? displayNames[tableName] : tableName;
                    string displayAccessType = displayNames.ContainsKey(accessType) ? displayNames[accessType] : accessType;

                    string displayString = $"{displayTableName}: {displayAccessType}";
                    filter_RolePermissionsList.Items.Add(new ComboBoxItem { Content = displayString, Tag = permissionString });
                }
            }
        }

        private void HideAllFilterPanels()
        {
            TrainsFilters.Visibility = Visibility.Collapsed;
            EmployeeFilters.Visibility = Visibility.Collapsed;
            RepairsFilters.Visibility = Visibility.Collapsed;
            RolesFilters.Visibility = Visibility.Collapsed;
            UsersFilters.Visibility = Visibility.Collapsed;
            BrigadesFilters.Visibility = Visibility.Collapsed;
            WarehouseFilters.Visibility = Visibility.Collapsed;
        }

        public void LoadDataForTable(string tableName)
        {
            HideAllFilterPanels();
            txtSearch.Text = "Search...";
            txtSearch.Foreground = Brushes.Gray;

            switch (tableName)
            {
                case "Trains":
                    db.Trains.Load();
                    list.ItemsSource = dataGrid.ItemsSource = db.Trains.Local.ToBindingList();
                    TrainsFilters.Visibility = Visibility.Visible;
                    break;
                case "Employee":
                    db.Employees.Load();
                    list.ItemsSource = dataGrid.ItemsSource = db.Employees.Local.ToBindingList();
                    EmployeeFilters.Visibility = Visibility.Visible;
                    break;
                case "Roles":
                    db.Roles.Load();
                    list.ItemsSource = dataGrid.ItemsSource = db.Roles.Local.ToBindingList();
                    RolesFilters.Visibility = Visibility.Visible;
                    break;
                case "Users":
                    db.Users.Include(u => u.Role).Load();
                    list.ItemsSource = dataGrid.ItemsSource = db.Users.Local.ToBindingList();
                    UsersFilters.Visibility = Visibility.Visible;
                    break;
                case "Repairs":
                    db.Repairs.Include(r => r.Train)
                              .Include(r => r.Brigades.Select(b => b.Employee))
                              .Load();
                    db.RepairTypes.Load();
                    db.Users.Load();
                    list.ItemsSource = dataGrid.ItemsSource = db.Repairs.Local.ToBindingList();
                    RepairsFilters.Visibility = Visibility.Visible;
                    break;
                case "Brigades":
                    db.Brigades.Include(b => b.Employee).Include(b => b.Repair).Load();
                    list.ItemsSource = dataGrid.ItemsSource = db.Brigades.Local.ToBindingList();
                    BrigadesFilters.Visibility = Visibility.Visible;
                    break;
                case "Warehouse":
                    db.Warehouses.Load();
                    list.ItemsSource = dataGrid.ItemsSource = db.Warehouses.Local.ToBindingList();
                    WarehouseFilters.Visibility = Visibility.Visible;
                    break;
                default:
                    list.ItemsSource = dataGrid.ItemsSource = null;
                    break;
            }
            RefreshBTNs();
        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!isInitialized || mv == null || string.IsNullOrWhiteSpace(mv.currentTable))
                return;

            switch (mv.currentTable)
            {
                case "Trains":
                    TrainFilter_Changed(null, null);
                    break;
                case "Employee":
                    EmployeeFilter_Changed(null, null);
                    break;
                case "Warehouse":
                    WarehouseFilter_Changed(null, null);
                    break;
                case "Roles":
                    RoleFilter_Changed(null, null);
                    break;
                case "Users":
                    UserFilter_Changed(null, null);
                    break;
                case "Repairs":
                    RepairFilter_Changed(null, null);
                    break;
                case "Brigades":
                    BrigadeFilter_Changed(null, null);
                    break;
            }
        }

        private void TrainFilter_Changed(object sender, EventArgs e)
        {
            if (!isInitialized || mv == null || mv.currentTable != "Trains")
                return;

            string generalSearchText = txtSearch.Text.Trim().ToLower();
            if (generalSearchText == "search...") generalSearchText = "";

            int? yearFrom = null;
            if (int.TryParse(filter_TrainManufactureYearFrom.Text.Trim(), out int yFrom))
            {
                yearFrom = yFrom;
            }

            int? yearTo = null;
            if (int.TryParse(filter_TrainManufactureYearTo.Text.Trim(), out int yTo))
            {
                yearTo = yTo;
            }

            int? mileageUpTo = null;
            if (int.TryParse(filter_TrainMileageUpTo.Text.Trim(), out int mUpTo))
            {
                mileageUpTo = mUpTo;
            }

            string statusFilter = (filter_TrainStatus.SelectedItem as ComboBoxItem)?.Content.ToString();

            var all = db.Trains.Local.ToList();

            var filtered = all.Where(t =>
                (string.IsNullOrWhiteSpace(generalSearchText) ||
                    (t.Model?.ToLower().Contains(generalSearchText) == true) ||
                    (t.Number?.ToLower().Contains(generalSearchText) == true) ||
                    (t.ManufactureYear?.ToLower().Contains(generalSearchText) == true) ||
                    (t.Mileage?.ToLower().Contains(generalSearchText) == true)) &&

                (yearFrom == null || (int.TryParse(t.ManufactureYear, out int trainYear) && trainYear >= yearFrom.Value)) &&
                (yearTo == null || (int.TryParse(t.ManufactureYear, out int trainYearTo) && trainYearTo <= yearTo.Value)) &&

                (mileageUpTo == null || (int.TryParse(t.Mileage, out int trainMileage) && trainMileage <= mileageUpTo.Value)) &&

                (statusFilter == "All" || (t.CurrentStatus?.Equals(statusFilter, StringComparison.OrdinalIgnoreCase) ?? false))
            ).ToList();

            list.ItemsSource = dataGrid.ItemsSource = new BindingList<Train>(filtered);
        }

        private void ClearTrainFilters_Click(object sender, RoutedEventArgs e)
        {
            filter_TrainManufactureYearFrom.Text = "";
            filter_TrainManufactureYearTo.Text = "";
            filter_TrainMileageUpTo.Text = "";
            filter_TrainStatus.SelectedIndex = 0;
            TrainFilter_Changed(null, null);
        }

        private void EmployeeFilter_Changed(object sender, EventArgs e)
        {
            if (!isInitialized || mv == null || mv.currentTable != "Employee")
                return;

            string generalSearchText = txtSearch.Text.Trim().ToLower();
            if (generalSearchText == "search...") generalSearchText = "";

            string specialityFilter = (filter_EmployeeSpeciality.SelectedItem as ComboBoxItem)?.Content.ToString();

            var all = db.Employees.Local.ToList();

            var filtered = all.Where(emp =>
                (string.IsNullOrWhiteSpace(generalSearchText) || emp.FullName.ToLower().Contains(generalSearchText) || emp.Speciality.ToLower().Contains(generalSearchText)) &&
                (specialityFilter == "All" || (emp.Speciality?.Equals(specialityFilter, StringComparison.OrdinalIgnoreCase) ?? false))
            ).ToList();

            list.ItemsSource = dataGrid.ItemsSource = new BindingList<Employee>(filtered);
        }

        private void ClearEmployeeFilters_Click(object sender, RoutedEventArgs e)
        {
            filter_EmployeeSpeciality.SelectedIndex = 0;
            EmployeeFilter_Changed(null, null);
        }

        private void RepairFilter_Changed(object sender, EventArgs e)
        {
            if (!isInitialized || mv == null || mv.currentTable != "Repairs")
                return;

            string generalSearchText = txtSearch.Text.Trim().ToLower();
            if (generalSearchText == "search...") generalSearchText = "";

            string trainNumberFilter = (filter_RepairTrainNumber.SelectedItem as ComboBoxItem)?.Content.ToString();
            string repairTypeFilter = (filter_RepairType.SelectedItem as ComboBoxItem)?.Content.ToString();
            string userLoginFilter = (filter_RepairUser.SelectedItem as ComboBoxItem)?.Content.ToString();
            string statusFilter = (filter_RepairStatus.SelectedItem as ComboBoxItem)?.Content.ToString();

            var all = db.Repairs.Local.ToList();

            var filtered = all.Where(r =>
                (string.IsNullOrWhiteSpace(generalSearchText) ||
                 (r.Description?.ToLower().Contains(generalSearchText) == true) ||
                 (r.Status?.ToLower().Contains(generalSearchText) == true) ||
                 (r.Train?.Number?.ToLower().Contains(generalSearchText) ?? false) ||
                 (db.RepairTypes.Any(rt => rt.id == r.TypeID && rt.Name.ToLower().Contains(generalSearchText))) ||
                 (db.Users.Any(u => u.id == r.UserID && u.Login.ToLower().Contains(generalSearchText))) ||
                 (r.Brigades != null && r.Brigades.Any(b =>
                     b.id.ToString().Contains(generalSearchText) ||
                     (b.Employee?.FullName?.ToLower().Contains(generalSearchText) ?? false)
                 ))
                ) &&

                (trainNumberFilter == "All" || (r.Train?.Number?.Equals(trainNumberFilter, StringComparison.OrdinalIgnoreCase) ?? false)) &&
                (repairTypeFilter == "All" || (db.RepairTypes.Any(rt => rt.id == r.TypeID && rt.Name.Equals(repairTypeFilter, StringComparison.OrdinalIgnoreCase)))) &&
                (userLoginFilter == "All" || (db.Users.Any(u => u.id == r.UserID && u.Login.Equals(userLoginFilter, StringComparison.OrdinalIgnoreCase)))) &&
                (statusFilter == "All" || (r.Status?.Equals(statusFilter, StringComparison.OrdinalIgnoreCase) ?? false))
            ).ToList();

            list.ItemsSource = dataGrid.ItemsSource = new BindingList<Repair>(filtered);
        }

        private void ClearRepairsFilters_Click(object sender, RoutedEventArgs e)
        {
            filter_RepairTrainNumber.SelectedIndex = 0;
            filter_RepairType.SelectedIndex = 0;
            filter_RepairUser.SelectedIndex = 0;
            filter_RepairStatus.SelectedIndex = 0;
            RepairFilter_Changed(null, null);
        }

        private void WarehouseFilter_Changed(object sender, EventArgs e)
        {
            if (!isInitialized || mv == null || mv.currentTable != "Warehouse")
                return;

            string generalSearchText = txtSearch.Text.Trim().ToLower();
            if (generalSearchText == "search...") generalSearchText = "";

            var all = db.Warehouses.Local.ToList();

            var filtered = all.Where(w =>
                (string.IsNullOrWhiteSpace(generalSearchText) || (w.Name?.ToLower().Contains(generalSearchText) == true) || (w.Number?.ToLower().Contains(generalSearchText) == true))
            ).ToList();

            list.ItemsSource = dataGrid.ItemsSource = new BindingList<Warehouse>(filtered);
        }

        private void ClearWarehouseFilters_Click(object sender, RoutedEventArgs e)
        {
            WarehouseFilter_Changed(null, null);
        }

        private void RoleFilter_Changed(object sender, EventArgs e)
        {
            if (!isInitialized || mv == null || mv.currentTable != "Roles")
                return;

            string generalSearchText = txtSearch.Text.Trim().ToLower();
            if (generalSearchText == "search...") generalSearchText = "";

            List<string> selectedPermissions = filter_RolePermissionsList.SelectedItems
                                                .Cast<ComboBoxItem>()
                                                .Select(item => item.Tag.ToString())
                                                .ToList();

            var all = db.Roles.Local.ToList();

            var filtered = all.Where(r =>
                (string.IsNullOrWhiteSpace(generalSearchText) ||
                 (r.Name?.ToLower().Contains(generalSearchText) == true) ||
                 (r.Description?.ToLower().Contains(generalSearchText) == true) ||
                 (r.Permissions?.ToLower().Contains(generalSearchText) == true)
                ) &&

                (selectedPermissions.Count == 0 ||
                 selectedPermissions.All(sp => r.PermissionSet.Contains(sp)))
            ).ToList();

            list.ItemsSource = dataGrid.ItemsSource = new BindingList<Role>(filtered);
        }

        private void ClearRolesFilters_Click(object sender, RoutedEventArgs e)
        {
            filter_RolePermissionsList.UnselectAll();
            RoleFilter_Changed(null, null);
        }

        private void UserFilter_Changed(object sender, EventArgs e)
        {
            if (!isInitialized || mv == null || mv.currentTable != "Users")
                return;

            string generalSearchText = txtSearch.Text.Trim().ToLower();
            if (generalSearchText == "search...") generalSearchText = "";

            string roleFilter = (filter_UserRole.SelectedItem as ComboBoxItem)?.Content.ToString();

            var all = db.Users.Local.ToList();

            var filtered = all.Where(u =>
                (string.IsNullOrWhiteSpace(generalSearchText) || (u.Login?.ToLower().Contains(generalSearchText) == true) || (u.Role?.Name?.ToLower().Contains(generalSearchText) ?? false)) &&
                (roleFilter == "All" || (u.Role?.Name?.Equals(roleFilter, StringComparison.OrdinalIgnoreCase) ?? false))
            ).ToList();

            list.ItemsSource = dataGrid.ItemsSource = new BindingList<User>(filtered);
        }

        private void ClearUsersFilters_Click(object sender, RoutedEventArgs e)
        {
            filter_UserRole.SelectedIndex = 0;
            UserFilter_Changed(null, null);
        }

        private void BrigadeFilter_Changed(object sender, EventArgs e)
        {
            if (!isInitialized || mv == null || mv.currentTable != "Brigades")
                return;

            string generalSearchText = txtSearch.Text.Trim().ToLower();
            if (generalSearchText == "search...") generalSearchText = "";

            var all = db.Brigades.Local.ToList();

            var filtered = all.Where(b =>
                (string.IsNullOrWhiteSpace(generalSearchText) ||
                 b.EmployeeID.ToString().Contains(generalSearchText) ||
                 b.RepairID.ToString().Contains(generalSearchText) ||
                 (b.Employee?.FullName?.ToLower().Contains(generalSearchText) ?? false) ||
                 (b.Repair?.Description?.ToLower().Contains(generalSearchText) ?? false)
                )
            ).ToList();

            list.ItemsSource = dataGrid.ItemsSource = new BindingList<Brigade>(filtered);
        }

        private void ClearBrigadesFilters_Click(object sender, RoutedEventArgs e)
        {
            BrigadeFilter_Changed(null, null);
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
                        mv.db.Trains.Add((trainWin.DataContext as TrainViewModel)._currentTrain);
                        mv.db.SaveChanges();
                    }
                    break;
                case "Employee":
                    EmployeeWindow employeeWin = new EmployeeWindow(new Employee());
                    if (employeeWin.ShowDialog() == true)
                    {
                        mv.db.Employees.Add((employeeWin.DataContext as EmployeeViewModel)._employee);
                        mv.db.SaveChanges();
                    }
                    break;
                case "Warehouse":
                    WarehouseWindow warehouseWin = new WarehouseWindow(new Warehouse());
                    warehouseWin.ShowDialog();
                    break;
                case "Roles":
                    RoleWindow roleWin = new RoleWindow(new Role());
                    if (roleWin.ShowDialog() == true)
                    {
                        mv.db.Roles.Add((roleWin.DataContext as RoleViewModel)._currentRole);
                        mv.db.SaveChanges();
                    }
                    break;
                case "Users":
                    UserWindow userWin = new UserWindow(new User());
                    userWin.ShowDialog();
                    break;
                case "Repairs":
                    RepairWindow repairWin = new RepairWindow(new Repair());
                    repairWin.ShowDialog();
                    break;
            }
            LoadDataForTable(mv.currentTable);
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
                        MessageBox.Show("Select a train to edit", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
                        MessageBox.Show("Select an employee to edit", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
                        MessageBox.Show("Select an item to edit", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
                        MessageBox.Show("Select a role to edit", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
                        MessageBox.Show("Select a user to edit", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
                        MessageBox.Show("Select a repair to edit", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    break;
            }
            LoadDataForTable(mv.currentTable);
        }

        private void btn_delete_Click(object sender, RoutedEventArgs e)
        {
            switch (mv.currentTable)
            {
                case "Employee":
                    if (list.SelectedItem != null)
                    {
                        Employee employee = list.SelectedItem as Employee;
                        if (MessageBox.Show($"Are you sure you want to delete {employee.FullName}?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
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
                                    MessageBox.Show($"Failed to delete photo: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                                }
                            }
                            mv.db.Employees.Remove(employee);
                            mv.db.SaveChanges();
                        }
                    }
                    else
                    {
                        MessageBox.Show("Select an employee to delete", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    break;
                case "Trains":
                    if (list.SelectedItem != null)
                    {
                        Train train = list.SelectedItem as Train;
                        if (MessageBox.Show($"Are you sure you want to delete train {train.Number}?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
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
                                    MessageBox.Show($"Failed to delete photo: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                                }
                            }
                            mv.db.Trains.Remove(train);
                            mv.db.SaveChanges();
                        }
                    }
                    else
                    {
                        MessageBox.Show("Select a train to delete", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    break;
                case "Warehouse":
                    if (list.SelectedItem != null)
                    {
                        Warehouse selectedWarehouse = list.SelectedItem as Warehouse;

                        if (MessageBox.Show($"Are you sure you want to delete item {selectedWarehouse.Name}?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                        {
                            var warehouseInDb = mv.db.Warehouses.Find(selectedWarehouse.id);
                            if (warehouseInDb != null)
                            {
                                mv.db.Warehouses.Remove(warehouseInDb);
                                mv.db.SaveChanges();
                            }
                            else
                            {
                                MessageBox.Show("Failed to find object in database.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                        }
                    }
                    break;
                case "Roles":
                    if (list.SelectedItem != null)
                    {
                        Role role = list.SelectedItem as Role;
                        if (MessageBox.Show($"Are you sure you want to delete role {role.Name}?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                        {
                            mv.db.Roles.Remove(role);
                            mv.db.SaveChanges();
                        }
                    }
                    else
                    {
                        MessageBox.Show("Select a role to delete", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    break;
                case "Users":
                    if (list.SelectedItem != null)
                    {
                        User user = list.SelectedItem as User;
                        if (MessageBox.Show($"Are you sure you want to delete user {user.Login}?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                        {
                            mv.db.Users.Remove(user);
                            mv.db.SaveChanges();
                        }
                    }
                    else
                    {
                        MessageBox.Show("Select a user to delete", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    break;
                case "Repairs":
                    if (list.SelectedItem != null)
                    {
                        Repair repair = list.SelectedItem as Repair;
                        if (MessageBox.Show($"Are you sure you want to delete repair {repair.TrainID}?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                        {
                            mv.db.Repairs.Remove(repair);
                            mv.db.SaveChanges();
                        }
                    }
                    else
                    {
                        MessageBox.Show("Select a repair to delete", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    break;
            }
            LoadDataForTable(mv.currentTable);
            list.Items.Refresh();
        }

        private void btn_swap_Click(object sender, RoutedEventArgs e)
        {
            if (list.Visibility == Visibility.Visible)
            {
                list.Visibility = Visibility.Collapsed;
                dataGrid.Visibility = Visibility.Visible;
            }
            else
            {
                list.Visibility = Visibility.Visible;
                dataGrid.Visibility = Visibility.Collapsed;
            }
        }

        private void txtSearch_GotFocus(object sender, RoutedEventArgs e)
        {
            if (txtSearch.Text == "Search...")
            {
                txtSearch.Text = "";
                txtSearch.Foreground = Brushes.Black;
            }
        }

        private void txtSearch_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtSearch.Text))
            {
                txtSearch.Text = "Search...";
                txtSearch.Foreground = Brushes.Gray;
            }
        }

        private void SpareFilter_Changed(object sender, TextChangedEventArgs e)
        {
        }

        private void ClearSparesFilters_Click(object sender, RoutedEventArgs e)
        {
        }
    }
}