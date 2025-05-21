using Microsoft.AspNetCore.Http;

namespace StoreExample.ErrorHandling;

public class ErrorOr<T>
{
    public T? Value;
    
    public ErrorType? ErrorType = null;

    public List<string> Errors { get; } = new();

    public bool IsError => Errors.Any();
    
    public int StatusCode => ErrorType switch
    {
        ErrorHandling.ErrorType.Validation => StatusCodes.Status400BadRequest,
        ErrorHandling.ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
        ErrorHandling.ErrorType.Forbidden => StatusCodes.Status403Forbidden,
        ErrorHandling.ErrorType.NotFound => StatusCodes.Status404NotFound,
        ErrorHandling.ErrorType.Conflict => StatusCodes.Status409Conflict,
        ErrorHandling.ErrorType.Failure => StatusCodes.Status500InternalServerError,
        _ => StatusCodes.Status500InternalServerError
    };
    
    private ErrorOr(T value)
    {
        Value = value;
    }
    
    private ErrorOr(ErrorType errorType, List<String> errors)
    {
        ErrorType = errorType;
        Errors = errors;
    }

    public static implicit operator ErrorOr<T>(T value) => new(value);
    public static implicit operator ErrorOr<T>(Error error) => new(error.ErrorType, error.Errors);
}

public enum ErrorType
{
    Failure,
    Unexpected,
    Validation,
    Conflict,
    NotFound,
    Unauthorized,
    Forbidden,
}

public class Error(ErrorType errorType, List<string> errors)
{
    public readonly ErrorType ErrorType = errorType;
    public readonly List<string> Errors = errors;
}