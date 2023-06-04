// Copyright (c) Wifft 2023
// Wifft licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;
using System.Threading;

using Microsoft.Extensions.Logging;

using OCRStudio.DataModels;
using OCRStudio.Interfaces;

namespace OCRStudio.Services
{
    internal sealed partial class OcrScreenshotService : IScopedProcessingService
    {
        private static readonly DecodedInfo _decodedInfo = new();

        private readonly ILogger<OcrRecorderService> _logger;

        public OcrScreenshotService(ILogger<OcrRecorderService> logger)
        {
            _logger = logger;
        }

        public async Task DoWork(CancellationToken stoppingToken)
        {

        }
    }
}
