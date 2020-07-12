namespace shopapp.entity
{
    public class ProductCategory
    {
        public int CategoryID { get; set; }
        public Category Category { get; set; }
        public int ProductId { get; set; } 
        public Product Product { get; set; }
    }
}