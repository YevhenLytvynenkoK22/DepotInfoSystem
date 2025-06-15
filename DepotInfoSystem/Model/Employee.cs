using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace DepotInfoSystem.Classes
{
    public class Employee : INotifyPropertyChanged
    {
        
        private string name;
        private string surname;
        private string patronymic;
        private string speciality;
        private string photo;

        public int id { get; set; }

        public string Name
        {
            get { return name; }
            set { name = value; OnPropertyChanged(); }
        }

        public string Surname
        {
            get { return surname; }
            set { surname = value; OnPropertyChanged(); }
        }

        public string Patronymic
        {
            get { return patronymic; }
            set { patronymic = value; OnPropertyChanged(); }
        }

        public string Speciality
        {
            get { return speciality; }
            set { speciality = value; OnPropertyChanged(); }
        }

        public string Photo
        {
            get { return photo; }
            set { photo = value; OnPropertyChanged(); OnPropertyChanged(nameof(PhotoImage)); }
        }
        public ImageSource PhotoImage
        {
            get
            {
                if (string.IsNullOrEmpty(Photo))
                    return null;

                string fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Photo);

                if (!File.Exists(fullPath))
                    return null;

                var bitmap = new BitmapImage();

                try
                {
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(fullPath, UriKind.Absolute);
                    bitmap.CacheOption = BitmapCacheOption.OnLoad; 
                    bitmap.EndInit();
                    bitmap.Freeze(); 
                }
                catch
                {
                    return null; 
                }
                

                return bitmap;
            }
        }

        public string FullName => $"{Surname} {Name} {Patronymic}".Trim();

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public override string ToString()
        {
            return $"{FullName} ({Speciality})";
        }
        public string PhotoFullPath =>
        string.IsNullOrEmpty(Photo) ? null : Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Photo);
    }
}
