namespace RestaurantManagement.Domain.Entities;

public class TableType
{
    public Ulid TableTypeId { get; set; }
    public byte[]? TableImage { get; set; }
    public decimal TablePrice { get; set; }
    public string? Description { get; set; }
    public ICollection<Table>? Tables { get; set; }
    
}
