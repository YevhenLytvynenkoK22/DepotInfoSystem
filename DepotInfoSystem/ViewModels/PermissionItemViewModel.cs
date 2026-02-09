using DepotInfoSystem.Classes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace DepotInfoSystem.ViewModels
{
    public class PermissionItemViewModel : INotifyPropertyChanged
    {
        public string Permission { get; }
        public string PermissionDisplayName { get; }
        private string _currentAccess;
        private Role _role;

        private static readonly Dictionary<string, string> PermissionDisplayNames = new Dictionary<string, string>
        {
            {"Trains", "Trains"},
            {"Employee", "Employees"},
            {"Repairs", "Repairs"},
            {"Warehouse", "Warehouse"},
            {"Roles", "Roles"},
            {"Users", "Users"}
        };

        public PermissionItemViewModel(string permission, Role role)
        {
            Permission = permission;
            string displayName;
            if (PermissionDisplayNames.TryGetValue(permission, out displayName))
            {
                PermissionDisplayName = displayName;
            }
            else
            {
                PermissionDisplayName = permission;
            }
            _role = role;
            InitializeAccess();
        }

        private void InitializeAccess()
        {
            if (_role.PermissionSet == null)
            {
                _role.PermissionSet = new HashSet<string>();
            }

            string accessEntry = _role.PermissionSet
                                      .FirstOrDefault(p => p.StartsWith(Permission + ":", StringComparison.OrdinalIgnoreCase));

            if (accessEntry != null)
            {
                _currentAccess = accessEntry.Split(':')[1];
            }
            else
            {
                _currentAccess = "Deny";
                _role.PermissionSet.Add($"{Permission}:{_currentAccess}");
            }
        }

        public bool IsWrite
        {
            get => _currentAccess == "Write";
            set
            {
                if (value)
                {
                    _currentAccess = "Write";
                    UpdateRolePermission();
                    NotifyAllPropertiesChanged();
                }
            }
        }

        public bool IsRead
        {
            get => _currentAccess == "Read";
            set
            {
                if (value)
                {
                    _currentAccess = "Read";
                    UpdateRolePermission();
                    NotifyAllPropertiesChanged();
                }
            }
        }

        public bool IsDeny
        {
            get => _currentAccess == "Deny";
            set
            {
                if (value)
                {
                    _currentAccess = "Deny";
                    UpdateRolePermission();
                    NotifyAllPropertiesChanged();
                }
            }
        }

        private void NotifyAllPropertiesChanged()
        {
            OnPropertyChanged(nameof(IsWrite));
            OnPropertyChanged(nameof(IsRead));
            OnPropertyChanged(nameof(IsDeny));
        }

        private void UpdateRolePermission()
        {
            var oldEntry = _role.PermissionSet.FirstOrDefault(p => p.StartsWith(Permission + ":"));
            if (oldEntry != null)
            {
                _role.PermissionSet.Remove(oldEntry);
            }
            _role.PermissionSet.Add($"{Permission}:{_currentAccess}");
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}