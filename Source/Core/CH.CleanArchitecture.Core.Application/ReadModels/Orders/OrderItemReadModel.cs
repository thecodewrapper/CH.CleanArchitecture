namespace CH.CleanArchitecture.Core.Application.ReadModels
{
    public class OrderItemReadModel
    {
        public string ProductName { get; set; }
        public decimal ProductPrice { get; set; }
        public int Quantity { get; set; }
    }
}
