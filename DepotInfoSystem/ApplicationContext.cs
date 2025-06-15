using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DepotInfoSystem.Classes;

namespace DepotInfoSystem
{
    public class ApplicationContext : DbContext
    {
        public DbSet<Brigade> Brigades { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<RepairType> RepairTypes { get; set; }
        public DbSet<Repair> Repairs { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Spare> Spares { get; set; }
        public DbSet<Train> Trains { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Warehouse> Warehouses { get; set; }

        public ApplicationContext() : base("DefaultConnection")
        {

        }
    }
}
