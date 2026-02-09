using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace DepotInfoSystem.Classes
{
    public class Role : INotifyPropertyChanged
    {
        public int id { get; set; }
        private string name;
        private string description;
        private string permissions;

        private HashSet<string> permissionSet = new HashSet<string>();
        public string Name
        {
            get => name;
            set
            {
                if (name != value)
                {
                    name = value;
                    OnPropertyChanged(nameof(Name));
                }
            }
        }
        public string Description
        {
            get => description;
            set
            {
                if (description != value)
                {
                    description = value;
                    OnPropertyChanged(nameof(Description));
                }
            }
        }
        public string Permissions
        {
            get => permissions;
            set
            {
                if (permissions != value)
                {
                    permissions = value;
                    SyncPermissionSetFromString();
                    OnPropertyChanged(nameof(Permissions));
                }
            }
        }
        public HashSet<string> PermissionSet
        {
            get => permissionSet;
            set
            {
                if (permissionSet != value)
                {
                    permissionSet = value;
                    SyncPermissionsFromSet();
                    OnPropertyChanged(nameof(PermissionSet));
                }
            }
        }

        public virtual ICollection<User> Users { get; set; }
        private void SyncPermissionSetFromString()
        {
            if (!string.IsNullOrEmpty(permissions))
                permissionSet = new HashSet<string>(permissions.Split(',', (char)StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()));
            else
                permissionSet = new HashSet<string>();
            OnPropertyChanged(nameof(PermissionSet));
        }


        private void SyncPermissionsFromSet()
        {
            permissions = string.Join(",", permissionSet);
            OnPropertyChanged(nameof(Permissions));
        }

        public void AddPermission(string permission)
        {
            if (permissionSet.Add(permission))
                SyncPermissionsFromSet();
        }

        public void RemovePermission(string permission)
        {
            if (permissionSet.Remove(permission))
                SyncPermissionsFromSet();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
