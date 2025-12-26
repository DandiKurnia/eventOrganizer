using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class VendorCategoryModel
    {
        public Guid VendorCategoryId { get; set; }
        public Guid VendorId { get; set; }
        public Guid CategoryId { get; set; }

        // JOIN display
        public string? CategoryName { get; set; }
    }
}