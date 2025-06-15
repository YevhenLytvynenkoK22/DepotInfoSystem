using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using DepotInfoSystem.Classes;

namespace DepotInfoSystem.Froms
{
    /// <summary>
    /// Логика взаимодействия для RepairWindow.xaml
    /// </summary>

    public partial class RepairWindow : Window
    {
        private ApplicationContext db = new ApplicationContext();
        private List<Employee> inBrigade = new List<Employee>();
        private List<Employee> notInBrigade = new List<Employee>();
        public Repair _repair;

        public RepairWindow(Repair repair)
        {
            InitializeComponent();
            _repair = repair ?? new Repair();
            LoadData();
        }

        private void LoadData()
        {
            TrainComboBox.ItemsSource = db.Trains.ToList();
            RepairTypeComboBox.ItemsSource = db.RepairTypes.ToList();
            UserComboBox.ItemsSource = db.Users.ToList();

            // Показываем ID ремонта
            RepairIdTextBox.Text = _repair.id != 0 ? _repair.id.ToString() : "Новий";

            var allEmployees = db.Employees.ToList();

            if (_repair.id != 0)
            {
                TrainComboBox.SelectedValue = _repair.TrainID;
                RepairTypeComboBox.SelectedValue = _repair.TypeID;
                UserComboBox.SelectedValue = _repair.UserID;
                DateTextBox.Text = _repair.Date;
                DescriptionTextBox.Text = _repair.Description;
                StatusTextBox.Text = _repair.Status;

                var brigadeEmployeeIds = db.Brigades
                    .Where(b => b.RepairID == _repair.id)
                    .Select(b => b.EmployeeID)
                    .ToList();

                inBrigade = allEmployees.Where(e => brigadeEmployeeIds.Contains(e.id)).ToList();
                notInBrigade = allEmployees.Where(e => !brigadeEmployeeIds.Contains(e.id)).ToList();
            }
            else
            {
                notInBrigade = new List<Employee>(allEmployees);
                inBrigade = new List<Employee>();
            }

            RefreshBrigadeLists();
        }
        private void AddToBrigade_Click(object sender, RoutedEventArgs e)
        {
            var selected = NotInBrigadeListBox.SelectedItems.Cast<Employee>().ToList();
            foreach (var emp in selected)
            {
                notInBrigade.Remove(emp);
                inBrigade.Add(emp);
            }
            RefreshBrigadeLists();
        }

        private void RemoveFromBrigade_Click(object sender, RoutedEventArgs e)
        {
            var selected = InBrigadeListBox.SelectedItems.Cast<Employee>().ToList();
            foreach (var emp in selected)
            {
                inBrigade.Remove(emp);
                notInBrigade.Add(emp);
            }
            RefreshBrigadeLists();
        }

        private void RefreshBrigadeLists()
        {
            InBrigadeListBox.ItemsSource = null;
            InBrigadeListBox.ItemsSource = inBrigade;
            NotInBrigadeListBox.ItemsSource = null;
            NotInBrigadeListBox.ItemsSource = notInBrigade;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            // Сохраняем ремонт
            _repair.TrainID = (int?)TrainComboBox.SelectedValue ?? 0;
            _repair.TypeID = (int?)RepairTypeComboBox.SelectedValue ?? 0;
            _repair.UserID = (int?)UserComboBox.SelectedValue ?? 0;
            _repair.Date = DateTextBox.Text;
            _repair.Description = DescriptionTextBox.Text;
            _repair.Status = StatusTextBox.Text;

            if (_repair.id == 0)
                db.Repairs.Add(_repair);

            db.SaveChanges();

            // Сохраняем бригаду
            var currentBrigades = db.Brigades.Where(b => b.RepairID == _repair.id).ToList();
            // Удаляем старых
            foreach (var b in currentBrigades)
                db.Brigades.Remove(b);
            db.SaveChanges();

            // Добавляем новых
            foreach (var emp in inBrigade)
            {
                db.Brigades.Add(new Brigade
                {
                    EmployeeID = emp.id,
                    RepairID = _repair.id
                });
            }
            db.SaveChanges();

            DialogResult = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}