using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using System;

public class LobbyManager : MonoBehaviourPunCallbacks
{

    public InputField playerNumberInputField;

    private string gameVersion = "1";
    public byte playerNumber = 2;

    // 로드할 씬 인덱스
    private int sceneIndex = 1;

    public Text connectionInfoText;
    public Button joinButton;

    // 룸 옵션
    RoomOptions ros = new RoomOptions();

    void Awake()
    {
        //Screen.SetResolution(1920, 1080, false);
    }

    void Start()
    {
        // 접속에 필요한 정보(게임 버전) 설정
        PhotonNetwork.GameVersion = gameVersion;
        // 설정한 정보로 마스터 서버 접속 시도
        PhotonNetwork.ConnectUsingSettings();

        // 룸 접속 버튼 잠시 비활성화
        joinButton.interactable = false;
        // 접속 시도 중임을 텍스트로 표시
        connectionInfoText.text = "마스터 서버에 접속 중...";
    }

    public void SetPlayerNumber()
    {
        //print(playerNumberInputField.text);
        try
        {
            playerNumber = (byte)int.Parse(playerNumberInputField.text);

        }catch(Exception e)
        {
            Debug.Log(e);
            playerNumber = 1;
        }
    }

    // 마스터 서버 접속 성공 시 자동 실행
    public override void OnConnectedToMaster()
    {
        joinButton.interactable = true;
        connectionInfoText.text = "온라인 : 마스터 서버와 연결됨";
    }

#if UNITY_ANDROID
// 마스터 서버 접속 실패 시 자동 실행
    public override void OnDisconnected(DisconnectCause cause)
    {
        joinButton.interactable = false;
        connectionInfoText.text = "오프라인 : 마스터 서버와 연결되지 않음\n 접속 재시도 중...";
        PhotonNetwork.ConnectUsingSettings();
    }
#endif

    // 룸 접속 시도
    public void Connect()
    {
        joinButton.interactable = false;

        if (PhotonNetwork.IsConnected)
        {
            connectionInfoText.text = "룸에 접속...";
            PhotonNetwork.JoinRandomRoom();
        }
        else
        {
            connectionInfoText.text = "오프라인 : 마스터 서버와 연결되지 않음\n접속 재시도 중...";
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    // (빈 방이 없어)랜덤 룸 참가에 실패한 경우 자동 실행
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        connectionInfoText.text = "빈 방이 없음, 새로운 방 생성...";

        ros.MaxPlayers = playerNumber;
        PhotonNetwork.CreateRoom(null, ros);
    }

    public void Play()
    {
        connectionInfoText.text = "참여 인원 " + PhotonNetwork.PlayerList.Length + "/" + playerNumber + "...";

        if (PhotonNetwork.PlayerList.Length >= playerNumber)
        {
            PhotonNetwork.CurrentRoom.IsOpen = false;
            PhotonNetwork.LoadLevel(sceneIndex);
        }
    }

    // 룸에 참가 완료된 경우 자동 실행
    public override void OnJoinedRoom()
    {
        Play();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Play();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        connectionInfoText.text = "참여 인원 " + PhotonNetwork.PlayerList.Length + "/" + playerNumber + "...";
    }
}