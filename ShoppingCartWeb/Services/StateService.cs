using ShoppingCartWeb.Models;
using ShoppingCartWeb.Models.Dto;
using ShoppingCartWeb.Services.IServices;
using ShoppingCartWeb.Utililty;

namespace ShoppingCartWeb.Services
{
	public class StateService : BaseService, IStateService
	{
		private readonly IHttpClientFactory _clientFactory;
		private string stateUrl;
		public StateService(IHttpClientFactory clientFactory, IConfiguration configuration) : base(clientFactory)
		{
			this._clientFactory = clientFactory;

			stateUrl = configuration.GetValue<string>("ServiceUrls:ShoppingCartAPI");
		}
        public Task<T> GetAllStateAsync<T>(string token)
        {
            return SendAsync<T>(new APIRequest()
            {
                ApiType = SD.ApiType.GET,
                Url = stateUrl + "/api/StateMaster/GetAllState",
                Token = token
            });
        }

        public Task<T> GetStateAsync<T>(int stateId, string token)
        {
            return SendAsync<T>(new APIRequest()
            {
                ApiType = SD.ApiType.GET,
                Url = stateUrl + "/api/StateMaster/GetState?stateId=" + stateId,
                Token = token
            });
        }

        public Task<T> GetAllStateByCountryIdAsync<T>(int countryId, string token)
        {
            return SendAsync<T>(new APIRequest()
            {
                ApiType = SD.ApiType.GET,
                Url = stateUrl + "/api/StateMaster/GetAllStateByCountryId?countryId=" + countryId,
                Token = token
            });
        }

        public Task<T> CreateStateAsync<T>(StateMasterDTO stateMasterDTO, string token)
        {
            return SendAsync<T>(new APIRequest()
            {
                ApiType = SD.ApiType.POST,
                Data = stateMasterDTO,
                Url = stateUrl + "/api/StateMaster/CreateState",
                Token = token
            });
        }


        public Task<T> RemoveStateAsync<T>(int stateId, string token)
		{
            return SendAsync<T>(new APIRequest()
            {
                ApiType = SD.ApiType.DELETE,
                Url = stateUrl + "/api/StateMaster/RemoveState?StateId=" + stateId,
                Token = token
            });
        }


        public Task<T> UpdateStateAsync<T>(StateMasterDTO stateMasterDTO, string token)
        {
            return SendAsync<T>(new APIRequest()
            {
                ApiType = SD.ApiType.PUT,
                Data = stateMasterDTO,
                Url = stateUrl + "/api/StateMaster/UpdateState",
                Token = token
            });
        }
    }
}
