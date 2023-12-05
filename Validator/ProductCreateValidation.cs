using ApiAryanakala.Models.DTO.ProductDtos;
using FluentValidation;

namespace ApiAryanakala.Validator;

public class ProductCreateValidation : AbstractValidator<ProductCreateDTO>
{
    public ProductCreateValidation()
    {
        // RuleFor(model => model.Title);
        RuleFor(model => model.Discount).InclusiveBetween(5, 100);
    }
}