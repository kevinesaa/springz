using System;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [System.Serializable]
    public struct CameraSizeByPlayerState {
        public PlayerController.PlayerState playerState;
        public float followVelocity;
        public float changeSpeed;
        public float size;
    }



    public PlayerController player;
    public CameraSizeByPlayerState[] cameraSizeByPlayerState;

    private Camera mCamera;
    private Vector3 relativeDistance;
    private Dictionary<PlayerController.PlayerState, CameraSizeByPlayerState> cameraSizeByPlayerStateDictionary;

    private void Awake()
    {
        mCamera = GetComponent<Camera>();
        relativeDistance = transform.position - player.transform.position;
        cameraSizeByPlayerStateDictionary = new Dictionary<PlayerController.PlayerState, CameraSizeByPlayerState>();
        foreach (CameraSizeByPlayerState state in cameraSizeByPlayerState)
        {
            if (!cameraSizeByPlayerStateDictionary.ContainsKey(state.playerState))
            {
                cameraSizeByPlayerStateDictionary.Add(state.playerState, state);
            }
            else
            {
                cameraSizeByPlayerStateDictionary[state.playerState] = state;
            }
        }

    }

    
    private void LateUpdate()
    {

        CameraSizeByPlayerState cameraByPlayerStae = cameraSizeByPlayerStateDictionary[player.State];
        float speed = cameraByPlayerStae.changeSpeed * Time.deltaTime;
        float size = Mathf.MoveTowards(mCamera.orthographicSize, cameraByPlayerStae.size, speed);
        float diff = mCamera.orthographicSize - size;

        mCamera.orthographicSize = size;

        Vector3 target = player.transform.position + relativeDistance;
        Vector3 position = transform.position;
        position = Vector3.MoveTowards(transform.position, target, cameraByPlayerStae.followVelocity * Time.deltaTime);

        transform.position = position;
    }

   
}

