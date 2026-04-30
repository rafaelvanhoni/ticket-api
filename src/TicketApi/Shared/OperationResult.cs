public class OperationResult<T>
{
    public ResultStatus Status { get; set; } = ResultStatus.Success;
    public bool IsSuccess => Status == ResultStatus.Success;
    public string? Message { get; set; }
    public T? Data { get; set; }

}