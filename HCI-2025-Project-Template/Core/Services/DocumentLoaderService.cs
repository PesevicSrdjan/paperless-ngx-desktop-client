using HCI_2025_Project_Template.Core.Interfaces;
using HCI_2025_Project_Template.Core.Models.Ui;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace HCI_2025_Project_Template.Core.Services
{
    public class DocumentLoaderService
    {
        private readonly IDocumentService  _documentService;
        private readonly ITagService _tagService;
        private readonly IDocTypeService _docTypeService;
        private readonly ICorrespondentsService _correspondentsService;
        public Dictionary<int, TagInfo> TagsDict { get; set; } = new();
        public Dictionary<int, DocTypeInfo> TypesDict { get; set; } = new();
        public Dictionary<int, CorrespondentsInfo> CorrespondentsDict { get; set; } = new();

        private readonly Dictionary<int, BitmapImage?> _thumbnailCache = new();

        public DocumentLoaderService()
        {
            _documentService = new DocumentService();
            _tagService = new TagService();
            _docTypeService = new DocTypeService();
            _correspondentsService = new CorrespondentsService();
        }

        public async Task InitializeAsync()
        {
            var tags = await _tagService.getTagsAsync();
            TagsDict = tags.ToDictionary
            (  
                t => t.Id,
                t => new TagInfo
                {
                    Id = t.Id,
                    Name = t.Name,
                    Color = t.TagColor,
                    DocumentCount = t.DocumentCount
                }
            );

            var types = await _docTypeService.getDocTypeAsync();
            TypesDict = types.ToDictionary
            (
                t => t.Id, 
                t => new DocTypeInfo
                {
                    Id = t.Id,
                    Name = t.Name,
                }
            );

            var correspondents = await _correspondentsService.getCorrespondentsAsync();

            CorrespondentsDict = correspondents.ToDictionary
            (
                t => t.Id,
                t => new CorrespondentsInfo
                {
                    Id = t.Id,
                    Name = t.Name
                }
            );
        }

        public async Task<List<Document>> LoadPageAsync(int page, int pageSize, List<int>? tagIds = null, List<int>? typeIds = null,
            List<int>? corrIds = null)
        {
            var rawDocs = await _documentService.getAllDocumentsAsync(page, pageSize, tagIds, typeIds, corrIds);

            if (rawDocs == null || rawDocs.Count == 0)
                return new List<Document>();

            var loadedDocs = new List<Document>();

            foreach (var d in rawDocs)
            {
                if (!_thumbnailCache.TryGetValue(d.Id, out var thumb))
                {
                    thumb = await _documentService.getDocumentThumbAsync(d.Id);
                    _thumbnailCache[d.Id] = thumb;
                }

                loadedDocs.Add(new Document
                {
                    Id = d.Id,
                    Title = d.Title,
                    Type = d.Type.HasValue && TypesDict.ContainsKey(d.Type.Value)
                            ? TypesDict[d.Type.Value].Name
                            : "Nepoznato",
                    Tags = d.Tags.Where(id => TagsDict.ContainsKey(id)).Select(id => TagsDict[id]).ToList(),
                    Date = d.Date,
                    Thumbnail = thumb
                });
            }

            return loadedDocs;
        }

        public async Task<int> GetTotalDocumentsAsync()
        {
            return await _documentService.getOneDocAsync();
        }
    }
}
