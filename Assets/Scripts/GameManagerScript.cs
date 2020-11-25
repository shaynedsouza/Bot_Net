using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerScript : MonoBehaviour
{

    //Singleton pattern
    public static GameManagerScript instance;

    [SerializeField] float packetSpawnMinTime = 1f, packetSpawnMaxTime = 3f;
    float serverHealth, antiVirusHealth, goodPackets, virusesTerminated;
    

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
    }


    void Start()
    {
        
    }

    void Update()
    {
        UpdateAntivirusBar();
    }


    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "goodPacket")
        {
            Destroy(other.gameObject);
        }
        else if(other.tag == "virus")
        {
            Destroy(other.gameObject);

        }
    }

    private void UpdateAntivirusBar()
    {

    }




    private void resetAntivirus()
    {
        antiVirusHealth = 0f;
    }





}
