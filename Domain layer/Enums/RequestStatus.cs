using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain_layer.Enums
{
    public enum RequestStatus
    {
        Pending = 0,    // waiting
        Accepted = 1,   // accepted
        InProgress = 2, // working on it
        Completed = 3,  // done
        Cancelled = 4  // cancelled
    }
}
