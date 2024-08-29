using System.Diagnostics;

internal class Program()
{
    private static void Awake()
    {
        ProcessStartInfo startInfo = new ProcessStartInfo();
        startInfo.UseShellExecute = false;
        startInfo.CreateNoWindow = true;
    }

    private static async Task Main(string[] args)
    {
        Client client = new Client();
        await client.ConnectToClientAsync();
    }
}