// Copyright (c) Wifft 2023
// Wifft licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using WifftOCR.Interfaces;

namespace WifftOCR.Services.Consumers
{
    internal sealed partial class OcrServiceConsumer : BackgroundService
    {
        private readonly ILogger<OcrService> _logger;

        public IServiceProvider Services { get; }

        public OcrServiceConsumer(IServiceProvider services, ILogger<OcrService> logger)
        {
            Services = services;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("OCR service started.");

            await DoWork(stoppingToken);
        }

        private async Task DoWork(CancellationToken stoppingToken)
        {
            using IServiceScope scope = Services.CreateScope();
            IScopedProcessingService service = scope.ServiceProvider.GetRequiredService<IScopedProcessingService>();

            await service.DoWork(stoppingToken);
        }

        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("OCR service stopping...");

            //ToDo: Add logic when service stops here.

            await base.StopAsync(stoppingToken);
        }
    }
}
