namespace RestaurantManagement.Domain.Shared;

public class Result //Result pattern => Đồng bộ hoá định dạng trả về 
{
    public Result(bool isSuccess, Error[]? errors)
    {
        if (isSuccess && errors != null) //Reult không thể có Error khi isSuccess = true
        {
            throw new InvalidOperationException();
        }
        if (!isSuccess && errors == null) //Result phải có Error khi isSuccess = false
        {
            throw new InvalidOperationException();
        }

        IsSuccess = isSuccess;
        Errors = errors;
    }
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public Error[]? Errors { get; set; }

    //Tạo ra một Result thành công
    public static Result Success() => new(true, null);
    //Tạo ra một Result thất bại
    public static Result Failure(Error[] errors) => new(false, errors);
}

public class Result<T>
{
    public Result(bool isSuccess, Error[]? errors, T value)
    {
        if (isSuccess && errors != null) //Reult không thể có Error khi isSuccess = true
        {
            throw new InvalidOperationException();
        }
        if (!isSuccess && errors == null) //Result phải có Error khi isSuccess = false
        {
            throw new InvalidOperationException();
        }

        IsSuccess = isSuccess;
        Errors = errors;
        Value = value;
    }

    public T Value { get; }
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public Error[]? Errors { get; set; }

    //Tạo ra một Result thành công
    public static Result<T> Success(T value) => new(true, null, value);
    //Tạo ra một Result thất bại
    public static Result<T> Failure(Error[] errors) => new(false, errors, default);
}

public record Error(string Code, string Message);
