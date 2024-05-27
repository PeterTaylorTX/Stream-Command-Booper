using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Twitch.Helpers
{
    public class httpRequests
    {
        /// <summary>
        /// A http Get request
        /// </summary>
        /// <param name="URL">The API RURL</param>
        public async static Task<object?> Get(string URL)
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {Twitch.Config.OAuthToken}");
            client.DefaultRequestHeaders.Add("Client-Id", Twitch.Config.ClientID);

            using (var response = await client.GetAsync(URL))
            {
                if (response.IsSuccessStatusCode) { return await response.Content.ReadAsStringAsync(); }
                string error = await response.Content.ReadAsStringAsync();
                return null;
            }
        }

        /// <summary>
        /// A http Post request
        /// </summary>
        /// <param name="URL">The API URL</param>
        /// <param name="content">The content to send to the API</param>
        /// <returns></returns>
        public async static Task<object?> Post(string URL, object content)
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {Twitch.Config.OAuthToken}");
            client.DefaultRequestHeaders.Add("Client-Id", Twitch.Config.ClientID);

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
                if (response.IsSuccessStatusCode) { return await response.Content.ReadAsStringAsync(); }
                string error = await response.Content.ReadAsStringAsync();
                return null;
            }
        }

    }
}
