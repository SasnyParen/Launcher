using Launcher;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;
using System.Diagnostics;
using static Tommy.TomlInteger;
using System.Collections;
using static System.Runtime.InteropServices.JavaScript.JSType;

public class WebSocketController
{
    private const int ReceiveBufferSize = 1024;
    private static ClientWebSocket clientWebSocket;
    private const string GetNewsCommand = "news";
    public async void Connect()
    {
        clientWebSocket = new ClientWebSocket();
        try
        {
            await clientWebSocket.ConnectAsync(new Uri("ws://82.146.61.52:1985"), CancellationToken.None);
        }catch(WebSocketException ex)
        {
            TextBlock myTextBlock = (TextBlock)Application.Current.MainWindow.FindName("TextDownload");
            myTextBlock.Text = "Не удалось подключиться к серверу...";
            TextBlock myTextBlock2 = (TextBlock)Application.Current.MainWindow.FindName("eRROR");
                myTextBlock2.Text = "НЕТ ПОДКЛЮЧЕНИЯ К СЕРВЕРУ";
        }

        Console.WriteLine("Connected to WebSocket server");

        await Task.WhenAll(ReceiveLoop());
    }
    private async Task ReceiveLoop()
    {
        byte[] buffer = new byte[ReceiveBufferSize];
        var receiveBuffer = new ArraySegment<byte>(buffer);

        while (clientWebSocket.State == WebSocketState.Open)
        {
            var result = await clientWebSocket.ReceiveAsync(receiveBuffer, CancellationToken.None);
            Trace.WriteLine("12312");
            if (result.MessageType == WebSocketMessageType.Text)
            {
                try
                {
                    string message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    string[] args = message.Split(' ');
                    args[0] = args[0] + " ";
                    char[] a = args[0].ToCharArray();
                      
                    string json_command = message.Trim(a);
                    Trace.WriteLine(json_command);
                    switch (args[0])
                    {
                        case "news ":
                            
                            Dictionary<string, object> news = JsonParser.Parse(json_command);
                            Trace.WriteLine(news.Count);
                            for (int index = 0; index < news.Count; index++)
                            {
                                var item = news.ElementAt(index);
                                var itemKey = item.Key;
                                var itemValue = item.Value;
                                Trace.WriteLine(itemValue);
                            }
                            break;
                    }
                }
                catch (JsonException e) {
                    Trace.WriteLine(e.ToString());

                }
                catch (FormatException e)
                {
                    Trace.WriteLine(e.ToString());

                }
            }
            else if (result.MessageType == WebSocketMessageType.Close)
            {
                await clientWebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None);
            }
        }
    }
    public static void AddNewsBlock()
    {
        StackPanel a = new StackPanel();
        a.Width = 290;
        a.Height = 437;
        a.Margin = new Thickness(11, 11, 11, 11);
        a.Background = new SolidColorBrush(Colors.Aqua);
        TextBlock b = new TextBlock();
        b.Text = "Заголовок 1";
        b.Width = 350;
        b.Height = 26;
        b.Margin = new Thickness(11, 350, 11, 11);
        b.FontSize = 20;
        TextBlock c = new TextBlock();
        c.Text = "Краткий тест новости 1";
        c.Width = 350;
        c.Height = 26;
        c.Margin = new Thickness(11, -11, 11, 11);
        c.FontSize = 15;
        c.TextWrapping = TextWrapping.Wrap;
        a.Children.Add(b);
        a.Children.Add(c);
        var myTextBlock = (WrapPanel)Application.Current.MainWindow.FindName("PanelW");
        myTextBlock.Children.Add(a);
        //Application.Current.MainWindow.FindName("dog").Children.Add(a);
    }
    public async void GetNews()
    {
        byte[] buffer = Encoding.UTF8.GetBytes(GetNewsCommand);
        var segment = new ArraySegment<byte>(buffer);
        try
        {
            await clientWebSocket.SendAsync(segment, WebSocketMessageType.Text, true, CancellationToken.None);
        }
        catch(InvalidOperationException e)
        {
           
        }
        
    }
}


public class JsonParser
{
    private const char ObjectStartChar = '{';
    private const char ObjectEndChar = '}';
    private const char ArrayStartChar = '[';
    private const char ArrayEndChar = ']';
    private const char StringStartChar = '"';
    private const char StringEndChar = '"';
    private const char ValueSeparator = ':';
    private const char ElementSeparator = ',';

    public static Dictionary<string, object> Parse(string jsonString)
    {
        var currentIndex = 0;
        return ParseObject("{["+jsonString+"}", ref currentIndex);
    }

    private static Dictionary<string, object> ParseObject(string jsonString, ref int currentIndex)
    {
        var result = new Dictionary<string, object>();

        // Skip '{' character

        while (currentIndex < jsonString.Length && jsonString[currentIndex] != ObjectEndChar)
        {
            Trace.WriteLine(currentIndex);
            var key = ParseString(jsonString, ref currentIndex);
            var value = ParseValue(jsonString, ref currentIndex);
            result[key] = value;

            if (jsonString[currentIndex] == ObjectEndChar)
                break;

            currentIndex++; // Skip ',' character
        }

        currentIndex++; // Skip '}' character
        return result;
    }

    private static List<object> ParseArray(string jsonString, ref int currentIndex)
    {
        var result = new List<object>();

        currentIndex++; // Skip '[' character

        while (currentIndex < jsonString.Length && jsonString[currentIndex] != ArrayEndChar)
        {
            var value = ParseValue(jsonString, ref currentIndex);
            result.Add(value);

            if (jsonString[currentIndex] == ArrayEndChar)
                break;

            currentIndex++; // Skip ',' character
        }

        currentIndex++; // Skip ']' character
        return result;
    }

    private static string ParseString(string jsonString, ref int currentIndex)
    {
        var startQuoteIndex = currentIndex + 1;
        var endQuoteIndex = jsonString.IndexOf(StringEndChar, startQuoteIndex);

        if (endQuoteIndex == -1)
            throw new FormatException("Invalid JSON string format.");

        currentIndex = endQuoteIndex + 1;
        return jsonString.Substring(startQuoteIndex, endQuoteIndex - startQuoteIndex);
    }

    private static object ParseValue(string jsonString, ref int currentIndex)
    {
        var c = jsonString[currentIndex];
        Trace.WriteLine(c);
        if (c == ObjectStartChar)
        {
            return ParseObject(jsonString, ref currentIndex);
        }
        else if (c == ArrayStartChar)
        {
            return ParseArray(jsonString, ref currentIndex);
        }
        else if (c == StringStartChar)
        {
            return ParseString(jsonString, ref currentIndex);
        }
        else if (IsDigitCharacter(c) || c == '-' || c == '.')
        {
            return ParseNumber(jsonString, ref currentIndex);
        }
        else if (c == 't' && jsonString.Length - currentIndex >= 4 && jsonString.Substring(currentIndex, 4) == "true")
        {
            currentIndex += 4;
            return true;
        }
        else if (c == 'f' && jsonString.Length - currentIndex >= 5 && jsonString.Substring(currentIndex, 5) == "false")
        {
            currentIndex += 5;
            return false;
        }
        else if (c == 'n' && jsonString.Length - currentIndex >= 4 && jsonString.Substring(currentIndex, 4) == "null")
        {
            currentIndex += 4;
            return null;
        }
        else
        {
            throw new FormatException("Invalid JSON value format.");
        }
    }

    private static object ParseNumber(string jsonString, ref int currentIndex)
    {
        var startNumIndex = currentIndex;
        var endNumIndex = currentIndex;

        while (endNumIndex < jsonString.Length && IsDigitCharacter(jsonString[endNumIndex]))
        {
            endNumIndex++;
        }

        var numberStr = jsonString.Substring(startNumIndex, endNumIndex - startNumIndex);

        if (int.TryParse(numberStr, out int intValue))
            return intValue;

        if (double.TryParse(numberStr, out double doubleValue))
            return doubleValue;

        throw new FormatException("Invalid JSON number format.");
    }

    private static bool IsDigitCharacter(char c)
    {
        return c >= '0' && c <= '9';
    }
}