using MongoDB.Driver;

namespace ASP.netcore_Project.Models
{
    public class CountService
    {
        private readonly IMongoCollection<CountModel> _countCollection;

        public CountService()
        {
            var client = new MongoClient("mongodb+srv://Nikunj:NikunjG2004@quickcart.dkxso.mongodb.net/?retryWrites=true&w=majority&appName=QuickCart");
            var database = client.GetDatabase("QuickCartDB");
            _countCollection = database.GetCollection<CountModel>("Count");
        }

        public void UpdateCounts(int productChange = 0, int orderChange = 0)
        {
            var filter = Builders<CountModel>.Filter.Eq(c => c.Id, "dashboard-counts");
            var update = Builders<CountModel>.Update
                .Inc(c => c.TotalProducts, productChange)
                .Inc(c => c.TotalOrders, orderChange);

            _countCollection.UpdateOne(filter, update, new UpdateOptions { IsUpsert = true });
        }

        public CountModel GetCounts()
        {
            return _countCollection.Find(c => c.Id == "dashboard-counts").FirstOrDefault()
                   ?? new CountModel();
        }
    }
}
