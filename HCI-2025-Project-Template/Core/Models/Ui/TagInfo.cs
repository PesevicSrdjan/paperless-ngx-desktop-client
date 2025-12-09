using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace HCI_2025_Project_Template.Core.Models.Ui
{
    public class TagInfo
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Color { get; set; }
        public int DocumentCount { get; set; } = 0;


    }
}
