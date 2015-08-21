using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace HttpLimited
{
	public class HttpRatelimitedClient
	{
		private HttpClient _httpClient;
		private RateLimiter _rateLimiter = new RateLimiter();

		public HttpRatelimitedClient()
		{
			_httpClient = new HttpClient();
		}

		public HttpRatelimitedClient(HttpMessageHandler handler)
		{
			_httpClient = new HttpClient(handler);
		}

		public HttpRatelimitedClient(HttpMessageHandler handler, bool disposeHanler)
		{
			_httpClient = new HttpClient(handler, disposeHanler);
		}


		public async Task<HttpResponseMessage> GetAsync(Uri requestUri)
		{
			await _rateLimiter.WaitForFree();

			return await _httpClient.GetAsync(requestUri);
		} 
	}
}
