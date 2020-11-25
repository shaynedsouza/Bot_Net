using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManagerScript : MonoBehaviour
{

    //Singleton pattern
    public static GameManagerScript instance;

    [SerializeField] float packetSpawnMinTime = 1f, packetSpawnMaxTime = 3f;
    [SerializeField] GameObject startPanel, endPanel;
    [SerializeField] TextMeshProUGUI goodPacketsText, virusesTerminatedText;
    [SerializeField] Image serverHealthImage, antiVirusHealthImage;
    [SerializeField] LayerMask layerMask;

    int serverHealth, serverHealthMax = 15, antiVirusHealth, antiVirusHealthMax = 30, goodPackets, virusesTerminated;

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
    }

    void Update()
    {
        //UpdateAntivirusBar();
        CheckClick();
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "goodPacket")
        {
            Destroy(other.gameObject);
            antiVirusHealth += 1;
            goodPacketsText.text = $"Good Packets : {antiVirusHealth}";
            antiVirusHealthImage.fillAmount = (float)antiVirusHealth / (float)antiVirusHealthMax;

        }
        else if (other.tag == "virus")
        {
            Destroy(other.gameObject);
            serverHealth -= 1;
            virusesTerminatedText.text = $"Viruses Terminated : {serverHealth}";
            serverHealthImage.fillAmount = (float)serverHealth / (float)serverHealthMax;

        }
    }

    private void UpdateAntivirusBar()
    {

    }




    //Checks if the user clicked on the screen
    private void CheckClick()
    {
        if (Input.GetMouseButton(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if(Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask)){
                Debug.Log("hit");
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
        startPanel.SetActive(false);
    }



}
