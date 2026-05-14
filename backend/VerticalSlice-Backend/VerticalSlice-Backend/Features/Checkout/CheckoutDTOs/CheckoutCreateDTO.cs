namespace VerticalSlice_Backend.Features.Checkout.CheckoutDTOs
{
    public record CheckoutCreateDTO
    (   int UserID = 1,
        string Address = "",
        List<OrderItemDTO>? Items = null
    );
    
}
