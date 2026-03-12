using Newtonsoft.Json;
using Shared;
using Shared.Dto.OGS;

namespace OGS
{
    public class OGSService : ServiceBase
    {
        private HttpClient _client;
        private string _baseAddress = "https://online-go.com";

        private double MIN_RATING = 100;
        private double MAX_RATING = 6000;
        private double A = 525;
        private double C = 23.15;

        public OGSService(HttpClient httpClient)
        {

            _client = httpClient;
        }

        public double RatingToRank(double rating)
        {
            return Math.Log(Math.Min(MAX_RATING, Math.Max(MIN_RATING, rating)) / A) * C;
        }

        public async Task<OGSPlayer?> GetPlayer(string userId)
        {
            string url = _baseAddress + "/api/v1/players?username=" + userId;

            var res = await _client.GetAsync(url);
            var content = await res.Content.ReadAsStringAsync();

            var results =  JsonConvert.DeserializeObject<GetPlayersResult>(content);

            if (results != null && results.Count == 1)
                return results.Results.First();
            return null;
        }
    }
}
