using DepotInfoSystem.Classes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using DepotInfoSystem.Utilities;

namespace DepotInfoSystem.ViewModels
{
    public class RepairViewModel : INotifyPropertyChanged
    {
        private ApplicationContext _dbContext;
        public Repair _currentRepair;

        public string RepairId => _currentRepair.id != 0 ? _currentRepair.id.ToString() : "New";

        private int _selectedTrainId;
        public int SelectedTrainId
        {
            get => _selectedTrainId;
            set
            {
                if (_selectedTrainId != value)
                {
                    _selectedTrainId = value;
                    OnPropertyChanged(nameof(SelectedTrainId));
                    _currentRepair.TrainID = value;
                }
            }
        }

        private int _selectedRepairTypeId;
        public int SelectedRepairTypeId
        {
            get => _selectedRepairTypeId;
            set
            {
                if (_selectedRepairTypeId != value)
                {
                    _selectedRepairTypeId = value;
                    OnPropertyChanged(nameof(SelectedRepairTypeId));
                    _currentRepair.TypeID = value;
                }
            }
        }

        private int _selectedUserId;
        public int SelectedUserId
        {
            get => _selectedUserId;
            set
            {
                if (_selectedUserId != value)
                {
                    _selectedUserId = value;
                    OnPropertyChanged(nameof(SelectedUserId));
                    _currentRepair.UserID = value;
                }
            }
        }

        private DateTime? _repairDate;
        public DateTime? RepairDate
        {
            get => _repairDate;
            set
            {
                if (_repairDate != value)
                {
                    _repairDate = value;
                    OnPropertyChanged(nameof(RepairDate));
                    _currentRepair.Date = value?.ToShortDateString();
                }
            }
        }

        private string _repairDescription;
        public string RepairDescription
        {
            get => _repairDescription;
            set
            {
                if (_repairDescription != value)
                {
                    _repairDescription = value;
                    OnPropertyChanged(nameof(RepairDescription));
                    _currentRepair.Description = value;
                }
            }
        }

        private string _repairStatus;
        public string RepairStatus
        {
            get => _repairStatus;
            set
            {
                if (_repairStatus != value && value.IsNormalized())
                {
                    value = value.Substring(value.IndexOf(' ') >= 0 ? value.IndexOf(' ') : 0);
                    _repairStatus = value;
                    OnPropertyChanged(nameof(RepairStatus));
                    _currentRepair.Status = value;
                }
            }
        }

        public ObservableCollection<Train> AvailableTrains { get; set; }
        public ObservableCollection<RepairType> AvailableRepairTypes { get; set; }
        public ObservableCollection<User> AvailableUsers { get; set; }

        public ObservableCollection<Employee> EmployeesInBrigade { get; set; }
        public ObservableCollection<Employee> EmployeesNotInBrigade { get; set; }

        public ICommand AddToBrigadeCommand { get; private set; }
        public ICommand RemoveFromBrigadeCommand { get; private set; }
        public ICommand SaveCommand { get; private set; }
        public ICommand CancelCommand { get; private set; }

        public event EventHandler RequestClose;
        public bool? DialogResult { get; private set; }

        public RepairViewModel(Repair repair)
        {
            _dbContext = new ApplicationContext();
            _currentRepair = repair ?? new Repair();

            AvailableTrains = new ObservableCollection<Train>();
            AvailableRepairTypes = new ObservableCollection<RepairType>();
            AvailableUsers = new ObservableCollection<User>();
            EmployeesInBrigade = new ObservableCollection<Employee>();
            EmployeesNotInBrigade = new ObservableCollection<Employee>();

            AddToBrigadeCommand = new RelayCommand(AddToBrigade, CanExecuteBrigadeCommand);
            RemoveFromBrigadeCommand = new RelayCommand(RemoveFromBrigade, CanExecuteBrigadeCommand);
            SaveCommand = new RelayCommand(SaveRepair);
            CancelCommand = new RelayCommand(Cancel);

            LoadData();
            SetInitialValues();
        }

        private void LoadData()
        {
            _dbContext.Trains.ToList().ForEach(AvailableTrains.Add);
            _dbContext.RepairTypes.ToList().ForEach(AvailableRepairTypes.Add);
            _dbContext.Users.ToList().ForEach(AvailableUsers.Add);

            var allEmployees = _dbContext.Employees.ToList();

            if (_currentRepair.id != 0)
            {
                var brigadeEmployeeIds = _dbContext.Brigades
                    .Where(b => b.RepairID == _currentRepair.id)
                    .Select(b => b.EmployeeID)
                    .ToList();

                allEmployees.Where(e => brigadeEmployeeIds.Contains(e.id)).ToList().ForEach(EmployeesInBrigade.Add);
                allEmployees.Where(e => !brigadeEmployeeIds.Contains(e.id)).ToList().ForEach(EmployeesNotInBrigade.Add);
            }
            else
            {
                allEmployees.ForEach(EmployeesNotInBrigade.Add);
            }
        }

        private void SetInitialValues()
        {
            SelectedTrainId = _currentRepair.TrainID;
            SelectedRepairTypeId = _currentRepair.TypeID;
            SelectedUserId = _currentRepair.UserID;

            if (DateTime.TryParse(_currentRepair.Date, out DateTime date))
            {
                RepairDate = date;
            }
            else
            {
                RepairDate = DateTime.Now;
            }

            RepairDescription = _currentRepair.Description;
            RepairStatus = !string.IsNullOrEmpty(_currentRepair.Status) ? _currentRepair.Status : "In Progress";
        }

        private void AddToBrigade(object parameter)
        {
            var selected = (parameter as System.Collections.IList)?.Cast<Employee>().ToList();
            if (selected == null || !selected.Any()) return;

            foreach (var emp in selected)
            {
                EmployeesNotInBrigade.Remove(emp);
                EmployeesInBrigade.Add(emp);
            }
            CommandManager.InvalidateRequerySuggested();
        }

        private void RemoveFromBrigade(object parameter)
        {
            var selected = (parameter as System.Collections.IList)?.Cast<Employee>().ToList();
            if (selected == null || !selected.Any()) return;

            foreach (var emp in selected)
            {
                EmployeesInBrigade.Remove(emp);
                EmployeesNotInBrigade.Add(emp);
            }
            CommandManager.InvalidateRequerySuggested();
        }

        private bool CanExecuteBrigadeCommand(object parameter)
        {
            var selected = (parameter as System.Collections.IList);
            return selected != null && selected.Count > 0;
        }


        private void SaveRepair(object parameter)
        {
            _currentRepair.TrainID = SelectedTrainId;
            _currentRepair.TypeID = SelectedRepairTypeId;
            _currentRepair.UserID = SelectedUserId;
            _currentRepair.Date = RepairDate?.ToShortDateString();
            _currentRepair.Description = RepairDescription;
            _currentRepair.Status = RepairStatus;

            if (_currentRepair.id == 0)
            {
                _dbContext.Repairs.Add(_currentRepair);
            }

            _dbContext.SaveChanges();

            var currentBrigades = _dbContext.Brigades.Where(b => b.RepairID == _currentRepair.id).ToList();
            _dbContext.Brigades.RemoveRange(currentBrigades);
            _dbContext.SaveChanges();

            foreach (var emp in EmployeesInBrigade)
            {
                _dbContext.Brigades.Add(new Brigade
                {
                    EmployeeID = emp.id,
                    RepairID = _currentRepair.id
                });
            }
            _dbContext.SaveChanges();

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
            CommandManager.InvalidateRequerySuggested();
        }
    }
}