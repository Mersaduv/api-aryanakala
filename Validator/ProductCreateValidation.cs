using ApiAryanakala.Models.DTO.ProductDto;
using FluentValidation;

namespace ApiAryanakala.Validator;

public class ProductCreateValidation : AbstractValidator<ProductCreateDTO>
{
    public ProductCreateValidation()
    {
        RuleFor(model => model.Title).NotEmpty();
        RuleFor(model => model.Discount).InclusiveBetween(5, 100);
    }
}