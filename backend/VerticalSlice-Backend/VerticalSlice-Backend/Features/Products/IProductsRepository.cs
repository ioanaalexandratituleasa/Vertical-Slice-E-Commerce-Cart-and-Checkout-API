using VerticalSlice_Backend.Features.Products;

namespace VerticalSlice_Backend.Features.Products
{
    public interface IProductsRepository
    {
        Task<ProductDetailDTO?> GetProductByIdAsync(int ProductID);
        Task<IEnumerable<ProductDetailDTO>> GetAllProductsAsync();
    }
}
