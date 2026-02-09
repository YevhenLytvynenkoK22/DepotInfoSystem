using DepotInfoSystem.Classes;
using System.Collections.ObjectModel;
using System.Linq;

namespace DepotInfoSystem.Services
{
    public interface IDataService
    {
        ObservableCollection<Employee> GetEmployees();
        ObservableCollection<Train> GetTrains();
        ObservableCollection<Warehouse> GetWarehouses();
        ObservableCollection<Role> GetRoles();
        ObservableCollection<User> GetUsers();
        ObservableCollection<Repair> GetRepairs();

        void AddEmployee(Employee employee);
        void UpdateEmployee(Employee employee);
        void DeleteEmployee(Employee employee);
        void LoadAllData();
        Role GetRoleById(int roleId);
        void SaveChanges();
    }
}