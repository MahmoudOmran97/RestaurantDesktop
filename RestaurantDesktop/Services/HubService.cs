using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;

namespace RestaurantDesktop.Services
{
    /// <summary>
    /// SignalR client — بيستقبل الأوردرات الجديدة real-time
    /// </summary>
    public class HubService : IAsyncDisposable
    {
        private HubConnection _connection;

        public event Action<int, string> OnOrderStatusChanged;
        public event Action<int> OnNewOrder;

        public bool IsConnected
        {
            get
            {
                if (_connection != null && _connection.State == HubConnectionState.Connected)
                    return true;
                else
                    return false;
            }
        }

        public async Task StartAsync()
        {
            if (_connection != null)
                await StopAsync();

            HubConnectionBuilder builder = new HubConnectionBuilder();
            builder.WithUrl(AppConfig.HubUrl, opts =>
            {
                opts.AccessTokenProvider = AccessTokenProvider;
                // تجاهل SSL في التطوير
                opts.HttpMessageHandlerFactory = HttpMessageHandlerFactory;
            });
            builder.WithAutomaticReconnect();

            _connection = builder.Build();

            // تسجيل الأحداث - بدون discard parameters
            _connection.On<int, string>("OrderStatusChanged", OnOrderStatusChangedHandler);
            _connection.On<int>("NewOrder", OnNewOrderHandler);

            try
            {
                await _connection.StartAsync();
            }
            catch
            {
                // السيرفر ممكن يكون مش شغال — مش مشكلة
            }
        }

        private Task<string> AccessTokenProvider()
        {
            return Task.FromResult(AppSession.Token);
        }

        private HttpMessageHandler HttpMessageHandlerFactory(HttpMessageHandler handler)
        {
            HttpClientHandler clientHandler = new HttpClientHandler();
            clientHandler.ServerCertificateCustomValidationCallback = ServerCertificateCustomValidationCallback;
            return clientHandler;
        }

        private bool ServerCertificateCustomValidationCallback(HttpRequestMessage request, System.Security.Cryptography.X509Certificates.X509Certificate2 certificate, System.Security.Cryptography.X509Certificates.X509Chain chain, System.Net.Security.SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }

        private void OnOrderStatusChangedHandler(int orderId, string status)
        {
            Action<int, string> handler = OnOrderStatusChanged;
            if (handler != null)
            {
                handler(orderId, status);
            }
        }

        private void OnNewOrderHandler(int orderId)
        {
            Action<int> handler = OnNewOrder;
            if (handler != null)
            {
                handler(orderId);
            }
        }

        public async Task StopAsync()
        {
            if (_connection != null)
            {
                await _connection.StopAsync();
                await _connection.DisposeAsync();
                _connection = null;
            }
        }

        public async ValueTask DisposeAsync()
        {
            await StopAsync();
        }
    }
}