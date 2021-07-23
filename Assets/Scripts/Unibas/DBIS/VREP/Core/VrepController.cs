using Unibas.DBIS.VREP.VREM;
using Unibas.DBIS.VREP.VREM.Model;
using Unibas.DBIS.VREP.World;
using UnityEngine;

namespace Unibas.DBIS.VREP.Core
{
  /// <summary>
  /// Controller component for VirtualExhibitionManager.
  /// </summary>
  public class VrepController : MonoBehaviour
  {
    private VremClient _vremClient;

    public Vector3 lobbySpawn = new Vector3(0, -9, 0);

    public Settings settings;
    public string settingsPath;

    public static VrepController Instance;
    private ExhibitionManager _exhibitionManager;

    /// <summary>
    /// Upon awaking, the settings are loaded and the VREM host address is sanitized and set.
    /// </summary>
    private void Awake()
    {
      if (Application.isEditor)
      {
        settings = string.IsNullOrEmpty(settingsPath) ? Settings.LoadSettings() : Settings.LoadSettings(settingsPath);
      }
      else
      {
        settings = Settings.LoadSettings();
      }

      SanitizeHost();

      // Set instance.
      Instance = this;
    }

    /// <summary>
    /// Fixes the server URL by adding the http:// prefix and/or trailing /.
    /// </summary>
    private void SanitizeHost()
    {
      if (!settings.VremAddress.EndsWith("/"))
      {
        settings.VremAddress += "/";
      }

      // TODO TLS support.
      if (!settings.VremAddress.StartsWith("http://"))
      {
        settings.VremAddress = "http://" + settings.VremAddress;
      }
    }

    /// <summary>
    /// Write settings on termination if the file doesn't already exist.
    /// </summary>
    private void OnApplicationQuit()
    {
      settings.StoreSettings();
    }

    /// <summary>
    /// Handles lobby start settings and loads the exhibition.
    /// </summary>
    private void Start()
    {
      // TODO After what happens in Awake, this is not necessary.
      settings ??= Settings.LoadSettings() ?? Settings.Default();

      var go = GameObject.FindWithTag("Player");
      if (go != null && settings.StartInLobby)
      {
        go.transform.position = new Vector3(0, -9.9f, 0);
      }

      var lby = GameObject.Find("Lobby");
      if (lby != null && !settings.StartInLobby)
      {
        lby.SetActive(false);
      }

      Debug.Log("Starting Exhibition manager.");
      _vremClient = gameObject.AddComponent<VremClient>();

      LoadAndCreateExhibition();
    }

    /// <summary>
    /// Creates and loads the exhibition specified in the configuration.
    /// </summary>
    public void LoadAndCreateExhibition()
    {
      _vremClient.serverUrl = settings.VremAddress;

      _vremClient.RequestExhibition(settings.ExhibitionId, ParseExhibition);
      Debug.Log("Requested exhibition.");
    }

    /// <summary>
    /// Restores exhibits upon pressing F12.
    /// </summary>
    private void Update()
    {
      if (Input.GetKey(KeyCode.F12))
      {
        _exhibitionManager.RestoreExhibits();
      }
    }

    /// <summary>
    /// Parses an exhibition in JSON format loaded from VREM and calls the exhibition manager to actually
    /// generate the loaded/parsed exhibition.
    /// </summary>
    /// <param name="json">The JSON string to parse.</param>
    private void ParseExhibition(string json)
    {
      if (json == null)
      {
        Debug.LogError("Couldn't load exhibition from backend.");
        Debug.Log("Loading placeholder instead.");
        var jtf = Resources.Load<TextAsset>("Configs/placeholder-exhibition");
        json = jtf.text;
      }

      Debug.Log(json);

      // Actual parsing to Exhibition model object.
      var ex = JsonUtility.FromJson<Exhibition>(json);

      // TODO Create lobby.

      // Create exhibition manager and generate the exhibition.
      _exhibitionManager = new ExhibitionManager(ex);
      _exhibitionManager.GenerateExhibition();

      if (Instance.settings.StartInLobby)
      {
        GameObject.Find("Lobby").GetComponent<Lobby>().ActivateRoomTrigger(_exhibitionManager);
      }
      else
      {
        GameObject.Find("Room").gameObject.transform.Find("Timer").transform.GetComponent<MeshRenderer>().enabled =
          true;
      }
    }
  }
}