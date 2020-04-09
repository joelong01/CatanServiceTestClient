
using Catan.Proxy;
using CatanServiceMonitor;
using CatanSharedModels;
using System;

using System.Threading.Tasks;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;


// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace CatanSvcTestClient
{


    public sealed partial class PlayerResourceCard : UserControl
    {
        public string _hostName { get; set; } = "http://localhost:5000"; // to do figure out how to set this via binding
        CatanProxy _proxy = new CatanProxy();
        private ServiceMonitor _serviceMonitor = new ServiceMonitor();
        ClientResourceModel _model = new ClientResourceModel();
        public ClientResourceModel ClientModel
        {
            get => _model;
            set
            {
                if (_model != value)
                {
                    _model = value;
                    if (String.IsNullOrEmpty(ClientModel.PlayerResources.PlayerName))
                    {
                        throw new Exception("PlayerName can't be null");
                    }

                    _serviceMonitor.GameName = ClientModel.PlayerResources.GameName;
                    _serviceMonitor.PlayerName = ClientModel.PlayerResources.PlayerName;
                    _serviceMonitor.HostName = _hostName;
                    _serviceMonitor.Start();
                }
            }
        }
        public event ServiceMonitorHandler PlayerControlCallback;

        public PlayerResourceCard()
        {
            this.InitializeComponent();
            _serviceMonitor.OnCallback += MonitorPlayerResources_OnCallback;
            _proxy.HostName = _hostName;

        }



        private void MonitorPlayerResources_OnCallback(CatanServiceMessage msg)
        {

            msg.ReceivingClient = ClientModel.PlayerResources.PlayerName;
            PlayerControlCallback?.Invoke(msg);


            foreach (var logEntry in msg.Log)
            {
                switch (logEntry.Action)
                {
                    case ServiceAction.Undefined:
                        break;
                    case ServiceAction.Purchased:
                        break;
                    case ServiceAction.PlayerAdded:
                        _model.Players.Clear();
                        foreach (var player in ((GameLog)logEntry).Players)
                        {
                            _model.Players.Add(player);
                        }
                        break;
                    case ServiceAction.UserRemoved:
                        break;
                    case ServiceAction.GameCreated:
                        break;
                    case ServiceAction.GameDeleted:
                        break;
                    case ServiceAction.TradeGold:
                        break;
                    case ServiceAction.GrantResources:
                        var playerResources = ((ResourceLog)logEntry).PlayerResources;
                        if (playerResources.PlayerName == _model.PlayerResources.PlayerName)
                        {
                            _model.PlayerResources = playerResources;
                        }
                        break;
                    case ServiceAction.TradeResources:
                        break;
                    case ServiceAction.TakeCard:
                        break;
                    case ServiceAction.Refund:
                        break;
                    case ServiceAction.MeritimeTrade:
                        break;
                    case ServiceAction.UpdatedTurn:
                        break;
                    case ServiceAction.LostToMonopoly:
                        break;
                    case ServiceAction.PlayedMonopoly:
                        break;
                    case ServiceAction.PlayedRoadBuilding:
                        break;
                    case ServiceAction.PlayedKnight:
                        break;
                    case ServiceAction.PlayedYearOfPlenty:
                        break;
                    default:
                        break;
                }
            }

        }


        private void OnTrade(object sender, RoutedEventArgs e)
        {

        }

        private async void OnBuyDevCard(object sender, RoutedEventArgs e)
        {

            _ = await PurchaseEntitlement(Entitlement.DevCard);


        }

        private async void OnBuySettlement(object sender, RoutedEventArgs e)
        {

            _ = await PurchaseEntitlement(Entitlement.Settlement);
        }

        private async void OnBuyCity(object sender, RoutedEventArgs e)
        {

            _ = await PurchaseEntitlement(Entitlement.City);

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

            //
            //  directly get the data an update the UI
            //string url = $"{_hostName}/api/catan/resource/{ClientModel.PlayerResources.GameName}/{ClientModel.PlayerResources.PlayerName}";
            //string playerResources = await _client.GetStringAsync(url);
            //ClientModel.PlayerResources = CatanSerializer.Deserialize<PlayerResources>(playerResources);
        }



        private async Task<bool> PurchaseEntitlement(Entitlement entitlement)
        {

            //var response = await _client.PostAsync($"{_hostName}/api/catan/purchase/{ClientModel.PlayerResources.GameName}/{ClientModel.PlayerResources.PlayerName}/{entitlement}", null);
            //if (response.IsSuccessStatusCode)
            //{
            //    string json = await response.Content.ReadAsStringAsync();
            //    ClientModel.PlayerResources = CatanSerializer.Deserialize<PlayerResources>(json);
            //    return true;
            //}
            //this.TraceMessage($"{response.ReasonPhrase}");
            //return false;
            throw new NotImplementedException();

        }

        private async void OnBuyRoad(object sender, RoutedEventArgs e)
        {
            await PurchaseEntitlement(Entitlement.Road);
        }

        private async void OnTradeGold(object sender, RoutedEventArgs e)
        {

            //var toTrade = ClientModel.TradeResources;
            //if (toTrade.GoldMine < 0) toTrade.GoldMine = -toTrade.GoldMine;



            //string url = $"{_hostName}/api/catan/resource/tradegold/{ClientModel.PlayerResources.GameName}/{ClientModel.PlayerResources.PlayerName}";
            //var response = await _client.PostAsync(url, new StringContent(CatanSerializer.Serialize<TradeResources>(toTrade), Encoding.UTF8, "application/json"));
            //if (!response.IsSuccessStatusCode)
            //{
            //    var msg = await response.Content.ReadAsStringAsync();
            //    this.TraceMessage($"{response.ReasonPhrase}: {msg}");

            //}
        }

        private async void OnGetLog(object sender, RoutedEventArgs e)
        {
            await _serviceMonitor.ExecuteHangingGet();
        }
    }
}
