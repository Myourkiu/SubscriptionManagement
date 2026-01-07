namespace SubscriptionManagement.Domain.Common;

public class Result<T>
{
    public bool IsSuccess { get; private set; }
    public bool IsFailure => !IsSuccess;
    public T? Value { get; private set; }
    public string Error { get; private set; } = string.Empty;

    private Result(T value)
    {
        IsSuccess = true;
        Value = value;
        Error = string.Empty;
    }

    private Result(string error)
    {
        IsSuccess = false;
        Value = default;
        Error = error;
    }

    public static Result<T> Success(T value) => new Result<T>(value);
    public static Result<T> Failure(string error) => new Result<T>(error);
}

public class Result
{
    public bool IsSuccess { get; private set; }
    public bool IsFailure => !IsSuccess;
    public string Error { get; private set; } = string.Empty;

    private Result()
    {
        IsSuccess = true;
        Error = string.Empty;
    }

    private Result(string error)
    {
        IsSuccess = false;
        Error = error;
    }

    public static Result Success() => new Result();
    public static Result Failure(string error) => new Result(error);
}
