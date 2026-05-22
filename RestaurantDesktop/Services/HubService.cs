using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
namespace RestaurantDesktop.Services
{
    

    
/// <summary>
/// SignalR client — بيستقبل الأوردرات الجديدة real-time
/// </summary>
public class HubService : IAsyncDisposable
    {
        private HubConnection? _connection;

        public event Action<int, string>? OnOrderStatusChanged;
        public event Action<int>? OnNewOrder;

        public bool IsConnected =>
            _connection?.State == HubConnectionState.Connected;

        public async Task StartAsync()
        {
            if (_connection != null) await StopAsync();

            _connection = new HubConnectionBuilder()
                .WithUrl(AppConfig.HubUrl, opts =>
                {
                    opts.AccessTokenProvider = () =>
                        Task.FromResult<string?>(AppSession.Token);
                // تجاهل SSL في التطوير
                opts.HttpMessageHandlerFactory = _ =>
                        new HttpClientHandler
                        {
                            ServerCertificateCustomValidationCallback =
                                (_, _, _, _) => true
                        };
                })
                .WithAutomaticReconnect()
                .Build();

            _connection.On<int, string>("OrderStatusChanged", (orderId, status) =>
                OnOrderStatusChanged?.Invoke(orderId, status));

            _connection.On<int>("NewOrder", orderId =>
                OnNewOrder?.Invoke(orderId));

            try { await _connection.StartAsync(); }
            catch { /* السيرفر ممكن يكون مش شغال — مش مشكلة */ }
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

        public async ValueTask DisposeAsync() => await StopAsync();
    }
    }
