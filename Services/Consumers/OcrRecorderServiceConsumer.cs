// Copyright (c) Wifft 2023
// Wifft licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using Windows.Storage;

using OCRStudio.Interfaces;


namespace OCRStudio.Services.Consumers
{
    internal sealed partial class OcrRecorderServiceConsumer : BackgroundService, IHostedService
    {
        private readonly ILogger<OcrRecorderService> _logger;

        public IServiceProvider Services { get; }

        public OcrRecorderServiceConsumer(IServiceProvider services, ILogger<OcrRecorderService> logger)
        {
            Services = services;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Starting OCR Recorder Service...");

            await DoWork(stoppingToken);
        }

        public async Task DoWork(CancellationToken stoppingToken)
        {
            using IServiceScope scope = Services.CreateScope();
            IScopedProcessingService service = scope.ServiceProvider.GetRequiredService<IScopedProcessingService>();

            await service.DoWork(stoppingToken);
        }

        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Stopping OCR Recorder Service...");
            _logger.LogInformation("Cleaning up temp folder...");

            await ClearTempFolder();

            await base.StopAsync(stoppingToken);
        }

        private static async Task ClearTempFolder()
        {
            IReadOnlyList<StorageFile> tempFolder = await ApplicationData.Current.TemporaryFolder.GetFilesAsync();
            foreach (StorageFile file in tempFolder) await file.DeleteAsync();
        }
    }
}
