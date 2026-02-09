using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;
namespace DepotInfoSystem.Classes
{
    

    public class Repair : INotifyPropertyChanged
    {
        public int id { get; set; }
        private int trainID;
        private string date;
        private int typeID;
        private int brigadaID;
        private string description;
        private int userID;
        private string status;

    

        public int TrainID
        {
            get { return trainID; }
            set { trainID = value; OnPropertyChanged(); }
        }

        public string Date
        {
            get { return date; }
            set { date = value; OnPropertyChanged(); }
        }

        public int TypeID
        {
            get { return typeID; }
            set { typeID = value; OnPropertyChanged(); }
        }

        public int BrigadaID
        {
            get { return brigadaID; }
            set { brigadaID = value; OnPropertyChanged(); }
        }

        public string Description
        {
            get { return description; }
            set { description = value; OnPropertyChanged(); }
        }

        public int UserID
        {
            get { return userID; }
            set { userID = value; OnPropertyChanged(); }
        }

        public string Status
        {
            get { return status; }
            set { status = value; OnPropertyChanged(); }
        }


        public virtual ICollection<Brigade> Brigades { get; set; } = new List<Brigade>();
        public virtual Train Train { get; set; }
        public RepairType Type { get; set; }
        public User User { get; set; }
      

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public override string ToString()
        {
            return $"Repair #{id} - {Status}";
        }
    }
}
