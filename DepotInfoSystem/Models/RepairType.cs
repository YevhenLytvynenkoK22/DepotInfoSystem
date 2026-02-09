using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace DepotInfoSystem.Classes
{
    public class RepairType : INotifyPropertyChanged
    {
        public int id { get; set; }
        private string name;

     

        public string Name
        {
            get { return name; }
            set { name = value; OnPropertyChanged(); }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public override string ToString()
        {
            return Name ?? "Unnamed Repair Type";
        }
    }
}
