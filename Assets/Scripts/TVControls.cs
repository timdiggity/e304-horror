using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TVControls : MonoBehaviour
{
    public float interactDistance = 6f;
    private bool canInteract = false;

    public GameObject hanwei;
    
    void Start() 
    {
        hanwei = GameObject.Find("HanweiScreen");
        hanwei.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && canInteract)
        {
            // Perform the interaction logic for the TV, e.g., turn it on or off, play a video, etc.
            InteractWithTV();
        }
    }

    void InteractWithTV()
    {
        // Add your specific interaction logic for the TV here
        Debug.Log("Interacting with TV!");

        // Turn off and on tv
        if(hanwei.activeSelf)
            hanwei.SetActive(false);
        else
            hanwei.SetActive(true);
    }

    void LateUpdate()
    {
        // Perform raycasting to check if the player is looking at the TV
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, interactDistance))
        {
            if (hit.collider.gameObject == gameObject)
            {
                canInteract = true;

            }
            else
            {
                canInteract = false;
            }
        }
        else
        {
            canInteract = false;
        }
    }
}