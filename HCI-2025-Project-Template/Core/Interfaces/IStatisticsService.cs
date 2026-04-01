using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HCI_2025_Project_Template.Core.Models.Api;

namespace HCI_2025_Project_Template.Core.Interfaces
{
    public interface IStatisticsService
    {
        Task<Statistics> GetStatisticsAsync();
    }
}
