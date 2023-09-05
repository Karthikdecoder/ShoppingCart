using ShoppingCartWeb.Models.Dto;

namespace ShoppingCartWeb.Services.IServices
{
	public interface IStateService
	{
		Task<T> GetAllStateAsync<T>(string token);
		
        Task<T> GetAllStateByCountryIdAsync<T>(int countryId, string token);
        Task<T> GetStateAsync<T>(int stateId, string token);
		Task<T> CreateStateAsync<T>(StateMasterDTO stateMasterDTO, string token);
		Task<T> UpdateStateAsync<T>(StateMasterDTO stateMasterDTO, string token);
		Task<T> RemoveStateAsync<T>(int stateId, string token);
	}
}
