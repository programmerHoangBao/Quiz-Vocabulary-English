using back_end.DTOs;

namespace back_end.Services.Interfaces
{
    public interface ISpeechService
    {
        Task<ApiResponse<object?>> ConvertTextToSpeechAsync(string text);
    }
}
