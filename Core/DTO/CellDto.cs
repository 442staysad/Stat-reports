using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTO
{
    public class CellDto
    {
        public string Value { get; set; } = string.Empty;
        public int RowSpan { get; set; } = 1;
        public int ColSpan { get; set; } = 1;
        public bool IsMerged { get; set; } = false;
    }
}
