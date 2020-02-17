using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class WebsocketClient : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        WebSocket();
    }
    public async void WebSocket()
    {
        try
        {
            ClientWebSocket ws = new ClientWebSocket();
            CancellationToken ct = new CancellationToken();
            //添加header
            //ws.Options.SetRequestHeader("X-Token", "eyJhbGciOiJIUzI1N");
            Uri url = new Uri("ws://127.0.0.1:8899/echo");
            await ws.ConnectAsync(url, ct);
            await ws.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes("hello")), WebSocketMessageType.Binary, true, ct); //发送数据
            while (true)
            {
                var result = new byte[1024];
                await ws.ReceiveAsync(new ArraySegment<byte>(result), new CancellationToken());//接受数据
                var str = Encoding.UTF8.GetString(result, 0, result.Length);
                Debug.Log(str);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}
