namespace VerticalSlice_Backend.Features.Checkout.CheckoutDTOs
{
    public record OrderItemDTO
      (   int ProductID = 1,
          decimal TotalPrice = 0,
          int Quantity = 0
      );
}
