using System.Text.Json;
using MongoDB.Driver;
using MongoDB.Entities;

namespace SearchService;

public class DbInitializer
{
    public static async Task InitDb(WebApplication app)
    {
        await DB.InitAsync("SearchDb", 
            MongoClientSettings.FromConnectionString(app.Configuration.GetConnectionString("MongoDbConnection")));
            await DB.Index<Item>()
                .Key(a => a.Make, KeyType.Text)
                .Key(a => a.Model, KeyType.Text)
                .Key(a => a.Color, KeyType.Text)
                .CreateAsync();
            var count = await DB.CountAsync<Item>();

        var scope = app.Services.CreateScope();
        var httpClient = scope.ServiceProvider.GetRequiredService<AuctionSvcHttpClient>();
        var items = await httpClient.GetItemsForSearchDb();
        System.Console.WriteLine($"Items count returned from Auction Service: {items.Count}");
        if (items.Count > 0) await DB.SaveAsync(items);
    }
}
