using ApiAryanakala.Models;

namespace ApiAryanakala.Entities;

public class Brand : BaseClass<int>
{
    public string Name { get; set; }
    public List<Product> Products { get; private set; }

}