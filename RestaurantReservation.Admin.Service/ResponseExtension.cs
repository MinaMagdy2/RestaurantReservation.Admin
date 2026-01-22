namespace RestaurantReservation.Admin.Service;

public static class ResponseExtension
{
    public static async Task<Result<TIn>> ReturnResult<TIn>(
        this Task<ApiResponse<TIn>> resultTask, string errorName, ErrorType errorType)
    {
        var result = await resultTask;
        if (result.IsSuccessStatusCode)
        {
            return result.Content!;
        }
        else
        {
            var message = result.GetErrorDescription();
            Error error = errorType switch
            {
                ErrorType.Conflict => Error.Conflict(errorName, message),
                ErrorType.Failure => Error.Failure(errorName, message),
                ErrorType.Validation => Error.Validation(errorName, message),
                ErrorType.NotFound => Error.NotFound(errorName, message),
                ErrorType.Problem => Error.Problem(errorName, message),
                ErrorType.None => throw new NotImplementedException(),
                _ => throw new NotImplementedException(),
            };
            return Result.Failure<TIn>(error);
        }
    }

    public static async Task<Result> ReturnResult(
        this Task<IApiResponse> resultTask, string errorName, ErrorType errorType)
    {
        var result = await resultTask;
        if (result.IsSuccessStatusCode)
        {
            return Result.Success();
        }
        else
        {
            var message = result.GetErrorDescription();
            Error error = errorType switch
            {
                ErrorType.Conflict => Error.Conflict(errorName, message),
                ErrorType.Failure => Error.Failure(errorName, message),
                ErrorType.Validation => Error.Validation(errorName, message),
                ErrorType.NotFound => Error.NotFound(errorName, message),
                ErrorType.Problem => Error.Problem(errorName, message),
                ErrorType.None => throw new NotImplementedException(),
                _ => throw new NotImplementedException(),
            };
            return Result.Failure(error);
        }
    }


    private static string GetErrorDescription(this IApiResponse response)
    {
        var errors = string.Empty;
        if (response.IsSuccessStatusCode) return errors;
        var responseError = response.Error;
        if (responseError == null) return errors;
        errors = $"Message: {responseError.Message}{Environment.NewLine}";
        if (!responseError.HasContent) return errors;
        var content = responseError.Content!;
        if (!string.IsNullOrEmpty(content))
        {
            errors += $"Content: {content}{Environment.NewLine}";
        }
        return errors;
    }
}
