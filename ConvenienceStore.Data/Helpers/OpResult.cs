namespace ConvenienceStore.Data.Helpers
{
    
    public class OpResult
    {
        public bool Success { get; private set; }
        public string Message { get; private set; }

        public static OpResult Ok(string message = "")
        {
            return new OpResult { Success = true, Message = message };
        }

        public static OpResult Fail(string message)
        {
            return new OpResult { Success = false, Message = message };
        }
    }
}
