// Copyright (c) Wifft 2023
// Wifft licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using OCRStudio.DataModels;
using OCRStudio.Services;

namespace OCRStudio.Clients
{
    internal sealed partial class HttpClient
    {
        public static async Task<HttpResponse> SendData(DecodedInfo decodedInfo, ILogger<OcrRecorderService> logger)
        {
            try {
                #nullable enable
                Settings? settings = await SettingsService.ReadSettingsAsync() ?? throw new Exception("Settings file not exists!");

                string rawBody = JsonSerializer.Serialize(decodedInfo);

                Dictionary<string, string>? body = new()
                {
                    { "ocr_data", rawBody }
                };

                string serverEndpoint = settings.ServerEndpoint ?? throw new Exception("Endpoint is empty! Check the app settings.");
                serverEndpoint = serverEndpoint.Replace("{observerId}", settings.Observer.ToString());

                return await MakeRequest(HttpMethod.Patch, serverEndpoint, body, settings.ServerKey, logger);
            } catch (Exception e) {
                logger.LogError(e.Message);

                throw new Exception(e.Message);
            }
        }

        #nullable enable
        private static async Task<HttpResponse> MakeRequest(
            HttpMethod method, 
            string endpoint, 
            Dictionary<string, string>? 
            data, 
            string? key,
            ILogger<OcrRecorderService> logger
        ) {
            System.Net.Http.HttpClient client = new();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            if (key?.Length > 0) {
                byte[] hashBytes = Encoding.ASCII.GetBytes(string.Format("{0}", key));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Convert.ToBase64String(hashBytes));
            }

            Uri uri = new(endpoint);

            HttpRequestMessage request = new() 
            {
                Method = method,
                RequestUri = uri,
                Content = new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json")
            };

            HttpResponseMessage response = await client.SendAsync(request);

            string apiResponse = await response.Content.ReadAsStringAsync();

            logger.LogInformation(apiResponse);

            try {
                HttpResponse? responseBody = JsonSerializer.Deserialize<HttpResponse>(apiResponse);
                if (responseBody == null) throw new Exception("Null response body!");
                else if (!responseBody.Code.Equals(200)) throw new Exception(responseBody.Message);

                return responseBody;
            } catch (Exception e) {
                throw new Exception($"An error ocurred while calling the API. It responded with the following message: {e.Message}");
            }
        }
    }
}
