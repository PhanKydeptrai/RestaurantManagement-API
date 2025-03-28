﻿namespace RestaurantManagement.Domain.Entities;

public class TableType
{
    public Ulid TableTypeId { get; set; }
    public string TableTypeName { get; set; }
    public int TableCapacity { get; set; }
    public string Status { get; set; }
    public string? ImageUrl { get; set; }
    public decimal TablePrice { get; set; }
    public string? Description { get; set; }
    public ICollection<Table>? Tables { get; set; }

}
