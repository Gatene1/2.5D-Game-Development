using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LedgeGrab : MonoBehaviour
{
    [SerializeField] private Vector3 _hangingPosition;  

    private Player _player;

    private Vector3 _standupPosition;

    // Start is called before the first frame update
    void Start()
    {
        _player = GameObject.Find("Player").GetComponent<Player>();
        if (_player == null) Debug.LogError("Cannot find Player");
        _standupPosition = transform.parent.gameObject.transform.Find("Standup_Pos").GetComponent<Transform>().transform.position;
        if (_standupPosition == null) Debug.LogError("Cannot Find _standupposition");
        //_standupPosition = transform.parent.gameObject.transform.GetChild(24).transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        bool isJumping = _player.GetJumpingStatus();
        Debug.Log(other.name + "Jump = " + isJumping);

        if (other.name == "Ledge_Grab_Checker" && isJumping)
        {
            Debug.Log("Throw Hands Up");
            _player.ThrowHandsUp(_hangingPosition, _standupPosition); //2nd parameter is to tell the player script where to go after the climbing animation before the standing up animation.
        }
    }
}
