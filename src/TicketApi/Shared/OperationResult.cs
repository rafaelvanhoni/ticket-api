public class OperationResult<T>
{
    public ResultStatus Status { get; set; } = ResultStatus.Success;
    public bool IsSuccess => Status is ResultStatus.Success or ResultStatus.Created;
    public string? Message { get; set; }
    public T? Data { get; set; }

}