using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PacketTravelScript : MonoBehaviour
{
    [SerializeField] float speed = 4f;
    Rigidbody rigidBody;
    Vector3 startPosition, targetPosition, direction;

    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();

    }

    //Subscribe to the event
    private void OnEnable()
    {
        transform.parent.GetComponent<ClientScript>().userHasClickedAction += FreezeTravel;
    }


    //Unsubscribe to the event
    private void OnDisable()
    {
        transform.parent.GetComponent<ClientScript>().userHasClickedAction -= FreezeTravel;
    }





    void FixedUpdate()
    {
        if (targetPosition != null)
        {
            travelToDestination();
        }
    }

    private void travelToDestination()
    {

        rigidBody.velocity = direction * speed;


    } 
    
    
    //Invoked via subscription to the clientScript event, used to stop the packet
    private void FreezeTravel()
    {
        Debug.Log("Unsubscribed");
        speed = 0f;
        //Invoke func to exploded virus
        Destroy(gameObject, 3f);
    }





    //Sets the target position when the packet is created
    public void SetTarget(Vector3 destination)
    {
        startPosition = transform.position;
        targetPosition = destination;
        direction = (targetPosition - startPosition).normalized;

    }

}
