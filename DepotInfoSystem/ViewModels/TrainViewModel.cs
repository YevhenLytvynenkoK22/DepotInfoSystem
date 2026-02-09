using DepotInfoSystem.Classes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using System.Windows;

namespace DepotInfoSystem.ViewModels
{
    public class TrainViewModel : INotifyPropertyChanged
    {
        public Train _currentTrain;
        private string _originalPhotoPath;
        private string _selectedLocalImagePath;

        public string Model
        {
            get => _currentTrain.Model;
            set
            {
                if (_currentTrain.Model != value)
                {
                    _currentTrain.Model = value;
                    OnPropertyChanged(nameof(Model));
                    (SaveCommand as RelayCommand)?.RaiseCanExecuteChanged();
                }
            }
        }

        public string Number
        {
            get => _currentTrain.Number;
            set
            {
                if (_currentTrain.Number != value)
                {
                    _currentTrain.Number = value;
                    OnPropertyChanged(nameof(Number));
                    (SaveCommand as RelayCommand)?.RaiseCanExecuteChanged();
                }
            }
        }

        public ObservableCollection<string> AvailableYears { get; set; }
        public string ManufactureYear
        {
            get => _currentTrain.ManufactureYear?.ToString();
            set
            {
                if (int.TryParse(value, out int year))
                {
                    if (!int.TryParse(_currentTrain.ManufactureYear, out int currentYear) || currentYear != year)
                    {
                        _currentTrain.ManufactureYear = year.ToString();
                        OnPropertyChanged(nameof(ManufactureYear));
                        (SaveCommand as RelayCommand)?.RaiseCanExecuteChanged();
                    }
                }
                else if (string.IsNullOrEmpty(value) && _currentTrain.ManufactureYear != null)
                {
                    _currentTrain.ManufactureYear = null;
                    OnPropertyChanged(nameof(ManufactureYear));
                    (SaveCommand as RelayCommand)?.RaiseCanExecuteChanged();
                }
            }
        }


        public ObservableCollection<string> AvailableStatuses { get; set; }
        public string CurrentStatus
        {
            get => _currentTrain.CurrentStatus;
            set
            {
                if (_currentTrain.CurrentStatus != value)
                {
                    _currentTrain.CurrentStatus = value;
                    OnPropertyChanged(nameof(CurrentStatus));
                    (SaveCommand as RelayCommand)?.RaiseCanExecuteChanged();
                }
            }
        }

        public int Mileage
        {
            get
            {
                int mileage = 0;
                return int.TryParse(_currentTrain.Mileage, out mileage) ? int.Parse(_currentTrain.Mileage.ToString()) : mileage;
            }
            set
            {
                _currentTrain.Mileage = value.ToString();
            }
        }


        private BitmapImage _PhotoImageSource;
        public BitmapImage PhotoImageSource
        {
            get => _PhotoImageSource;
            set
            {
                if (_PhotoImageSource != value)
                {
                    _PhotoImageSource = value;
                    OnPropertyChanged(nameof(PhotoImageSource));
                }
            }
        }

        public ICommand SelectPhotoCommand { get; private set; }
        public ICommand SaveCommand { get; private set; }
        public ICommand CancelCommand { get; private set; }

        public event EventHandler RequestClose;
        public bool? DialogResult { get; private set; }

        public TrainViewModel(Train train)
        {
            _currentTrain = train ?? new Train();
            _originalPhotoPath = _currentTrain.Photo;

            AvailableYears = new ObservableCollection<string>();
            for (int i = DateTime.Now.Year; i >= 1950; i--)
            {
                AvailableYears.Add(i.ToString());
            }

            AvailableStatuses = new ObservableCollection<string>
            {
                "In Operation",
                "Needs Decommissioning",
                "Needs Repair",
                "Under Repair",
                "Decommissioned"
            };

            LoadPhotoImage();

            SelectPhotoCommand = new RelayCommand(SelectPhoto);
            SaveCommand = new RelayCommand(SaveTrain, CanSaveTrain);
            CancelCommand = new RelayCommand(Cancel);
        }

        private void LoadPhotoImage()
        {
            if (!string.IsNullOrEmpty(_currentTrain.Photo) && !_currentTrain.Photo.StartsWith("pack://"))
            {
                string fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _currentTrain.Photo);
                if (File.Exists(fullPath))
                {
                    PhotoImageSource = LoadImageFromFile(fullPath);
                }
                else
                {
                    PhotoImageSource = new BitmapImage(new Uri("pack://application:,,,/Assets/train_image.jpg"));
                    _currentTrain.Photo = "pack://application:,,,/Assets/train_image.jpg";
                }
            }
            else if (_currentTrain.Photo?.StartsWith("pack://") == true)
            {
                PhotoImageSource = new BitmapImage(new Uri(_currentTrain.Photo));
            }
            else
            {
                PhotoImageSource = new BitmapImage(new Uri("pack://application:,,,/Assets/train_image.jpg"));
                _currentTrain.Photo = "pack://application:,,,/Assets/train_image.jpg";
            }
        }

        private BitmapImage LoadImageFromFile(string filePath)
        {
            BitmapImage bitmap = new BitmapImage();
            using (FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.StreamSource = stream;
                bitmap.EndInit();
            }
            bitmap.Freeze();
            return bitmap;
        }

        private void SelectPhoto(object parameter)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Image files (*.jpg, *.jpeg, *.png)|*.jpg;*.jpeg;*.png|All files (*.*)|*.*",
                Title = "Select train photo"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                _selectedLocalImagePath = openFileDialog.FileName;
                PhotoImageSource = LoadImageFromFile(_selectedLocalImagePath);
            }
        }

        private bool CanSaveTrain(object parameter)
        {
            return !string.IsNullOrWhiteSpace(Model) &&
                   !string.IsNullOrWhiteSpace(Number) &&
                   ManufactureYear != null &&
                   Mileage >= 0 &&
                   !string.IsNullOrWhiteSpace(CurrentStatus);
        }

        private void SaveTrain(object parameter)
        {
            try
            {
                if (!string.IsNullOrEmpty(_selectedLocalImagePath))
                {
                    string imagesDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images/Trains");
                    if (!Directory.Exists(imagesDir))
                    {
                        Directory.CreateDirectory(imagesDir);
                    }

                    string extension = Path.GetExtension(_selectedLocalImagePath);
                    string newFileName = $"train_{Guid.NewGuid()}{extension}";
                    string destPath = Path.Combine(imagesDir, newFileName);

                    if (!string.IsNullOrEmpty(_originalPhotoPath) && !_originalPhotoPath.StartsWith("pack://"))
                    {
                        string oldFullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _originalPhotoPath);
                        if (File.Exists(oldFullPath) && oldFullPath != destPath)
                        {
                            DeleteImageSafely(oldFullPath);
                        }
                    }

                    File.Copy(_selectedLocalImagePath, destPath, overwrite: true);
                    _currentTrain.Photo = Path.Combine("Images/Trains", newFileName);
                }
                else if (_currentTrain.Photo?.StartsWith("pack://") == true && _currentTrain.Photo == "pack://application:,,,/Assets/train_image.jpg")
                {
                }

                DialogResult = true;
                RequestClose?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving train: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                DialogResult = false;
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
                    GC.Collect();
                    GC.WaitForPendingFinalizers();

                    File.Delete(filePath);
                    return;
                }
                catch (IOException) when (i < maxAttempts - 1)
                {
                    Thread.Sleep(delayMs);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error deleting old photo: {ex.Message}", "File Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    break;
                }
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