using DepotInfoSystem.Classes;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace DepotInfoSystem.ViewModels
{
    public class WarehouseViewModel : INotifyPropertyChanged
    {
        public Warehouse _currentWarehouseItem;
        private ApplicationContext _dbContext;

        public string ItemNumber
        {
            get => _currentWarehouseItem.Number;
            set
            {
                if (_currentWarehouseItem.Number != value)
                {
                    _currentWarehouseItem.Number = value;
                    OnPropertyChanged(nameof(ItemNumber));
                    (SaveCommand as RelayCommand)?.RaiseCanExecuteChanged();
                }
            }
        }

        public string ItemName
        {
            get => _currentWarehouseItem.Name;
            set
            {
                if (_currentWarehouseItem.Name != value)
                {
                    _currentWarehouseItem.Name = value;
                    OnPropertyChanged(nameof(ItemName));
                    (SaveCommand as RelayCommand)?.RaiseCanExecuteChanged();
                }
            }
        }

        public int ItemCount
        {
            get => _currentWarehouseItem.Count;
            set
            {
                if (_currentWarehouseItem.Count != value)
                {
                    _currentWarehouseItem.Count = value;
                    OnPropertyChanged(nameof(ItemCount));
                    (SaveCommand as RelayCommand)?.RaiseCanExecuteChanged();
                }
            }
        }

        public decimal ItemPrice
        {
            get => (decimal)_currentWarehouseItem.Price;
            set
            {
                if (Convert.ToDecimal(_currentWarehouseItem.Price) != value)
                {
                    _currentWarehouseItem.Price = (double)value;
                    OnPropertyChanged(nameof(ItemPrice));
                    (SaveCommand as RelayCommand)?.RaiseCanExecuteChanged();
                }
            }
        }

        public ICommand SaveCommand { get; private set; }
        public ICommand CancelCommand { get; private set; }

        public event EventHandler RequestClose;
        public bool? DialogResult { get; private set; }

        public WarehouseViewModel(Warehouse warehouse)
        {
            _currentWarehouseItem = warehouse ?? new Warehouse();
            _dbContext = new ApplicationContext();

            SaveCommand = new RelayCommand(SaveWarehouseItem, CanSaveWarehouseItem);
            CancelCommand = new RelayCommand(Cancel);
        }

        private bool CanSaveWarehouseItem(object parameter)
        {
            return !string.IsNullOrWhiteSpace(ItemNumber) &&
                   !string.IsNullOrWhiteSpace(ItemName) &&
                   ItemCount >= 0 &&
                   ItemPrice >= 0;
        }

        private void SaveWarehouseItem(object parameter)
        {
            try
            {
                _currentWarehouseItem.Number = ItemNumber;
                _currentWarehouseItem.Name = ItemName;
                _currentWarehouseItem.Count = ItemCount;
                _currentWarehouseItem.Price = (double)ItemPrice;

                if (_currentWarehouseItem.id == 0)
                {
                    _dbContext.Warehouses.Add(_currentWarehouseItem);
                }

                _dbContext.SaveChanges();

                DialogResult = true;
                RequestClose?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving item: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                DialogResult = false;
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