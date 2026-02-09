using DepotInfoSystem.Classes;
using DepotInfoSystem.Froms;
using DepotInfoSystem.Services;
using DepotInfoSystem.Views;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using System.Runtime.CompilerServices;

namespace DepotInfoSystem.ViewModels
{
    public class DbControlViewModel : INotifyPropertyChanged
    {
        private readonly ApplicationContext _dbContext;
        private readonly User _currentUser;
        private string _currentTable;

        public ObservableCollection<object> Items { get; set; } = new ObservableCollection<object>();
        public ObservableCollection<string> FilterProperties { get; set; } = new ObservableCollection<string>();

        private object _selectedItem;
        public object SelectedItem
        {
            get => _selectedItem;
            set
            {
                if (_selectedItem != value)
                {
                    _selectedItem = value;
                    OnPropertyChanged();
                    EditCommand.RaiseCanExecuteChanged();
                    DeleteCommand.RaiseCanExecuteChanged();
                }
            }
        }

        private bool _canAdd;
        public bool CanAdd
        {
            get => _canAdd;
            set
            {
                if (_canAdd != value)
                {
                    _canAdd = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _canEdit;
        public bool CanEdit
        {
            get => _canEdit;
            set
            {
                if (_canEdit != value)
                {
                    _canEdit = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _canDelete;
        private IDataService dataService;
        private string currentTableName;

        public bool CanDelete
        {
            get => _canDelete;
            set
            {
                if (_canDelete != value)
                {
                    _canDelete = value;
                    OnPropertyChanged();
                }
            }
        }

        public RelayCommand AddCommand { get; private set; }
        public RelayCommand EditCommand { get; private set; }
        public RelayCommand DeleteCommand { get; private set; }
        public RelayCommand FilterCommand { get; private set; }
        public RelayCommand ResetFilterCommand { get; private set; }


        public DbControlViewModel(ApplicationContext dbContext, User currentUser, string currentTable)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _currentUser = currentUser ?? throw new ArgumentNullException(nameof(currentUser));
            _currentTable = currentTable ?? throw new ArgumentNullException(nameof(currentTable));

            InitializeCommands();
            LoadData();
            UpdatePermissions();
        }

        public DbControlViewModel(IDataService dataService, string currentTableName)
        {
            this.dataService = dataService;
            this.currentTableName = currentTableName;
        }

        private void InitializeCommands()
        {
            AddCommand = new RelayCommand(AddAction, CanAddAction);
            EditCommand = new RelayCommand(EditAction, CanEditAction);
            DeleteCommand = new RelayCommand(DeleteAction, CanDeleteAction);
            FilterCommand = new RelayCommand(FilterData);
            ResetFilterCommand = new RelayCommand(ResetFilter);
        }

        private void LoadData()
        {
            Items.Clear();

            switch (_currentTable)
            {
                case "Users":
                    _dbContext.Users.Include(u => u.Role).Load();
                    _dbContext.Users.Local.ToList().ForEach(Items.Add);
                    PopulateFilterProperties(typeof(User));
                    break;
                case "Trains":
                    _dbContext.Trains.Load();
                    _dbContext.Trains.Local.ToList().ForEach(Items.Add);
                    PopulateFilterProperties(typeof(Train));
                    break;
                case "Employees":
                    _dbContext.Employees.Load();
                    _dbContext.Employees.Local.ToList().ForEach(Items.Add);
                    PopulateFilterProperties(typeof(Employee));
                    break;
                case "Warehouses":
                    _dbContext.Warehouses.Load();
                    _dbContext.Warehouses.Local.ToList().ForEach(Items.Add);
                    PopulateFilterProperties(typeof(Warehouse));
                    break;
                case "Roles":
                    _dbContext.Roles.Load();
                    _dbContext.Roles.Local.ToList().ForEach(Items.Add);
                    PopulateFilterProperties(typeof(Role));
                    break;
                case "Repairs":
                    _dbContext.Repairs
    .Include(r => r.Train)
    .Include(r => r.Brigades.Select(b => b.Employee))
    .Load();

                    _dbContext.Repairs.Local.ToList().ForEach(Items.Add);
                    PopulateFilterProperties(typeof(Repair));
                    break;
                default:
                    break;
            }
            AddCommand.RaiseCanExecuteChanged();
            EditCommand.RaiseCanExecuteChanged();
            DeleteCommand.RaiseCanExecuteChanged();
        }
        public void UpdatePermissions()
        {
            var roleName = _currentUser.Role?.Name;

            switch (_currentTable)
            {
                case "Users":
                    CanAdd = roleName == "Admin";
                    CanEdit = roleName == "Admin";
                    CanDelete = roleName == "Admin";
                    break;
                case "Trains":
                case "Employees":
                case "Warehouses":
                case "Roles":
                case "Repairs":
                    CanAdd = roleName == "Admin" || roleName == "Manager";
                    CanEdit = roleName == "Admin" || roleName == "Manager";
                    CanDelete = roleName == "Admin" || roleName == "Manager";
                    break;
                default:
                    CanAdd = false;
                    CanEdit = false;
                    CanDelete = false;
                    break;
            }
            AddCommand.RaiseCanExecuteChanged();
            EditCommand.RaiseCanExecuteChanged();
            DeleteCommand.RaiseCanExecuteChanged();
        }

        private void AddAction(object parameter)
        {
            bool? dialogResult = false;
            switch (_currentTable)
            {
                case "Users":
                    var newUser = new User();
                    UserWindow userWin = new UserWindow(newUser);
                    dialogResult = userWin.ShowDialog();
                    if (dialogResult == true)
                    {
                        _dbContext.Users.Add(newUser);
                    }
                    break;
                case "Trains":
                    var newTrain = new Train();
                    TrainWindow trainWin = new TrainWindow(newTrain);
                    dialogResult = trainWin.ShowDialog();
                    if (dialogResult == true)
                    {
                        _dbContext.Trains.Add(newTrain);
                    }
                    break;
                case "Employees":
                    var newEmployee = new Employee();
                    EmployeeWindow empWin = new EmployeeWindow(newEmployee);
                    dialogResult = empWin.ShowDialog();
                    if (dialogResult == true)
                    {
                        _dbContext.Employees.Add(newEmployee);
                    }
                    break;
                case "Warehouses":
                    var newWarehouse = new Warehouse();
                    WarehouseWindow warehouseWin = new WarehouseWindow(newWarehouse);
                    dialogResult = warehouseWin.ShowDialog();
                    if (dialogResult == true)
                    {
                        _dbContext.Warehouses.Add(newWarehouse);
                    }
                    break;
                case "Roles":
                    var newRole = new Role();
                    RoleWindow roleWin = new RoleWindow(newRole);
                    dialogResult = roleWin.ShowDialog();
                    if (dialogResult == true)
                    {
                        _dbContext.Roles.Add(newRole);
                    }
                    break;
                case "Repairs":
                    var newRepair = new Repair();
                    RepairWindow repairWin = new RepairWindow(newRepair);
                    dialogResult = repairWin.ShowDialog();
                    if (dialogResult == true)
                    {
                        _dbContext.Repairs.Add(newRepair);
                    }
                    break;
            }

            if (dialogResult == true)
            {
                _dbContext.SaveChanges();
                LoadData();
            }
        }

        private bool CanAddAction(object parameter)
        {
            return CanAdd;
        }

        private void EditAction(object parameter)
        {
            if (SelectedItem == null) return;

            bool? dialogResult = false;
            switch (_currentTable)
            {
                case "Users":
                    UserWindow userWin = new UserWindow(SelectedItem as User);
                    dialogResult = userWin.ShowDialog();
                    break;
                case "Trains":
                    TrainWindow trainWin = new TrainWindow(SelectedItem as Train);
                    dialogResult = trainWin.ShowDialog();
                    break;
                case "Employees":
                    EmployeeWindow empWin = new EmployeeWindow(SelectedItem as Employee);
                    dialogResult = empWin.ShowDialog();
                    break;
                case "Warehouses":
                    WarehouseWindow warehouseWin = new WarehouseWindow(SelectedItem as Warehouse);
                    dialogResult = warehouseWin.ShowDialog();
                    break;
                case "Roles":
                    RoleWindow roleWin = new RoleWindow(SelectedItem as Role);
                    dialogResult = roleWin.ShowDialog();
                    break;
                case "Repairs":
                    RepairWindow repairWin = new RepairWindow(SelectedItem as Repair);
                    dialogResult = repairWin.ShowDialog();
                    break;
            }

            if (dialogResult == true)
            {
                _dbContext.SaveChanges();
                LoadData();
            }
        }

        private bool CanEditAction(object parameter)
        {
            return CanEdit && SelectedItem != null;
        }

        private void DeleteAction(object parameter)
        {
            if (SelectedItem == null) return;

            try
            {
                switch (_currentTable)
                {
                    case "Users":
                        _dbContext.Users.Remove(SelectedItem as User);
                        break;
                    case "Trains":
                        _dbContext.Trains.Remove(SelectedItem as Train);
                        break;
                    case "Employees":
                        _dbContext.Employees.Remove(SelectedItem as Employee);
                        break;
                    case "Warehouses":
                        _dbContext.Warehouses.Remove(SelectedItem as Warehouse);
                        break;
                    case "Roles":
                        _dbContext.Roles.Remove(SelectedItem as Role);
                        break;
                    case "Repairs":
                        _dbContext.Repairs.Remove(SelectedItem as Repair);
                        break;
                }
                _dbContext.SaveChanges();
                LoadData(); 
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Failed to delete: {ex.Message}", "Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }

        private bool CanDeleteAction(object parameter)
        {
            return CanDelete && SelectedItem != null;
        }

        private void FilterData(object parameter)
        {

        }

        private void ResetFilter(object parameter)
        {

        }

        private void PopulateFilterProperties(Type itemType)
        {
            FilterProperties.Clear();
            foreach (var prop in itemType.GetProperties())
            {
                FilterProperties.Add(prop.Name);
            }
        }
    
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}