using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

public class WebSocketController
{
    private const int ReceiveBufferSize = 1024;
    private static ClientWebSocket clientWebSocket;
    private const string GetNewsCommand = "command news";
    public async void Connect()
    {
        clientWebSocket = new ClientWebSocket();
        await clientWebSocket.ConnectAsync(new Uri("ws://194.93.2.109:1985"), CancellationToken.None);

        Console.WriteLine("Connected to WebSocket server");

        await Task.WhenAll(ReceiveLoop());
    }
    private static async Task ReceiveLoop()
    {
        byte[] buffer = new byte[ReceiveBufferSize];
        var receiveBuffer = new ArraySegment<byte>(buffer);

        while (clientWebSocket.State == WebSocketState.Open)
        {
            var result = await clientWebSocket.ReceiveAsync(receiveBuffer, CancellationToken.None);

            if (result.MessageType == WebSocketMessageType.Text)
            {
                try
                {
                    string message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    string[] args = message.Split(' ');
                    char[] a = args[1].ToCharArray();
                    string json_command = message.Trim(a);
                    switch (args[1])
                    {
                        case "news":
                            NewsJson news = JsonSerializer.Deserialize<NewsJson>(json_command);
                            break;
                    }
                }
                catch (Exception e) { 

                }
            }
            else if (result.MessageType == WebSocketMessageType.Close)
            {
                await clientWebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None);
            }
        }
    }
    public async void GetNews()
    {
        byte[] buffer = Encoding.UTF8.GetBytes(GetNewsCommand);
        var segment = new ArraySegment<byte>(buffer);
        await clientWebSocket.SendAsync(segment, WebSocketMessageType.Text, true, CancellationToken.None);
    }
}