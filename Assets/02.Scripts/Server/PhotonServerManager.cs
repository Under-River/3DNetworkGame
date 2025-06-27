// Photon API 네임스페이스
using UnityEngine;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using Debug = UnityEngine.Debug;
using Player = Photon.Realtime.Player;
// 역할 : 포톤 서버 관리자(서버 연결, 로비 입장, 방 입장, 게임 입장)
public class PhotonServerManager : MonoBehaviourPunCallbacks
{
    // MonoBehaviourPunCallbacks : 유니티 이벤트 말고도 PUN 서버 이벤트를 받을 수 있다.
    private readonly string _gameVersion = "1.0.0";
    private string _nickname = "Ponyo";
    private void Start()
    {
        // 설정
        // 0. 데이터 송수신 빈도를 매 초당 30회로 설정한다. (기본은 10)
        PhotonNetwork.SendRate = 60;
        PhotonNetwork.SerializationRate = 60;

        // 1. 버전 : 버전이 다르면 다른 서버로 접속이 된다.
        PhotonNetwork.GameVersion = _gameVersion;

        // 2. 닉네임 : 게임에서 사용할 사용자의 별명(중복 가능) (판별을 위해서는 Actor ID)
        PhotonNetwork.NickName = _nickname;

        // 방장이 로드한 씬으로 다른 참여자가 똑같이 이동하게끔 동기화 해주는 옵션
        // 방장 : 방을 만든 소유자이자 "마스터 클라이언트" (방마다 한명의 마스터 클라이언트가 존재)
        PhotonNetwork.AutomaticallySyncScene = true;

        // 설정 값들을 이용해 서버 접속 시도
        // 네임 서버 접속 -> 방 목록이 있는 마스터 서버까지 접속이 됩니다.
        PhotonNetwork.ConnectUsingSettings();
    }
    // 포톤 서버에 접속 후 호출되는 콜백(이벤트) 함수
    public override void OnConnected()
    {
        Debug.Log("네임 서버 접속 완료");
        Debug.Log(PhotonNetwork.CloudRegion);
    }
    // 포톤 마스터 서버에 접속하면 호출되는 함수
    public override void OnConnectedToMaster()
    {
        Debug.Log("마스터 서버 접속 완료");
        Debug.Log(PhotonNetwork.InLobby);  // 로비 입장 유무


        // 디폴트 로비(채널)입장
        PhotonNetwork.JoinLobby();
        // photonNetwork.JoinRandomRoom(TypedLobby.Default);
    }

    // 로비에 접속하면 호출되는 함수
    public override void OnJoinedLobby()
    {
        Debug.Log("로비(채널) 접속 완료");
        Debug.Log(PhotonNetwork.InLobby); // 로비 입장 유무

        // 랜덤 방에 들어간다.
        PhotonNetwork.JoinRandomRoom();
    }

    // 방에 입장한 후 호출되는 함수
    public override void OnJoinedRoom()
    {
        Debug.Log($"방 입장 {PhotonNetwork.InRoom} {PhotonNetwork.CurrentRoom.Name}");
        Debug.Log($"플레이어 : {PhotonNetwork.CurrentRoom.PlayerCount}");

        // 방에 접속한 사용자 정보
        Dictionary<int, Photon.Realtime.Player> players = PhotonNetwork.CurrentRoom.Players;
        foreach (var player in players)
        {
            Debug.Log($"{player.Value.NickName} : {player.Value.ActorNumber}");
            // ActorNumber는 Room안에서의 플레이어에 대한 판별 ID
            // ㄴ 이것만으로 충분하다.

            // 진짜 고유 아이디
            Debug.Log(player.Value.UserId); // 친구 기능, 귓속말 등등에 쓰이지만... 플젝때 알아서쓰세요.
        }

        // 방에 입장 완료가되면 플레이어를 생성한다.
        // 포톤에서는 게임 오브젝트 생성후 포톤 서버에 등록까지해야 한다.
        PhotonNetwork.Instantiate("Player", Vector3.zero, Quaternion.identity);
    }

    // 방 입장에 실패했을 때 호출되는 함수
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log($"랜덤 방 입장에 실패했습니다: {returnCode} : {message}");

        // Room 속성 정의
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 20;
        roomOptions.IsOpen = true; // 룸 입장 가능 여부
        roomOptions.IsVisible = true; // 로비(채널) 룸 목록에 노출시킬지 여부

        // Room 생성
        PhotonNetwork.CreateRoom("test", roomOptions);
    }

    // 룸 생성에 실패했을때 호출되는 함수   
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log($"방 생성에 실패했습니다: {returnCode} : {message}");
    }

    // 룸 생성에 성공했을때 호출되는 함수
    public override void OnCreatedRoom()
    {
        Debug.Log($"방 생성에 성공했습니다: {PhotonNetwork.CurrentRoom.Name}");
    }
}