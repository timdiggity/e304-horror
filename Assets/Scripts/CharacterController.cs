/* 
 * author : jiankaiwang
 * description : The script provides you with basic operations of first person control.
 * platform : Unity
 * date : 2017/12
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : MonoBehaviour {

    public float speed = 10.0f;
    private float translation;
    private float straffe;

    public Transform head;

    public float itemPickupDistance;

    //Pickup objects runtime variables
    Transform attachedObject = null;
    float attachedDistance = 0f;

    // Use this for initialization
    void Start () {
        // turn off the cursor
        Cursor.lockState = CursorLockMode.Locked;		
	}
	
	// Update is called once per frame
	void Update () {
        // Input.GetAxis() is used to get the user's input
        // You can furthor set it on Unity. (Edit, Project Settings, Input)
        translation = Input.GetAxis("Vertical") * speed * Time.deltaTime;
        straffe = Input.GetAxis("Horizontal") * speed * Time.deltaTime;
        transform.Translate(straffe, 0, translation);

        if (Input.GetKeyDown("escape")) {
            // turn on the cursor
            Cursor.lockState = CursorLockMode.None;
        }

        // Picking objects - https://www.youtube.com/watch?v=Tz_gecLwX2s
        RaycastHit hit;
        bool cast = Physics.Raycast(head.position, head.forward, out hit, itemPickupDistance);

        if (Input.GetKeyDown(KeyCode.F)) 
        {
            //  Drop the picked object
            if (attachedObject != null) {
                attachedObject.SetParent(null);

                //  Disable is kinematic for the rigidbody, if any
                if (attachedObject.GetComponent<Rigidbody>() != null)
                    attachedObject.GetComponent<Rigidbody>().isKinematic = false;

                //  Enable the collider, if any
                if (attachedObject.GetComponent<Collider>() != null)
                    attachedObject.GetComponent<Collider>().enabled = true;

                attachedObject = null;
            }

            //  Pick up an object
            else 
            {
                if (cast) 
                {
                    if (hit.transform.CompareTag("Pickup")) 
                    {
                        attachedObject = hit.transform;
                        attachedObject.SetParent(transform);

                        attachedDistance = Vector3.Distance(attachedObject.position, head.position);

                        //  Enable is kinematic for the rigidbody, if any
                        if (attachedObject.GetComponent<Rigidbody>() != null)
                            attachedObject.GetComponent<Rigidbody>().isKinematic = true;

                        //  Disable the collider, if any
                        //  This is necessary
                        if (attachedObject.GetComponent<Collider>() != null)
                            attachedObject.GetComponent<Collider>().enabled = false;
                    }
                }
            }
        }
    }
}