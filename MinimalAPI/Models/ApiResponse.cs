using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MinimalAPI.Models
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ApiResponseStatus
    {
        [EnumMember(Value = "success")]
        Success,
        
        [EnumMember(Value = "fail")]
        Fail,
        
        [EnumMember(Value = "error")]
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

    /// <summary>
    /// Represents an empty response payload
    /// </summary>
    public class EmptyResponse
    {
    }

    public static class ApiResponse
    {
        public static ApiResponse<EmptyResponse> SuccessNoData(string? message = null)
        {
            return new ApiResponse<EmptyResponse>
            {
                Status = ApiResponseStatus.Success,
                Data = new EmptyResponse(),
                Message = message
            };
        }
        
        public static ApiResponse<EmptyResponse> Fail(string message, List<string>? errors = null)
        {
            return new ApiResponse<EmptyResponse>
            {
                Status = ApiResponseStatus.Fail,
                Message = message,
                Errors = errors ?? new List<string>()
            };
        }
        
        public static ApiResponse<EmptyResponse> Error(string message, List<string>? errors = null)
        {
            return new ApiResponse<EmptyResponse>
            {
                Status = ApiResponseStatus.Error,
                Message = message,
                Errors = errors ?? new List<string>()
            };
        }
    }
}
