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

    void Update()
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





    //Sets the target position when the packet is created
    public void SetTarget(Vector3 destination)
    {
        startPosition = transform.position;
        targetPosition = destination;
        //Debug.Log($" start : {startPosition},  target : {targetPosition}");
        direction = (targetPosition - startPosition).normalized;
        //direction = targetPosition - startPosition;

    }

}
