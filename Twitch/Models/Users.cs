using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Twitch.Models.Users;

namespace Twitch.Models
{
    public class Users
    {
        /// <summary>
        /// A list of channels
        /// </summary>
        public class Channels
        {
            /// <summary>
            /// A list of channels
            /// </summary>
            public List<Channel> Data { get; set; } = new();
            /// <summary>
            /// The data on a single channel
            /// </summary>
            public class Channel
            {
                public string ID { get; set; } = string.Empty;
                public string Login { get; set; } = string.Empty;
                public string Display_Name { get; set; } = string.Empty;
                public string Type { get; set; } = string.Empty;
                public string Broadcaster_Type { get; set; } = string.Empty;
                public string Description { get; set; } = string.Empty;
                public string Profile_Image_URL { get; set; } = string.Empty;
                public string Offline_Image_URL { get; set; } = string.Empty;
                public Int32 View_Count { get; set; } = 0;
                public string Email { get; set; } = string.Empty;
                public string Created_At { get; set; } = string.Empty;
            }
        }
        #region User Moderation Channels
        /// <summary>
        /// A list of channels
        /// </summary>
        public class User_Moderation_Channels
        {
            /// <summary>
            /// A list of channels
            /// </summary>
            public List<User_Moderation_Channels_Data> Data = new();
            /// <summary>
            /// The current page
            /// </summary>
            public User_Moderation_Channels_Pagination Pagination = new();
            /// <summary>
            /// The data on a single channel
            /// </summary>
            public class User_Moderation_Channels_Data
            {
                public string Broadcaster_ID { get; set; } = string.Empty;
                public string Broadcaster_Login { get; set; } = string.Empty;
                public string Broadcaster_Name { get; set; } = string.Empty;
            }
            /// <summary>
            /// The current page
            /// </summary>
            public class User_Moderation_Channels_Pagination
            {
                public string Cursor { get; set; } = string.Empty;
            }
        }
        #endregion

        /// <summary>
        /// The ban user response
        /// </summary>
        public class Ban_User_Response
        {
            /// <summary>
            /// A list that contains the user you successfully banned or put in a timeout.
            /// </summary>
            public List<bans> Data { get; set; } = new();
            public class bans
            {
                /// <summary>
                /// The broadcaster whose chat room the user was banned from chatting in.
                /// </summary>
                public string Broadcaster_ID { get; set; } = string.Empty;
                /// <summary>
                /// The moderator that banned or put the user in the timeout.
                /// </summary>
                public string Moderator_ID { get; set; } = string.Empty;
                /// <summary>
                /// The user that was banned or put in a timeout.
                /// </summary>
                public string User_ID { get; set; } = string.Empty;

                /// <summary>
                ///  The UTC date and time (in RFC3339 format) that the ban or timeout was placed.
                /// </summary>
                public string Created_At { get; set; } = string.Empty;
                /// <summary>
                /// The UTC date and time(in RFC3339 format) that the timeout will end.Is null if the user was banned instead of being put in a timeout.
                /// </summary>
                public string End_Time { get; set; } = string.Empty;
            }
        }

    }
}
