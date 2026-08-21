using back_end.Records;

namespace back_end.DTOs
{
    public class ApiResponse<T>
    {
        public bool IsSuccess { get; set; }
        public string ResponseCode { get; set; } = string.Empty;
        public int HttpStatusCode { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
        public static ApiResponse<T> ErrorResponse(ErrorRecord errorRecord, T? data = default)
        {
            return new ApiResponse<T>
            {
                IsSuccess = false,
                ResponseCode = errorRecord.ResponseCode,
                HttpStatusCode = errorRecord.HttpStatus,
                Message = errorRecord.Message,
                Data = data
            };
        }

        public static ApiResponse<T> MessageResponse(MessageRecord messageRecord, T? data = default)
        {
            return new ApiResponse<T>
            {
                IsSuccess = true,
                ResponseCode = messageRecord.ResponseCode,
                HttpStatusCode = messageRecord.HttpStatus,
                Message = messageRecord.Message,
                Data = data
            };
        }
    }
}
