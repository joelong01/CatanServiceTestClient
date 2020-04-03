
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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace CatanSvcTestClient
{

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        ObservableCollection<string> _firstPlayerList = new ObservableCollection<string>();
        ObservableCollection<string> _secondPlayerList = new ObservableCollection<string>();
        ObservableCollection<string> _lstPlayerList = new ObservableCollection<string>();

        private string _hostName = "http://localhost:50919";
        private string _gameName = "Game";
        DispatcherTimer _timer = null;

        public static readonly DependencyProperty RESTReturnValueProperty = DependencyProperty.Register("RESTReturnValue", typeof(string), typeof(MainPage), new PropertyMetadata(""));
        public static readonly DependencyProperty ChangeLogProperty = DependencyProperty.Register("ChangeLog", typeof(string), typeof(MainPage), new PropertyMetadata(""));
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
            _lstPlayerList.Add("Joe");
            _lstPlayerList.Add("Dodgy");
            _lstPlayerList.Add("Doug");
            _lstPlayerList.Add("Chris");
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromMilliseconds(1);
            _timer.Tick += Timer_CallBack;
            

        }
        private async void Timer_CallBack(object sender, object o)
        {
            _timer.Stop();
            HttpClient client = new HttpClient
            {
                Timeout = TimeSpan.FromHours(12)
            };
            while (true)
            {
                string url = $"{_hostName}/api/monitor/resources/all";
                try
                {
                    string playerResources = await client.GetStringAsync(url);
                    await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                    {
                        ChangeLog = Environment.NewLine + playerResources + Environment.NewLine + ChangeLog ;

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
        private async void OnStart(object sender, RoutedEventArgs e)
        {
            HttpClient client = new HttpClient();
            _ = await client.DeleteAsync($"{_hostName}/api/catan/game/delete/{_gameName}/");
            foreach (var player in _lstPlayerList)
            {
                _ = await client.PostAsync($"{_hostName}/api/catan/game/register/{_gameName}/{player}", null);
                var resources = new PlayerResources
                {
                    Wheat = 1,
                    Wood = 1,
                    Ore = 1,
                    Sheep = 1,
                    Brick = 1,
                    GoldMine = 1
                };
                //
                //  give the player some resources
                _ = await client.PostAsync($"{_hostName}/api/catan/resourcecards/add/{_gameName}/{player}", new StringContent(JsonSerializer.Serialize<PlayerResources>(resources), Encoding.UTF8, "application/json"));
            }

            
            
          

            RESTReturnValue = FormatJson<List<string>>(await client.GetStringAsync($"{_hostName}/api/catan/game"));

            //
            //  test to make sure the users we added are there
            string url = $"{_hostName}/api/catan/game/users/{_gameName}";
            string users = await client.GetStringAsync(url);
           

            List<string> players = JsonSerializer.Deserialize<List<string>>(users);
          
            //
            //  add the list to the UI controls
            _player1Resources.AddPlayers(players);
            _player2Resources.AddPlayers(players);

            _timer.Start();

        }

        private async void OnBadTradeRequest(object sender, RoutedEventArgs e)
        {
            HttpClient client = new HttpClient();
            var resources = new PlayerResources
            {
                Wheat = -2,
                Wood = 0,
                Ore = -2,
                Sheep = 0,
                Brick = 0,
                GoldMine = 0
            };

            var response = await client.PostAsync($"{_hostName}/api/catan/resourcecards/add/{_gameName}/{_player1Resources.SelectedPlayer}", new StringContent(JsonSerializer.Serialize<PlayerResources>(resources), Encoding.UTF8, "application/json"));
            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                RESTReturnValue = await response.Content.ReadAsStringAsync();
            }
        }

        private async void OnTradeInGold(object sender, RoutedEventArgs e)
        {
            HttpClient client = new HttpClient();
            var resources = new PlayerResources
            {
                Wheat = 1,
                Wood = 0,
                Ore = 0,
                Sheep = 0,
                Brick = 0,
                GoldMine = -1
            };

            var response = await client.PostAsync($"{_hostName}/api/catan/resourcecards/tradegold/{_gameName}/{_player1Resources.SelectedPlayer}", new StringContent(JsonSerializer.Serialize<PlayerResources>(resources), Encoding.UTF8, "application/json"));
           RESTReturnValue = FormatJson<PlayerResources>(await response.Content.ReadAsStringAsync());
           
        }

        private async void OnDoTrade(object sender, RoutedEventArgs e)
        {
            HttpClient client = new HttpClient();           
            var resources = new PlayerResources[2] { _player1Trade.PlayerResources, _player2Trade.PlayerResources };            
            var response = await client.PostAsync($"{_hostName}/api/catan/resourcecards/trade/{_gameName}/{_player1Resources.SelectedPlayer}/{_player2Resources.SelectedPlayer}", new StringContent(JsonSerializer.Serialize<PlayerResources[]>(resources), Encoding.UTF8, "application/json"));
            RESTReturnValue = FormatJson<PlayerResources[]>(await response.Content.ReadAsStringAsync());
        }

        private async void OnTakeCard(object sender, RoutedEventArgs e)
        {
            HttpClient client = new HttpClient();
            var response = await client.PostAsync($"{_hostName}/api/catan/resourcecards/take/{_gameName}/{_player1Resources.SelectedPlayer}/{_player2Resources.SelectedPlayer}", null);
            RESTReturnValue = FormatJson<PlayerResources>(await response.Content.ReadAsStringAsync());
        }

        private string FormatJson<T>(string json)
        {
            try
            {
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true
                };

                T obj = JsonSerializer.Deserialize<T>(json);
                return JsonSerializer.Serialize<T>(obj, options);
                
            }
            catch
            {
                return json;
            }
        }

        private async void OnTradeThreeForOne(object sender, RoutedEventArgs e)
        {
            HttpClient client = new HttpClient();
            var resources = new PlayerResources
            {
                Wheat = 1,
                Wood = 0,
                Ore = 0,
                Sheep = 0,
                Brick = 0,
                GoldMine = 0
            };

            _ = await client.PostAsync($"{_hostName}/api/catan/resourcecards/add/{_gameName}/{_player1Resources.SelectedPlayer}", new StringContent(JsonSerializer.Serialize<PlayerResources>(resources), Encoding.UTF8, "application/json"));
            var response = await client.PostAsync($"{_hostName}/api/catan/resourcecards/meritimetrade/{_gameName}/{_player1Resources.SelectedPlayer}/Wheat/3/Wood", null);
            RESTReturnValue = FormatJson<PlayerResources>(await response.Content.ReadAsStringAsync());
        }

        private async void OnGrantResources(object sender, RoutedEventArgs e)
        {
            HttpClient client = new HttpClient();
            var response = await client.PostAsync($"{_hostName}/api/catan/resourcecards/add/{_gameName}/{_cmbPlayers.SelectedItem}", new StringContent(JsonSerializer.Serialize<PlayerResources>(_bank.PlayerResources), Encoding.UTF8, "application/json"));
             RESTReturnValue = FormatJson<PlayerResources>(await response.Content.ReadAsStringAsync());            

        }
    }
    
}
