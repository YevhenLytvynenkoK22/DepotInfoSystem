using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
namespace DepotInfoSystem.Classes
{
    public class User : INotifyPropertyChanged
    {
        public int id { get; set; }
        private string login;
        private string password;
        private int roleID;
        public string Login
        {
            get { return login; }
            set { login = value; OnPropertyChanged(); }
        }

        public string Password
        {
            get { return password; }
            set { password = value; OnPropertyChanged(); }
        }

        public int RoleID
        {
            get { return roleID; }
            set { roleID = value; OnPropertyChanged(); }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

}
