using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System;

public class GameManagerScript : MonoBehaviour
{

    //Singleton pattern
    public static GameManagerScript instance;

    [Header("Particle settings")]
    [SerializeField] float defaulPacketSpeed = 1f;
    [SerializeField] float godModePacketSpeed = 10f, packetSpawnMinTime = 6f, godModeTime = 5f;
    [SerializeField] GameObject startPanel, endPanel;
    [SerializeField] List<GameObject> clients;
    [SerializeField] TextMeshProUGUI goodPacketsText, virusesTerminatedText;
    [SerializeField] Image serverHealthImage, antiVirusHealthImage;
    [SerializeField] LayerMask layerMask;
    public static Action<float> spawnTimerAction, godModeAction;


    int serverHealth, serverHealthMax = 15, antiVirusHealthMax = 10, virusesTerminated, timeForSpawnReduction = 5;
    public int score, antiVirusHealth;
    bool gameStarted = false, godMode = false;

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
        antiVirusHealth = 0;
        serverHealth = serverHealthMax;
        goodPacketsText.text = "Good Packets : 0";
        virusesTerminatedText.text = "Viruses Terminated : 0";
        startPanel.SetActive(true);
        score = 0;
        spawnTimerAction.Invoke(packetSpawnMinTime);
        godModeAction.Invoke(defaulPacketSpeed);

    }




    void Update()
    {
        CheckClick();
    }

    //Starts the game
    public void StartGame()
    {
        gameStarted = true;
        ClientScript[] clientScripts = FindObjectsOfType<ClientScript>();

        foreach (ClientScript child in clientScripts)
        {
            child.StartTrasmission();
        }
        startPanel.SetActive(false);
        goodPacketsText.text = "Good Packets : 0";
        virusesTerminatedText.text = "Viruses Terminated : 0";
    }


    //end the game 
    public void EndGame()
    {
        ClientScript[] clientScripts = FindObjectsOfType<ClientScript>();
        Debug.Log("array length     " + clientScripts.Length);
        foreach (ClientScript child in clientScripts)
        {
            child.StopTransmission();
        }


        endPanel.SetActive(true);
        //Stop emmitting packets
        //Display restart and score?
    }


    //Restarts the game
    public void RestartGame()
    {
        SceneManager.LoadScene(0);
    }



    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "goodPacket")
        {

            GoodPacket(other);

        }
        else if (other.tag == "virus")
        {
            BadPacket(other);

        }
    }


    //OnTrigger good packet
    private void GoodPacket(Collider other)
    {
        Destroy(other.gameObject);

        if (gameStarted)
        {
            antiVirusHealth += 1;
            score += 1;
            goodPacketsText.text = $"Good Packets : {score}";
            antiVirusHealthImage.fillAmount = (float)antiVirusHealth / (float)antiVirusHealthMax;
            if (antiVirusHealth == antiVirusHealthMax)
            {
                resetAntivirus();
            }

            if(score % timeForSpawnReduction == 0 && packetSpawnMinTime>1f)
            {
                packetSpawnMinTime -= 1f;
                spawnTimerAction.Invoke(packetSpawnMinTime);
                Debug.Log($"Min spawn time : {packetSpawnMinTime}");
            }



        }
    }




    //OnTrigger bad packet
    private void BadPacket(Collider other)
    {

        Destroy(other.gameObject);

        if (gameStarted && !godMode)
        {
            serverHealth -= 1;
            if (serverHealth == 0)
            {
                EndGame();
            }
            else
            {
                serverHealthImage.fillAmount = (float)serverHealth / (float)serverHealthMax;
            }
        }
    }


  


    //Checks if the user clicked on the screen
    private void CheckClick()
    {
        if (Input.GetMouseButton(0) && gameStarted)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if(Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask)){
                if (hit.transform.parent.parent.GetComponent<ClientScript>())
                {
                    ClientScript tempClientScript = hit.transform.parent.parent.GetComponent<ClientScript>();


                    //Will disable if not done yet
                    if (tempClientScript.CanDisable())
                    {
                        //Checks if the packet is a virus, to update the required field
                        if (tempClientScript.isVirus())
                        {
                            virusesTerminated += 1;
                            virusesTerminatedText.text = $"Viruses Terminated : {virusesTerminated}";
                        }
                        tempClientScript.DisableClient();
                    }
                }
                else
                {
                    Debug.Log("Client script in ancestor missing");
                }

            }
        }
    }


    private void resetAntivirus()
    {
        //Delay
        //speed up?
        if(score == antiVirusHealthMax)
        {
            clients[3].SetActive(true);
            clients[4].SetActive(true);
        }
        else if(score == antiVirusHealthMax * 2)
        {
            clients[5].SetActive(true);
            clients[6].SetActive(true);
        }

        godMode = true;
        spawnTimerAction.Invoke(0f);
        godModeAction.Invoke(godModePacketSpeed);
        StartCoroutine(DisableGodMode());
    }

    //-1 Means that the default speed in the script will  be set
    IEnumerator DisableGodMode()
    {
        yield return new WaitForSeconds(godModeTime);
        godMode = false;
        spawnTimerAction.Invoke(packetSpawnMinTime);
        godModeAction.Invoke(defaulPacketSpeed);
        antiVirusHealth = 0;
        antiVirusHealthImage.fillAmount = (float)antiVirusHealth / (float)antiVirusHealthMax;

    }



}
