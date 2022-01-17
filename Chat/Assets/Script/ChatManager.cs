using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
using Photon.Voice.Unity;

public class ChatManager : MonoBehaviourPunCallbacks
{
    public Button sendBtn;
    public Text chatLog;
    public Text chattingList;
    public InputField input;
    ScrollRect scroll_rect = null;
    string chatters;
    public Recorder recorder;
    // Start is called before the first frame update
    void Start()
    {
        PhotonNetwork.IsMessageQueueRunning = true;
        scroll_rect = GameObject.FindObjectOfType<ScrollRect>();

        // Recorder 송신 비활성화, true를 주면 활성화된다.
        recorder.TransmitEnabled = false;
    }
    public void SendButtonOnClicked()
    {
        if (input.text.Equals("")) { Debug.Log("Empty"); return; } // 공백을 입력 받으면 로그를 출력하고 처리 안함
        string msg = string.Format("[{0}] {1}", PhotonNetwork.LocalPlayer.NickName, input.text);
        
        // 다른 유저에게 메시지 전송
        photonView.RPC("ReceiveMsg", RpcTarget.OthersBuffered, msg);
        ReceiveMsg(msg);
        
        input.ActivateInputField(); // 반대는 input.select(); (반대로 토글)
        input.text = "";
    }
    void Update()
    {
        chatterUpdate();
        if (Input.GetKeyDown(KeyCode.Return) && !input.isFocused) SendButtonOnClicked();

        if (Input.GetKeyDown(KeyCode.BackQuote)) { // `키로 보이스챗 말하기
            recorder.TransmitEnabled = true;
            string msg = string.Format("{0} {1}", PhotonNetwork.LocalPlayer.NickName, "님이 보이스챗 중입니다!");
            
            TranMsg(msg);
        }

        if (Input.GetKey(KeyCode.BackQuote)) {
            // `키를 지운다.
            input.text = input.text.Replace("`", "");
        }

        if (Input.GetKeyUp(KeyCode.BackQuote)) { // 보이스챗 종료
            // `키를 지운다.
            input.text = input.text.Replace("`", "");

            recorder.TransmitEnabled = false;
            string msg = string.Format("{0} {1}", PhotonNetwork.LocalPlayer.NickName, "님이 보이스챗을 종료했습니다!");

            TranMsg(msg);
        }

        if (Input.GetKey(KeyCode.Escape)){ // ESC키 입력을 받으면 종료
            Application.Quit();
        }
    }
    void chatterUpdate()
    {
        chatters = "Player List\n";
        foreach (Player p in PhotonNetwork.PlayerList)
        {
            chatters += p.NickName + "\n";
        }
        chattingList.text = chatters;
    }
    void TranMsg(string msg) // 다른 유저들에게 메시지를 전송한다.
    {
        photonView.RPC("ReceiveMsg", RpcTarget.OthersBuffered, msg);
        ReceiveMsg(msg);
    }
    [PunRPC]
    public void ReceiveMsg(string msg)
    {
        chatLog.text += "\n" + msg;
        scroll_rect.verticalNormalizedPosition = 0.0f;
    }
}