using HCI_2025_Project_Template.Core.Interfaces;
using HCI_2025_Project_Template.Core.Models.Ui;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace HCI_2025_Project_Template.Core.Services
{
    public class DocumentLoaderService
    {
        private readonly IDocumentService _documentService;
        private readonly ITagService _tagService;
        private readonly IDocTypeService _docTypeService;
        private readonly ICorrespondentsService _correspondentsService;
        public Dictionary<int, TagInfo> TagsDict { get; set; } = new();
        public Dictionary<int, DocTypeInfo> TypesDict { get; set; } = new();
        public Dictionary<int, CorrespondentsInfo> CorrespondentsDict { get; set; } = new();

        private readonly Dictionary<int, BitmapImage?> _thumbnailCache = new();

        // Default thumbnail pri učitavanju - "pravi" thumbnail stiže postepno - lazy loading.
        private static readonly BitmapImage PlaceholderImage = new BitmapImage(new Uri("pack://application:,,,/Assets/plholder.png"));
        public DocumentLoaderService()
        {
            _documentService = new DocumentService();
            _tagService = new TagService();
            _docTypeService = new DocTypeService();
            _correspondentsService = new CorrespondentsService();
        }

        public async Task<int> GetTotalDocumentsAsync(List<int>? tagIds = null, List<int>? typeIds = null, List<int>? corrIds = null, string? title = null)
        {
            return await _documentService.getTotalDocumentsAsync(tagIds, typeIds, corrIds, title);
        }

        public async Task InitializeAsync()
        {
            var tagsTask = _tagService.getTagsAsync();
            var typesTask = _docTypeService.getDocTypeAsync();
            var corrTask = _correspondentsService.getCorrespondentsAsync();

            await Task.WhenAll(tagsTask, typesTask, corrTask);

            var tags = tagsTask.Result;
            var types = typesTask.Result;
            var correspondents = corrTask.Result;

            TagsDict = tags.ToDictionary(
                t => t.Id,
                t => new TagInfo
                {
                    Id = t.Id,
                    Name = t.Name,
                    Color = t.TagColor,
                    DocumentCount = t.DocumentCount
                }
            );

            TypesDict = types.ToDictionary(
                t => t.Id,
                t => new DocTypeInfo
                {
                    Id = t.Id,
                    Name = t.Name
                }
            );

            CorrespondentsDict = correspondents.ToDictionary(
                t => t.Id,
                t => new CorrespondentsInfo
                {
                    Id = t.Id,
                    Name = t.Name
                }
            );
        }


        public async Task<List<Document>> LoadPageAsync(
            int page, int pageSize,
            List<int>? tagIds = null, 
            List<int>? typeIds = null,
            List<int>? corrIds = null, 
            string? title = null)
        {
            var rawDocs = await _documentService.getAllDocumentsAsync(page, pageSize, tagIds, typeIds, corrIds, title);

            if (rawDocs == null || rawDocs.Count == 0)
                return new List<Document>();

            var docs = rawDocs.Select(d => new Document
            {
                Id = d.Id,
                Title = d.Title,
                Type = d.Type.HasValue && TypesDict.ContainsKey(d.Type.Value)
                       ? TypesDict[d.Type.Value].Name
                       : "Nepoznato",
                Tags = d.Tags.Where(id => TagsDict.ContainsKey(id)).Select(id => TagsDict[id]).ToList(),
                Date = d.Date,
                Thumbnail = PlaceholderImage,
                IsThumbnailLoading = true

            }).ToList();

            _ = LoadThumbnailsAsync(docs);

            return docs;
        }


        public async Task LoadThumbnailsAsync(List<Document> docs)
        {
            // Max 2 paralelnih thumbnaila se učitava - jer želimo da učitavanje dokumenata bude brže.
            var semaphore = new SemaphoreSlim(2); 
            var tasks = docs.Select(async doc =>
            {
                await semaphore.WaitAsync();
                try
                {
                    if (!_thumbnailCache.TryGetValue(doc.Id, out var thumb))
                    {
                        thumb = await _documentService.getDocumentThumbAsync(doc.Id);
                        _thumbnailCache[doc.Id] = thumb;
                    }

                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        doc.Thumbnail = thumb ?? PlaceholderImage;
                        doc.IsThumbnailLoading = false;
                    });
                }
                finally
                {
                    semaphore.Release();
                }
            });

            await Task.WhenAll(tasks);
        }
    }
}
