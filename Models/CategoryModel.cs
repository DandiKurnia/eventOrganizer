using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class CategoryModel
    {
        public Guid CategoryId { get; set; }
        public string CategoryName { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
    }
}

