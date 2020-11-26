using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientScript : MonoBehaviour
{

    [SerializeField] GameObject goodPacketPrefab, virusPrefab;

    float packetSpawnMinTime = 6f, packetSpawnMaxTime, disabledTime = 5f;

    float packetSpeed = 1f; //Will be modified by game manage through action


    bool canGeneratePacket = true, isDisabled = false, gameRunning = true;
    Vector3 packetSpawnPos;
    GameObject packet;
    public Action<float> userHasClickedAction;



    private void OnEnable()
    {
        GameManagerScript.spawnTimerAction += SetSpawnSpeed;
        GameManagerScript.godModeAction += ModifySpeed;

    }


    private void OnDisable()
    {
        GameManagerScript.spawnTimerAction -= SetSpawnSpeed;
        GameManagerScript.godModeAction -= ModifySpeed;

    }

    void Start()
    {
        packetSpawnMaxTime = packetSpawnMinTime + 1f;
        SetPacketSpawnPos();
    }




    void Update()
    {
        GeneratePackets();
    }



    private void SetSpawnSpeed(float minSpawnSpeed)
    {
        packetSpawnMinTime = minSpawnSpeed;
        
        if(minSpawnSpeed == 0f)
        {
            packetSpawnMaxTime = minSpawnSpeed + 0.3f;

        }
        else
        {
            packetSpawnMaxTime = minSpawnSpeed + 1f;

        }
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

    //Random values 1-70 generate a good packet and 71-100 a virus 
    IEnumerator Deploy()
    {
        canGeneratePacket = false;
        int randomValue = UnityEngine.Random.Range(0, 101);

        if (randomValue <= 70)
        {
            packet = Instantiate(goodPacketPrefab, packetSpawnPos, Quaternion.identity, transform);
        }
        else
        {
            packet = Instantiate(virusPrefab, packetSpawnPos, Quaternion.identity, transform);

        }

        packet.GetComponent<PacketTravelScript>().SetTarget(transform.position);
        packet.GetComponent<PacketTravelScript>().SetSpeed(packetSpeed);
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



    public bool CanDisable()
    {
        return (!isDisabled && packet!=null);
    }

    public bool isVirus()
    {

        if (packet != null && packet.tag == "virus")
        {
            return true;

        }
        else
        { 
            return false;
        }

    }



    //Modifies speed during start and end of god mode
    private void ModifySpeed(float newSpeed)
    {
        packetSpeed = newSpeed;

        if (packet)
        {
            packet.GetComponent<PacketTravelScript>().SetSpeed(packetSpeed);
        }
    }



}
