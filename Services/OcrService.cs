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

namespace WifftOCR.Services
{
    internal sealed partial class OcrService : IScopedProcessingService
    {
        private static readonly DecodedInfo _decodedInfo = new();
        private static readonly SettingsService _settingsService = App.GetService<SettingsService>();
        
        private readonly ILogger<OcrService> _Logger;

        private readonly ConsoleSpinner _spinner = new();

        public OcrService(ILogger<OcrService> logger)
        { 
            _Logger = logger;
        }

        public async Task DoWork(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested) {
                await GetOcrResultsAsync().ContinueWith(task => ProcessResultsAsync(task.Result, _Logger), stoppingToken);

                _spinner.Turn(displayMsg: "\u001b[31m[WifftOCR]\u001b[1m\u001b[37m Gathering text from provided capture areas", sequenceCode: 4);

                await Task.Delay(1000, stoppingToken);
            }
        }

        private static async Task<List<OcrResult>> GetOcrResultsAsync()
        {
            List<OcrResult> results = new();

            Settings settings = await _settingsService.ReadFromFileAsync() ?? throw new Exception("Settings file is null!");
            List<CaptureArea> captureAreas = settings.CaptureAreas.Where(ca => ca.Active).ToList();

            List<string> screenShoots = await ScreenReadingHelper.GetScreenshoots(captureAreas);
            foreach (string screenShoot in screenShoots)
            {
                Uri fileUri = new($"ms-appdata:///roaming/{screenShoot}");
                StorageFile storageFile = await StorageFile.GetFileFromApplicationUriAsync(fileUri);

                using StreamReader reader = new(await storageFile.OpenStreamForReadAsync());
                FileStream fileStream = (FileStream)reader.BaseStream;

                BitmapDecoder bitmapDecoder = await BitmapDecoder.CreateAsync(fileStream.AsRandomAccessStream());
                SoftwareBitmap softwareBitmap = await bitmapDecoder.GetSoftwareBitmapAsync();

                fileStream.Close();

                OcrEngine ocrEngine = OcrEngine.TryCreateFromUserProfileLanguages();
                OcrResult result = await ocrEngine.RecognizeAsync(softwareBitmap);

                results.Add(result);
            }

            return results;
        }

        private static async void ProcessResultsAsync(List<OcrResult> results, ILogger<OcrService> logger)
        {
            foreach (OcrResult result in results) ProcessLines(result.Lines.ToList());

            logger.LogInformation($"\u001b[37mDEBUG-> {JsonSerializer.Serialize(_decodedInfo)}\u001b[1m\u001b[37m");

            await HttpClient.SendData(_decodedInfo, logger);
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
