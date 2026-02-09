using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using System.Windows;
using DepotInfoSystem.Classes;
using DepotInfoSystem.Froms;
using DepotInfoSystem.Utilities;
using System;

namespace DepotInfoSystem.ViewModels
{
    public class LoginViewModel : ObservableObject
    {
        private readonly Window _view;
        private ApplicationContext _db;

        private string _login;
        public string Login
        {
            get => _login;
            set
            {
                _login = value;
                OnPropertyChanged(nameof(Login));
                LoginCommand.RaiseCanExecuteChanged();
            }
        }

        private string _password;
        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                OnPropertyChanged(nameof(Password));
                LoginCommand.RaiseCanExecuteChanged();
            }
        }

        public RelayCommand LoginCommand { get; private set; }
        public RelayCommand CancelCommand { get; private set; }

        public LoginViewModel(Window view)
        {
            _view = view;
            _db = new ApplicationContext();
            LoginCommand = new RelayCommand(ExecuteLogin, CanExecuteLogin);
            CancelCommand = new RelayCommand(ExecuteCancel);

            Login = "admin";
            Password = "admin";
        }

        private void ExecuteLogin(object parameter)
        {
            if (string.IsNullOrWhiteSpace(Login) || string.IsNullOrWhiteSpace(Password))
            {
                MessageBox.Show("Please enter login and password.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var user = _db.Users.FirstOrDefault(u => u.Login == Login && u.Password == Password);

            if (user != null)
            {
                MainWindow mainWindow = new MainWindow(user);
                mainWindow.Show();
                _view.Close();
            }
            else
            {
                MessageBox.Show("Invalid login or password.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool CanExecuteLogin(object parameter)
        {
            return !string.IsNullOrWhiteSpace(Login) && !string.IsNullOrWhiteSpace(Password);
        }

        private void ExecuteCancel(object parameter)
        {
            if (_view.Owner != null)
            {
                _view.DialogResult = false;
            }
            _view.Close();
        }
    }
}