namespace VerticalSlice_Backend.Features.Checkout.CheckoutDTOs
{
    public record CheckoutCreateDTO
    (   int ? UserID,
        string Address = "",
        List<OrderItemDTO>? Items = null
    );
    
}
