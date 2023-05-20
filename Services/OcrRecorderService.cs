// Copyright (c) Wifft 2023
// Wifft licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using Windows.Globalization;
using Windows.Graphics.Imaging;
using Windows.Media.Ocr;
using Windows.Storage;

using OCRStudio.Clients;
using OCRStudio.DataModels;
using OCRStudio.Helpers;
using OCRStudio.Interfaces;

namespace OCRStudio.Services
{
    internal sealed partial class OcrRecorderService : IScopedProcessingService
    {
        private static readonly DecodedInfo _decodedInfo = new();
        
        private readonly ILogger<OcrRecorderService> _logger;

        public OcrRecorderService(ILogger<OcrRecorderService> logger)
        { 
            _logger = logger;
        }

        public async Task DoWork(CancellationToken stoppingToken)
        {
            _logger.LogInformation("OCR service started.");

            while (!stoppingToken.IsCancellationRequested) {
                try {
                    await GetOcrResultsAsync().ContinueWith(task => ProcessResultsAsync(task.Result, _logger), stoppingToken);
                    await Task.Delay(1000, stoppingToken);
                } catch (TaskCanceledException) {
                    _logger.LogInformation("OCR Service stopped.");
                } catch (Exception e) {
                    _logger.LogError(e.Message);
                    _logger.LogError(e.StackTrace);
                }
            }
        }

        private static async Task<List<OcrResult>> GetOcrResultsAsync()
        {
            Language language = new("es");
            if (!OcrEngine.IsLanguageSupported(language)) 
                throw new Exception($"{language.LanguageTag} is not supported in this system.");

            List<OcrResult> results = new();

            Settings settings = await SettingsService.ReadSettingsForOcrServiceAsync() ?? throw new Exception("Settings file is null!");
            List<CaptureArea> captureAreas = settings.CaptureAreas.Where(ca => ca.Active).ToList();

            List<string> screenShoots = ScreenReadingHelper.GetScreenshoots(captureAreas);
            foreach (string screenShoot in screenShoots)
            {
                Uri fileUri = new($"ms-appdata:///temp/{screenShoot}");
                StorageFile storageFile = await StorageFile.GetFileFromApplicationUriAsync(fileUri);

                FileStream fileStream = File.OpenRead(storageFile.Path);

                BitmapDecoder bitmapDecoder = await BitmapDecoder.CreateAsync(fileStream.AsRandomAccessStream());
                SoftwareBitmap softwareBitmap = await bitmapDecoder.GetSoftwareBitmapAsync();

                fileStream.Close();

                OcrEngine ocrEngine = OcrEngine.TryCreateFromLanguage(language);
                OcrResult result = await ocrEngine.RecognizeAsync(softwareBitmap);

                results.Add(result);
            }

            return results;
        }

        private static async Task ProcessResultsAsync(List<OcrResult> results, ILogger<OcrRecorderService> logger)
        {
            foreach (OcrResult result in results) ProcessLines(result.Lines.ToList());

            if (_decodedInfo.Text == null) logger.LogInformation("Result: no text found");
            else logger.LogInformation($"Result: found text '{_decodedInfo.Text}'");

            if (_decodedInfo.Text != null) await HttpClient.SendData(_decodedInfo, logger);
        }

        private static void ProcessLines(List<OcrLine> lines)
        {
            foreach (OcrLine line in lines) _decodedInfo.Text = line.Text;

            if (lines.Count == 0) _decodedInfo.Text = null;
        }
    }
}
