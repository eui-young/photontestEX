using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class FollowCam : MonoBehaviour
{
    GameObject player;
    public Transform mainCamera;
    public Transform CameraAxis;
    // Start is called before the first frame update

    // Update is called once per frame
    void Update()
    {
        if (player != null)
        {
            CameraAxis.position = player.transform.position;
        }
    }
    
    public void setPlayer(GameObject _player)
    {
        player = _player;
    }
}
