using System.Windows;
using System.Windows.Controls;
using DepotInfoSystem.Classes;

namespace DepotInfoSystem
{
    public class EntityTemplateSelector : DataTemplateSelector
    {
        public DataTemplate EmployeeTemplate { get; set; }
        public DataTemplate TrainTemplate { get; set; }
        public DataTemplate WarehouseTemplate { get; set; }
        public DataTemplate RoleTemplate { get; set; }
        public DataTemplate UserTemplate { get; set; }
        public DataTemplate RepairTemplate { get; set; }
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is Employee)
                return EmployeeTemplate;
            else if (item is Train)
                return TrainTemplate;
            else if (item is Warehouse)
                return WarehouseTemplate;
            else if (item is Role)
                return RoleTemplate;
            else if (item is User)
                return UserTemplate;
            else if (item is Repair)
                return RepairTemplate;
            else
                return base.SelectTemplate(item, container);
        }
    }
}
