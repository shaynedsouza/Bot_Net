using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientScript : MonoBehaviour
{

    [SerializeField] GameObject goodPacketPrefab, virusPrefab;

    float packetSpawnMinTime = 4f, packetSpawnMaxTime = 7f;
    bool canGeneratePacket = true;
    Vector3 packetSpawnPos;



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
                //Debug.Log(packetSpawnPos);
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
        if (canGeneratePacket)
        {
            StartCoroutine(Deploy());
        }
    }

    //Random values 1-75 generate a good packet and 76-100 a virus 
    IEnumerator Deploy()
    {
        canGeneratePacket = false;
        float waitTime = UnityEngine.Random.Range(packetSpawnMinTime, packetSpawnMaxTime);
        yield return new WaitForSeconds(waitTime);

        int randomValue = UnityEngine.Random.Range(0, 101);

        if (randomValue <= 75)
        {
            GameObject packet = Instantiate(goodPacketPrefab, packetSpawnPos, Quaternion.identity, transform);
            packet.GetComponent<PacketTravelScript>().SetTarget(transform.position);
        }
        else
        {
            GameObject packet = Instantiate(virusPrefab, packetSpawnPos, Quaternion.identity, transform);
            packet.GetComponent<PacketTravelScript>().SetTarget(transform.position);

        }
        canGeneratePacket = true;

    }





}
