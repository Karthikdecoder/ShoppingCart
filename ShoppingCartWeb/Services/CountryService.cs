using ShoppingCartWeb.Models;
using ShoppingCartWeb.Models.Dto;
using ShoppingCartWeb.Services.IServices;
using ShoppingCartWeb.Utililty;

namespace ShoppingCartWeb.Services
{
	public class CountryService : BaseService, ICountryService
	{
		private readonly IHttpClientFactory _clientFactory;
		private string countryUrl;
		public CountryService(IHttpClientFactory clientFactory, IConfiguration configuration) : base(clientFactory)
		{
			this._clientFactory = clientFactory;

			countryUrl = configuration.GetValue<string>("ServiceUrls:ShoppingCartAPI");
		}
		public Task<T> CreateCountryAsync<T>(CountryMasterDTO countryMasterDTO, string token)
		{
			throw new NotImplementedException();
		}

		public Task<T> DeleteCountryAsync<T>(int countryId, string token)
		{
			throw new NotImplementedException();
		}

		public Task<T> GetAllCountryAsync<T>(string token)
		{
			return SendAsync<T>(new APIRequest()
			{
				ApiType = SD.ApiType.GET,
				Url = countryUrl + "/api/CountryMaster/GetAllCountry",
				Token = token
			});
		}

		public Task<T> GetCountryAsync<T>(int countryId, string token)
		{
			return SendAsync<T>(new APIRequest()
			{
				ApiType = SD.ApiType.GET,
				Url = countryUrl + "/api/CountryMaster/GetCountry?countryId=" + countryId,
				Token = token
			});
		}

		public Task<T> UpdateCountryAsync<T>(CountryMasterDTO countryMasterDTO, string token)
		{
			throw new NotImplementedException();
		}
	}
}
