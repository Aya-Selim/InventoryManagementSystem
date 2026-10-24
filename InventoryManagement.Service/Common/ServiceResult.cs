namespace InventoryManagement.Service.Common
{
    public class ServiceResult
    {
        public bool IsSuccess { get; }
        public string Message { get; }

        private ServiceResult(bool isSuccess, string message)
        {
            IsSuccess = isSuccess;
            Message = message;
        }

        public static ServiceResult Ok()
        {
            return new ServiceResult(true, null);
        }

        public static ServiceResult Fail(string message)
        {
            return new ServiceResult(false, message);
        }
    }
}