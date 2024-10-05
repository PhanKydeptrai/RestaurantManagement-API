namespace RestaurantManagement.Domain.Shared;

public class ResultForLog
{
    protected internal ResultForLog(bool isSuccess, Error error)
    {
        if (isSuccess && error != Error.None)
        {
            throw new InvalidOperationException();
        }

        if (!isSuccess && error == Error.None)
        {
            throw new InvalidOperationException();
        }

        IsSuccess = isSuccess;
        Error = error;
    }

    public bool IsSuccess { get; }

    public bool IsFailure => !IsSuccess;

    public Error Error { get; }

    public static ResultForLog Success() => new(true, Error.None);

    public static ResultForLog<TValue> Success<TValue>(TValue value) => new(value, true, Error.None);

    public static ResultForLog Failure(Error error) => new(false, error);

    public static ResultForLog<TValue> Failure<TValue>(Error error) => new(default, false, error);

    public static ResultForLog Create(bool condition) => condition ? Success() : Failure(Error.ConditionNotMet);

    public static ResultForLog<TValue> Create<TValue>(TValue? value) => value is not null ? Success(value) : Failure<TValue>(Error.NullValue);
}

public class ResultForLog<TValue> : ResultForLog
{
    private readonly TValue? _value;

    protected internal ResultForLog(TValue? value, bool isSuccess, Error error)
        : base(isSuccess, error) =>
        _value = value;

    public TValue Value => IsSuccess
        ? _value!
        : throw new InvalidOperationException("The value of a failure result can not be accessed.");

    public static implicit operator ResultForLog<TValue>(TValue? value) => Create(value);
}

public record Error(string Code, string Message)
{
    public static readonly Error None = new(string.Empty, string.Empty);

    public static readonly Error NullValue = new("Error.NullValue", "The specified result value is null.");

    public static readonly Error ConditionNotMet = new("Error.ConditionNotMet", "The specified condition was not met.");
}