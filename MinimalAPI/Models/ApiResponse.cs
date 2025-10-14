namespace MinimalAPI.Models
{
    public enum ApiResponseStatus
    {
        Success,
        Fail,
        Error,
    }

    public class ApiResponse<T>
    {
        public ApiResponseStatus Status { get; set; }
        public T? Data { get; set; }
        public string? Message { get; set; }
        public List<string> Errors { get; set; } = new();

        public static ApiResponse<T> Success(T data, string? message = null)
        {
            return new ApiResponse<T>
            {
                Status = ApiResponseStatus.Success,
                Data = data,
                Message = message
            };
        }

        public static ApiResponse<T> Fail(string message, List<string>? errors = null)
        {
            return new ApiResponse<T>
            {
                Status = ApiResponseStatus.Fail,
                Message = message,
                Errors = errors ?? new List<string>()
            };
        }

        public static ApiResponse<T> Error(string message, List<string>? errors = null)
        {
            return new ApiResponse<T>
            {
                Status = ApiResponseStatus.Error,
                Message = message,
                Errors = errors ?? new List<string>()
            };
        }
    }
}
