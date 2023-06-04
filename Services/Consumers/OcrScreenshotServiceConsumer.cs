// Copyright (c) Wifft 2023
// Wifft licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using OCRStudio.Interfaces;
using OCRStudio.Helpers;

namespace OCRStudio.Services.Consumers
{
    internal sealed partial class OcrScreenshotServiceConsumer : BackgroundService, IHostedService
    {
        public IServiceProvider Services { get; }

        public OcrScreenshotServiceConsumer(IServiceProvider services)
        {
            Services = services;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
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
            await TempFolderHelper.ClearTempFolder();

            await base.StopAsync(stoppingToken);
        }
    }
}
