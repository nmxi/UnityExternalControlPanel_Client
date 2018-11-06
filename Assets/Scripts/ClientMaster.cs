using MiniJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;
using UniRx;

public class ClientMaster : MonoBehaviour {

    [SerializeField] private string _serverAddress;
    [SerializeField] private int _port;

    [SerializeField] private string _lastCatchMsg;

    private WebSocket ws;

    public bool _isEnableQuit;

    private void Awake() {
        var cCatchMessageStr = gameObject.ObserveEveryValueChanged(_ => _lastCatchMsg);
        cCatchMessageStr.Subscribe(x => OnCatchMessageStrChanged(x));

        var ca = "ws://" + _serverAddress + ":" + _port.ToString() + "/";
        Debug.Log("Connect to " + ca);

        _isEnableQuit = false;
        
        ws = new WebSocket(ca);

        //Add Events
        //On catch message event
        ws.OnMessage += (object sender, MessageEventArgs e) => {
            //Debug.Log(e.Data);
            _lastCatchMsg = e.Data;
        };

        //On error event
        ws.OnError += (sender, e) => {
            Debug.Log("WebSocket Error Message: " + e.Message);
        };

        //On WebSocket close event
        ws.OnClose += (sender, e) => {
            Debug.Log("Disconnected Server");
        };

        ws.Connect();

        //ws.Send("Hello");
    }

    private void OnApplicationQuit() {
        if (!_isEnableQuit){
            Application.CancelQuit();   
        }
        else{
            ws.Close();   
        }
    }

    /// <summary>
    /// 何か文字列がサーバから送られてきたとき
    /// </summary>
    /// <param name="s">サーバから送られてきた文字列</param>
    private void OnCatchMessageStrChanged(string s) {
        var um = gameObject.GetComponent<UIManager>();
        um.JsonStr = _lastCatchMsg;
        um.SignalBehaviour();
    }

    /// <summary>
    /// サーバに信号としてJson文字列を送る
    /// </summary>
    /// <param name="jsonStr">Json文字列</param>
    public void SendSignal(string jsonStr) {
        ws.Send(jsonStr);
    }
}
