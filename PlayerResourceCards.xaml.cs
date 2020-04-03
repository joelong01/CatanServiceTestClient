
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
        public string WssHostName { get; set; } = "ws://localhost:50919";
        public string _gameName { get; set; } = "Game";
        HttpClient _client = new HttpClient();
        string PlayerName { get; set; } = "";
        DispatcherTimer _timer = null;

        PlayerResources _playerResources = new PlayerResources();
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        }
        public static readonly DependencyProperty CaptionProperty = DependencyProperty.Register("Caption", typeof(string), typeof(PlayerResourceCard), new PropertyMetadata(""));
        public static readonly DependencyProperty ReadOnlyProperty = DependencyProperty.Register("ReadOnly", typeof(bool), typeof(PlayerResourceCard), new PropertyMetadata(false, ReadOnlyChanged));
        public static readonly DependencyProperty ShowUsersProperty = DependencyProperty.Register("ShowPlayers", typeof(bool), typeof(PlayerResourceCard), new PropertyMetadata(true, ShowPlayersChanged));

        public PlayerResources PlayerResources
        {
            get => _playerResources;
            set
            {
                _playerResources = value;
                NotifyPropertyChanged();
            }
        }
        
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
                string url = $"{_hostName}/api/monitor/resources/{_gameName}/{PlayerName}";
                try
                {
                    string playerResources = await client.GetStringAsync(url);
                    PlayerResources resources = JsonSerializer.Deserialize<PlayerResources>(playerResources);
                    await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                    {
                        this.PlayerResources = resources;
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
        
       
        public async Task UpdatePlayerResources()
        {
            //
            //  directly get the data an update the UI
            string url = $"{_hostName}/api/catan/resourcecards/{_gameName}/{PlayerName}";
            string playerResources = await _client.GetStringAsync(url);
            PlayerResources = JsonSerializer.Deserialize<PlayerResources>(playerResources);             
           
        }
    }
}
