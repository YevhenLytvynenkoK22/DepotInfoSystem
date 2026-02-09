using DepotInfoSystem.Classes;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Windows;

namespace DepotInfoSystem.Services
{
    public class DataService : IDataService
    {
        private readonly ApplicationContext _db;

        public DataService(ApplicationContext dbContext)
        {
            _db = dbContext;
        }

        public ObservableCollection<Employee> GetEmployees()
        {
            _db.Employees.Load();
            return _db.Employees.Local;
        }

        public ObservableCollection<Train> GetTrains()
        {
            _db.Trains.Load();
            return _db.Trains.Local;
        }

        public ObservableCollection<Warehouse> GetWarehouses()
        {
            _db.Warehouses.Load();
            return _db.Warehouses.Local;
        }

        public ObservableCollection<Role> GetRoles()
        {
            _db.Roles.Load();
            return _db.Roles.Local;
        }

        public ObservableCollection<User> GetUsers()
        {
            _db.Users.Load();
            return _db.Users.Local;
        }

        public ObservableCollection<Repair> GetRepairs()
        {
            _db.Repairs.Load();
            return _db.Repairs.Local;
        }

        public void AddEmployee(Employee employee)
        {
            _db.Employees.Add(employee);
            SaveChanges();
        }

        public void UpdateEmployee(Employee employee)
        {
            var existing = _db.Employees.Find(employee.id);
            if (existing != null)
            {
                _db.Entry(existing).CurrentValues.SetValues(employee);
            }
            SaveChanges();
        }

        public void DeleteEmployee(Employee employee)
        {
            _db.Employees.Remove(employee);
            SaveChanges();
        }

        public void LoadAllData()
        {
            _db.Employees.Load();
            _db.Trains.Load();
            _db.Warehouses.Load();
            _db.Roles.Load();
            _db.Users.Load();
            _db.Repairs.Load();
        }

        public Role GetRoleById(int roleId)
        {
            return _db.Roles.FirstOrDefault(r => r.id == roleId);
        }

        public void SaveChanges()
        {
            try
            {
                _db.SaveChanges();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Failed to save data: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}