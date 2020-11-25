using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
public class GameManagerScript : MonoBehaviour
{

    //Singleton pattern
    public static GameManagerScript instance;

    [SerializeField] float packetSpawnMinTime = 1f, packetSpawnMaxTime = 3f;
    [SerializeField] GameObject startPanel, endPanel;
    [SerializeField] TextMeshProUGUI goodPacketsText, virusesTerminatedText;
    [SerializeField] Image serverHealthImage, antiVirusHealthImage;
    [SerializeField] LayerMask layerMask;

    int serverHealth, serverHealthMax = 1, antiVirusHealth, antiVirusHealthMax = 30, goodPackets, virusesTerminated;
    bool gameStarted = false;

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

    }

    void Update()
    {
        CheckClick();
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
            goodPacketsText.text = $"Good Packets : {antiVirusHealth}";
            antiVirusHealthImage.fillAmount = (float)antiVirusHealth / (float)antiVirusHealthMax;
        }
    }




    //OnTrigger bad packet
    private void BadPacket(Collider other)
    {

        Destroy(other.gameObject);

        if (gameStarted)
        {
            serverHealth -= 1;
            if (serverHealth == 0)
            {
                EndGame();
            }
            else
            {
                virusesTerminatedText.text = $"Viruses Terminated : {serverHealth}";
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
                    hit.transform.parent.parent.GetComponent<ClientScript>().DisableClient();

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
        antiVirusHealth = 0;
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
        Debug.Log("array length" + clientScripts.Length);
        foreach(ClientScript child in clientScripts)
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
}
