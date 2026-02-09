using DepotInfoSystem.Classes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace DepotInfoSystem.ViewModels
{
    public class RoleViewModel : INotifyPropertyChanged
    {
        public Role _currentRole;

        public string RoleName
        {
            get => _currentRole.Name;
            set
            {
                if (_currentRole.Name != value)
                {
                    _currentRole.Name = value;
                    OnPropertyChanged(nameof(RoleName));
                }
            }
        }

        public string RoleDescription
        {
            get => _currentRole.Description;
            set
            {
                if (_currentRole.Description != value)
                {
                    _currentRole.Description = value;
                    OnPropertyChanged(nameof(RoleDescription));
                }
            }
        }

        public ObservableCollection<PermissionItemViewModel> PermissionViewModels { get; set; }

        public ICommand SaveCommand { get; private set; }
        public ICommand CancelCommand { get; private set; }

        public event EventHandler RequestClose;
        public bool? DialogResult { get; private set; }

        public RoleViewModel(Role role)
        {
            _currentRole = role ?? new Role();

            PermissionViewModels = new ObservableCollection<PermissionItemViewModel>(
                new[] { "Trains", "Employee", "Repairs", "Warehouse", "Roles", "Users" }
                .Select(p => new PermissionItemViewModel(p, _currentRole)));

            SaveCommand = new RelayCommand(SaveRole);
            CancelCommand = new RelayCommand(Cancel);
        }

        private void SaveRole(object parameter)
        {
            _currentRole.Permissions = string.Join(",", _currentRole.PermissionSet);

            DialogResult = true;
            RequestClose?.Invoke(this, EventArgs.Empty);
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