using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Twitch.Helpers
{
    public class httpRequests
    {
        public async static Task<object?> Get(string URL, Dictionary<string, string> headers)
        {
            HttpClient client = new HttpClient();

            foreach (KeyValuePair<string, string> kvp in headers)
            {
                client.DefaultRequestHeaders.Add(kvp.Key, kvp.Value);
            }

            using (var response = await client.GetAsync(URL))
            {
                if (response.IsSuccessStatusCode) { return await response.Content.ReadAsStringAsync(); }
                string error = await response.Content.ReadAsStringAsync();
                return null;
            }
        }

        public async static Task<object?> Post(string URL, Dictionary<string, string> headers, object content)
        {
            HttpClient client = new HttpClient();

            foreach (KeyValuePair<string, string> kvp in headers)
            {
                client.DefaultRequestHeaders.Add(kvp.Key, kvp.Value);
            }

            StringContent Content;
            if (content.GetType() == typeof(string))
            {
                string? strContent = content.ToString();
                if(strContent == null) { return null; }
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
