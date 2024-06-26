namespace ApiAryanakala.Const;


public static class Constants
{
    public const string Add = "add";
    public const string Admin = "admin";
    public const string Api = "api";
    public const string Base = "base";
    public const string Auth = "auth";
    public const string Cart = "cart";
    public const string ChangePassword = "changePassword";
    public const string Checkout = "checkout";
    public const string Count = "count";
    public const string Login = "login";
    public const string Payment = "payment";
    public const string Products = "products";
    public const string Main = "main";
    public const string Register = "register";
    public const string GenerateRefreshToken = "generateToken";
    public const string Search = "search";
    public const string SearchSuggestions = "search-suggestions";
    public const string UpdateQuantity = "update-quantity";
    public const string ProductImages = "product-images";
    public const string CategoryImages = "category-images";
    public const string Images = "images";
    public const string Brand = "brand";
    public const string Brands = "brands";
    public const string Details = "details";
    public const string AllDetails = "all-details";
    public const string Banner = "banner";
    public const string Banners = "banners";
    public const string Review = "review";
    public const string Reviews = "reviews";
    public const string Slider = "slider";
    public const string Sliders = "sliders";

    public const string Address = "address";
    public const string Category = "category";
    public const string Categories = "categories";
    public const string Order = "order";
    public const string Product = "product";
    private const string AuthApi = $"{Api}/{Auth}";
    private const string BaseApi = $"{Base}";
    private const string PaymentApi = $"{Api}/{Payment}";

    public const string AddressApi = $"{Api}/{Address}";
    public const string ImageApi = $"{BaseApi}/{Images}";
    public const string AdminCategoryApi = $"{CategoryApi}/{Admin}";
    public const string AdminProductApi = $"{ProductApi}/{Admin}";
    public const string AuthChangePasswordApi = $"{AuthApi}/{ChangePassword}";
    public const string AuthLoginApi = $"{AuthApi}/{Login}";
    public const string AuthRegisterApi = $"{AuthApi}/{Register}";
    public const string AuthGenerateRefreshTokenApi = $"{AuthApi}/{GenerateRefreshToken}";
    public const string CartAddApi = $"{CartApi}/{Add}";
    public const string CartApi = $"{Api}/{Cart}";
    public const string CartCountApi = $"{CartApi}/{Count}";
    public const string CartProductsApi = $"{CartApi}/{Products}";
    public const string CartUpdateQuantityApi = $"{CartApi}/{UpdateQuantity}";
    public const string CategoryApi = $"{Api}/{Category}";
    public const string OrderApi = $"{Api}/{Order}";
    public const string PaymentCheckoutApi = $"{PaymentApi}/{Checkout}";
    public const string ProductApi = $"{Api}/{Product}";
    public const string ProductImagesApi = $"{ProductApi}/{ProductImages}";
    public const string CategoryImagesApi = $"{CategoryApi}/{CategoryImages}";
    public const string ProductReviewApi = $"{ProductApi}/{Review}";
    public const string ProductCategoryApi = $"{ProductApi}/{Category}";
    public const string ProductCategoriesApi = $"{ProductApi}/{Categories}";
    public const string ProductSearchApi = $"{ProductApi}/{Search}";
    public const string ProductSearchSuggestionsApi = $"{ProductApi}/{SearchSuggestions}";
    public const string SliderCategoryImages = $"{Slider}/{CategoryImages}";

}
