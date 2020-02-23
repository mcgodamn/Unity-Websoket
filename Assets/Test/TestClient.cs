using UnityEngine;
using System.Text;

public class TestClient : MonoBehaviour
{
    WebSocketClient client;

    void Start()
    {
        client = new WebSocketClient(
            "ws://127.0.0.1:8899/gameserver",
            (result)=>{
                var str = Encoding.UTF8.GetString(result, 0, result.Length);
                Debug.Log(str);
            }
        );

        client.Start();
    }

    void Update()
    {
        foreach (KeyCode vKey in System.Enum.GetValues(typeof(KeyCode)))
        {
            if (Input.GetKeyDown(vKey))
            {
                if (vKey == KeyCode.Escape)
                    client.Dispose();
                else
                {
                    client.Send(
                        Encoding.UTF8.GetBytes(
                            ((char)vKey.GetHashCode()).ToString().ToCharArray()
                        )
                    );
                }
            }
        }
    }

    void OnApplicationQuit()
    {
        client.Dispose();
    }
}