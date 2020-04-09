
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
using Catan.Proxy;

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

        private string _hostName = "http://localhost:5000";
        private string GameName { get; } = "Game";
        

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
            using (CatanProxy proxy = new CatanProxy() { HostName = _hostName })
            {
                await proxy.StartGame(GameName);

            }
        }

        private void AddToChangeLog(string message)
        {
            ChangeLog += message + Environment.NewLine;
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

        private async void OnGrantResources(object sender, RoutedEventArgs e)
        {
            using (CatanProxy proxy = new CatanProxy() { HostName = _hostName })
            {
                var resources = new TradeResources
                {
                    Wheat = 1,
                    Wood = 1,
                    Ore = 1,
                    Sheep = 1,
                    Brick = 1,
                    GoldMine = 1,
                };
                //
                //  give the player some resources
                //   await proxy.GrantResources(GameName, CurrentPlayer.PlayerName, resources);

            }

        }

        private async void OnGrantResourcesToAll(object sender, RoutedEventArgs e)
        {
            using (CatanProxy proxy = new CatanProxy() { HostName = _hostName })
            {
                foreach (var player in Players)
                {
                    var resources = new TradeResources
                    {
                        Wheat = 1,
                        Wood = 1,
                        Ore = 1,
                        Sheep = 1,
                        Brick = 1,
                        GoldMine = 1,
                    };
                    //
                    //  give the player some resources
                    await proxy.GrantResources(GameName, player.PlayerResources.PlayerName, resources);
                }
            }
        }

        private void OnGetResources(object sender, RoutedEventArgs e)
        {

        }

        private void OnTest(object sender, RoutedEventArgs e)
        {
            List<object> list = new List<object>();
            list.Add(new Sub()
            {
                Description = "one",
                Field1 = "two",
                Foo = 5
            });
            list.Add(new Sub()
            {
                Description = "two",
                Field1 = "three",
                Foo = 6
            });

            string json = CatanSerializer.Serialize<List<object>>(list, true);
            this.TraceMessage(json);
            var test = CatanSerializer.Deserialize<List<object>>(json);
        }



        private void Player_OnCallback(CatanServiceMessage message)
        {
            
            AddToChangeLog($"Begin: [Reciever={message.ReceivingClient}]");            
            foreach (object o in message.Log)
            {                
                AddToChangeLog(CatanSerializer.Serialize<object>(o, true));
            }
            AddToChangeLog($"End: [Reciever={message.ReceivingClient}]");
            AddToChangeLog("-------------------------------------------------------------------------------------------------------");
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CurrentPlayer = e.AddedItems[0] as PlayerResources;
        }

        private void OnClearLog(object sender, RoutedEventArgs e)
        {
            ChangeLog = "";
        }

        private async void AddPlayers(object sender, RoutedEventArgs e)
        {
            ChangeLog = "";
            using (CatanProxy proxy = new CatanProxy() { HostName = _hostName })
            {


                Players.Clear();


                var players = new string[] { "Joe", "Dodgy" };
                var response = await proxy.DeleteGame(GameName);
                if (response == null)
                {
                    AddToChangeLog(proxy.LastErrorString);
                }
                else
                {
                    AddToChangeLog(CatanSerializer.Serialize<CatanResult>(response, true));
                }

                foreach (var player in players)
                {
                    var resources = await proxy.Register(new GameInfo(), GameName, player);
                    if (resources != null)
                    {
                        ClientResourceModel model = new ClientResourceModel()
                        {
                            PlayerResources = resources
                        };
                        model.Players.Add(player);

                        Players.Add(model); // created the control -- note PlayerName already set
                        AddToChangeLog(CatanSerializer.Serialize<PlayerResources>(resources, true));

                    }
                    else
                    {
                        AddToChangeLog("Error!");
                        if (proxy.LastError != null)
                        {
                            AddToChangeLog(CatanSerializer.Serialize<CatanResult>(proxy.LastError, true));
                        }
                        else
                        {
                            AddToChangeLog(proxy.LastErrorString);
                        }
                    }
                }

            }
        }
    }

}
