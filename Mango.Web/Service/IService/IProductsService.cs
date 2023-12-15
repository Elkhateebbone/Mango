using Mango.Web.Models;
using Mango.Web.Models.DTO;

namespace Mango.Web.Service.IService
{
    public interface IProductsService
    {
        Task<ResponseDTO> GetAllProductAsync();
        Task<ResponseDTO> GetProductByIdAsync(int id);
        Task<ResponseDTO> UpdateProductAsync(ProductDTO productDTO);
        Task<ResponseDTO> DeleteProductAsync(int id);
        Task<ResponseDTO> CreateProductAsync(ProductDTO productDTO);

    }
}
