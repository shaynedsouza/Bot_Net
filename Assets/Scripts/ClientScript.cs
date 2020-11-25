using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.Events;

public class ClientScript : MonoBehaviour
{

    [SerializeField] GameObject goodPacketPrefab, virusPrefab;

    float packetSpawnMinTime = 4f, packetSpawnMaxTime = 7f, disabledTime = 5f;
    bool canGeneratePacket = true, isDisabled = false, gameRunning = true;
    Vector3 packetSpawnPos;
    GameObject packet;
    public Action<float> userHasClickedAction;

    void Start()
    {
        SetPacketSpawnPos();
    }








    void Update()
    {
        GeneratePackets();
    }



    //Gets the spawn point for the packets
    private void SetPacketSpawnPos()
    {
        bool linkPresent = false;
        foreach (Transform child in transform)
        {
            if (child.tag == "link")
            {
                packetSpawnPos = child.position;
                linkPresent = true;
            }
        }


        if (!linkPresent)
        {
            Debug.Log("Link gameobject missing");
        }
    }





    //Generates a packet after a delay using a coroutine 
    private void GeneratePackets()
    {
        if (canGeneratePacket && !isDisabled && packet == null && gameRunning)
        {
            StartCoroutine(Deploy());
        }
    }

    //Random values 1-75 generate a good packet and 76-100 a virus 
    IEnumerator Deploy()
    {
        canGeneratePacket = false;
        int randomValue = UnityEngine.Random.Range(0, 101);

        if (randomValue <= 75)
        {
            packet = Instantiate(goodPacketPrefab, packetSpawnPos, Quaternion.identity, transform);
        }
        else
        {
            packet = Instantiate(virusPrefab, packetSpawnPos, Quaternion.identity, transform);

        }

        packet.GetComponent<PacketTravelScript>().SetTarget(transform.position);
        float waitTime = UnityEngine.Random.Range(packetSpawnMinTime, packetSpawnMaxTime);
        yield return new WaitForSeconds(waitTime);
        if (gameRunning)
        {
            canGeneratePacket = true;
        }
    }



    //Called when the user clicks on the link
    public void DisableClient()
    {

        if (!isDisabled && packet != null)
        {
            packet = null;
            isDisabled = true;
            userHasClickedAction.Invoke(3f);
            StartCoroutine(EnableClient());
        }

    }


    IEnumerator EnableClient()
    {
        yield return new WaitForSeconds(disabledTime);
        

        //Avoids chance of getting enabled if coroutine starts before the game is lost
        if (gameRunning)
        {
            isDisabled = false;
        }
    }


    //Invoked when the user clicks on the start button
    public void StartTrasmission()
    {
        if (packet != null)
        {
            canGeneratePacket = true;
            isDisabled = false;
            packet = null;
            userHasClickedAction.Invoke(0f);
        }
    }




    //Invoked when the game ends
    public void StopTransmission()
    {
        {
            isDisabled = true; // Not needed?
            packet = null;
            gameRunning = false;
            if (userHasClickedAction != null)
            {
                userHasClickedAction.Invoke(UnityEngine.Random.Range(0.5f, 1f));
            }
        }
    }



}
