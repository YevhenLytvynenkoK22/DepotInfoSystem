using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DepotInfoSystem.Classes;

namespace DepotInfoSystem
{
    public static class AccessManager
    {
        public static User CurrentUser { get; set; }
        public static Role CurrentRole { get; set; }

        public static bool HasPermission(string permission, string accessType = "Write")
        {
            if (CurrentRole == null) return false;
            return CurrentRole.PermissionSet.Any(p =>
                p.StartsWith(permission + ":", StringComparison.OrdinalIgnoreCase) &&
                p.EndsWith(accessType, StringComparison.OrdinalIgnoreCase)
            );
        }
    }

}
