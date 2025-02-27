using MongoDB.Entities;

namespace SearchService;

public class AuctionSvcHttpClient(HttpClient httpClient, IConfiguration config)
{
    private readonly HttpClient httpClient = httpClient;
    private readonly IConfiguration config = config;

    public async Task<List<Item>> GetItemsForSearchDb()
    {
        var lastUpdated = await DB.Find<Item, string>() // Find all items in the database
            .Sort(x => x.UpdatedAt, Order.Descending) // Sort by the UpdatedAt property in descending order
            .Project(x => x.UpdatedAt.ToString()) // Project the UpdatedAt property as a string
            .ExecuteFirstAsync(); // Execute the query and return the first result which will be the latest UpdatedAt date (string)

        return await httpClient.GetFromJsonAsync<List<Item>>(
            config["AuctionServiceUrl"] + "/api/auctions" + $"?date={lastUpdated}");
    }
}
