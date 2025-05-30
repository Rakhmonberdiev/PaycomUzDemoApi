using PaycomUzDemoApi.Models;
using System.Text.Json;

namespace PaycomUzDemoApi.Services
{
    public class MockDataService
    {
        public List<User> Users { get; private set; } = new();
        public List<Product> Products { get; private set; } = new();
        public List<Order> Orders { get; private set; } = new();

        public MockDataService(IWebHostEnvironment env)
        {
            var basePath = Path.Combine(env.ContentRootPath, "Mock");

            Users = LoadFromJson<User>(Path.Combine(basePath, "users.json"));
            Products = LoadFromJson<Product>(Path.Combine(basePath, "products.json"));
            Orders = LoadFromJson<Order>(Path.Combine(basePath, "orders.json"));
        }
        private List<T> LoadFromJson<T>(string path)
        {
            if (!File.Exists(path)) return new List<T>();

            var json = File.ReadAllText(path);
            return JsonSerializer.Deserialize<List<T>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? new List<T>();
        }
    }

}
