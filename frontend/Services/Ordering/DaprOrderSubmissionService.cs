using Dapr.Client;
using GloboTicket.Frontend.Models.Api;
using GloboTicket.Frontend.Models.View;
using Microsoft.Win32.SafeHandles;

namespace GloboTicket.Frontend.Services.Ordering
{
    public class DaprOrderSubmissionService(IShoppingBasketService shoppingBasketService, DaprClient daprClient, ILogger<DaprOrderSubmissionService> logger) : IOrderSubmissionService
    {
        public async Task<Guid> SubmitOrder(CheckoutViewModel checkoutViewModel)
        {
            var lines = await shoppingBasketService.GetLinesForBasket(checkoutViewModel.BasketId);
            var order = new OrderForCreation();
            order.Date = DateTimeOffset.Now;
            order.OrderId = Guid.NewGuid();
            order.Lines = lines.Select(line => new OrderLine() { EventId = line.EventId, Price = line.Price, TicketCount = line.TicketAmount }).ToList();
            order.CustomerDetails = new CustomerDetails()
            {
                Address = checkoutViewModel.Address,
                CreditCardNumber = checkoutViewModel.CreditCard,
                Email = checkoutViewModel.Email,
                Name = checkoutViewModel.Name,
                PostalCode = checkoutViewModel.PostalCode,
                Town = checkoutViewModel.Town,
                CreditCardExpiryDate = checkoutViewModel.CreditCardDate
            };
            logger.LogInformation("Posting order event to dapr pubsub");
            await daprClient.PublishEventAsync(pubsubName: "pubsub", topicName: "orders", data: order);
            return order.OrderId;
        }
    }
}
