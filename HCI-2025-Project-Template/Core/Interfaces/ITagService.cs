using HCI_2025_Project_Template.Core.Models.Api;
using HCI_2025_Project_Template.Core.Models.Responses;
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
        Task<TagResponse> getTagsAsync(int page = 1, int pageSize = 25);
        Task<bool> DeleteTagAsync(int tagId);
        Task<bool> UpdateTagAsync(TagInfo tag);
        Task<Tag?> CreateTagAsync(TagInfo tag);
        Task<List<Tag>> GetAllTagsAsync();
    }
}
