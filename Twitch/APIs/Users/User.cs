using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Twitch.APIs.Users
{
    public class User
    {
        //RESPONSE STRUCTURE
        /// <summary>
        /// A list of channels
        /// </summary>
        public struct Users { public List<Data> Data; }
        /// <summary>
        /// The data on a single channel
        /// </summary>
        public struct Data
        {
            public string ID { get; set; }
            public string Login { get; set; }
            public string Display_Name { get; set; }
            public string Type { get; set; }
            public string Broadcaster_Type { get; set; }
            public string Description { get; set; }
            public string Profile_Image_URL { get; set; }
            public string Offline_Image_URL { get; set; }
            public Int32 View_Count { get; set; }
            public string Email { get; set; }
            public string Created_At { get; set; }
        }
        //RESPONSE STRUCTURE

        /// <summary>
        /// Get a list of User details. Max 100 Usernames and ID (Combined)
        /// To access the Email address, scope required: user:read:email is not provided, email will be empty
        /// </summary>
        /// <param name="UserIDs">A list of User ID (Not Usernames)</param>
        /// <param name="Usernames">A list of Usernames (Not User IDs)</param>
        public static async Task<Users> GetAsync(List<String>? UserIDs = null, List<string>? Usernames = null)
        {
            Dictionary<string, string> authHeaders = new();
            authHeaders.Add("Authorization", $"Bearer {Twitch.Config.OAuthToken}");
            authHeaders.Add("Client-Id", Twitch.Config.ClientID);
            string URL = $"https://api.twitch.tv/helix/users?";

            if (UserIDs != null)
            {
                foreach (string UserID in UserIDs)
                {
                    URL += $"&id={UserID}";
                }
            }
            if (Usernames != null)
            {
                foreach (string Username in Usernames)
                {
                    URL += $"&login={Username}";
                }
            }
            URL = URL.Replace("?&", "?");

            string? result = (string?)await Twitch.Helpers.httpRequests.Get(URL, authHeaders);
            if (result == null) { return new(); }

            Users users = Newtonsoft.Json.JsonConvert.DeserializeObject<Users>(result);
            return users;
        }
    }
}
