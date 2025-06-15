using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Логика взаимодействия для UserWindow.xaml
    /// </summary>
    public partial class UserWindow : Window
    {
        public User _user { get; set; }
        public ObservableCollection<Role> Roles { get; set; }
        ApplicationContext db = new ApplicationContext();
        public UserWindow(User user)
        {
            InitializeComponent();
            _user = user;
            Roles = new ObservableCollection<Role>(db.Roles.ToList());
            DataContext = this;
            
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
                MessageBox.Show($"Користувача '{_user.Login}' збережено. Роль ID: {_user.RoleID}", "Успіх", MessageBoxButton.OK, MessageBoxImage.Information);
                DialogResult = true;
                Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
