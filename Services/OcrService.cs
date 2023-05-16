// Copyright (c) Wifft 2023
// Wifft licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using Windows.Graphics.Imaging;
using Windows.Media.Ocr;
using Windows.Storage;

using WifftOCR.Clients;
using WifftOCR.DataModels;
using WifftOCR.Helpers;
using WifftOCR.Interfaces;
using Windows.Globalization;
using Microsoft.UI.Dispatching;

namespace WifftOCR.Services
{
    internal sealed partial class OcrService : IScopedProcessingService
    {
        private static readonly DecodedInfo _decodedInfo = new();
        
        private readonly ILogger<OcrService> _logger;

        //private readonly ConsoleSpinner _spinner = new();

        public OcrService(ILogger<OcrService> logger)
        { 
            _logger = logger;
        }

        public async Task DoWork(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested) {
                try {
                    await GetOcrResultsAsync().ContinueWith(task => ProcessResultsAsync(task.Result, _logger), stoppingToken);

                    //_spinner.Turn(displayMsg: "\u001b[31m[WifftOCR]\u001b[1m\u001b[37m Gathering text from provided capture areas", sequenceCode: 4);

                    await Task.Delay(1000, stoppingToken);
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

        private static async Task ProcessResultsAsync(List<OcrResult> results, ILogger<OcrService> logger)
        {
            foreach (OcrResult result in results) ProcessLines(result.Lines.ToList());

            if (_decodedInfo.Text is null) logger.LogInformation("Result: no text found");
            else logger.LogInformation($"Result: found text '{_decodedInfo.Text}'");

            if (_decodedInfo.Text is not null) await HttpClient.SendData(_decodedInfo, logger);
        }

        private static void ProcessLines(List<OcrLine> lines)
        {
            foreach (OcrLine line in lines) _decodedInfo.Text = line.Text;

            if (lines.Count == 0) _decodedInfo.Text = null;
        }

        private sealed class ConsoleSpinner
        {
            #nullable enable
            private static string[,]? sequence = null;

            public const int DELAY = 300;

            private readonly int totalSequences = 0;
            private int counter;

            public ConsoleSpinner()
            {
                counter = 0;
                sequence = new string[,] {
                    { "/", "-", "\\", "|" },
                    { ".", "o", "0", "o" },
                    { "+", "x","+","x" },
                    { "V", "<", "^", ">" },
                    { ".   ", "..  ", "... ", "...." },
                    { "=>   ", "==>  ", "===> ", "====>" },
                };

                totalSequences = sequence.GetLength(0);
            }

            public void Turn(string displayMsg = "", int sequenceCode = 0)
            {
                counter++;

                Thread.Sleep(DELAY);

                sequenceCode = sequenceCode > totalSequences - 1 ? 0 : sequenceCode;

                int counterValue = counter % 4;

                string fullMessage = displayMsg + sequence?[sequenceCode, counterValue];

                Console.Write(fullMessage);

                Console.SetCursorPosition(0, Console.CursorTop);
            }
        }
    }
}
