using System;
using Unibas.DBIS.DynamicModelling;
using Unibas.DBIS.DynamicModelling.Models;
using Unibas.DBIS.VREP;
using Unibas.DBIS.VREP.Puzzle;
using Unibas.DBIS.VREP.UI;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Valve.VR.InteractionSystem;

namespace World {
  public class SteamVRPuzzleButton : MonoBehaviour {
    
    [Serializable]
    public class ButtonModel {
      public float Size;
      public float Border;
      public float Height;
      public Material PedestalMaterial;
      public Material PlateMaterial;
      public Material ButtonMaterial;
      public bool HasPedestal;

      public ButtonModel(float size, float border, float height, Material pedestalMaterial,
        Material plateMaterial, Material buttonMaterial, bool hasPedestal = true) {
        Size = size;
        Border = border;
        Height = height;
        PedestalMaterial = pedestalMaterial;
        PlateMaterial = plateMaterial;
        ButtonMaterial = buttonMaterial;
        HasPedestal = hasPedestal;
      }
    }

    public Displayal Displayal;
    private bool pressed = false;

    [SerializeField] public ButtonModel Model;

    public string Text;
    public Sprite Image;


    public GameObject UIElement;

    public GameObject Pedestal;
    public GameObject Button;

    public Action OnTeleportStart;
    public Action OnTeleportEnd;


    public static SteamVRPuzzleButton Create(GameObject parent, Vector3 position, Displayal displayal,
      ButtonModel model, string text) {
      var go = new GameObject("TeleportButton");
      SteamVRPuzzleButton tp = go.AddComponent<SteamVRPuzzleButton>();
      tp.transform.SetParent(parent.transform, false);
      tp.transform.localPosition = position;
      tp.Displayal = displayal;
      tp.Model = model;
      tp.Text = text;


      return tp;
    }

    public static SteamVRPuzzleButton Create(GameObject parent, Vector3 position, Displayal displayal,
      ButtonModel model, Sprite image) {
      var go = new GameObject("TeleportButton");
      SteamVRPuzzleButton tp = go.AddComponent<SteamVRPuzzleButton>();
      tp.transform.SetParent(parent.transform, false);
      tp.transform.localPosition = position;
      tp.Displayal = displayal;
      tp.Model = model;
      tp.Image = image;


      return tp;
    }

    public static SteamVRPuzzleButton Create(GameObject parent, Vector3 position, Displayal displayal,
      ButtonModel model)
    {
      var go = new GameObject("TeleportButton");
      SteamVRPuzzleButton tp = go.AddComponent<SteamVRPuzzleButton>();
      tp.transform.SetParent(parent.transform, false);
      tp.transform.localPosition = position;
      tp.Displayal = displayal;
      tp.Model = model;

      return tp;
    }

    private void Start() {
      Generate();
    }

    private float GetButtonSize() {
      return Model.Size - 2 * Model.Border;
    }

    private float GetButtonHeight() {
      return Model.Border / 2f;
    }

    private float GetButtonBorder() {
      return Model.Border / 2f;
    }

    private ComplexCuboidModel CreateButtonModel() {
      var size = GetButtonSize();
      var height = GetButtonHeight();
      var border = GetButtonBorder();
      ComplexCuboidModel model = new ComplexCuboidModel();
      model.Add(Vector3.zero, new CuboidModel(size, size, height, Model.PlateMaterial));
      model.Add(new Vector3(border, border, -height),
        new CuboidModel(size - 2 * border, size - 2 * border, height, Model.ButtonMaterial));
      return model;
    }

    private CuboidModel CreatePedestalModel() {
      return new CuboidModel(Model.Size - Model.Border, Model.Height, Model.Size - Model.Border,
        Model.PedestalMaterial);
    }

    public void Generate() {
      GenerateModel();
      SetupInteraction();
      SetupUIElement();
    }

    private void GenerateModel() {
      name = "TeleportButton";

      if (Model.HasPedestal) {
        Pedestal = ModelFactory.CreateCuboid(CreatePedestalModel());
        Pedestal.transform.SetParent(transform, false);
        Pedestal.transform.localPosition = Vector3.zero;
        Pedestal.name = "Pedestal";
      } else {
        Model.Height = 0;
      }


      Button = ModelFactory.CreateModel(CreateButtonModel());
      Button.transform.SetParent(transform, false);
      Button.transform.localPosition =
        new Vector3(Model.Border / 2, Model.Height + GetButtonHeight(), Model.Border / 2);
      if (Model.HasPedestal) {
        Button.transform.Rotate(Vector3.right, 90);
      }

      Button.name = "Button";
    }

    private void SetupInteraction() {
      BoxCollider col = Button.AddComponent<BoxCollider>();
      var size = GetButtonSize();
      var height = GetButtonHeight();
      var border = GetButtonBorder();
      col.size = new Vector3(size, size, border * 2);
      col.center = new Vector3(size / 2f, size / 2f, -border);
      Button.AddComponent<Button>();
      var hand = new CustomEvents.UnityEventHand();
      hand.AddListener(h => { ButtonPress(); });
      var clickable = col.gameObject.AddComponent<Clickable>();
      clickable.Action = b => ButtonPress();
      Button.AddComponent<UIElement>().onHandClick = hand;
    }


    public void ButtonPress() {
      Debug.Log("Puzzle Press");
      if (!pressed) {
        FadeController.Instance.FadeInOut();
        PuzzleManager.GetInstance().SetPuzzle(Displayal);
        pressed = true;
      } else
      {
        pressed = false;
        Debug.Log("Puzzle removed");
        PuzzleManager.GetInstance().RemovePuzzle();
      }
    }

    private void SetupUIElement() {
      if (string.IsNullOrEmpty(Text) && Image == null)
      {
        SetupFromPrefab();
      }

      if (!string.IsNullOrEmpty(Text)) {
        SetupText(Text);
      } else if (Image != null) {
        SetupImage(Image);
      }
    }

    private void SetupText(string text) {
      var canvas = CreateAndPositionCanvas();
      var to = new GameObject("Text");
      to.transform.SetParent(canvas.transform, false);
      var txt = to.AddComponent<Text>();
      txt.text = text;
      txt.alignment = TextAnchor.MiddleCenter;
      txt.resizeTextForBestFit = true;
      var f = Resources.Load<Font>("Fonts/Glacial-Indifference/GlacialIndifference-Regular");
      if (f == null) {
        f = Font.CreateDynamicFontFromOSFont("Arial", 14);
      }

      txt.font = f;
      UIElement = to;
    }

    private Canvas CreateAndPositionCanvas() {
      var co = new GameObject("Canvas");
      var canvas = co.AddComponent<Canvas>();
      var rt = (RectTransform) canvas.transform;
      //rt.sizeDelta = new Vector2(512,512);
      co.transform.localScale = new Vector3((GetButtonSize() - 2 * GetButtonBorder()) / rt.rect.width,
        (GetButtonSize() - 2 * GetButtonBorder()) / rt.rect.height);
      co.transform.SetParent(Button.transform, false);
      co.transform.localPosition = new Vector3(GetButtonSize() / 2, GetButtonSize() / 2,
        -(GetButtonHeight() + GetButtonBorder() / 100f));
      return canvas;
    }


    private void SetupImage(Sprite img) {
      var canvas = CreateAndPositionCanvas();
      var io = new GameObject("Image");
      var image = io.AddComponent<Image>();
      image.sprite = img;
      io.transform.SetParent(canvas.transform, false);
      UIElement = io;
    }

    private void SetupFromPrefab()
    {
      var pref = VREPController.Instance.GetBuildingManager().FancyButtonPrefab;
      var canvas = CreateAndPositionCanvas();
      var io = Instantiate(pref);
      io.transform.SetParent(canvas.transform, false);
      UIElement = io;
    }

    private class Clickable : MonoBehaviour
    {

      public Action<bool> Action;
      
      private void OnMouseDown()
      {
        Debug.Log("Clickable: Down!");
        Action.Invoke(true);
      }
    }
  }
}