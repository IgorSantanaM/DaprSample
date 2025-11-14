using GloboTicket.Frontend.Models.Api;

namespace GloboTicket.Frontend.Services.ShoppingBasket.Dapr
{
    public class StateStoreBasket
    {
        public Guid BasketId { get; set; }
        public List<BasketLine> Lines { get; set; } = new List<BasketLine>();
        public Guid UserId { get; set; }
    }
}
