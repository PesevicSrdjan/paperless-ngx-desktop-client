using HCI_2025_Project_Template.Core.Models.Api;
using HCI_2025_Project_Template.Core.Models.Ui;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCI_2025_Project_Template.Core.Interfaces
{
    public interface ITagService
    {
        Task<List<Tag>> getTagsAsync();
        Task<bool> DeleteTagAsync(int tagId);
        Task<bool> UpdateTagAsync(TagInfo tag);
        Task<Tag?> CreateTagAsync(TagInfo tag);
    }
}
