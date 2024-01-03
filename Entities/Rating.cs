namespace ApiAryanakala.Entities;

public class Rating
{
    public Guid Id { get; set; }
    public string UserName { get; set; }
    public string Comment { get; set; }
    public int Rate { get; set; }
    public string ImageUrl { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime ModifiedAt { get; set; }
    public Guid ProductId { get; set; }
    public virtual Product Product { get; set; }

}