using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

// https://gist.github.com/scattered-code/e8c221058d63cfbce3dda867be4707bb
namespace HCI_2025_Project_Template.Core.Services
{
    public class ProgressableStreamContent : HttpContent
    {
        private readonly HttpContent _content;
        private readonly int _bufferSize;
        private readonly IProgress<double>? _progress;

        public ProgressableStreamContent(HttpContent content, int bufferSize, IProgress<double>? progress)
        {
            _content = content ?? throw new ArgumentNullException(nameof(content));
            _bufferSize = bufferSize;
            _progress = progress;

            foreach (var header in _content.Headers)
                Headers.Add(header.Key, header.Value);
        }

        protected override async Task SerializeToStreamAsync(Stream stream, TransportContext? context)
        {
            var buffer = new byte[_bufferSize];
            TryComputeLength(out long size);
            long uploaded = 0;

            using var contentStream = await _content.ReadAsStreamAsync();

            int read;
            while ((read = await contentStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
            {
                await stream.WriteAsync(buffer, 0, read);
                uploaded += read;
                _progress?.Report((uploaded * 100.0) / size);
            }
        }

        protected override bool TryComputeLength(out long length)
        {
            if (_content.Headers.ContentLength.HasValue)
            {
                length = _content.Headers.ContentLength.Value;
                return true;
            }

            length = -1;
            return false;
        }
    }
}
