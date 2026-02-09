using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace DepotInfoSystem.Classes
{
    public class Spare : INotifyPropertyChanged
    {
        public int id { get; set; }
        private int count;
        private int spareID;
        private int remontID;

        

        public int Count
        {
            get { return count; }
            set { count = value; OnPropertyChanged(); }
        }

        public int SpareID
        {
            get { return spareID; }
            set { spareID = value; OnPropertyChanged(); }
        }

        public int RemontID
        {
            get { return remontID; }
            set { remontID = value; OnPropertyChanged(); }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public override string ToString()
        {
            return $"Spare Usage: {Count} units of #{SpareID} for repair #{RemontID}";
        }
    }
}
