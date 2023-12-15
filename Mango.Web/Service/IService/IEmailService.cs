using Mango.Web.Models;

namespace Mango.Web.Service.IService
{
    public interface IEmailService
    {
        Task EmailCartLog(CartDTO cartDTO);
    }
}
