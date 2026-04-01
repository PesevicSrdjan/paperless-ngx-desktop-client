using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCI_2025_Project_Template.Core.Interfaces
{
    public interface INameDialogViewModel
    {
        string Name { get; set; }
        string Title { get; }
        Task SaveAsync();
        bool Validate(out string error);
    }
}
