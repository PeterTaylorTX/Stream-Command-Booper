
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
                /// <summary>
                /// An ID that identifies the user.
                /// </summary>
                public string ID { get; set; } = string.Empty;
                /// <summary>
                /// The user’s login name.
                /// </summary>
                public string Login { get; set; } = string.Empty;
                /// <summary>
                /// The user’s display name
                /// </summary>
                public string Display_Name { get; set; } = string.Empty;
                /// <summary>
                /// The type of user. Possible values are: admin — Twitch administrator, global_mod, staff — Twitch staff, "" — Normal user
                /// </summary>
                public string Type { get; set; } = string.Empty;
                /// <summary>
                /// The type of broadcaster. Possible values are: affiliate — An affiliate broadcaster affiliate broadcaster, partner — A partner broadcaster partner broadcaster, "" — A normal broadcaster
                /// </summary>
                public string Broadcaster_Type { get; set; } = string.Empty;
                /// <summary>
                /// The user’s description of their channel
                /// </summary>
                public string Description { get; set; } = string.Empty;
                /// <summary>
                /// A URL to the user’s profile image.
                /// </summary>
                public string Profile_Image_URL { get; set; } = string.Empty;
                /// <summary>
                /// A URL to the user’s offline image.
                /// </summary>
                public string Offline_Image_URL { get; set; } = string.Empty;
                /// <summary>
                /// (deprecated) The number of times the user’s channel has been viewed.
                /// </summary>
                public Int32 View_Count { get; set; } = 0;
                /// <summary>
                /// The user’s verified email address. The object includes this field only if the user access token includes the user:read:email scope
                /// </summary>
                public string Email { get; set; } = string.Empty;
                /// <summary>
                /// The UTC date and time that the user’s account was created. The timestamp is in RFC3339 format
                /// </summary>
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
                /// <summary>
                /// The Channel ID
                /// </summary>
                public string Broadcaster_ID { get; set; } = string.Empty;
                /// <summary>
                /// The Channel Username
                /// </summary>
                public string Broadcaster_Login { get; set; } = string.Empty;
                /// <summary>
                /// The Channel Display Name
                /// </summary>
                public string Broadcaster_Name { get; set; } = string.Empty;
                /// <summary>
                /// Is the Channel Selected
                /// </summary>
                public bool IsSelected { get; set; } = false;
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
        /// A Timeout request
        /// </summary>
        public class Timeout_User_Request
        {
            /// <summary>
            /// A list of users to Timeout
            /// </summary>
            public Timeout_User_Request_Data data { get; set; } = new();
            /// <summary>
            /// The user to Timeout
            /// </summary>
            public class Timeout_User_Request_Data
            {
                /// <summary>
                /// The UserID to Timeout
                /// </summary>
                public string user_id { get; set; } = string.Empty;
                /// <summary>
                /// The reason for the Timeout
                /// </summary>
                public string reason { get; set; } = string.Empty;
                /// <summary>
                /// The timeout duration in seconds
                /// </summary>
                public Int32 duration { get; set; } = 60;
            }
        }

        /// <summary>
        /// A Ban Request
        /// </summary>
        public class Ban_User_Request
        {
            /// <summary>
            /// A list of users to ban
            /// </summary>
            public Ban_User_Request_Data data { get; set; } = new();
            /// <summary>
            /// The user to ban
            /// </summary>
            public class Ban_User_Request_Data
            {
                /// <summary>
                /// The UserID to ban
                /// </summary>
                public string user_id { get; set; } = string.Empty;
                /// <summary>
                /// The reason for the ban
                /// </summary>
                public string reason { get; set; } = string.Empty;
            }
        }
        /// <summary>
        /// The ban user response
        /// </summary>
        public class Ban_User_Response
        {
            /// <summary>
            /// A list that contains the user you successfully banned or put in a timeout.
            /// </summary>
            public List<Bans> Data { get; set; } = new();
            /// <summary>
            /// The response to a ban request
            /// </summary>
            public class Bans
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

        /// <summary>
        /// The Ban status
        /// </summary>
        public enum BannedResponse
        {
            Banned = 0,
            AlreadyBanned = 1,
            NotBanned = 2,
            TooManyRequests = 3
        }
    }
}
