namespace VerticalSlice_Backend.Features.Checkout
{
    public class CheckoutDTO
    {
        public string Address { get; set; }
        public List<CartItemDTO> Items { get; set; }
    }

    public class CartItemDTO
    {
        public int ProductID { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}
