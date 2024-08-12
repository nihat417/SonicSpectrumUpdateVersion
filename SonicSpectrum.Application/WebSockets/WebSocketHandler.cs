using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using SonicSpectrum.Domain.Entities;
using SonicSpectrum.Persistence.Data;
using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;

namespace SonicSpectrum.Application.WebSockets
{
    public class WebSocketHandler : IMiddleware
    {
        private static ConcurrentDictionary<string, WebSocket> _sockets = new ConcurrentDictionary<string, WebSocket>();
        private readonly IServiceProvider _serviceProvider;

        public WebSocketHandler(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            if (context.WebSockets.IsWebSocketRequest)
            {
                var socket = await context.WebSockets.AcceptWebSocketAsync();
                var socketId = Guid.NewGuid().ToString();
                _sockets.TryAdd(socketId, socket);

                await Receive(socket, async (result, serializedMessage) =>
                {
                    if (result.MessageType == WebSocketMessageType.Text)
                    {
                        var message = JsonConvert.DeserializeObject<Message>(serializedMessage);
                        await HandleMessage(socketId, message);
                    }
                    else if (result.MessageType == WebSocketMessageType.Close)
                    {
                        _sockets.TryRemove(socketId, out _);
                        await socket.CloseAsync(result.CloseStatus!.Value, result.CloseStatusDescription, CancellationToken.None);
                    }
                });
            }
            else
            {
                await next(context);
            }
        }

        private async Task HandleMessage(string socketId, Message message)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                var user = await context.Users.FindAsync(message.SenderId);
                if (user == null) return;

                var receiver = await context.Users.FindAsync(message.ReceiverId);
                if (receiver == null) return;

                message.Sender = user;
                message.Receiver = receiver;

                context.Messages.Add(message);
                await context.SaveChangesAsync();

                var responseMessage = JsonConvert.SerializeObject(new
                {
                    SenderId = message.SenderId,
                    ReceiverId = message.ReceiverId,
                    Content = message.Content,
                    CreatedTime = message.CreatedTime,
                    IsRead = message.IsRead
                });

                var encodedMessage = Encoding.UTF8.GetBytes(responseMessage);
                var buffer = new ArraySegment<byte>(encodedMessage, 0, encodedMessage.Length);

                foreach (var socket in _sockets.Values)
                {
                    if (socket.State == WebSocketState.Open)
                    {
                        await socket.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);
                    }
                }
            }
        }

        private async Task Receive(WebSocket socket, Action<WebSocketReceiveResult, string> handleMessage)
        {
            var buffer = new byte[1024 * 4];
            while (socket.State == WebSocketState.Open)
            {
                var result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                var serializedMessage = Encoding.UTF8.GetString(buffer, 0, result.Count);
                handleMessage(result, serializedMessage);
            }
        }

        public async Task NotifyClientsAsync(MessageDto message)
        {
            var responseMessage = JsonConvert.SerializeObject(message);
            var encodedMessage = Encoding.UTF8.GetBytes(responseMessage);
            var buffer = new ArraySegment<byte>(encodedMessage, 0, encodedMessage.Length);

            foreach (var socket in _sockets.Values)
            {
                if (socket.State == WebSocketState.Open)
                {
                    await socket.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);
                }
            }
        }

    }
}
