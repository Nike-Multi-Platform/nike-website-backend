
using nike_website_backend.Dtos;

namespace nike_website_backend.Interfaces
{
    public interface IPaymentRepository
    {
        Task<Response<string>> CreatePaymentUrl(PaymentInformationModel model, HttpContext context);
        Task<Response<String>> PaymentExecute(IQueryCollection collections);
        Task<Response<Boolean>> checkoutByCOD(PaymentInformationModel model);
    }
}
