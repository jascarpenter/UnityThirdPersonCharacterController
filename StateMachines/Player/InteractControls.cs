using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion.FinalIK;

public class InteractControls : MonoBehaviour
{
    public InteractionSystem interactionSystem;

    void OnGUI() 
    {
        // If not paused, find the closest InteractionTrigger that the character is in contact with
        int closestTriggerIndex = interactionSystem.GetClosestTriggerIndex();

        // ...if none found, do nothing
        if (closestTriggerIndex == -1) return;

        if (Input.GetKey(KeyCode.E)) {
            interactionSystem.TriggerInteraction(closestTriggerIndex, false);
        }
	}
}
