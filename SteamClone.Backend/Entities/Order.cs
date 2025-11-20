namespace SteamClone.Backend.Entities;

/// <summary>
/// Represents the current status of an order in the fulfillment process
/// </summary>
public enum OrderStatus
{
    /// <summary>Order created but not yet processed</summary>
    Pending,
    /// <summary>Order successfully processed and delivered</summary>
    Completed,
    /// <summary>Order processing failed</summary>
    Failed,
    /// <summary>Order was refunded to the customer</summary>
    Refunded,
    /// <summary>Order was cancelled before completion</summary>
    Cancelled
}

/// <summary>
/// Represents a customer order containing one or more game purchases
/// </summary>
public class Order
{
    /// <summary>Unique identifier for the order</summary>
    public int OrderId { get; set; }

    /// <summary>ID of the user who placed the order</summary>
    public int UserId { get; set; }

    /// <summary>Navigation property to the user entity</summary>
    public User User { get; set; } = null!;

    /// <summary>Collection of items (games) included in this order</summary>
    public List<OrderItem> Items { get; set; } = new();

    /// <summary>Total price of all items in the order</summary>
    public decimal TotalPrice { get; set; }

    /// <summary>Timestamp when the order was placed</summary>
    public DateTime OrderDate { get; set; } = DateTime.UtcNow;

    /// <summary>Current status of the order (Pending, Completed, etc.)</summary>
    public OrderStatus Status { get; set; } = OrderStatus.Completed;
}