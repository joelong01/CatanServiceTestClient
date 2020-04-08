
using CatanSharedModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using CatanServiceMonitor;
using Windows.Foundation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace CatanSvcTestClient
{

    class Header
    {
        public string Description { get; set; }

    }

    class Sub : Header
    {
        public string Field1 { get; set; } = "field1";
        public int Foo { get; set; } = 10;
    }

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {


        ObservableCollection<ClientResourceModel> Players { get; set; } = new ObservableCollection<ClientResourceModel>();

        private string _hostName = "http://localhost:50919";
        private string GameName { get; } = "Game";
        ServiceMonitor _logMonitor;


        public static readonly DependencyProperty RESTReturnValueProperty = DependencyProperty.Register("RESTReturnValue", typeof(string), typeof(MainPage), new PropertyMetadata(""));
        public static readonly DependencyProperty ChangeLogProperty = DependencyProperty.Register("ChangeLog", typeof(string), typeof(MainPage), new PropertyMetadata(""));
        public static readonly DependencyProperty CurrentPlayerProperty = DependencyProperty.Register("CurrentPlayer", typeof(PlayerResources), typeof(MainPage), new PropertyMetadata(null));

        public PlayerResources CurrentPlayer
        {
            get => (PlayerResources)GetValue(CurrentPlayerProperty);
            set => SetValue(CurrentPlayerProperty, value);
        }


        public string ChangeLog
        {
            get => (string)GetValue(ChangeLogProperty);
            set => SetValue(ChangeLogProperty, value);
        }
        public string RESTReturnValue
        {
            get => (string)GetValue(RESTReturnValueProperty);
            set => SetValue(RESTReturnValueProperty, value);
        }

        public MainPage()
        {
            this.InitializeComponent();

        }



        private async void OnStart(object sender, RoutedEventArgs e)
        {
            HttpClient client = new HttpClient
            {
                Timeout = TimeSpan.FromMinutes(60)
            };

            Players.Clear();


            var players = new string[] { "Joe", "Dodgy", "Doug", "Chris" };
            var response = await client.DeleteAsync($"{_hostName}/api/catan/game/delete/{GameName}/");
            if (!response.IsSuccessStatusCode)
            {
                ChangeLog = $"Delete returned: {CatanSerializer.Serialize<HttpResponseMessage>(response, true)}";

            }
            foreach (var player in players)
            {
                var url = $"{_hostName}/api/catan/game/register/{GameName}/{player}";
                response = await client.PostAsync(url, null);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();

                    PlayerResources resources = CatanSerializer.Deserialize<PlayerResources>(json);
                    ClientResourceModel model = new ClientResourceModel()
                    {
                        PlayerResources = resources
                    };
                    model.Players.Add(player);

                    Players.Add(model); // created the control -- note PlayerName already set

                }

            }
        }



        private void OnBadTradeRequest(object sender, RoutedEventArgs e)
        {


            //HttpClient client = new HttpClient();
            //var resources = new PlayerResources
            //{
            //    Wheat = -2,
            //    Wood = 0,
            //    Ore = -2,
            //    Sheep = 0,
            //    Brick = 0,
            //    GoldMine = 0
            //};

            //var response = await client.PostAsync($"{_hostName}/api/catan/resourcecards/add/{_gameName}/{_player1Resources.PlayerName}", new StringContent(JsonSerializer.Serialize<PlayerResources>(resources), Encoding.UTF8, "application/json"));
            //if (response.StatusCode == HttpStatusCode.BadRequest)
            //{
            //    RESTReturnValue = await response.Content.ReadAsStringAsync();
            //}
        }

        private void OnTradeInGold(object sender, RoutedEventArgs e)
        {
            //HttpClient client = new HttpClient();
            //var resources = new PlayerResources
            //{
            //    Wheat = 1,
            //    Wood = 0,
            //    Ore = 0,
            //    Sheep = 0,
            //    Brick = 0,
            //    GoldMine = -1
            //};

            //var response = await client.PostAsync($"{_hostName}/api/catan/resourcecards/tradegold/{_gameName}/{_player1Resources.PlayerName}", new StringContent(JsonSerializer.Serialize<PlayerResources>(resources), Encoding.UTF8, "application/json"));
            //RESTReturnValue = FormatJson<PlayerResources>(await response.Content.ReadAsStringAsync());

        }

        private void OnDoTrade(object sender, RoutedEventArgs e)
        {
            //    HttpClient client = new HttpClient();           
            //    var resources = new PlayerResources[2] { _player1Trade.PlayerResources, _player2Trade.PlayerResources };            
            //    var response = await client.PostAsync($"{_hostName}/api/catan/resourcecards/trade/{_gameName}/{_player1Resources.PlayerName}/{_player2Resources.PlayerName}", new StringContent(JsonSerializer.Serialize<PlayerResources[]>(resources), Encoding.UTF8, "application/json"));
            //    RESTReturnValue = FormatJson<PlayerResources[]>(await response.Content.ReadAsStringAsync());
            //}

            //private async void OnTakeCard(object sender, RoutedEventArgs e)
            //{
            //    HttpClient client = new HttpClient();
            //    var response = await client.PostAsync($"{_hostName}/api/catan/resourcecards/take/{_gameName}/{_player1Resources.PlayerName}/{_player2Resources.PlayerName}", null);
            //    RESTReturnValue = FormatJson<PlayerResources>(await response.Content.ReadAsStringAsync());
        }



        private void OnTradeThreeForOne(object sender, RoutedEventArgs e)
        {
            //    HttpClient client = new HttpClient();
            //    var resources = new PlayerResources
            //    {
            //        Wheat = 1,
            //        Wood = 0,
            //        Ore = 0,
            //        Sheep = 0,
            //        Brick = 0,
            //        GoldMine = 0
            //    };

            //    _ = await client.PostAsync($"{_hostName}/api/catan/resourcecards/add/{_gameName}/{_player1Resources.PlayerName}", new StringContent(JsonSerializer.Serialize<PlayerResources>(resources), Encoding.UTF8, "application/json"));
            //    var response = await client.PostAsync($"{_hostName}/api/catan/resourcecards/meritimetrade/{_gameName}/{_player1Resources.PlayerName}/Wheat/3/Wood", null);
            //    RESTReturnValue = FormatJson<PlayerResources>(await response.Content.ReadAsStringAsync());
        }

        private void OnGrantResources(object sender, RoutedEventArgs e)
        {
            //HttpClient client = new HttpClient();
            //var response = await client.PostAsync($"{_hostName}/api/catan/resourcecards/add/{_gameName}/{_cmbPlayers.SelectedItem}", new StringContent(JsonSerializer.Serialize<PlayerResources>(_bank.PlayerResources), Encoding.UTF8, "application/json"));
            // RESTReturnValue = FormatJson<PlayerResources>(await response.Content.ReadAsStringAsync());            

        }

        private async void OnGrantResourcesToAll(object sender, RoutedEventArgs e)
        {
            HttpClient client = new HttpClient();
            foreach (var player in Players)
            {
                var resources = new PlayerResources
                {
                    Wheat = 1,
                    Wood = 1,
                    Ore = 1,
                    Sheep = 1,
                    Brick = 1,
                    GoldMine = 1,
                    PlayerName = player.PlayerResources.PlayerName,
                    GameName = GameName
                };
                //
                //  give the player some resources
                _ = await client.PostAsync($"{_hostName}/api/catan/resource/grant/{GameName}/{player.PlayerResources.PlayerName}", new StringContent(CatanSerializer.Serialize<PlayerResources>(resources), Encoding.UTF8, "application/json"));
            }
        }

        private void OnGetResources(object sender, RoutedEventArgs e)
        {

        }

        private void OnTest(object sender, RoutedEventArgs e)
        {

        }



        private void Player_OnCallback(CatanServiceMessage message)
        {
            ChangeLog = Environment.NewLine + message.MessageType + ":" + Environment.NewLine + message.Message + Environment.NewLine + ChangeLog;
        }
    }

}
