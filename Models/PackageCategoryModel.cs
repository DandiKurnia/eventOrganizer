using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class PackageCategoryModel
    {
        public Guid PackageCategoryId { get; set; }
        public Guid PackageEventId { get; set; }
        public Guid CategoryId { get; set; }

        // join display
        public string? CategoryName { get; set; }
    }
}
