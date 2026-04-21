using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain_layer.Enums
{
    public enum NotificationType
    {
        NewRequest = 0, // For worker only
        RequestAccepted = 1, // For customer
        ServiceCompleted = 2 // For customer
    }
}
