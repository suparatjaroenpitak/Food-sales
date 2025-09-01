namespace TestQuit.Models
{
    public class Food
    {
        public DateTime OrderDate { get; set; }
        public string Region { get; set; }
        public string City { get; set; }
        public string Category { get; set; }
        public string Product { get; set; }
        public string Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
