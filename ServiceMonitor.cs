using CatanSharedModels;
using CatanSvcTestClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.Xaml;

namespace CatanServiceMonitor
{
    public enum CatanServiceMessageType { Normal, Error };
    public delegate void ServiceMonitorHandler(CatanServiceMessage message);

    public class CatanServiceMessage
    {
        public CatanServiceMessageType MessageType { get; set; }
        
        public List<ServiceLogEntry> Log { get; set; }
        public string Message { get; internal set; }
        public string ReceivingClient { get; internal set; }
    }



    /// <summary>
    ///     Assumes that it is called on a UI thread.  does non-blocking calls to the service to get updates.
    /// </summary>
    public class ServiceMonitor
    {
        public event ServiceMonitorHandler OnCallback;
        CatanProxy CatanProxy = new CatanProxy();

        public string GameName { get; set; }
        public string PlayerName { get; set; }
        
        public string HostName 
        {
            get => CatanProxy.HostName;
            set
            {
               
                CatanProxy.HostName = value;
            } 
        }
        private bool _go = true;

        public ServiceMonitor() { }

        public ServiceMonitor(string game, string player)
        {
            GameName = game;
            PlayerName = player;

        }
        public async void Start()
        {
            _go = true; 


            try
            {

                await Window.Current.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                {
                    

                   // this.TraceMessage($"Starting while loop [Game={GameName}] [Player={PlayerName}]");
                    int count = 0;
                    while (_go)
                    {

                        this.TraceMessage($"[Game={GameName}] [Player={PlayerName}] call count={count++}");
                        await ExecuteHangingGet();
                    }

                });

               // this.TraceMessage($"Exiting Monitoring for [Game={GameName}] [Player={PlayerName}]");

            }
            finally
            {

            }


        }

        public async Task ExecuteHangingGet()
        {

            if (OnCallback == null)
            {
                throw new Exception($"The callback for the Service Monitor is not set Url=[Game={GameName}] [Player={PlayerName}]");
            }

            if (String.IsNullOrEmpty(GameName) || String.IsNullOrEmpty(PlayerName))
            {
                throw new Exception("GameName and PlayerName cannot be null");

            }

            try
            {

                var response = await CatanProxy.Monitor(GameName, PlayerName);
                if (response != null)
                {                    
                    OnCallback.Invoke(new CatanServiceMessage { MessageType = CatanServiceMessageType.Normal, Log = response});
                }
                


            }
            catch
            {
                OnCallback.Invoke(new CatanServiceMessage { MessageType = CatanServiceMessageType.Error, Message = $"[Url=[Game={GameName}] [Player={PlayerName}]] Exception Message: {CatanProxy.LastErrorString}" });
                return;
            }
        }



        public void Stop()
        {
            _go = false;
            this.TraceMessage($"stopped timer for URL=[Game={GameName}] [Player={PlayerName}]");
        }

    }

}
