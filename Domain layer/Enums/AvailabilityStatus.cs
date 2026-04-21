using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain_layer.Enums
{
    public enum AvailabilityStatus
    {
        PendingApproval = 0, // Registered but not yet verified by Admin
        Offline = 1, // Approved, but not currently on the site
        Available = 2, // Approved and logged in
        Busy = 3 // Approved and currently working on an order
    }
}
