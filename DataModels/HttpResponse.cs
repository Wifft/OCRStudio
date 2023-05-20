// Copyright (c) Wifft 2023
// Wifft licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Text.Json.Serialization;

namespace OCRStudio.DataModels
{
    internal sealed partial class HttpResponse
    {
        [JsonPropertyName("code")]
        public uint Code { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; }
    }
}
