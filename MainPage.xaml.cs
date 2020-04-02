using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Networking.Sockets;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace CatanSvcTestClient
{
    public class JsonObservableCollectionConverter : DefaultContractResolver
    {
        public JsonObservableCollectionConverter()
        {

        }

        public override JsonContract ResolveContract(Type type)
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(ICollection<>))
            {
                return ResolveContract(typeof(ObservableCollection<>).MakeGenericType(type.GetGenericArguments()));
            }
            return base.ResolveContract(type);
        }
    }
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

        }
        private async void OnStart(object sender, RoutedEventArgs e)
        {
            HttpClient client = new HttpClient();
            _ = await client.DeleteAsync($"{_hostName}/api/catan/game/delete/{_gameName}/");
            foreach (var player in _lstPlayerList)
            {
                _ = await client.PostAsync($"{_hostName}/api/catan/game/register/{_gameName}/{player}", null);
                var resources = new ResourceCountClass
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
                _ = await client.PostAsync($"{_hostName}/api/catan/cards/add/{_gameName}/{player}", new StringContent(JsonConvert.SerializeObject(resources), Encoding.UTF8, "application/json"));
            }

            
            
          

            RESTReturnValue = FormatJson(await client.GetStringAsync($"{_hostName}/api/catan/games"));

            //
            //  test to make sure the users we added are there
            string url = $"{_hostName}/api/catan/game/users/{_gameName}";
            string users = await client.GetStringAsync(url);
            var settings = new JsonSerializerSettings
            {
                ContractResolver = new JsonObservableCollectionConverter(),
            };

            List<string> players = JsonConvert.DeserializeObject<List<string>>(users, settings);
          
            //
            //  add the list to the UI controls
            _player1Resources.AddPlayers(players);
            _player2Resources.AddPlayers(players);



        }

        private async void OnBadTradeRequest(object sender, RoutedEventArgs e)
        {
            HttpClient client = new HttpClient();
            var resources = new ResourceCountClass
            {
                Wheat = -2,
                Wood = 0,
                Ore = -2,
                Sheep = 0,
                Brick = 0,
                GoldMine = 0
            };

            var response = await client.PostAsync($"{_hostName}/api/catan/cards/add/{_gameName}/{_player1Resources.SelectedPlayer}", new StringContent(JsonConvert.SerializeObject(resources), Encoding.UTF8, "application/json"));
            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                RESTReturnValue = FormatJson(response.Content.ReadAsStringAsync().Result);
            }
        }

        private async void OnTradeInGold(object sender, RoutedEventArgs e)
        {
            HttpClient client = new HttpClient();
            var resources = new ResourceCountClass
            {
                Wheat = 1,
                Wood = 0,
                Ore = 0,
                Sheep = 0,
                Brick = 0,
                GoldMine = -1
            };

            var response = await client.PostAsync($"{_hostName}/api/catan/cards/tradegold/{_gameName}/{_player1Resources.SelectedPlayer}", new StringContent(JsonConvert.SerializeObject(resources), Encoding.UTF8, "application/json"));
           RESTReturnValue = FormatJson(await response.Content.ReadAsStringAsync());
           
        }

        private async void OnDoTrade(object sender, RoutedEventArgs e)
        {
            HttpClient client = new HttpClient();           
            var resources = new ResourceCountClass[2] { _player1Resources.ResourceCount, _player2Resources.ResourceCount };            
            var response = await client.PostAsync($"{_hostName}/api/catan/cards/trade/{_gameName}/{_player1Resources.SelectedPlayer}/{_player2Resources.SelectedPlayer}", new StringContent(JsonConvert.SerializeObject(resources), Encoding.UTF8, "application/json"));
            RESTReturnValue = FormatJson(await response.Content.ReadAsStringAsync());
        }

        private async void OnTakeCard(object sender, RoutedEventArgs e)
        {
            HttpClient client = new HttpClient();
            var response = await client.PostAsync($"{_hostName}/api/catan/cards/take/{_gameName}/{_player1Resources.SelectedPlayer}/{_player2Resources.SelectedPlayer}", null);
            RESTReturnValue = FormatJson(await response.Content.ReadAsStringAsync());
        }

        private string FormatJson(string json)
        {
            try
            {
                string jsonFormatted = JValue.Parse(json).ToString(Formatting.Indented);
                return jsonFormatted;
            }
            catch
            {
                return json;
            }
        }

        private async void OnTradeThreeForOne(object sender, RoutedEventArgs e)
        {
            HttpClient client = new HttpClient();
            var resources = new ResourceCountClass
            {
                Wheat = 1,
                Wood = 0,
                Ore = 0,
                Sheep = 0,
                Brick = 0,
                GoldMine = 0
            };

            _ = await client.PostAsync($"{_hostName}/api/catan/cards/add/{_gameName}/{_player1Resources.SelectedPlayer}", new StringContent(JsonConvert.SerializeObject(resources), Encoding.UTF8, "application/json"));
            var response = await client.PostAsync($"{_hostName}/api/catan/cards/meritimetrade/{_gameName}/{_player1Resources.SelectedPlayer}/Wheat/3/Wood", null);
            RESTReturnValue = FormatJson(await response.Content.ReadAsStringAsync());
        }

        private async void OnGrantResources(object sender, RoutedEventArgs e)
        {
            HttpClient client = new HttpClient();
            var response = await client.PostAsync($"{_hostName}/api/catan/cards/add/{_gameName}/{_cmbPlayers.SelectedItem}", new StringContent(JsonConvert.SerializeObject(_bank.ResourceCount), Encoding.UTF8, "application/json"));
             RESTReturnValue = FormatJson(await response.Content.ReadAsStringAsync());            

        }
    }
    
}
