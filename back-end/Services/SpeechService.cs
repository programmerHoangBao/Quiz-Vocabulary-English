using back_end.Configurations.Settings;
using back_end.DTOs;
using back_end.Exceptions;
using back_end.Services.Interfaces;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using back_end.Records;

namespace back_end.Services
{
    public class SpeechService : ISpeechService
    {
        private readonly ElevenLabsSetting _elevenLabsSetting;
        private readonly HttpClient _httpClient;

        public SpeechService(IOptions<ElevenLabsSetting> options, HttpClient httpClient)
        {
            _elevenLabsSetting = options.Value;
            _httpClient = httpClient;
        }

        public async Task<ApiResponse<object?>> ConvertTextToSpeechAsync(string text)
        {
            var url = $"https://api.elevenlabs.io/v1/text-to-speech/{_elevenLabsSetting.VoiceId}";
            var requestBody = new
            {
                text = text,
                model_id = _elevenLabsSetting.ModelId
            };
            var json = JsonSerializer.Serialize(requestBody);

            using var request = new HttpRequestMessage(
                HttpMethod.Post,
                url
            );

            request.Headers.Add("xi-api-key", _elevenLabsSetting.ApiKey);

            request.Content = new StringContent(
                json,
                Encoding.UTF8,
                "application/json"
            );

            request.Headers.Accept.Add(
                new MediaTypeWithQualityHeaderValue("audio/mpeg")
            );

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"ElevenLabs Status: {response.StatusCode}");
                Console.WriteLine($"ElevenLabs Error: {error}");
                throw new BusinessException(ErrorRecord.ConvertTextToSpeechFailed);
            }

            return ApiResponse<object?>.MessageResponse(
                MessageRecord.ConvertTextToSpeechSuccess,
                await response.Content.ReadAsByteArrayAsync()
            );
        }
    }
}
