using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Debug = UnityEngine.Debug;

/// <summary> サーバーへの接続に必要なデータの取得、クライアントへの受け渡しを行うクラス </summary>
public class BootstrapServer
{
    private string _serverIPAddress = "";

    private readonly int _port = 7000;

    public BootstrapServer(int port) => _port = port;

    public async Task<string> SendServerAddressRequest(int timeOutLimit = 3)
    {
        var udpClient = new UdpClient() { EnableBroadcast = true };

        var data = Encoding.UTF8.GetBytes("I want to know server ip address");
        await udpClient.SendAsync(data, data.Length, new IPEndPoint(IPAddress.Broadcast, _port));
        Debug.Log("Waiting...");

        Task<UdpReceiveResult> receive = udpClient.ReceiveAsync();
        if (await Task.WhenAny(receive, Task.Delay(timeOutLimit * 1000)) == receive)
        {
            var result = await receive;
            var receivedData = result.Buffer;
            string response = Encoding.UTF8.GetString(receivedData);
            Debug.Log("Server response: " + response);

            if (IPAddress.TryParse(response, out var ipAddress)) { _serverIPAddress = ipAddress.ToString(); }
        }
        else { Debug.Log($"TimeOut : ローカルリクエストに失敗しました"); }

        udpClient.Close();

        return _serverIPAddress;
    }
}
