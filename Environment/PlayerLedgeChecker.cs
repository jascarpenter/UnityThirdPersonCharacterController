// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// public class PlayerLedgeChecker : MonoBehaviour
// {
//     [SerializeField] Transform _handPosition, _standPosition;
//     [SerializeField] private float yOffset = 6.5f;

//     private Vector3 newHandPos;

//     // Start is called before the first frame update
//     void Start()
//     {
//         newHandPos = new Vector3(_handPosition.position.x, _handPosition.position.y - yOffset, _handPosition.position.z);
//     }

//     // Update is called once per frame
//     void Update()
//     {
        
//     }

//     private void OnTriggerEnter(Collider other) {
//         if (other.CompareTag("LedgeChecker"))
//         {
//             var player = other.GetComponent<Player>();
//             player.GrabLedge(newHandPos, this);
//         }
//     }

//     public Vector3 GetStandUpPos()
//     {
//         return _standPosition.position;
//     }

//     public void GrabLedge(Vector3 handPos, PlayerLedgeChecker currentLedge)
//     {
//         Ctx.CharacterController.enabled = false;

//         _isGrabbingLedge = true;
//         transform.position = handPos;
//         _isJumping = false;
//         _activeLedge = currentLedge;
//     })
// }
