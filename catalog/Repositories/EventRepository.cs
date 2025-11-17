using Dapr.Client;
using System.Runtime.CompilerServices;

namespace GloboTicket.Catalog.Repositories;

public class EventRepository : IEventRepository
{
    private readonly DaprClient _daprClient;
    private List<Event> events = new List<Event>();
    private readonly ILogger<EventRepository> logger;

    public EventRepository(ILogger<EventRepository> logger, DaprClient daprClient)
    {
        _daprClient = daprClient;
        this.logger = logger;

        LoadSampleData();
    }

    private void LoadSampleData()
    {
        var johnEgbertGuid = Guid.Parse("{CFB88E29-4744-48C0-94FA-B25B92DEA317}");
        var nickSailorGuid = Guid.Parse("{CFB88E29-4744-48C0-94FA-B25B92DEA318}");
        var michaelJohnsonGuid = Guid.Parse("{CFB88E29-4744-48C0-94FA-B25B92DEA319}");

        events.Add(new Event
        {
            EventId = johnEgbertGuid,
            Name = "John Egbert Live",
            Price = 65,
            Artist = "John Egbert",
            Date = DateTime.Now.AddMonths(6),
            Description = "Join John for his farwell tour across 15 continents. John really needs no introduction since he has already mesmerized the world with his banjo.",
            ImageUrl = "/img/banjo.jpg",
        });

        events.Add(new Event
        {
            EventId = michaelJohnsonGuid,
            Name = "The State of Affairs: Michael Live!",
            Price = 85,
            Artist = "Michael Johnson",
            Date = DateTime.Now.AddMonths(9),
            Description = "Michael Johnson doesn't need an introduction. His 25 concert across the globe last year were seen by thousands. Can we add you to the list?",
            ImageUrl = "/img/michael.jpg",
        });

        events.Add(new Event
        {
            EventId = nickSailorGuid,
            Name = "To the Moon and Back",
            Price = 135,
            Artist = "Nick Sailor",
            Date = DateTime.Now.AddMonths(8),
            Description = "The critics are over the moon and so will you after you've watched this sing and dance extravaganza written by Nick Sailor, the man from 'My dad and sister'.",
            ImageUrl = "/img/musical.jpg",
        });
    }

    public async Task<IEnumerable<Event>> GetEvents()
    {
        try
        {
            var connectionString = await GetConnectionStringAsync();
            logger.LogInformation($"Success pulling the connection string from Dapr Secret Store: {connectionString}");
        }
        catch 
        {
            logger.LogError("Error pulling the connection string from the Dapr secret Store.");
        }
        return events;
    }

    public async Task<string> GetConnectionStringAsync()
    {
        var secretstoreName = Environment.GetEnvironmentVariable("SECRET_STORE_NAME") ?? throw new InvalidOperationException("SECRET_STORE_NAME environment variable is not set");
        var secretName = "eventcatalogdb";
        var secret = await _daprClient.GetSecretAsync(secretstoreName, secretName);
        return secret[secretName];
    }

    public Task<Event> GetEventById(Guid eventId)
    {
        var @event = events.FirstOrDefault(e => e.EventId == eventId);
        if (@event == null)
        {
            throw new InvalidOperationException("Event not found");
        }
        return Task.FromResult(@event);
    }

    // scheduled task calls this periodically to put one item on special offer
    public void UpdateSpecialOffer()
    {
        // reset all tickets to their default
        events.Clear();
        LoadSampleData();
        // pick a random one to put on special offer
        var random = new Random();
        var specialOfferEvent = events[random.Next(0,events.Count)];
        // 20 percent off
        specialOfferEvent.Price = (int)(specialOfferEvent.Price * 0.8);
    }
}
