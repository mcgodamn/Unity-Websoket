using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class TestClient : MonoBehaviour
{
    [Serializable]
    public class InputProtocol
    {
        public int keycode;
        public bool isKeyDown;
    }

    [SerializeField]
    GameObject Character;

    InputProtocol send, receive;

    WebSocketClient client;
    
    List<KeyCode> Commands;

    void Start()
    {
        receive = new InputProtocol();
        send = new InputProtocol();

        Commands = new List<KeyCode>();
        client = new WebSocketClient(
            "ws://127.0.0.1:8899/gameserver", ExecuteCommand);

        client.Start();
    }


    void ExecuteCommand(byte[] result)
    {
        var json = Encoding.UTF8.GetString(result, 0, result.Length);
        JsonUtility.FromJsonOverwrite(json, receive);
        Debug.Log(receive.isKeyDown.ToString() + receive.keycode + json);
        if (receive.isKeyDown)
        {
            Commands.Add((KeyCode)receive.keycode);
        }
        else
        {
            Commands.Remove((KeyCode)receive.keycode);
        }
    }

    void Update()
    {
        if (client == null) return;

        foreach (KeyCode vKey in System.Enum.GetValues(typeof(KeyCode)))
        {
            if (Input.GetKeyDown(vKey) || (Input.GetKeyUp(vKey))) 
            {
                if (vKey == KeyCode.Escape)
                    client.Dispose();
                else
                {
                    send.isKeyDown = Input.GetKeyDown(vKey);
                    send.keycode = vKey.GetHashCode();
                    client.Send(
                        System.Text.Encoding.UTF8.GetBytes(
                            JsonUtility.ToJson(send)
                        )
                    );
                }
            }
        }

        foreach(KeyCode k in Commands)
        {
            switch(k)
            {
                case KeyCode.W:
                    Character.transform.Translate(Vector3.up * Time.deltaTime);
                    break;
                case KeyCode.S:
                    Character.transform.Translate(Vector3.down * Time.deltaTime);
                    break;
                case KeyCode.D:
                    Character.transform.Translate(Vector3.right * Time.deltaTime);
                    break;
                case KeyCode.A:
                    Character.transform.Translate(Vector3.left * Time.deltaTime);
                    break;
            }
        }
    }

    void OnApplicationQuit()
    {
        if (client != null)
            client.Dispose();
    }
}