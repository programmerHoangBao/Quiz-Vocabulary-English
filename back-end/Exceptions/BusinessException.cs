using back_end.Records;

namespace back_end.Exceptions
{
    public class BusinessException : Exception
    {
        public ErrorRecord ErrorRecord { get; set; }
        public BusinessException(ErrorRecord errorRecord) : base(errorRecord.Message)
        {
            ErrorRecord = errorRecord;
        }
    }
}
