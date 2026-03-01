using HCI_2025_Project_Template.Core.Interfaces;
using HCI_2025_Project_Template.Core.Models.Api;
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
        private CancellationTokenSource? _thumbCts;
        private readonly SemaphoreSlim _thumbnailSemaphore = new SemaphoreSlim(2);
        private Task? _currentThumbnailTask;
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

        public async Task<(List<DocumentJson> Results, int Count)> GetDocumentsAsync(
            int page,
            int pageSize,
            List<int>? tagIds = null,
            List<int>? typeIds = null,
            List<int>? corrIds = null,
            string? title = null)
        {
            return await _documentService.GetDocumentsAsync
            (
                page,
                pageSize,
                tagIds,
                typeIds,
                corrIds,
                title
            );
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

        public async Task<(List<Document> Documents, int Count)> LoadPageAsync(
        int page,
        int pageSize,
        List<int>? tagIds = null,
        List<int>? typeIds = null,
        List<int>? corrIds = null,
        string? title = null)
        {
            if (_thumbCts != null)
            {
                _thumbCts.Cancel();
                _thumbCts.Dispose();
            }

            if (_currentThumbnailTask != null && !_currentThumbnailTask.IsCompleted)
            {
                try { await _currentThumbnailTask; } catch { /* ignore */ }
            }

            _thumbCts = new CancellationTokenSource();
            var token = _thumbCts.Token;

            var (rawDocs, totalCount) = await _documentService.GetDocumentsAsync(
                page, pageSize, tagIds, typeIds, corrIds, title
            );

            if (rawDocs == null || rawDocs.Count == 0)
                return (new List<Document>(), totalCount);

            var docs = rawDocs.Select(d => new Document
            {
                Id = d.Id,
                Title = d.Title,
                Type = d.Type.HasValue && TypesDict.ContainsKey(d.Type.Value)
                       ? TypesDict[d.Type.Value].Name
                       : "Nepoznato",
                Tags = d.Tags.Where(id => TagsDict.ContainsKey(id))
                             .Select(id => TagsDict[id])
                             .ToList(),
                Date = d.Date,
                Thumbnail = _thumbnailCache.ContainsKey(d.Id) && _thumbnailCache[d.Id] != null
                           ? _thumbnailCache[d.Id]
                           : PlaceholderImage,
                IsThumbnailLoading = !_thumbnailCache.ContainsKey(d.Id) || _thumbnailCache[d.Id] == null
            }).ToList();

            var docsWithoutThumbnails = docs.Where(d => !_thumbnailCache.ContainsKey(d.Id) || _thumbnailCache[d.Id] == null).ToList();

            if (docsWithoutThumbnails.Any() && !token.IsCancellationRequested)
            {
                _currentThumbnailTask = LoadThumbnailsAsync(docsWithoutThumbnails, token);
            }

            return (docs, totalCount);
        }

        private async Task LoadThumbnailsAsync(List<Document> docs, CancellationToken token)
        {
            try
            {
                if (docs == null || docs.Count == 0 || token.IsCancellationRequested)
                    return;

                var tasks = new List<Task>();

                foreach (var doc in docs)
                {
                    if (token.IsCancellationRequested)
                        break;

                    await _thumbnailSemaphore.WaitAsync(token);

                    tasks.Add(Task.Run(async () =>
                    {
                        try
                        {
                            if (token.IsCancellationRequested)
                                return;

                            BitmapImage? thumb = null;
                            try
                            {
                                thumb = await _documentService.getDocumentThumbAsync(doc.Id);
                            }
                            catch (Exception ex)
                            {
                                System.Diagnostics.Debug.WriteLine($"Greška pri učitavanju thumbnails za dokument {doc.Id}: {ex.Message}");
                            }

                            if (token.IsCancellationRequested)
                                return;

                            lock (_thumbnailCache)
                            {
                                _thumbnailCache[doc.Id] = thumb;
                            }

                            await Application.Current.Dispatcher.InvokeAsync(() =>
                            {
                                if (!token.IsCancellationRequested)
                                {
                                    doc.Thumbnail = thumb ?? PlaceholderImage;
                                    doc.IsThumbnailLoading = false;
                                }
                            });
                        }
                        finally
                        {
                            _thumbnailSemaphore.Release();
                        }
                    }, token));
                }

                await Task.WhenAll(tasks);
            }
            catch (OperationCanceledException)
            {
                System.Diagnostics.Debug.WriteLine("Učitavanje thumbnails je otkazano.");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Greška u LoadThumbnailsAsync: {ex.Message}");
            }
        }
    }
}

