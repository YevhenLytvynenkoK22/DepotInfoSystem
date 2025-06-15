using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace DepotInfoSystem.Classes
{
    public class Train : INotifyPropertyChanged
    {
        public int id { get; set; }
        private string model;
        private string number;
        private string manufactureYear;
        private string mileage;
        private string currentStatus;
        private string photo;

        public string Model
        {
            get => model;
            set { model = value; OnPropertyChanged(); }
        }

        public string Number
        {
            get => number;
            set { number = value; OnPropertyChanged(); }
        }

        public string ManufactureYear
        {
            get => manufactureYear;
            set { manufactureYear = value; OnPropertyChanged(); }
        }

        public string Mileage
        {
            get => mileage;
            set { mileage = value; OnPropertyChanged(); }
        }

        public string CurrentStatus
        {
            get => currentStatus;
            set { currentStatus = value; OnPropertyChanged(); }
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
        public string PhotoFullPath =>
       string.IsNullOrEmpty(Photo) ? null : Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Photo);
        public string FullInfo => $"{Model} #{Number} ({ManufactureYear}) - {CurrentStatus}";

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public override string ToString()
        {
            return $"{Model} #{Number}";
        }
    }
}
