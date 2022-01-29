using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems; //키보드, 마우스, 터치를 이벤트로 오브젝트에 보낼 수 있는 기능을 지원
using Photon.Pun;

public class VirtualJoystick : MonoBehaviourPunCallbacks, IBeginDragHandler,IDragHandler,IEndDragHandler
{
    [SerializeField]
    private RectTransform lever;
    private RectTransform rectTransform;

    [SerializeField,Range(10,150)]
    private float leverRange;

    [SerializeField]
    private Canvas mainCanvas;
    private Vector2 inputDirection;
    private bool isInput;

    private GameObject myPlayer;

    public enum JoystickType { Move, Rotate }
    public JoystickType joystickType;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }
    void IBeginDragHandler.OnBeginDrag(PointerEventData eventData) //레버에 터치 시작
    {
      
        ControlJoyStickLever(eventData);
        isInput = true;
    }

    void IDragHandler.OnDrag(PointerEventData eventData) //레버 드래그 중
    {
        ControlJoyStickLever(eventData);
    }

    void IEndDragHandler.OnEndDrag(PointerEventData eventData) //레버에서 손을 떼어냈을 때
    {
        lever.anchoredPosition = Vector2.zero;
        isInput = false;
        myPlayer.GetComponent<MoveCtrl>().Move(Vector2.zero);
    }

    private void ControlJoyStickLever(PointerEventData eventData) //joystick 움직임 관련 메소드
    {
        //var scaledAnchoredPosition = rectTransform.anchoredPosition * mainCanvas.transform.lossyScale.x;
        var inputPos = eventData.position - rectTransform.anchoredPosition;
        var inputVector = inputPos.magnitude < leverRange ? inputPos : inputPos.normalized * leverRange;
        lever.anchoredPosition = inputVector;
        inputDirection = (inputVector / leverRange);
    }
    // Update is called once per frame
   
    private void InputControlVetor()
    {
        if (myPlayer != null)
        {
            switch (joystickType)
            {
                case JoystickType.Move:
                    myPlayer.GetComponent<MoveCtrl>().Move(inputDirection);//캐릭터 움직임 함수에 좌표값전달
                    break;
                case JoystickType.Rotate:
                    myPlayer.GetComponent<MoveCtrl>().LookAround(inputDirection);//캐릭터 움직임 함수에 좌표값전달
                    break;
            }
        }
    }
    void Update()
    {
        if (isInput) 
        { 
            InputControlVetor();
        }
    }

    public void SetPlayer(GameObject player)
    {
        myPlayer = player;
    }
}
