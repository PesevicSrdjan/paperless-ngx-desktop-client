using HCI_2025.Core.Helpers;
using HCI_2025.Core.Interfaces;
using HCI_2025.Core.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCI_2025.ViewModel
{
    public class DocumentsViewModel
    {
        private readonly IDocumentService _documentService;
        private readonly ITagService _tagService;
        private readonly IDocTypeService _docTypeService;
        public Dictionary<int, string> TagsDict { get;  set; } = new();
        public Dictionary<int, string> TypesDict { get; set; } = new();

        public ObservableCollection<Document> Documents { get; set; } = new();
        public DocumentsViewModel(ITagService tagService, IDocTypeService docTypeService,IDocumentService documentService)
        { 
            _documentService = documentService;
            _tagService = tagService;
            _docTypeService = docTypeService;
        }

        public async Task loadDataAsync()
        { 
            var tags = await _tagService.getTagsAsync();
            TagsDict = tags.ToDictionary(t => t.Id, t => t.Name);

            var types = await _docTypeService.getDocTypeAsync();
            TypesDict = types.ToDictionary(t => t.Id, t => t.Name);

            var rawDocs = await _documentService.getAllDocumentsAsync();

            Documents.Clear();

            foreach (var d in rawDocs)
            {
                Documents.Add(new Document
                {
                    Title = d.Title,
                    Type = d.Type.HasValue && TypesDict.ContainsKey(d.Type.Value) ? TypesDict[d.Type.Value] : "Nepoznato",
                    Tags = d.Tags.Select(id => TagsDict.ContainsKey(id) ? TagsDict[id] : "Nepoznato").ToList(),
                    Date = d.Date
                });
            }

        }
    }
}
