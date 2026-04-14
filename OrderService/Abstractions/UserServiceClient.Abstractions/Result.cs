namespace UserServiceClient.Abstractions;

public record Result
{
    public Result(bool isSuccess, Error error)
    {
        if (isSuccess && error != Error.None)
            throw new InvalidOperationException();

        if (!isSuccess & error == Error.None)
            throw new InvalidOperationException();

        Error = error;
        IsSuccess = isSuccess;
    }
    
    public Error Error { get; }
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;

    public static Result Success() => new(true, Error.None);

    public static implicit operator Result(Error error) => new(false, error);
}

public sealed record Result<TValue> : Result
{
    private readonly TValue _value;
    
    public Result(TValue value, bool isSuccess, Error error)
        : base(isSuccess, error)
    {
        _value = value;
    }

    public TValue Value => IsSuccess
        ? _value
        : throw new InvalidOperationException("The value of a failure result can not be accessed.");

    public static Result<TValue> Success(TValue value) => new(value, true, Error.None);

    public static implicit operator Result<TValue>(TValue value) => new(value, true, Error.None);

    public static implicit operator Result<TValue>(Error error) => new(default!, false, error);
}

public sealed record Error
{
    public static readonly Error None = new(string.Empty, ErrorType.None);

    public string Message { get; }
    public ErrorType Type { get; }
    
    private Error(string message, ErrorType type)
    {
        Message = message;
        Type = type;
    }

    public static Error Validation(string message) =>
        new(message, ErrorType.Validation);

    public static Error NotFound(string message) =>
        new(message, ErrorType.NotFound);

    public static Error Failure(string message) =>
        new(message, ErrorType.Failure);

    public static Error Conflict(string message) =>
        new(message, ErrorType.Conflict);

    public static Error Forbidden(string message) =>
        new(message, ErrorType.Forbidden);
}

public enum ErrorType
{
    None,
    Validation,
    NotFound,
    Failure,
    Forbidden,
    Conflict
}