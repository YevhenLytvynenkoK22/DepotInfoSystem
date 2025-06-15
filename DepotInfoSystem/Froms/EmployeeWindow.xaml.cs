using System;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Media.Imaging;
using DepotInfoSystem.Classes;
using Microsoft.Win32;

namespace DepotInfoSystem.Froms
{
    /// <summary>
    /// Логика взаимодействия для EmployeeWindow.xaml
    /// </summary>
    public partial class EmployeeWindow : Window
    {
        public Employee _employee { get; set; }
        public EmployeeWindow(Employee employee)
        {
            InitializeComponent();
            _employee = employee;
            if (_employee.Photo == null)
            {
                _employee.Photo = "pack://application:,,,/Assets/profile_image.jpg";
                img_employee.Source = new BitmapImage(new Uri("pack://application:,,,/Assets/profile_image.jpg"));
            }
            this.DataContext = _employee;
        }

        string imagesDir;
        string extension;
        string newFileName;
        string destPath;
        string openFileDialogFileName;
        private void Button_Select_Image_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Image files (*.jpg, *.jpeg, *.png)|*.jpg;*.jpeg;*.png|All files (*.*)|*.*",
                Title = "Оберіть фото працівника"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                imagesDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images/Employee");
                extension = Path.GetExtension(openFileDialog.FileName);
                newFileName = $"employee_{_employee.id}_{DateTime.Now:yyyyMMddHHmmss}{extension}";
                destPath = Path.Combine(imagesDir, newFileName);
                openFileDialogFileName = openFileDialog.FileName;
                img_employee.Source = new BitmapImage(new Uri(openFileDialogFileName));
            }
        }
        private void DeleteImageSafely(string filePath)
        {
            if (!File.Exists(filePath)) return;

            const int maxAttempts = 5;
            const int delayMs = 200;

            for (int i = 0; i < maxAttempts; i++)
            {
                try
                {
                    File.Delete(filePath);
                    return; 
                }
                catch (IOException) when (i < maxAttempts - 1)
                {
                    Thread.Sleep(delayMs); 
                }
            }
            throw new IOException($"Не вдалося видалити файл: {filePath}");
        }

        private void btn_save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(_employee.Photo))
                {
                    string oldFullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _employee.Photo);
                    DeleteImageSafely(oldFullPath);
                }
                File.Copy(openFileDialogFileName, destPath, overwrite: true);
                string relativePath = Path.Combine("Images/Employee", newFileName);
                _employee.Photo = relativePath;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка при оновленні Фотографії: {ex.Message}",
                              "Помилка",
                              MessageBoxButton.OK,
                              MessageBoxImage.Error);
            }
            this.DialogResult = true;
            this.Close();
        }

        private void btn_cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
