namespace RestaurantManagement.Domain.Response;

public class Result<T> 
{
    public T? ResultValue { get; set; }
    public bool IsSuccess { get; set; }
    public List<string>? Errors { get; set; }
}
