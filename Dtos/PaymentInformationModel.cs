using Microsoft.AspNetCore.Routing.Constraints;

namespace nike_website_backend.Dtos
{
    public class PaymentInformationModel
    {
        public string OrderType { get; set; }
        public double Amount { get; set; }
        public string OrderDescription { get; set; }
        public string Name { get; set; }
        public UserOrderDTO Order { get; set; }

    }
}
