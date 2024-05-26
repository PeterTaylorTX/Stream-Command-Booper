using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;
using TwitchLib.Client.Models;
using TwitchLib.Communication.Interfaces;
using System.Diagnostics;
using TwitchLib.Client.Events;
using TwitchLib.Communication.Events;

namespace Twitch
{
    public class Client : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public bool Initialized { get; set; } = false;

        /// <summary>
        /// Twitch Client
        /// </summary>
        public TwitchLib.Client.TwitchClient TwitchClient
        {
            get => _TwitchClient;
            set { _TwitchClient = value; if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("TwitchClient")); } }
        }
        protected TwitchLib.Client.TwitchClient _TwitchClient = new TwitchLib.Client.TwitchClient();

        /// <summary>
        /// Twitch pubSub, API's for accessing subscriber, Bit, and Channel Point notifications
        /// </summary>
        public PubSub pubSub;
        public Client()
        {
            if (this.TwitchClient == null) { this.TwitchClient = new TwitchLib.Client.TwitchClient(); }
            this.pubSub = new PubSub(this);
        }

        /// <summary>
        /// Connect to Twitch Services
        /// </summary>
        public bool Connect()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(Config.OAuthToken)) { return false; }
                if (string.IsNullOrWhiteSpace(Config.ChannelName)) { return false; }

                if (TwitchClient.JoinedChannels != null && TwitchClient.JoinedChannels.Count() >= 1) { return true; };
                TwitchClient.Initialize(new ConnectionCredentials(Config.ChannelName, Config.OAuthToken), Config.ChannelName);

                TwitchClient.OnConnectionError += onConnectionError;
                TwitchClient.OnError += onError;
                TwitchClient.OnConnected += onConnected;
                TwitchClient.OnJoinedChannel += onJoinedChannel;
                TwitchClient.OnDisconnected += onDisconnected;
                TwitchClient.OnFailureToReceiveJoinConfirmation += onFailureToReceiveJoinConfirmation;
                TwitchClient.OnIncorrectLogin += onIncorrectLogin;
                //TwitchClient.OnMessageReceived += onMessageReceived;
                //TwitchClient.OnChatCleared += onChatClearedReceived;
                //TwitchClient.OnNewSubscriber += onSubscriberReceived;
                //TwitchClient.OnReSubscriber += onReSubscriberReceived;
                //TwitchClient.OnChatCommandReceived += onChatCommandReceived;
                TwitchClient.Connect();
                pubSub.Connect();
                this.Initialized = TwitchClient.IsConnected;
                return TwitchClient.IsConnected;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Bot joined the channel
        /// </summary>
        private void onJoinedChannel(object? sender, OnJoinedChannelArgs e)
        {
            Console.WriteLine($"joined {e.Channel}");
            if (!Debugger.IsAttached)
            {
                //TwitchClient.SendMessage(this.Config.channelName, $"Chat Interaction Bot Joined");
            }
            Config.Save(); // Save the config file on successful connection
        }

        /// <summary>
        /// Log in Error
        /// </summary>
        private void onIncorrectLogin(object? sender, OnIncorrectLoginArgs e)
        {
            Console.WriteLine($"[ERROR] Incorrect Log In | {e.Exception.Message}");
        }

        /// <summary>
        /// Join Channel confirmation not received
        /// </summary>
        private void onFailureToReceiveJoinConfirmation(object? sender, OnFailureToReceiveJoinConfirmationArgs e)
        {
            Console.WriteLine($"[ERROR] Failed to join channel {e.Exception.Channel ?? string.Empty}");
        }

        /// <summary>
        /// Disconnected from Twitch
        /// </summary>
        private void onDisconnected(object? sender, OnDisconnectedEventArgs e)
        {
            Console.WriteLine($"DISCONNECTED");
        }

        /// <summary>
        /// Connected to Twitch
        /// </summary>
        private void onConnected(object? sender, OnConnectedArgs e)
        {
            Console.WriteLine($"CONNECTED as {e.BotUsername}");
        }

        /// <summary>
        /// On Twitch Error
        /// </summary>
        private void onError(object? sender, OnErrorEventArgs e)
        {
            Console.WriteLine($"[ERROR] {e.Exception.Message}");
        }

        /// <summary>
        /// Connection Error
        /// </summary>
        private void onConnectionError(object? sender, OnConnectionErrorArgs e)
        {
            Console.WriteLine($"[ERROR] Failed to connect: {e.BotUsername}");
        }

    }
}
