using ApiAryanakala.Models.DTO.ProductDto;
using FluentValidation;

namespace ApiAryanakala.Validator;

public class ProductUpdateValidation : AbstractValidator<ProductUpdateDTO>
{
    public ProductUpdateValidation()
    {
        RuleFor(model => model.Id).NotEmpty();
        RuleFor(model => model.Title).NotEmpty();
        RuleFor(model => model.Discount).InclusiveBetween(1, 100);
    }
}