using ShoppingCartWeb.Models;

namespace ShoppingCartWeb.Services.IServices
{
    public interface IBaseService 
    {
        APIResponse responseModel { get; set; }
        Task<T> SendAsync<T>(APIRequest apiRequest);  
                                                      
    }
}

