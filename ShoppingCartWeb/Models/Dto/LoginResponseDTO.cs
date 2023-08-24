namespace ShoppingCartWeb.Models.Dto
{
    public class LoginResponseDTO
    {
        public RegistrationDTO User { get; set; }
        public string Token { get; set; }
    }
}

