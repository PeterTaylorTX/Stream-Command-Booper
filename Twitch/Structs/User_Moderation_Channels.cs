using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Twitch.Structs
{
    public class User_Moderation_Channels
    {
        public struct Channels { public List<Data> Data; }
        public struct Data
        {
            public string broadcaster_id { get; set; }
            public string broadcaster_login { get; set; }
            public string broadcaster_name { get; set; }
        }

    }
}
