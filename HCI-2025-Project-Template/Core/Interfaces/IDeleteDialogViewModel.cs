using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCI_2025_Project_Template.Core.Interfaces
{
    public interface IDeleteDialogViewModel
    {
        Task DeleteAsync();
        string DeleteTitle { get; }
        string DeleteSubtitle { get; }
    }
}
