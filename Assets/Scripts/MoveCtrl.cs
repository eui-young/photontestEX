using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
using UnityEngine.UI;


public class MoveCtrl : MonoBehaviourPunCallbacks,IPunObservable
{
    private Rigidbody rb;
    private Transform tr;
    public PhotonView pv;
    public Text NameText;
    private Vector3 moveInput;
    
    Vector3 velocity;
    Vector3 curPos;

    public float speed = 5f;
    public float rotSpeed = 60.0f;

    [SerializeField] float camSpeed;
    private Animator ani; //animator를 다루기 위한 변수


    private VirtualJoystick joyManager;
    bool aniActive;

    [SerializeField]
    private GameObject cameraArm;

    // Start is called before the first frame update
    private void Awake()
    {
        cameraArm = GameObject.Find("CameraAxis");
        aniActive = false;
        joyManager = FindObjectOfType<VirtualJoystick>();
        NameText.color = pv.IsMine ? Color.green : Color.red;
        if(pv.IsMine)
        {
            NameText.text = PhotonNetwork.NickName;
            pv.RPC("SettingNickname", RpcTarget.AllBuffered, PhotonNetwork.NickName);
            PhotonNetwork.ConnectUsingSettings();
        }
        rb = GetComponent<Rigidbody>();
        tr = GetComponent<Transform>();
        ani = GetComponent<Animator>();
        if (ani != null)
        {
            aniActive = true;
        }
    }

    public void Move(Vector2 inputDirection) //조이스틱을 이용하여 움직임을 받을 때
    {
        if (pv.IsMine)
        {
            moveInput = new Vector3(inputDirection.x, 0, inputDirection.y);
            velocity = moveInput.normalized * speed;
            rb.MovePosition(rb.position + velocity * Time.fixedDeltaTime);
            tr.LookAt(tr.position + velocity);
            if (aniActive == true)
            {
                if (moveInput != Vector3.zero && ani.GetBool("isWalking") == false) //방향키를 누르고 있고 걷고 있는 상태가 아니라면
                {
                    //걷기 애니를 시작
                    ani.SetBool("isWalking", true);
                }
                else if (ani.GetBool("isWalking") == true && moveInput == Vector3.zero) // 걷기 애니가 실행되는 중 방향키를 누르는 상태가 아니라면
                {
                    // 움직임을 멈추기 위해 멈춤 애니를 실행
                    ani.SetBool("isWalking", false);
                    ani.SetTrigger("Stop");
                }
            }
        }
        else if ((transform.position - curPos).sqrMagnitude >= 100) transform.position = curPos;
        else transform.position = Vector3.Lerp(transform.position, curPos, Time.deltaTime * 10);
    }
    [PunRPC]
    void DestroyRPC()
    {
        Destroy(gameObject);
    }

    [PunRPC]
    void SettingNickname(string nickname)
    {
        NameText.text = nickname;
        //pv.RPC("SettingNickname", RpcTarget.Others,nickname);
    }
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
        }
        else
        {
            curPos = (Vector3)stream.ReceiveNext();
        }
    }
    public void LookAround(Vector3 inputDirection)
    {
        Debug.Log("call LookAround");
        Vector3 mouseDelta = inputDirection;
        
        Vector3 camAngle = cameraArm.transform.rotation.eulerAngles;

        float x = camAngle.x - mouseDelta.y;

        if (x < 180f)
        {
            x = Mathf.Clamp(x, -1f, 70f);
        }
        else
        {
            x = Mathf.Clamp(x, 335f, 361f);
        }
        cameraArm.transform.rotation = Quaternion.Euler(new Vector3(x, cameraArm.transform.rotation.y + mouseDelta.x, camAngle.z) * camSpeed);
        this.transform.rotation = Quaternion.Euler(new Vector3(0, this.transform.rotation.y + mouseDelta.x, 0) * camSpeed);
    }
}