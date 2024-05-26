using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Twitch
{
    public class PubSub
    {
        Client twitch;
        public TwitchLib.Api.TwitchAPI api;
        public TwitchLib.PubSub.TwitchPubSub pub;
        List<string> broadcaster = new();
        string channelID = string.Empty;

        public PubSub(Client _Twitch)
        {
            twitch = _Twitch;

            api = new TwitchLib.Api.TwitchAPI();
            pub = new TwitchLib.PubSub.TwitchPubSub();

            pub.OnPubSubServiceConnected += Pub_onPubSubServiceConnected;
            //pub.OnChannelPointsRewardRedeemed += Pub_OnChannelPointsRewardRedeemed;
            //pub.OnBitsReceivedV2 += twitch.Pub_OnBitsReceivedV2;
            pub.OnListenResponse += Pub_OnListenResponse;
            //Task.Run(async () => { await updateSubsList(); });
        }

        public void Connect()
        {
            api.Settings.ClientId = Twitch.Config.ClientID;
            api.Settings.AccessToken = Twitch.Config.OAuthToken;
            pub.Connect();
            if (String.IsNullOrEmpty(Twitch.Config.ChannelName)) { return; }
            broadcaster.Add(Twitch.Config.ChannelName);
        }

        private void Pub_OnListenResponse(object? sender, TwitchLib.PubSub.Events.OnListenResponseArgs e)
        {
            if (!e.Successful)
                Console.WriteLine($"Failed to listen! Response: {e.Response.Error}|{e.Topic}");
        }

        private async void Pub_onPubSubServiceConnected(object? sender, EventArgs e)
        {
            TwitchLib.Api.Helix.Models.Users.GetUsers.GetUsersResponse user = await api.Helix.Users.GetUsersAsync(null, broadcaster, Twitch.Config.OAuthToken);
            channelID = user.Users[0].Id;
            pub.ListenToBitsEventsV2(channelID);
            pub.ListenToChannelPoints(channelID);

            pub.SendTopics(Twitch.Config.OAuthToken);
        }


    }
}
