using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Web;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace CatanSvcTestClient
{
    public sealed partial class PlayerResourceCard : UserControl
    {
        ObservableCollection<string> _lstPlayerList = new ObservableCollection<string>();
        public string _hostName { get; set; } = "http://localhost:50919";
        public string WssHostName { get; set; } = "ws://localhost:50919";
        public string _gameName { get; set; } = "Game";
        HttpClient _client = new HttpClient();
        string PlayerName { get; set; } = "";
        DispatcherTimer _timer = null;

        public static readonly DependencyProperty CaptionProperty = DependencyProperty.Register("Caption", typeof(string), typeof(PlayerResourceCard), new PropertyMetadata(""));
        public static readonly DependencyProperty ReadOnlyProperty = DependencyProperty.Register("ReadOnly", typeof(bool), typeof(PlayerResourceCard), new PropertyMetadata(false, ReadOnlyChanged));
        public static readonly DependencyProperty ShowUsersProperty = DependencyProperty.Register("ShowPlayers", typeof(bool), typeof(PlayerResourceCard), new PropertyMetadata(true, ShowPlayersChanged));
        public bool ShowUsers
        {
            get => (bool)GetValue(ShowUsersProperty);
            set => SetValue(ShowUsersProperty, value);
        }
        private static void ShowPlayersChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var depPropClass = d as PlayerResourceCard;
            var depPropValue = (bool)e.NewValue;
            depPropClass?.SetShowPlayers(depPropValue);
        }
        private void SetShowPlayers(bool value)
        {
            _cmbPlayers.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
        }

        public bool ReadOnly
        {
            get => (bool)GetValue(ReadOnlyProperty);
            set => SetValue(ReadOnlyProperty, value);
        }
        private static void ReadOnlyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var depPropClass = d as PlayerResourceCard;
            var depPropValue = (bool)e.NewValue;
            depPropClass?.SetReadOnly(depPropValue);
        }
        private void SetReadOnly(bool value)
        {
            _goldMine.ReadOnly = value;
            _wood.ReadOnly = value;
            _brick.ReadOnly = value; 
            _sheep.ReadOnly = value;
            _wheat.ReadOnly = value;
            _ore.ReadOnly = value;
            _goldMine.ReadOnly = value;
        }

        public string Caption
        {
            get => (string)GetValue(CaptionProperty);
            set => SetValue(CaptionProperty, value);
        }
        public PlayerResourceCard()
        {
            this.InitializeComponent();

        }

        private void StartCallBack()
        {
            _timer = new DispatcherTimer();
            _timer.Interval = new TimeSpan(0, 0, 1);
            _timer.Tick += Timer_CallBack;
            _timer.Start();
        }

        private async void Timer_CallBack(object sender, object o)
        {
            _timer.Stop();
            HttpClient client = new HttpClient();
            client.Timeout = TimeSpan.FromHours(12);
            while (true)
            {
                string url = $"{_hostName}/api/catan/cards/async/{_gameName}/{PlayerName}";
                try
                {
                    string playerResources = await client.GetStringAsync(url);
                    PlayerResources resources = JsonConvert.DeserializeObject<PlayerResources>(playerResources);
                    await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                    {
                        ResourceCount = resources.ResourceCount;
                    });
                }
                catch (Exception requestException)
                {
                    if (requestException.InnerException is WebException webException && webException.Status == WebExceptionStatus.Timeout)
                    {
                        this.TraceMessage($"[Player={_cmbPlayers.SelectedValue}] Timed out!");
                    }
                    else
                    {
                        this.TraceMessage($"[Player={_cmbPlayers.SelectedValue}] Exception Message: {requestException}");
                    }

                }                

            }
           
            

        }

        public void AddPlayers(IEnumerable<string> players)
        {
            foreach (var s in players)
            {
                _lstPlayerList.Add(s);
            }
        }

        public string SelectedPlayer => (string)_cmbPlayers.SelectedItem;

        

        public ResourceCountClass ResourceCount
        {
            get
            {
                return new ResourceCountClass
                {
                    GoldMine = _goldMine.Count,
                    Wood = _wood.Count,
                    Brick = _brick.Count,
                    Sheep = _sheep.Count,
                    Wheat = _wheat.Count,
                    Ore = _ore.Count
                };

            }
            set
            {
                _goldMine.Count = value.GoldMine;
                _wood.Count = value.Wood;
                _brick.Count = value.Brick;
                _sheep.Count = value.Sheep;
                _ore.Count = value.Ore;
                _wheat.Count = value.Wheat;

            }
        }

        private async void OnUserChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems is null) return;
            if ((string)e.AddedItems[0] != "")
            {

                PlayerName = (string)e.AddedItems[0];
                await UpdatePlayerResources();
                StartCallBack();
            }
        }
        private MessageWebSocket _messageWebSocket;
        private async Task EstablishWebSocketConnection()
        {
            if (_messageWebSocket != null)
            {
                _messageWebSocket.Dispose();
            }
            _messageWebSocket = new MessageWebSocket();

            // In this example, we send/receive a string, so we need to set the MessageType to Utf8.
            _messageWebSocket.Control.MessageType = SocketMessageType.Utf8;

            _messageWebSocket.MessageReceived += WebSocket_MessageReceived;
            _messageWebSocket.Closed += WebSocket_Closed;

            try
            {
                var uri = new Uri($"{WssHostName}/api/registersocket/{_gameName}/{PlayerName}");

                await _messageWebSocket.ConnectAsync(uri);
            }
            catch (Exception ex)
            {
                WebErrorStatus webErrorStatus = WebSocketError.GetStatus(ex.GetBaseException().HResult);
                this.TraceMessage($"Error creating websocket: {webErrorStatus.ToString()}");
            }
        }
        private async Task SendMessageUsingMessageWebSocketAsync(string message)
        {
            using (var dataWriter = new DataWriter(this._messageWebSocket.OutputStream))
            {
                dataWriter.WriteString(message);
                await dataWriter.StoreAsync();
                dataWriter.DetachStream();
            }
            this.TraceMessage("Sending message using MessageWebSocket: " + message);
        }

        private void WebSocket_Closed(IWebSocket sender, WebSocketClosedEventArgs args)
        {
            this.TraceMessage("WebSocket_Closed; Code: " + args.Code + ", Reason: \"" + args.Reason + "\"");
        }

        private void WebSocket_MessageReceived(MessageWebSocket sender, MessageWebSocketMessageReceivedEventArgs args)
        {
            try
            {
                using (DataReader dataReader = args.GetDataReader())
                {
                    dataReader.UnicodeEncoding = Windows.Storage.Streams.UnicodeEncoding.Utf8;
                    string message = dataReader.ReadString(dataReader.UnconsumedBufferLength);
                    this.TraceMessage("Message received from MessageWebSocket: " + message);

                }
            }
            catch (Exception ex)
            {
                Windows.Web.WebErrorStatus webErrorStatus = Windows.Networking.Sockets.WebSocketError.GetStatus(ex.GetBaseException().HResult);
                // Add additional code here to handle exceptions.
            }
        }

        public async Task UpdatePlayerResources()
        {
            //
            //  directly get the data an update the UI
            string url = $"{_hostName}/api/catan/cards/{_gameName}/{PlayerName}";
            string playerResources = await _client.GetStringAsync(url);
            PlayerResources resources = JsonConvert.DeserializeObject<PlayerResources>(playerResources);
            ResourceCount = resources.ResourceCount;
           
        }
    }
}
