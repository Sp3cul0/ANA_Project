using ANA_Server.Abstract;
using Newtonsoft.Json;
using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace ANA_Server
{
    public class Server
    {
        private Socket client;
        private byte[] buffer;
        private IPEndPoint ipEndPoint;

        private static string filePath = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "Mocks/", "Data.json");

        public Server(byte[] abuffer, IPEndPoint aendPoint, Socket asocket)
        {
            buffer = abuffer;
            ipEndPoint = aendPoint;
            client = asocket;
        }

        public async Task Listener()
        {
            try
            {
                // Bind Hosting Client
                client.Bind(ipEndPoint);

                // Listen Hosting Client
                client.Listen(0);
                Console.WriteLine("Server is listening for connections...");

                // Await the response of the client and verify if hosting client is exist
                client = await client.AcceptAsync();

                // Waiting for somes data ^^
                Console.WriteLine("Client connected.");
                await DataReceive();
            }
            catch (SocketException se)
            {
                Console.WriteLine($"{se.Message}");
                Console.WriteLine($"{se.ErrorCode}");
            }
        }

        private async Task DataReceive()
        {
            try
            {
                // Receive data from client (if it existed)...
                ArraySegment<byte> bufferSegment = new ArraySegment<byte>(buffer);
                int receiveBytes = await client.ReceiveAsync(bufferSegment, SocketFlags.None);
                string receivedData = Encoding.ASCII.GetString(buffer, 0, receiveBytes);
                Console.WriteLine($"Received data: {receivedData}");

                // Process for receive data...
                List<DataModel> processedData = ProcessData(receivedData);

                // Writing into Json file...
                string json = JsonConvert.SerializeObject(processedData, Formatting.Indented);
                await File.AppendAllTextAsync(filePath, json);
                Console.WriteLine("Data has been written to data.json");

                // Response from Statement...
                string[] responseString = { "Executed Command Successfully !!!", "Waiting response from client..." };

                // Response from buffer...
                byte[] responseFromBuffer = Encoding.ASCII.GetBytes(responseString[0]);
                ArraySegment<byte> responseSegment = new ArraySegment<byte>(responseFromBuffer);
                await client.SendAsync(responseSegment, SocketFlags.None);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e.Message}");
            }
        }

        private List<DataModel> ProcessData(string data)
        {
            var dataParts = data.Split(';');
            var dataModels = dataParts.Select(d => new DataModel { Content = d.Trim(), Timestamp = DateTime.UtcNow }).ToList();
            return dataModels;
        }
    }
}
