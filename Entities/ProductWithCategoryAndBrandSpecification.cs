// namespace ApiAryanakala.Entities;

// public class ProductWithCategoryAndBrandSpecification : BaseSpecification<Product>
// {
//     public ProductWithCategoryAndBrandSpecification(RequestQueryParameters productsParams)
//         : base(x =>
//                     (string.IsNullOrEmpty(productsParams.Search) || x.Title.Contains(productsParams.Search)) &&
//                     (!productsParams.Brand.HasValue || x.BrandId == productsParams.Brand) &&
//                     (!productsParams.Category.HasValue || x.CategoryId == productsParams.Category)
//         )
//     {
//         AddInclude(p => p.Brand);
//         AddInclude(p => p.Category);
//         //ApplyPaging(0,5);
//         ApplyPaging(productsParams.PageSize * (productsParams.PageIndex - 1), productsParams.PageSize);

//         if (!string.IsNullOrEmpty(productsParams.Sort))
//         {
//             switch (productsParams.Sort)
//             {
//                 case "nameAsc":
//                     AddOrderBy(p => p.Name);
//                     break;
//                 case "nameDesc":
//                     AddOrderByDescending(p => p.Name);
//                     break;
//                 case "priceAsc":
//                     AddOrderBy(p => p.Price);
//                     break;
//                 case "priceDesc":
//                     AddOrderByDescending(p => p.Price);
//                     break;
//                 case "descriptionAsc":
//                     AddOrderBy(p => p.Description);
//                     break;
//                 case "descriptionDesc":
//                     AddOrderByDescending(p => p.Description);
//                     break;
//                 default:
//                     AddOrderBy(p => p.Name);
//                     break;
//             }
//         }
//     }

//     public ProductWithCategoryAndBrandSpecification(int id) : base(x => x.Id == id)
//     {
//         AddInclude(p => p.Brand);
//         AddInclude(p => p.Category);
//     }
// }