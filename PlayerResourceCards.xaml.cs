
using CatanServiceMonitor;
using CatanSharedModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.Web;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace CatanSvcTestClient
{
    public sealed partial class PlayerResourceCard : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        ObservableCollection<string> _lstPlayerList = new ObservableCollection<string>();
        public string _hostName { get; set; } = "http://localhost:50919";
        HttpClient _client = new HttpClient();
        private ServiceMonitor _serviceMonitor = new ServiceMonitor();

        PlayerResources _playerResources = new PlayerResources();


        public PlayerResourceCard()
        {
            this.InitializeComponent();

        }


        public PlayerResources PlayerResources
        {
            get => _playerResources;
            set
            {
                if (value != _playerResources)
                {
                    _playerResources = value;
                    NotifyPropertyChanged();
                    if (_serviceMonitor != null)
                    {
                        _serviceMonitor.OnCallback -= MonitorPlayerResources_OnCallback;
                        _serviceMonitor.Stop();
                    }
                    _serviceMonitor.OnCallback += MonitorPlayerResources_OnCallback;
                    _serviceMonitor.MonitorUrl = $"{_hostName}/api/monitor/resources/{value.GameName}/{value.PlayerName}";
                    _serviceMonitor.Start();
                }
            }
        }

        private void MonitorPlayerResources_OnCallback(CatanServiceMessage msg)
        {
            if (msg.MessageType == CatanServiceMessageType.Error)
            {
                throw new Exception("Error returned by Monitor");
            }

            var pr = JsonSerializer.Deserialize<PlayerResources>(msg.Message);

            _playerResources.Brick = pr.Brick;
            _playerResources.Wood = pr.Wood;
            _playerResources.Wheat = pr.Wheat;
            _playerResources.Ore = pr.Ore;
            _playerResources.Sheep = pr.Sheep;
            _playerResources.GoldMine = pr.GoldMine;
            _playerResources.Players = pr.Players;
            _playerResources.DevCards = pr.DevCards;
            _playerResources.GoldMine = pr.GoldMine;

            _playerResources.DevCards = pr.DevCards;
        }

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        }

        public void AddPlayers(IEnumerable<string> players)
        {
            foreach (var s in players)
            {
                _lstPlayerList.Add(s);
            }
        }

        public async Task UpdatePlayerResources()
        {
            //
            //  directly get the data an update the UI
            string url = $"{_hostName}/api/catan/resourcecards/{PlayerResources.GameName}/{PlayerResources.PlayerName}";
            string playerResources = await _client.GetStringAsync(url);
            PlayerResources = JsonSerializer.Deserialize<PlayerResources>(playerResources);

        }



        private void OnTrade(object sender, RoutedEventArgs e)
        {

        }

        private async void OnBuyDevCard(object sender, RoutedEventArgs e)
        {

            var pr = new PlayerResources()
            {
                Brick = 0,
                Wood = 0,
                Sheep = -1,
                Ore = -1,
                Wheat = -1,
                GoldMine = 0,
                GameName = PlayerResources.GameName,
                PlayerName = PlayerResources.PlayerName
            };

            var response = await _client.PostAsync($"{_hostName}/api/catan/resourcecards/add/{PlayerResources.GameName}/{PlayerResources.PlayerName}",
                           new StringContent(JsonSerializer.Serialize<PlayerResources>(pr), Encoding.UTF8, "application/json"));

            this.TraceMessage($"{response.ReasonPhrase}");
        }

        private async void OnBuySettlement(object sender, RoutedEventArgs e)
        {

            var pr = new PlayerResources()
            {
                Brick = -1,
                Wood = -1,
                Sheep = -1,
                Ore = 0,
                Wheat = -1,
                GoldMine = 0,
                GameName = PlayerResources.GameName,
                PlayerName = PlayerResources.PlayerName
            };

            var response = await _client.PostAsync($"{_hostName}/api/catan/resourcecards/add/{PlayerResources.GameName}/{PlayerResources.PlayerName}",
                           new StringContent(JsonSerializer.Serialize<PlayerResources>(pr), Encoding.UTF8, "application/json"));
            this.TraceMessage($"{response.ReasonPhrase}");

        }

        private async void OnBuyCity(object sender, RoutedEventArgs e)
        {

            var pr = new PlayerResources()
            {
                Brick = 0,
                Wood = 0,
                Sheep = 0,
                Ore = -3,
                Wheat = -2,
                GoldMine = 0,
                GameName = PlayerResources.GameName,
                PlayerName = PlayerResources.PlayerName
            };

            var response = await _client.PostAsync($"{_hostName}/api/catan/resourcecards/add/{PlayerResources.GameName}/{PlayerResources.PlayerName}",
                           new StringContent(JsonSerializer.Serialize<PlayerResources>(pr), Encoding.UTF8, "application/json"));
            this.TraceMessage($"{response.ReasonPhrase}");

        }

        private void OnMeritimeTrade(object sender, RoutedEventArgs e)
        {

        }

        private void OnTakeCard(object sender, RoutedEventArgs e)
        {

        }

        private void OnPlayDevCard(object sender, RoutedEventArgs e)
        {

        }

        private async void OnGetResources(object sender, RoutedEventArgs e)
        {

            var newResources = GetTradeResources();
            var response = await _client.PostAsync($"{_hostName}/api/catan/resourcecards/add/{PlayerResources.GameName}/{PlayerResources.PlayerName}",
                            new StringContent(JsonSerializer.Serialize<PlayerResources>(newResources), Encoding.UTF8, "application/json"));

            this.TraceMessage($"{response}");
        }

        private PlayerResources GetTradeResources()
        {
            var pr = new PlayerResources()
            {
                Brick = PlayerResources.TradeResources.Brick,
                Wood = PlayerResources.TradeResources.Wood,
                Sheep = PlayerResources.TradeResources.Sheep,
                Ore = PlayerResources.TradeResources.Ore,
                Wheat = PlayerResources.TradeResources.Wheat,
                GoldMine = PlayerResources.TradeResources.GoldMine,
                GameName = PlayerResources.GameName,
                PlayerName = PlayerResources.PlayerName

            };
            return pr;
        }

        private async void OnBuyRoad(object sender, RoutedEventArgs e)
        {
            var pr = new PlayerResources()
            {
                Brick = -1,
                Wood = -1,
                Sheep = 0,
                Ore = 0,
                Wheat = 0,
                GoldMine = 0,
                GameName = PlayerResources.GameName,
                PlayerName = PlayerResources.PlayerName
            };

            var response = await _client.PostAsync($"{_hostName}/api/catan/resourcecards/add/{PlayerResources.GameName}/{PlayerResources.PlayerName}",
                           new StringContent(JsonSerializer.Serialize<PlayerResources>(pr), Encoding.UTF8, "application/json"));

            this.TraceMessage($"{response.ReasonPhrase}");
        }

        private async void OnTradeGold(object sender, RoutedEventArgs e)
        {

            var toTrade = GetTradeResources();
            if (toTrade.GoldMine < 0) toTrade.GoldMine = -toTrade.GoldMine;

            if (toTrade.TotalResources - 2 * toTrade.GoldMine != 0)
            {
                this.TraceMessage($"Invalid Gold Trade.  Gold={toTrade.GoldMine} Total={toTrade.TotalResources}");
                return;
            }

            string url = $"{_hostName}/api/catan/resourcecards/tradegold/{toTrade.GameName}/{toTrade.PlayerName}";
            var response = await _client.PostAsync(url, new StringContent(JsonSerializer.Serialize<PlayerResources>(toTrade), Encoding.UTF8, "application/json"));
            if (!response.IsSuccessStatusCode)
            {
                var msg = await response.Content.ReadAsStringAsync();
                this.TraceMessage($"{response.ReasonPhrase}: {msg}");
            }



        }
    }
}
