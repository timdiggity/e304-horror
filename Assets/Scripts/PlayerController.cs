using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    [Header ("References")]
    public Rigidbody rb;
    public Transform head;
    public Camera camera;

    [Header ("Configurations")]
    public float walkSpeed;
    public float runSpeed;
    public float lookSensitivity;
    public float jumpSpeed;
    public float itemPickupDistance;

    Vector3 newVelocity;

    [Header ("Camera Effects")]
    public float baseCameraFov = 60f;
    public float baseCameraHeight = .85f;

    public float walkBobbingRate = .75f;
    public float runBobbingRate = 1f;
    public float maxWalkBobbingOffset = .2f;
    public float maxRunBobbingOffset = .3f;

    [Header ("Runtime")]
    bool isGrounded = false;
    bool isJumping = false;
    string activeAudioName = "soft";
    Transform attachedObject = null;
    float attachedDistance = 2f;

    [Header ("Audio")]
    public AudioSource audioWalkSoft;
    public AudioSource audioWalkHard;
    // public AudioSource audioWind;
    public float windPitchMultiplier;

    // Start is called before the first frame update
    void Start()
    {
        // Cursor control for first person
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        // Horizontal rotation
        transform.Rotate(Vector3.up * Input.GetAxis("Mouse X") * lookSensitivity);
    

        newVelocity = Vector3.up * rb.velocity.y;
        // new Vector3(0f, rb.velocity.y, 0f)

        // Movement
        float speed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed;
        newVelocity.x = Input.GetAxis("Horizontal") * speed;
        newVelocity.z = Input.GetAxis("Vertical") * speed;

        if (isGrounded) 
        {
            if (Input.GetKeyDown(KeyCode.Space) && !isJumping)
            {
                newVelocity.y = jumpSpeed;
                isJumping = true;
            }
        }

        bool isMovingOnGround = (Input.GetAxis ("Vertical") != 0f || Input.GetAxis("Horizontal") != 0f && isGrounded);

        if (isMovingOnGround)
        {
            float bobbingRate = Input.GetKey(KeyCode.LeftShift) ? runBobbingRate : walkBobbingRate;
            float bobbingOffset = Input.GetKey(KeyCode.LeftShift) ? maxRunBobbingOffset : maxWalkBobbingOffset;
            Vector3 targetHeadPosition = Vector3.up * baseCameraHeight + Vector3.up * (Mathf.PingPong(Time.time * bobbingRate, bobbingOffset) - bobbingOffset * 0.5f);
            head.localPosition = Vector3.Lerp(head.localPosition, targetHeadPosition, 0.1f);
        }   

        // Change forward to direction mouse is pointing
        rb.velocity = transform.TransformDirection(newVelocity);

        // Audio

        audioWalkSoft.enabled = isMovingOnGround && activeAudioName == "soft";
        audioWalkSoft.pitch = Input.GetKey(KeyCode.LeftShift) ? 1.75f : 1f;

        audioWalkHard.enabled = isMovingOnGround && activeAudioName == "hard";
        audioWalkHard.pitch = Input.GetKey(KeyCode.LeftShift) ? 1.75f : 1f;

       // audioWind.enabled = true;
       // audioWind.pitch = Mathf.Clamp(Mathf.Abs(rb.velocity.y * windPitchMultiplier), 0f, 2f) + Random.Range(-0.1f, 0.1f);

        // Picking objects
        RaycastHit hit;
        bool cast = Physics.Raycast(camera.transform.position, camera.transform.forward, out hit, itemPickupDistance);

        if (Input.GetKeyDown(KeyCode.F))
        {
            if (attachedObject != null)
            {
                attachedObject.SetParent(null);

                if (attachedObject.GetComponent<Rigidbody>() != null)
                    attachedObject.GetComponent<Rigidbody>().isKinematic = false;

                if (attachedObject.GetComponent<Collider>() != null)
                    attachedObject.GetComponent<Collider>().enabled = true;

                attachedObject = null;
            }

            else
            {
                if (cast)
                {
                    if (hit.transform.CompareTag("Pickup"))
                    {
                        attachedObject = hit.transform;
                        attachedObject.SetParent(transform);

                        if (attachedObject.GetComponent<Rigidbody>() != null)
                            attachedObject.GetComponent<Rigidbody>().isKinematic = true;

                        if (attachedObject.GetComponent<Collider>() != null)
                            attachedObject.GetComponent<Collider>().enabled = true;

                    }
                }
            }
        }
    }

    void FixedUpdate()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 1f))
            isGrounded = true;

        else 
            isGrounded = false;
    }

    void LateUpdate()
    {
        // Vertical Rotation
        float verticalRotation = -Input.GetAxis("Mouse Y") * lookSensitivity;
        verticalRotation = RestrictAngle(verticalRotation, -85f, 85f);

        // Rotate the camera vertically
        Vector3 newCameraEulerAngles = camera.transform.localEulerAngles + new Vector3(verticalRotation, 0f, 0f);
        newCameraEulerAngles.x = RestrictAngle(newCameraEulerAngles.x, -85f, 85f);
        camera.transform.localEulerAngles = newCameraEulerAngles;

        // FOV
        float fovOffset = (rb.velocity.y < 0f) ? Mathf.Sqrt(Mathf.Abs(rb.velocity.y)) : 0f;
        camera.fieldOfView = Mathf.Lerp(camera.fieldOfView, baseCameraFov + fovOffset, 0.25f);

        if (attachedObject != null)
        {
            attachedObject.position = head.position + head.forward * attachedDistance;
            attachedObject.Rotate(transform.right * Input.mouseScrollDelta.y * 30f, Space.World);
        }
    }

    public static float RestrictAngle (float angle, float angleMin, float angleMax)
    {
        if (angle >180)
            angle -= 360;

        else if (angle < -180)
            angle += 360;

        if (angle > angleMax)
            angle = angleMax;
        if (angle < angleMin)
            angle = angleMin;

        return angle;
    }

    void OnTriggerEnter(Collider other) 
    {
        Debug.Log("COLLIDED POG");
        
		if (other.gameObject.tag == "hard") {
			activeAudioName = "hard";
            Debug.Log("HARD GROUND");
		}

        else
            activeAudioName = "soft";
    }

    void OnTriggerExit(Collider other)
    {
        activeAudioName = "soft";
    }

    void OnCollisionStay (Collision col)
    {
        isGrounded = true;
        isJumping = false;
    }

    void OnCollisionExit (Collision col)
    {
        isGrounded = false;
    }
}
