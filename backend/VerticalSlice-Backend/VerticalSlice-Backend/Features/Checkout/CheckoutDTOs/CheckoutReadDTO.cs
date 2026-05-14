namespace VerticalSlice_Backend.Features.Checkout.CheckoutDTOs
{
    public record CheckoutReadDTO
   (
        int OrderID = 0,
        string Address = "",
        DateTime DateOrder = new DateTime(),
        List<OrderItemDTO>? Items = null
    );

}
