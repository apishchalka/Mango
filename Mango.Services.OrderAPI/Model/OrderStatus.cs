namespace Mango.Services.OrderAPI.Model
{
    public enum OrderStatus
    {
        Pending = 0, 
        Approved = 1,
        ReadyForPickup = 2,
        Completed = 3,
        Refunded = 4,
        Cancelled = 5,
    }
}
