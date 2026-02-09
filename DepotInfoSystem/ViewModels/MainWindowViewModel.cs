using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using DepotInfoSystem.Classes;

namespace DepotInfoSystem.ViewModels
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private readonly ApplicationContext _dbContext;
        private readonly User _currentUser;
        MainWindow win;

        private object _currentPage;
        public object CurrentPage
        {
            get => _currentPage;
            set
            {
                if (_currentPage != value)
                {
                    _currentPage = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _isTrainsVisible;
        public bool IsTrainsVisible
        {
            get => _isTrainsVisible;
            set { if (_isTrainsVisible != value) { _isTrainsVisible = value; OnPropertyChanged(); } }
        }

        private bool _isEmployeesVisible;
        public bool IsEmployeesVisible
        {
            get => _isEmployeesVisible;
            set { if (_isEmployeesVisible != value) { _isEmployeesVisible = value; OnPropertyChanged(); } }
        }

        private bool _isRepairsVisible;
        public bool IsRepairsVisible
        {
            get => _isRepairsVisible;
            set { if (_isRepairsVisible != value) { _isRepairsVisible = value; OnPropertyChanged(); } }
        }

        private bool _isWarehousesVisible;
        public bool IsWarehousesVisible
        {
            get => _isWarehousesVisible;
            set { if (_isWarehousesVisible != value) { _isWarehousesVisible = value; OnPropertyChanged(); } }
        }

        private bool _isRolesVisible;
        public bool IsRolesVisible
        {
            get => _isRolesVisible;
            set { if (_isRolesVisible != value) { _isRolesVisible = value; OnPropertyChanged(); } }
        }

        private bool _isUsersVisible;
        public bool IsUsersVisible
        {
            get => _isUsersVisible;
            set { if (_isUsersVisible != value) { _isUsersVisible = value; OnPropertyChanged(); } }
        }

        public RelayCommand NavigateCommand { get; private set; }

        public MainWindowViewModel(ApplicationContext dbContext, User currentUser, MainWindow win)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _currentUser = currentUser ?? throw new ArgumentNullException(nameof(currentUser));

            InitializeCommands();
            ApplyUserPermissions();
            NavigateTo("Employee");
            this.win = win;
        }

        private void InitializeCommands()
        {
            NavigateCommand = new RelayCommand(ExecuteNavigate);
        }

        private void ExecuteNavigate(object parameter)
        {
            if (parameter is string tableName)
            {
                NavigateTo(tableName);
            }
        }

        public void NavigateTo(string tableName)
        {
            CurrentPage = new db_control(win);
        }

        private void ApplyUserPermissions()
        {
            if (_currentUser.RoleID == 1)
            {
                IsTrainsVisible = true;
                IsEmployeesVisible = true;
                IsRepairsVisible = true;
                IsWarehousesVisible = true;
                IsRolesVisible = true;
                IsUsersVisible = true;
                return;
            }

            var role = _dbContext.Roles.FirstOrDefault(r => r.id == _currentUser.RoleID);
            if (role != null)
            {
                var permissionStrings = role.Permissions?.Split(',') ?? Array.Empty<string>();

                foreach (var permissionString in permissionStrings)
                {
                    var parts = permissionString.Split(':');
                    if (parts.Length == 2)
                    {
                        var resource = parts[0];
                        var access = parts[1];

                        if (!access.Equals("Deny", StringComparison.OrdinalIgnoreCase))
                        {
                            switch (resource)
                            {
                                case "Trains": IsTrainsVisible = true; break;
                                case "Employee": IsEmployeesVisible = true; break;
                                case "Repairs": IsRepairsVisible = true; break;
                                case "Warehouse": IsWarehousesVisible = true; break;
                                case "Roles": IsRolesVisible = true; break;
                                case "Users": IsUsersVisible = true; break;
                            }
                        }
                    }
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}