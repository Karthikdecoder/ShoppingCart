using System.Net;

namespace ShoppingCartWeb.Models
{
    public class APIResponse
    {
        public APIResponse()
        {
            ResponseMessage = new List<string>();
        }
        public HttpStatusCode StatusCode { get; set; }
        public bool IsSuccess { get; set; } = true;
        public List<string> ResponseMessage { get; set; }
        public object Result { get; set; } 

    }
}
