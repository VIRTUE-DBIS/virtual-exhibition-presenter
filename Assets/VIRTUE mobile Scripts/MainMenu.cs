
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public InputField exField;
    public InputField ipField;
    public static string exText;
    public static string ipText;

    private  string ipAdress;
    private  string exhibitionID;
    
    // Default exhibition without inserting an IP / host name or exhibition ID in the menu
    private static string DEFAULT_IP = "http://10.34.58.45:9876/" ;
    private static string DEFAULT_ID = "5dc999c707aa7b0f23a335ec";

    // Starts the exhibition 
    public void startScene()
    {
        exText = exField.text;
        ipText = ipField.text;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }
}
