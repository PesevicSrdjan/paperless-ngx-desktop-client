using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCI_2025_Project_Template.Core.Models.Ui
{
    public class FileTypeItem
    {
        public string Name { get; set; }
        public int Count { get; set; } 
        public double Percentage { get; set; } 
        public double Width => Percentage * 3;
        public string Color { get; set; }
    }
}
