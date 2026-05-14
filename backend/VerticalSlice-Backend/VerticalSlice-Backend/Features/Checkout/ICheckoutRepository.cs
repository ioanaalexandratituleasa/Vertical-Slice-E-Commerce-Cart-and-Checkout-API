using VerticalSlice_Backend.Features.Checkout.CheckoutDTOs;

namespace VerticalSlice_Backend.Features.Checkout
{
    public interface ICheckoutRepository
    {
        Task PlaceOrderAsync(CheckoutCreateDTO data);
    }
}
