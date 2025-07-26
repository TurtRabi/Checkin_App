
namespace Common
{
    public class ServiceResult
    {
        public bool Succeeded { get; set; }
        public string? Message { get; set; }
        public string? ErrorCode { get; set; }

        public static ServiceResult Success(string? message = null)
        {
            return new ServiceResult { Succeeded = true, Message = message };
        }

        public static ServiceResult Failed(string errorCode, string message)
        {
            return new ServiceResult { Succeeded = false, ErrorCode = errorCode, Message = message };
        }
    }
}