namespace MinimalAPI.Models
{
    public class ServiceResult<T>
    {
        public bool IsSuccess { get; set; }
        public T? Data { get; set; }
        public string? ErrorMessage { get; set; }
        public List<string> ValidationErrors { get; set; } = new();

        public static ServiceResult<T> Success(T data)
        {
            return new ServiceResult<T>
            {
                IsSuccess = true,
                Data = data
            };
        }

        public static ServiceResult<T> Failure(string errorMessage, List<string>? validationErrors = null)
        {
            return new ServiceResult<T>
            {
                IsSuccess = false,
                ErrorMessage = errorMessage,
                ValidationErrors = validationErrors ?? new List<string>()
            };
        }
    }
}
