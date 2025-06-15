using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
using System.Windows.Shapes;
using DepotInfoSystem.Classes;

namespace DepotInfoSystem.Froms
{
    /// <summary>
    /// Логика взаимодействия для RoleWindow.xaml
    /// </summary>
    public partial class RoleWindow : Window, INotifyPropertyChanged
    {
        public Role _role { get; set; }
        public static ObservableCollection<PermissionViewModel> PermissionViewModels { get; set; }

        public RoleWindow(Role role)
        {
            InitializeComponent();
            _role = role;

            PermissionViewModels = new ObservableCollection<PermissionViewModel>(
                new[] { "Trains", "Employee", "Repairs", "Warehouse", "Roles", "Users" }
                .Select(p => new PermissionViewModel(p, _role)));

            DataContext = this;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            _role.Name = RoleNameTextBox.Text;
            _role.Description = RoleDescriptionTextBox.Text;
            MessageBox.Show($"Роль '{_role.Name}' сохранена.", "Сохранение", MessageBoxButton.OK, MessageBoxImage.Information);
            DialogResult = true;
            Close();
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public class PermissionViewModel : INotifyPropertyChanged
        {
            public string Permission { get; }
            private string currentAccess;
            private Role role;

            public PermissionViewModel(string permission, Role role)
            {
                Permission = permission;
                this.role = role;
                currentAccess = GetAccessFromRole();
            }

            public bool IsWrite
            {
                get => currentAccess == "Write";
                set
                {
                    if (value)
                    {
                        currentAccess = "Write";
                        UpdateRole();
                        NotifyAll();
                    }
                }
            }

            public bool IsRead
            {
                get => currentAccess == "Read";
                set
                {
                    if (value)
                    {
                        currentAccess = "Read";
                        UpdateRole();
                        NotifyAll();
                    }
                }
            }

            public bool IsDeny
            {
                get => currentAccess == "Deny" || string.IsNullOrEmpty(currentAccess);
                set
                {
                    if (value)
                    {
                        currentAccess = "Deny";
                        UpdateRole();
                        NotifyAll();
                    }
                }
            }

            private void NotifyAll()
            {
                OnPropertyChanged(nameof(IsWrite));
                OnPropertyChanged(nameof(IsRead));
                OnPropertyChanged(nameof(IsDeny));
            }

            private string GetAccessFromRole()
            {
                foreach (var p in role.PermissionSet)
                {
                    if (p.StartsWith(Permission + ":", StringComparison.OrdinalIgnoreCase))
                        return p.Split(':')[1];
                }
                return "Deny";
            }

            private void UpdateRole()
            {
                var old = role.PermissionSet.FirstOrDefault(p => p.StartsWith(Permission + ":"));
                if (old != null)
                    role.PermissionSet.Remove(old);
                role.PermissionSet.Add($"{Permission}:{currentAccess}");
                role.Permissions = string.Join(",", role.PermissionSet);
            }

            public event PropertyChangedEventHandler PropertyChanged;
            protected virtual void OnPropertyChanged(string propertyName) =>
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

}
