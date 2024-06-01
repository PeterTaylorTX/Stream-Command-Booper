
using System.Text;

namespace Twitch.Helpers
{
    public class httpRequests
    {
        /// <summary>
        /// The max number of requests in a period
        /// </summary>
        public static Int32 Ratelimit_Max { get; set; }
        /// <summary>
        /// The number of requests left in this period
        /// </summary>
        public static Int32 Ratelimit_Remaining { get; set; }

        /// <summary>
        /// A http Get request
        /// </summary>
        /// <param name="URL">The API RURL</param>
        public async static Task<object?> Get(string URL, Twitch.Config config)
        {
            if (config.OAuthToken == null) { return null; }
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {Encoding.Unicode.GetString(config.OAuthToken)}");
            client.DefaultRequestHeaders.Add("Client-Id", config.ClientID);

            using (var response = await client.GetAsync(URL))
            {
                if (response.IsSuccessStatusCode) { GetRateLimits(response); return await response.Content.ReadAsStringAsync(); }
                string error = await response.Content.ReadAsStringAsync();
                return $"[ERROR]{error}";
            }
        }

        /// <summary>
        /// A http Post request
        /// </summary>
        /// <param name="URL">The API URL</param>
        /// <param name="content">The content to send to the API</param>
        /// <returns></returns>
        public async static Task<object?> Post(string URL, object content, Twitch.Config config)
        {
            if (config.OAuthToken == null) { return null; }
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {Encoding.Unicode.GetString(config.OAuthToken)}");
            client.DefaultRequestHeaders.Add("Client-Id", config.ClientID);

            StringContent Content;
            if (content.GetType() == typeof(string))
            {
                string? strContent = content.ToString();
                if (strContent == null) { return null; }
                Content = new StringContent(strContent, System.Text.Encoding.UTF8, "application/json");
            }
            else
            {
                string json = Newtonsoft.Json.JsonConvert.SerializeObject(content);
                Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            }

            using (var response = await client.PostAsync(URL, Content))
            {
                if (response.IsSuccessStatusCode) { GetRateLimits(response); return await response.Content.ReadAsStringAsync(); }
                if (response.StatusCode == System.Net.HttpStatusCode.TooManyRequests) { return "too many requests"; }
                string error = await response.Content.ReadAsStringAsync();
                return $"[ERROR]{error}";
            }
        }

        protected static void GetRateLimits(HttpResponseMessage response)
        {
            //RATE LIMIT
            var limitHeader = response.Headers.Where(h => h.Key == "Ratelimit-Limit").FirstOrDefault();
            string? strLimit = limitHeader.Value.FirstOrDefault();
            if (!string.IsNullOrWhiteSpace(strLimit)) { Ratelimit_Max = Int32.Parse(strLimit); }
            //RATE LIMIT

            //RATE LIMIT REMAINING
            var limitHeaderRemaining = response.Headers.Where(h => h.Key == "Ratelimit-Remaining").FirstOrDefault();
            string? strLimitRemaining = limitHeaderRemaining.Value.FirstOrDefault();
            if (!string.IsNullOrWhiteSpace(strLimitRemaining)) { Ratelimit_Remaining = Int32.Parse(strLimitRemaining); }
            //RATE LIMIT REMAINING

        }
    }
}
