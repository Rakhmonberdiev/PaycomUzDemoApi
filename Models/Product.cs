namespace PaycomUzDemoApi.Models
{
    public class Product
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        



        public string? IkpuCode { get; set; }
        public string? UnitCode { get; set; }
    }
}
