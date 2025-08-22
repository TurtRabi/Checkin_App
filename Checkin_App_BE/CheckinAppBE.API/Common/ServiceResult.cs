namespace Common
{
    public class ServiceResult
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = string.Empty;
        public int StatusCode { get; set; }

        public static ServiceResult Success(string message = "Operation successful", int statusCode = 200)
        {
            return new ServiceResult { IsSuccess = true, Message = message, StatusCode = statusCode };
        }

        public static ServiceResult Fail(string message = "Operation failed", int statusCode = 400)
        {
            return new ServiceResult { IsSuccess = false, Message = message, StatusCode = statusCode };
        }
    }

    public class ServiceResult<T> : ServiceResult
    {
        public T Data { get; set; } = default(T)!;

        public static ServiceResult<T> Success(T data, string message = "Operation successful", int statusCode = 200)
        {
            return new ServiceResult<T> { IsSuccess = true, Data = data, Message = message, StatusCode = statusCode };
        }

        public new static ServiceResult<T> Fail(string message = "Operation failed", int statusCode = 400)
        {
            return new ServiceResult<T> { IsSuccess = false, Message = message, StatusCode = statusCode };
        }
    }
}