using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PacketTravelScript : MonoBehaviour
{
    float speed = 1f;
    Rigidbody rigidBody;
    Vector3 startPosition, targetPosition, direction;
    ParticleSystem myParticleSystem;

    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        myParticleSystem = GetComponentInChildren<ParticleSystem>();
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
    private void FreezeTravel(float destroyTime)
    {
        speed = 0f;
        //Invoke func to exploded virus
        //Destroy(gameObject, destroyTime);
        StartCoroutine(SelfDestruct(destroyTime));
        
    }



    IEnumerator SelfDestruct(float destroyTime)
    {
        yield return new WaitForSeconds(destroyTime);
        gameObject.GetComponent<MeshRenderer>().enabled = false;
        myParticleSystem.Play();
        yield return new WaitForSeconds(myParticleSystem.main.duration);
        Destroy(gameObject);

    }



    //Sets the target position when the packet is created
    public void SetTarget(Vector3 destination)
    {
        startPosition = transform.position;
        targetPosition = destination;
        direction = (targetPosition - startPosition).normalized;

    }

    public void SetSpeed(float newSpeed)
    {
        speed = newSpeed;
    }




}
