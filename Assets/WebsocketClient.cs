using System;
using System.Net.WebSockets;
using System.Threading;
using UnityEngine;

public class WebSocketClient : IDisposable
{
    
    private ClientWebSocket ws;
    private CancellationToken ct;

    private Action<Byte[]> receiveDelegate;
    private string serverUrl;

    private SemaphoreSlim semaphore;

    public WebSocketClient(string serverUrl, Action<Byte[]> receiveDelegate)
    {
        this.receiveDelegate = receiveDelegate;
        this.serverUrl = serverUrl;
    }

    public async void Start()
    {
        try
        {
            ws = new ClientWebSocket();
            ct = new CancellationToken();
            semaphore = new SemaphoreSlim(1,1);
            await ws.ConnectAsync(new Uri(serverUrl), ct);
            StartReceive();
        }
        catch (Exception ex)
        {
            Debug.Log(ex.Message);
        }
    }

    public async void Send(byte[] msg)
    {
        try
        {
            if (ws.State != WebSocketState.Open) return;

            await semaphore.WaitAsync();
            try
            {
                await ws.SendAsync(
                    new ArraySegment<byte>(msg),
                    WebSocketMessageType.Binary,
                    true,
                    ct
                );
            }
            finally
            {
                semaphore.Release();
            }
        }
        catch (Exception ex)
        {
            Debug.Log(ex.Message);
        }
    }

    private async void StartReceive()
    {
        while (ws.State == WebSocketState.Open)
        {
            var result = new byte[1024];
            await ws.ReceiveAsync(new ArraySegment<byte>(result), ct);
            receiveDelegate(result);
        }
    }

    public void Dispose()
    {
        DisposeWebSocket();
    }

    private async void DisposeWebSocket()
    {
        await ws.CloseAsync(WebSocketCloseStatus.NormalClosure,"NormalClose",ct);
        ws.Dispose();
        ws = null;
    }
}
