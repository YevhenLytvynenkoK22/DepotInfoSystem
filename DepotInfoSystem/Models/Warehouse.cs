using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace DepotInfoSystem.Classes
{
    public class Warehouse : INotifyPropertyChanged
    {
        public int id { get; set; }
        private string name;
        private int count;
        private double price;
        private string number;

      

        public string Name
        {
            get { return name; }
            set { name = value; OnPropertyChanged(); }
        }

        public int Count
        {
            get { return count; }
            set { count = value; OnPropertyChanged(); }
        }

        public double Price
        {
            get { return price; }
            set { price = value; OnPropertyChanged(); }
        }

        public string Number
        {
            get { return number; }
            set { number = value; OnPropertyChanged(); }
        }
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
