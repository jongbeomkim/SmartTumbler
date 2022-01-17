using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    public Button loginBtn;
    public Text IDText;
    public Text ConnectionStatus;

    // Start is called before the first frame update
    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
        loginBtn.interactable = false;
        ConnectionStatus.text = "Connecting to Master Server...";
    }

    void Update() {
        if (Input.GetKey(KeyCode.Escape)){ // ESC키 입력을 받으면 종료
            Application.Quit();
        }
    }

    public void Connect()
    {
        if (IDText.text.Equals("")) { // ID가 공백이라면 리턴
            return;
        } else {
            PhotonNetwork.LocalPlayer.NickName = IDText.text;
            loginBtn.interactable = false;

            if (PhotonNetwork.IsConnected) { // 포톤 서버에 접속되었다면
                ConnectionStatus.text = "Connecting to Room...";
                PhotonNetwork.JoinRandomRoom();
            } else {
                ConnectionStatus.text = "Offline : Fail to connect\nReconnecting...";
                PhotonNetwork.ConnectUsingSettings();
            }
        }
    }

    public override void OnConnectedToMaster()
    {
        loginBtn.interactable = true;
        ConnectionStatus.text = "Online : connected to Master server";
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        loginBtn.interactable = false;
        ConnectionStatus.text = "Offline : Fail to connect\nReconnecting...";
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        ConnectionStatus.text = "No empty room. Creating new room...";
        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = 20 } );
    }

    public override void OnJoinedRoom()
    {
        ConnectionStatus.text = "Success to Join Room";
        PhotonNetwork.LoadLevel("Main");
    }

}
