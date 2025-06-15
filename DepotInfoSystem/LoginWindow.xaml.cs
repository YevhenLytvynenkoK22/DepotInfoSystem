using System;
using System.Collections.Generic;
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
using DepotInfoSystem.Froms;

namespace DepotInfoSystem
{
    /// <summary>
    /// Логика взаимодействия для LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        private ApplicationContext db;
        public LoginWindow()
        {
            InitializeComponent();
            db = new ApplicationContext();
        }
        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string login = LoginTextBox.Text.Trim();
            string password = PasswordBox.Password.Trim();

            if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Введіть логін і пароль.", "Помилка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            var user = db.Users.FirstOrDefault(u => u.Login == login && u.Password == password);
            if (user != null)
            {
               
                MainWindow mainWindow = new MainWindow(user);
                mainWindow.Show();
                Close();
            }
            else
            {
                MessageBox.Show("Невірний логін або пароль.", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
