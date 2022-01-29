using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    public InputField NameInput;
    public GameObject connectPanel;
    public GameObject Joystick;
    public GameObject camJoystick;
    public GameObject controller;
    private FollowCam camManager;

    [SerializeField] string[] CharList = { "Player1", "Player2","Player3" };
    int CharIndex;


    // Start is called before the first frame update
    private void Awake()
    {
        camManager = FindObjectOfType<FollowCam>();
        Screen.SetResolution(400, 800, false);
        PhotonNetwork.SendRate = 60;
        PhotonNetwork.SerializationRate = 30;
    }

    public void Connect() => PhotonNetwork.ConnectUsingSettings();

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.LocalPlayer.NickName = NameInput.text;
        PhotonNetwork.JoinOrCreateRoom("Room", new RoomOptions { MaxPlayers = 4 }, null);
    }

    public override void OnJoinedRoom()
    {
        connectPanel.SetActive(false);
        controller.SetActive(true);
        Spawn();
    }

    public void CharselectBt(int index)
    {
        CharIndex = index;
    }
    public void Spawn()
    {
        GameObject player= PhotonNetwork.Instantiate(CharList[CharIndex], new Vector3(Random.Range(-3, 3), 0, Random.Range(-3,3)), Quaternion.identity);
        camManager.setPlayer(player);
        Joystick.GetComponent<VirtualJoystick>().SetPlayer(player);
        camJoystick.GetComponent<VirtualJoystick>().SetPlayer(player);
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && PhotonNetwork.IsConnected) { PhotonNetwork.Disconnect(); }
    }
    public override void OnDisconnected(DisconnectCause cause)
    {
        Joystick.GetComponent<VirtualJoystick>().SetPlayer(null);
        camJoystick.GetComponent<VirtualJoystick>().SetPlayer(null);
        camManager.setPlayer(null);
        connectPanel.SetActive(true);
        controller.SetActive(false);
    }
}
