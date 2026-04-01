using HCI_2025_Project_Template.Core.Models.Api;
using HCI_2025_Project_Template.Core.Models.Responses;
using HCI_2025_Project_Template.Core.Models.Ui;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace HCI_2025_Project_Template.Core.Interfaces
{
    public interface IDocumentService
    {
        Task<bool?> DownloadDocumentAsync(Document doc);
        Task<BitmapImage?> getDocumentThumbAsync(int idDoc);

        Task<byte[]> GetDocumentPreviewAsync(int documentId);

        Task<DocumentsResponse> GetDocumentsAsync(
            int page = 1,
            int pageSize = 50,
            List<int>? tagIds = null,
            List<int>? typeIds = null,
            List<int>? corrIds = null,
            string? title = null);

        Task<string?> UploadDocumentAsync(FileMetadata file, IProgress<double>? progress = null);
        Task<bool> UpdateDocMetadataAsync(int documentId, DocumentUpdateRequest request);
    }
}
