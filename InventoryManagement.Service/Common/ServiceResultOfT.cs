using System;

namespace InventoryManagement.Service.Common
{
    public class ServiceResult<T>
    {
        public bool IsSuccess { get; }
        public string Message { get; }
        public T Data { get; }

        private ServiceResult(bool isSuccess, string message, T data)
        {
            IsSuccess = isSuccess;
            Message = message;
            Data = data;
        }

        public static ServiceResult<T> Ok(T data)
        {
            return new ServiceResult<T>(true, null, data);
        }

        public static ServiceResult<T> Fail(string message)
        {
            return new ServiceResult<T>(false, message, default(T));
        }
    }
}