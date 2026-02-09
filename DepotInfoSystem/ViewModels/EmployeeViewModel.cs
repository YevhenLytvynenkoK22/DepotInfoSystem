using System;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;
using DepotInfoSystem.Classes;
using DepotInfoSystem.Utilities;

namespace DepotInfoSystem.ViewModels
{
    public class EmployeeViewModel : ObservableObject
    {
        private readonly Window _view;
        public Employee _employee;
        private string _selectedPhotoPath;

        public Employee Employee
        {
            get => _employee;
            set => SetProperty(ref _employee, value);
        }

        public ICommand SelectPhotoCommand { get; private set; }
        public ICommand SaveCommand { get; private set; }
        public ICommand CancelCommand { get; private set; }

        public EmployeeViewModel(Employee employee, Window view)
        {
            _view = view;
            Employee = employee;

            SelectPhotoCommand = new RelayCommand(ExecuteSelectPhoto);
            SaveCommand = new RelayCommand(ExecuteSave, CanExecuteSave);
            CancelCommand = new RelayCommand(ExecuteCancel);

            if (string.IsNullOrEmpty(Employee.Photo))
            {
                Employee.Photo = "pack://application:,,,/Assets/profile_image.jpg";
            }
        }

        private void ExecuteSelectPhoto(object parameter)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Image files (*.jpg, *.jpeg, *.png)|*.jpg;*.jpeg;*.png|All files (*.*)|*.*",
                Title = "Select employee photo"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                _selectedPhotoPath = openFileDialog.FileName;
                Employee.Photo = _selectedPhotoPath;
            }
        }

        private bool CanExecuteSave(object parameter)
        {
            return Employee != null &&
                   !string.IsNullOrWhiteSpace(Employee.Surname) &&
                   !string.IsNullOrWhiteSpace(Employee.Name) &&
                   !string.IsNullOrWhiteSpace(Employee.Speciality);
        }

        private void ExecuteSave(object parameter)
        {
            try
            {
                if (!string.IsNullOrEmpty(_selectedPhotoPath) && _selectedPhotoPath != Employee.PhotoFullPath)
                {
                    string imagesDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images", "Employee");
                    if (!Directory.Exists(imagesDir))
                    {
                        Directory.CreateDirectory(imagesDir);
                    }

                    string extension = Path.GetExtension(_selectedPhotoPath);
                    string newFileName = $"employee_{Employee.id}_{DateTime.Now:yyyyMMddHHmmss}{extension}";
                    string destPath = Path.Combine(imagesDir, newFileName);

                    if (!string.IsNullOrEmpty(Employee.Photo) && !Employee.Photo.StartsWith("pack://application:"))
                    {
                        string oldPhotoFullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Employee.Photo);
                        if (File.Exists(oldPhotoFullPath) && !string.Equals(oldPhotoFullPath, _selectedPhotoPath, StringComparison.OrdinalIgnoreCase))
                        {
                            DeleteImageSafely(oldPhotoFullPath);
                        }
                    }

                    File.Copy(_selectedPhotoPath, destPath, overwrite: true);
                    string relativePath = Path.Combine("Images", "Employee", newFileName);
                    Employee.Photo = relativePath;
                }
                else if (string.IsNullOrEmpty(Employee.Photo) && string.IsNullOrEmpty(_selectedPhotoPath))
                {
                    Employee.Photo = "pack://application:,,,/Assets/profile_image.jpg";
                }

                MessageBox.Show("Employee data saved successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                _view.DialogResult = true;
                _view.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving data: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                _view.DialogResult = false;
            }
        }

        private void ExecuteCancel(object parameter)
        {
            _view.DialogResult = false;
            _view.Close();
        }

        private void DeleteImageSafely(string filePath)
        {
            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath) || filePath.StartsWith("pack://application:")) return;

            const int maxAttempts = 5;
            const int delayMs = 200;

            for (int i = 0; i < maxAttempts; i++)
            {
                try
                {
                    using (FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.None))
                    {
                        stream.Close();
                    }
                    File.Delete(filePath);
                    return;
                }
                catch (IOException ex) when (i < maxAttempts - 1)
                {

                    Thread.Sleep(delayMs);
                }
                catch (Exception ex)
                {
                    throw new IOException($"Unexpected error deleting file: {filePath}", ex);
                }
            }
            throw new IOException($"Failed to delete file after {maxAttempts} attempts: {filePath}");
        }
    }
}