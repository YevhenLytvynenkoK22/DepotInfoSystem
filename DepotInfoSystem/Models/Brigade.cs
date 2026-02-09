using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace DepotInfoSystem.Classes
{
    public class Brigade : INotifyPropertyChanged
    {
        public int id { get; set; }
        private int employeeID;
        private int repairID;
       
        public int EmployeeID
        {
            get { return employeeID; }
            set { employeeID = value; OnPropertyChanged(); }
        }

        public int RepairID
        {
            get { return repairID; }
            set { repairID = value; OnPropertyChanged(); }
        }
        public virtual Employee Employee { get; set; }
        public virtual Repair Repair { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public override string ToString()
        {
            return $"Brigade Assignment: Employee {EmployeeID} → Repair {RepairID}";
        }
    }
  
}
