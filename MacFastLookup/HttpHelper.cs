using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace MacFastLookup
{
    public class HttpHelper
    {
        private static readonly HttpClient _httpClient = new HttpClient();
        public async Task<string> GetAsync(string url)
        {
            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                return responseBody;
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"Request error: {e.Message}");
                return null;
            }
        }
    }
}
