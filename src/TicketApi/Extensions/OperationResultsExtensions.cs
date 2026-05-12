public static class OperationResultsExtensions
{
    public static IResult ToHttpResult<T>(this OperationResult<T> result)
    {
        var response = new ApiResponse<T>
        {
            Success = result.IsSuccess,
            Message = result.Message,
            Data = result.Data,
        };

        return result.Status switch
        {
            ResultStatus.Created => Results.Created($"/tickets/", response),
            ResultStatus.Success => Results.Ok(response),
            ResultStatus.NotFound => Results.NotFound(response),
            ResultStatus.ValidationError => Results.BadRequest(response),
            ResultStatus.BusinessError => Results.BadRequest(response),
            _ => Results.BadRequest(response)
        };
    }
}