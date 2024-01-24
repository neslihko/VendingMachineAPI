namespace VendingMachineAPI.Helpers
{
    public class Result<T>
    {
        public bool Success { get; set; }

        public string Message { get; set; }

        public T Data { get; set; }
        public Result(T t)
            : this(true, "OK", t)
        {
        }

        public Result(Exception ex)
            : this(false, ex.Message)
        {
        }

        public Result(bool success, string message)
            : this(success, message, default(T))
        {
        }

        public Result(bool success, string message, T t)
        {
            this.Success = success;
            this.Message = message;
            this.Data = t;
        }
    }
}