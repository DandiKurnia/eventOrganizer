using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class DashboardViewModel
    {
        public int TotalOrders { get; set; }
        public int WaitingValidation { get; set; }
        public int VendorConfirmation { get; set; }
        public int BookingConfirmed { get; set; }

        public decimal TotalRevenue { get; set; }

        public int ActiveVendors { get; set; }
        public int UpcomingEvents { get; set; }
    }
}
