namespace ApiAryanakala.Entities.Product;

public class CartItem
{
    public Guid UserId { get; set; }
    public Guid ProductId { get; set; }
    public int CategoryId { get; set; }
    public int Quantity { get; set; } = 1;
}