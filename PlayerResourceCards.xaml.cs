
using CatanServiceMonitor;
using CatanSharedModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
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
        ServiceMonitor _logMonitor;

        PlayerResources _playerResources = new PlayerResources();
        PlayerResources _tradeResources = new PlayerResources();

        public PlayerResourceCard()
        {
            this.InitializeComponent();

        }


        public PlayerResources PlayerResources
        {
            get => _playerResources;
            set
            {
                _playerResources = value;                
                NotifyPropertyChanged();
            }
        }
        public PlayerResources TradeResources
        {
            get => _tradeResources;
            set
            {
                _tradeResources = value;
                NotifyPropertyChanged();
            }
        }

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        }


        private void StartCallBack()
        {
            string url = $"{_hostName}/api/monitor/resources/{PlayerResources.GameName}/{PlayerResources.PlayerName}";
            _logMonitor = new ServiceMonitor(url);
            _logMonitor.OnCallback += OnMonitorPlayerResources;

            _logMonitor.Start(); ;

        }

        private async void OnMonitorPlayerResources(CatanServiceMessage message)
        {
            if (message.MessageType == CatanServiceMessageType.Error)
            {
                this.TraceMessage($"Error from service: {message.Message}");
                _logMonitor.Stop();
            }
            PlayerResources resources = JsonSerializer.Deserialize<PlayerResources>(message.Message);
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                this.PlayerResources = resources;
            });
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
    }
}
