using HCI_2025_Project_Template.Core.Interfaces;
using HCI_2025_Project_Template.Core.Models.Ui;
using HCI_2025_Project_Template.Core.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using static HCI_2025_Project_Template.Core.Models.Ui.FileMetadata;

namespace HCI_2025_Project_Template.ViewModels
{
    public class MetaDataViewModel
    {
        public ObservableCollection<FileMetadata> FilesMetadata { get; set; } = new();
        public ObservableCollection<TagInfo> TagsList { get; set; } = new();
        public ObservableCollection<DocTypeInfo> TypesList { get; set; } = new();
        public ObservableCollection<CorrespondentsInfo> CorrespondentsList { get; set; } = new();

        private readonly DocumentLoaderService _docLoader;
        private readonly IDocumentService _documentService;
        public MetaDataViewModel(string[] filePaths)
        {
            _docLoader = new DocumentLoaderService();
            _documentService = new DocumentService();

            foreach (var file in filePaths)
            {
                FilesMetadata.Add(new FileMetadata
                {
                    FilePath = file,
                    DocumentName = System.IO.Path.GetFileNameWithoutExtension(file),
                    DocumentDate = null
                });
            }
        }
        public async Task InitializeAsync()
        {
            await _docLoader.InitializeAsync();

            TagsList.Clear();
            foreach (var tag in _docLoader.TagsDict.Values)
                TagsList.Add(tag);

            TypesList.Clear();
            foreach (var type in _docLoader.TypesDict.Values)
                TypesList.Add(type);

            CorrespondentsList.Clear();
            foreach (var corr in _docLoader.CorrespondentsDict.Values)
                CorrespondentsList.Add(corr);
        }

        public async Task PrepareFilesAsync()
        {
            foreach (var file in FilesMetadata)
            {
                if (!System.IO.File.Exists(file.FilePath))
                {
                    file.Stage = FileMetadata.FileStage.FailedInPreparing;
                    file.UploadProgress = 0;
                    continue;
                }


                file.Stage = FileStage.Waiting;
                file.UploadProgress = 0;

                for (int i = 1; i <= 100; i += 10)
                {
                    await Task.Delay(150);
                    file.UploadProgress = i;
                }

                file.Stage = FileStage.ReadyToUpload;
                file.UploadProgress = 100;
            }
        }
        public async Task<bool> UploadAllAsync(List<FileMetadata> files)
        {
            bool allSuccess = true;

            foreach (var file in files)
            {
                var progress = new Progress<double>();
                progress.ProgressChanged += (s, p) => file.UploadProgress = p;

                var taskId = await _documentService.UploadDocumentAsync(file, progress);

                if (taskId != null)
                {
                    file.Stage = FileStage.Uploaded;
                    file.UploadProgress = 100;
                }
                else
                {
                    file.Stage = FileStage.Failed;
                    allSuccess = false;
                }
            }
            return allSuccess;
        }

        public bool AreAllFilesValid(out List<FileMetadata> invalidFiles)
        {
            invalidFiles = new List<FileMetadata>();

            foreach (var file in FilesMetadata)
            {
                if (string.IsNullOrWhiteSpace(file.DocumentName) ||
                    file.DocumentType == null ||
                    !file.DocumentDate.HasValue)
                {
                    invalidFiles.Add(file);
                }
            }
            return invalidFiles.Count == 0;
        }
    }
}
