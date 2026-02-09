using DepotInfoSystem.Classes;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace DepotInfoSystem.ViewModels
{
    public class UserViewModel : INotifyPropertyChanged
    {
        public User _currentUser;
        private ApplicationContext _dbContext;

        public string UserLogin
        {
            get => _currentUser.Login;
            set
            {
                if (_currentUser.Login != value)
                {
                    _currentUser.Login = value;
                    OnPropertyChanged(nameof(UserLogin));
                    (SaveCommand as RelayCommand)?.RaiseCanExecuteChanged();
                }
            }
        }

        public string UserPassword
        {
            get => _currentUser.Password;
            set
            {
                if (_currentUser.Password != value)
                {
                    _currentUser.Password = value;
                    OnPropertyChanged(nameof(UserPassword));
                    (SaveCommand as RelayCommand)?.RaiseCanExecuteChanged();
                }
            }
        }

        public ObservableCollection<Role> AvailableRoles { get; set; }

        private int _selectedRoleId;
        public int SelectedRoleId
        {
            get => _selectedRoleId;
            set
            {
                if (_selectedRoleId != value)
                {
                    _selectedRoleId = value;
                    OnPropertyChanged(nameof(SelectedRoleId));
                    _currentUser.RoleID = value;
                    (SaveCommand as RelayCommand)?.RaiseCanExecuteChanged();
                }
            }
        }

        public ICommand SaveCommand { get; private set; }
        public ICommand CancelCommand { get; private set; }

        public event EventHandler RequestClose;
        public bool? DialogResult { get; private set; }

        public UserViewModel(User user)
        {
            _currentUser = user ?? new User();
            _dbContext = new ApplicationContext();

            AvailableRoles = new ObservableCollection<Role>();
            LoadRoles();

            SelectedRoleId = _currentUser.RoleID;

            SaveCommand = new RelayCommand(SaveUser, CanSaveUser);
            CancelCommand = new RelayCommand(Cancel);
        }

        private void LoadRoles()
        {
            _dbContext.Roles.ToList().ForEach(AvailableRoles.Add);
        }

        private bool CanSaveUser(object parameter)
        {
            return !string.IsNullOrWhiteSpace(UserLogin) &&
                   !string.IsNullOrWhiteSpace(UserPassword) &&
                   SelectedRoleId != 0;
        }

        private void SaveUser(object parameter)
        {
            try
            {
                _currentUser.Login = UserLogin;
                _currentUser.Password = UserPassword;
                _currentUser.RoleID = SelectedRoleId;

                if (_currentUser.id == 0)
                {
                    _dbContext.Users.Add(_currentUser);
                }

                _dbContext.SaveChanges();

                DialogResult = true;
                RequestClose?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving user: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                DialogResult = false;
            }
        }

        private void Cancel(object parameter)
        {
            DialogResult = false;
            RequestClose?.Invoke(this, EventArgs.Empty);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}