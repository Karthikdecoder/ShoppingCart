using ShoppingCartWeb.Models;
using ShoppingCartWeb.Models.Dto;
using ShoppingCartWeb.Services.IServices;
using ShoppingCartWeb.Utililty;

namespace ShoppingCartWeb.Services
{
	public class StateService : BaseService, IStateService
	{
		private readonly IHttpClientFactory _clientFactory;
		private string stateURL;
		public StateService(IHttpClientFactory clientFactory, IConfiguration configuration) : base(clientFactory)
		{
			this._clientFactory = clientFactory;

			stateURL = configuration.GetValue<string>("ServiceUrls:ShoppingCartAPI");
		}
		public Task<T> CreateStateAsync<T>(StateMasterDTO stateMasterDTO, string token)
		{
			throw new NotImplementedException();
		}

		public Task<T> RemoveStateAsync<T>(int stateId, string token)
		{
			throw new NotImplementedException();
		}

		public Task<T> GetAllStateAsync<T>(string token)
		{
			return SendAsync<T>(new APIRequest()
			{
				ApiType = SD.ApiType.GET,
				Url = stateURL + "/api/StateMaster/GetAllState",
				Token = token
			});
		}

		public Task<T> GetStateAsync<T>(int stateId, string token)
		{
			return SendAsync<T>(new APIRequest()
			{
				ApiType = SD.ApiType.GET,
				Url = stateURL + "/api/StateMaster/GetState?stateId=" + stateId,
				Token = token
			});
		}

		public Task<T> UpdateStateAsync<T>(StateMasterDTO stateMasterDTO, string token)
		{
			throw new NotImplementedException();
		}
	}
}
