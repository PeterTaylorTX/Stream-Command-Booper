using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Twitch.Helpers
{
    public class httpRequests
    {
        public async static Task<object?> Get(string URL, Dictionary<string,string> headers)
        {
            HttpClient client = new HttpClient();

            foreach(KeyValuePair<string,string> kvp in headers)
            {
                client.DefaultRequestHeaders.Add(kvp.Key, kvp.Value);
            }

            using (var response = await client.GetAsync(URL))
            {
                if(response.IsSuccessStatusCode) { return await response.Content.ReadAsStringAsync(); }
                return null;
            }
        }
    }
}
