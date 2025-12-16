namespace Domain.Models.DbUtils;

public class DbOperationResult<T>
{
    public ResultStatus Status { get; set; }
    
    public string ErrorMessage { get; set; } = "";
    
    public T Data { get; set; }
}

public enum ResultStatus
{
    Success,
    Failure,
}