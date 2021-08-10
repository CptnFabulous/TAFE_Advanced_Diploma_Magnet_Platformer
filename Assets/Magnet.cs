using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Magnet : MonoBehaviour
{
    public PlayerController player;

    [Header("Pulling")]
    public float range = 25;
    public float pullAngle = 20;
    public float pullSpeed = 10;
    public LayerMask detection = ~0;

    [Header("Grabbing")]
    public Rigidbody pickedUpObject;
    Collider pickedUpCollider;
    public float grabDistance;
    public Transform grabLocation;
    bool isDropping;

    [Header("Rotating")]
    public float rotateX = 100;
    public float rotateY = 100;

    [Header("Launching")]
    public float pushForce = 50;
    public float pushRange = 3;
    public float pushAngle = 5;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (pickedUpObject == null)
        {
            if (Input.GetButtonUp("Mouse Secondary"))
            {
                isDropping = false;
            }

            if (Input.GetButton("Mouse Secondary") && isDropping == false)
            {
                Debug.Log("Pulling on frame " + Time.frameCount);
                // Pull every object in the specified range and angle
                Collider[] physicsObjects = Physics.OverlapSphere(transform.position, range, detection);
                for (int i = 0; i < physicsObjects.Length; i++)
                {
                    Rigidbody rb = physicsObjects[i].GetComponent<Rigidbody>();
                    float angle = Vector3.Angle(transform.forward, physicsObjects[i].bounds.center - transform.position);
                    if (rb != null && rb.isKinematic == false && angle <= pullAngle)
                    {
                        rb.MovePosition(rb.transform.position + (transform.position - rb.transform.position) * pullSpeed * Time.deltaTime);
                        if (Vector3.Distance(rb.transform.position, transform.position) <= grabDistance)
                        {
                            pickedUpObject = rb;
                            pickedUpObject.transform.SetParent(grabLocation);
                            pickedUpObject.isKinematic = true;
                            pickedUpCollider = pickedUpObject.GetComponent<Collider>();
                            pickedUpCollider.enabled = false;
                            pickedUpObject.transform.position = grabLocation.position;
                        }
                    }
                }
            }
        }
        else
        {
            if (Input.GetButton("Mouse Tertiary"))
            {
                player.canLook = false;
                Vector3 rotationAngles = new Vector3(Input.GetAxis("Mouse Y") * rotateY, Input.GetAxis("Mouse X") * rotateX, 0);
                pickedUpObject.transform.Rotate(rotationAngles * Time.deltaTime);
                //pickedUpObject.transform.localRotation = Quaternion.Euler(pickedUpObject.transform.localRotation * rotationAngles * Time.deltaTime);
            }
            else
            {
                player.canLook = true;
            }

            if (Input.GetButtonDown("Mouse Secondary"))
            {
                Debug.Log("Setting down object");
                // Set down object
                isDropping = true;
                pickedUpObject.transform.SetParent(null);
                pickedUpObject.isKinematic = false;
                pickedUpCollider.enabled = true;
                pickedUpObject = null;
                player.canLook = true; // Sanity check
            }
        }

        if (Input.GetButtonDown("Mouse Primary"))
        {
            if (pickedUpObject != null)
            {
                pickedUpObject.transform.SetParent(null);
                pickedUpObject.isKinematic = false;
                pickedUpCollider.enabled = true;
                pickedUpObject.AddForce(transform.forward * pushForce, ForceMode.Impulse);
                pickedUpObject = null;

            }
            else
            {

                Debug.Log("General push");
                // Push away objects that are close to you
                Collider[] physicsObjects = Physics.OverlapSphere(transform.position, pushRange, detection);
                for (int i = 0; i < physicsObjects.Length; i++)
                {
                    Rigidbody rb = physicsObjects[i].GetComponent<Rigidbody>();
                    float angle = Vector3.Angle(transform.forward, physicsObjects[i].bounds.center - transform.position);
                    if (rb != null && rb.isKinematic == false && angle <= pushAngle)
                    {
                        rb.AddForce(transform.forward * pushForce);
                    }
                }
            }
        }
    }
}
