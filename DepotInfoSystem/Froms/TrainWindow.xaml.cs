using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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
using Microsoft.Win32;

namespace DepotInfoSystem.Froms
{
    /// <summary>
    /// Логика взаимодействия для TrainWindow.xaml
    /// </summary>
    public partial class TrainWindow : Window
    {
        public Train _train { get; set; }
        public TrainWindow(Train train)
        {
            InitializeComponent();
            _train = train;
            for(int i = 2025; i >= 1950;i-- )
            {
                cb_year.Items.Add(i.ToString());
            }

            if(_train.Photo == null)
            {
                _train.Photo = "pack://application:,,,/Assets/train_image.jpg";
                img_train.Source = new BitmapImage(new Uri("pack://application:,,,/Assets/train_image.jpg"));
            }
            this.DataContext = _train;
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
                imagesDir = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images/Trains");
                extension = System.IO.Path.GetExtension(openFileDialog.FileName);
                newFileName = $"train_{_train.id}_{DateTime.Now:yyyyMMddHHmmss}{extension}";
                destPath = System.IO.Path.Combine(imagesDir, newFileName);
                openFileDialogFileName = openFileDialog.FileName;
                img_train.Source = new BitmapImage(new Uri(openFileDialogFileName));
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
                if (!string.IsNullOrEmpty(_train.Photo))
                {
                    string oldFullPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _train.Photo);
                    DeleteImageSafely(oldFullPath);
                }
                File.Copy(openFileDialogFileName, destPath, overwrite: true);
                string relativePath = System.IO.Path.Combine("Images/Trains", newFileName);
                _train.Photo = relativePath;
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
