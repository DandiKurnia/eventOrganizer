using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class DashboardViewModel
    {
        // Order Statistics
        public int TotalOrders { get; set; }
        public int WaitingValidation { get; set; }
        public int VendorConfirmation { get; set; }
        public int BookingConfirmed { get; set; }
        public int UpcomingEvents { get; set; }

        // Revenue
        public decimal TotalRevenue { get; set; }

        // Vendor Statistics
        public int TotalVendors { get; set; }
        public int ActiveVendors { get; set; }
        public int VendorPendingConfirmation { get; set; }
        public int VendorAccepted { get; set; }
        public int VendorRejected { get; set; }

        // Package Statistics
        public int TotalPackages { get; set; }
        public int ActivePackages { get; set; }
        public int InactivePackages { get; set; }

        // Category Statistics
        public int TotalCategories { get; set; }

        // User Statistics
        public int TotalUsers { get; set; }
        public int ActiveUsers { get; set; }
        public int AdminUsers { get; set; }
        public int VendorUsers { get; set; }
        public int CustomerUsers { get; set; }

        // Top Categories (for display)
        public List<CategoryStat> TopCategories { get; set; } = new();
    }

    public class CategoryStat
    {
        public string CategoryName { get; set; } = string.Empty;
        public int UsageCount { get; set; }
    }
}
