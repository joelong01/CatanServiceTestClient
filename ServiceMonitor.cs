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
        public string Message { get; set; }

    }



    /// <summary>
    ///     Assumes that it is called on a UI thread.  does non-blocking calls to the service to get updates.
    /// </summary>
    public class ServiceMonitor
    {
        public event ServiceMonitorHandler OnCallback;
        HttpClient _client = new HttpClient()
        {
            Timeout = TimeSpan.FromHours(12)
        };

        public string MonitorUrl { get; set; } = "";
        private bool _go = true;

        public ServiceMonitor() { }

        public ServiceMonitor(string url)
        {
            MonitorUrl = url;
        }
        public async void Start()
        {
            _go = true;


            try
            {

                await Window.Current.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                {
                    await ExecuteHangingGet();

                    this.TraceMessage($"Starting while loop url={this.MonitorUrl}");
                    int count = 0;
                    while (_go)
                    {

                        this.TraceMessage($"{MonitorUrl} call count={count++}");
                    }

                });

                this.TraceMessage($"Exiting Monitoring for {MonitorUrl}");

            }
            finally
            {

            }


        }

        public async Task ExecuteHangingGet()
        {

            if (OnCallback == null)
            {
                throw new Exception("The callback for the Service Monitor is not set Url={MonitorUrl}");
            }

            if (String.IsNullOrEmpty(MonitorUrl))
            {
                throw new Exception("The Url for the monitor is not set");

            }

            try
            {

                string json = await _client.GetStringAsync(MonitorUrl);
                OnCallback.Invoke(new CatanServiceMessage { MessageType = CatanServiceMessageType.Normal, Message = json });

            }
            catch (Exception requestException)
            {
                if (requestException.InnerException is WebException webException && webException.Status == WebExceptionStatus.Timeout)
                {
                    OnCallback.Invoke(new CatanServiceMessage { MessageType = CatanServiceMessageType.Error, Message = $"[Url={MonitorUrl}] Timed out!" });
                }
                else
                {
                    OnCallback.Invoke(new CatanServiceMessage { MessageType = CatanServiceMessageType.Error, Message = $"[Url={MonitorUrl}] Exception Message: {requestException}" });
                    return;
                }

            }
        }



        public void Stop()
        {
            _go = false;
            this.TraceMessage($"stopped timer for URL={MonitorUrl}");
        }

    }

    public class MonitorPlayerResources
    {

        private PlayerResources _playerResources;
        private ServiceMonitor _serviceMonitor = new ServiceMonitor();
        public MonitorPlayerResources(string gameName, string player, string hostName, PlayerResources pr)
        {
            _playerResources = pr;
            _serviceMonitor.OnCallback += MonitorPlayerResources_OnCallback;
            _serviceMonitor.MonitorUrl = $"{hostName}/api/monitor/resources/{gameName}/{player}";
        }

        public void Start()
        {
            _serviceMonitor.Start();
        }

        public void Stop()
        {
            _serviceMonitor.Stop();
        }

        /// <summary>
        ///     update the properties by value -- the properties are protected from updating when not changed.
        /// </summary>
        /// <param name="msg"></param>
        private void MonitorPlayerResources_OnCallback(CatanServiceMessage msg)
        {
            if (msg.MessageType == CatanServiceMessageType.Error)
            {
                throw new Exception("Error returned by Monitor");
            }

            this.TraceMessage(msg.Message);

            // var pr = JsonSerializer.Deserialize<PlayerResources>(msg.Message);

        }
    }
}
