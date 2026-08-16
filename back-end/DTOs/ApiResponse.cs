using back_end.Records;

namespace back_end.DTOs
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public MessageCode MessageCode { get; set; }
        public T? Data { get; set; }
        public static ApiResponse<T> Response(
            MessageCode messageCode,
            T? data = default)
        {
            bool success = !string.IsNullOrEmpty(messageCode.ResponseCode)
                           && messageCode.ResponseCode[0] != 'E';
            return new ApiResponse<T>
            {
                Success = success,
                MessageCode = messageCode,
                Data = data
            };
        }
    }
}
