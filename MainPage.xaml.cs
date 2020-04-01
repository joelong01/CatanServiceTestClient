using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using Windows.Foundation;
using Windows.Foundation.Collections;
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
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            HttpClient client = new HttpClient();

            var response = await client.DeleteAsync("http://localhost:50919/api/catan/game/delete/Game1/");
            _  = await client.PostAsync("http://localhost:50919/api/catan/game/register/Game1/joe", null);
            response = await client.PostAsync("http://localhost:50919/api/catan/game/register/Game1/dodgy", null);
            response = await client.PostAsync("http://localhost:50919/api/catan/game/register/Game1/doug", null);
            response = await client.PostAsync("http://localhost:50919/api/catan/game/register/Game1/chris", null);

            var resources = new ResourceCountClass
            {
                Wheat = 1,
                Wood = 1,
                Ore = 1,
                Sheep = 1,
                Brick = 1,
                GoldMine = 1
            };


           
           
            _ = await client.PostAsync("http://localhost:50919/api/catan/cards/add/Game1/joe", new StringContent(JsonConvert.SerializeObject(resources), Encoding.UTF8, "application/json"));
            _ = await client.PostAsync("http://localhost:50919/api/catan/cards/add/Game1/dodgy", new StringContent(JsonConvert.SerializeObject(resources), Encoding.UTF8, "application/json"));
            _ = await client.PostAsync("http://localhost:50919/api/catan/cards/add/Game1/doug", new StringContent(JsonConvert.SerializeObject(resources), Encoding.UTF8, "application/json"));
            _ = await client.PostAsync("http://localhost:50919/api/catan/cards/add/Game1/chris", new StringContent(JsonConvert.SerializeObject(resources), Encoding.UTF8, "application/json"));

            _txt.Text = FormatJson(await client.GetStringAsync("http://localhost:50919/api/catan/games"));
           
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

            var response = await client.PostAsync("http://localhost:50919/api/catan/cards/add/Game1/joe", new StringContent(JsonConvert.SerializeObject(resources), Encoding.UTF8, "application/json"));
            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                _txt.Text = FormatJson(response.Content.ReadAsStringAsync().Result);
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

            var response = await client.PostAsync("http://localhost:50919/api/catan/cards/tradegold/Game1/joe", new StringContent(JsonConvert.SerializeObject(resources), Encoding.UTF8, "application/json"));
           _txt.Text = FormatJson(await response.Content.ReadAsStringAsync());
           
        }

        private async void OnDoTrade(object sender, RoutedEventArgs e)
        {

            HttpClient client = new HttpClient();
            var fromResources = new ResourceCountClass
            {

                Wheat = 1,
                Wood = 0,
                Ore = 0,
                Sheep = 0,
                Brick = 0,
                GoldMine = 0
            };
            var toResources = new ResourceCountClass
            {             
                Wheat = 0,
                Wood = 1,
                Ore = 0,
                Sheep = 0,
                Brick = 0,
                GoldMine = 0            
            };

            var resources = new ResourceCountClass[2];
            resources[0] = fromResources;
            resources[1] = toResources;

            var response = await client.PostAsync("http://localhost:50919/api/catan/cards/trade/Game1/joe/dodgy", new StringContent(JsonConvert.SerializeObject(resources), Encoding.UTF8, "application/json"));
            _txt.Text = FormatJson(await response.Content.ReadAsStringAsync());
        }

        private async void OnTakeCard(object sender, RoutedEventArgs e)
        {
            HttpClient client = new HttpClient();
            var response = await client.PostAsync("http://localhost:50919/api/catan/cards/take/Game1/joe/dodgy", null);
            _txt.Text = FormatJson(await response.Content.ReadAsStringAsync());
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

            _ = await client.PostAsync("http://localhost:50919/api/catan/cards/add/Game1/dodgy", new StringContent(JsonConvert.SerializeObject(resources), Encoding.UTF8, "application/json"));
            var response = await client.PostAsync("http://localhost:50919/api/catan/cards/meritimetrade/Game1/Dodgy/Wheat/3/Wood", null);
            _txt.Text = FormatJson(await response.Content.ReadAsStringAsync());
        }
    }
    
}
