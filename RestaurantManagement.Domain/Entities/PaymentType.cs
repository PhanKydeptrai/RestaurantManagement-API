namespace RestaurantManagement.Domain.Entities;

public class PaymentType
{
    public Ulid PaymentTypeId { get; set; }
    public string Name { get; set; }
    public ICollection<Bill> Bills { get; set; }
}
