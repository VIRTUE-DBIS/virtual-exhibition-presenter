using System;
using Unibas.DBIS.DynamicModelling;
using Unibas.DBIS.DynamicModelling.Models;
using Unibas.DBIS.VREP.Movement;
using UnityEngine;
using UnityEngine.UI;
using Valve.VR.InteractionSystem;

namespace Unibas.DBIS.VREP.World
{
  public class SteamVRTeleportButton : MonoBehaviour
  {
    [Serializable]
    public class TeleportButtonModel
    {
      public float size;
      public float border;
      public float height;
      public Material pedestalMaterial;
      public Material plateMaterial;
      public Material buttonMaterial;
      public bool hasPedestal;

      public TeleportButtonModel(float size, float border, float height, Material pedestalMaterial,
        Material plateMaterial, Material buttonMaterial, bool hasPedestal = true)
      {
        this.size = size;
        this.border = border;
        this.height = height;
        this.pedestalMaterial = pedestalMaterial;
        this.plateMaterial = plateMaterial;
        this.buttonMaterial = buttonMaterial;
        this.hasPedestal = hasPedestal;
      }
    }

    public Vector3 destination;

    [SerializeField] public TeleportButtonModel model;

    public string text;
    public Sprite image;

    public GameObject uiElement;
    public GameObject pedestal;
    public GameObject button;

    public PlayerTeleporter teleporter;

    public Action OnTeleportStart;
    public Action OnTeleportEnd;

    public static SteamVRTeleportButton Create(GameObject parent, Vector3 position, Vector3 destination,
      TeleportButtonModel model, string text)
    {
      var go = new GameObject("TeleportButton");
      var tp = go.AddComponent<SteamVRTeleportButton>();
      var t = tp.transform;

      t.SetParent(parent.transform, false);
      t.localPosition = position;
      tp.destination = destination;
      tp.model = model;
      tp.text = text;

      return tp;
    }

    public static SteamVRTeleportButton Create(GameObject parent, Vector3 position, Vector3 destination,
      TeleportButtonModel model, Sprite image)
    {
      var go = new GameObject("TeleportButton");
      var tp = go.AddComponent<SteamVRTeleportButton>();
      var t = tp.transform;

      t.SetParent(parent.transform, false);
      t.localPosition = position;
      tp.destination = destination;
      tp.model = model;
      tp.image = image;

      return tp;
    }

    private void Start()
    {
      Generate();
    }

    private float GetButtonSize()
    {
      return model.size - 2 * model.border;
    }

    private float GetButtonHeight()
    {
      return model.border / 2f;
    }

    private float GetButtonBorder()
    {
      return model.border / 2f;
    }

    private ComplexCuboidModel CreateButtonModel()
    {
      var size = GetButtonSize();
      var height = GetButtonHeight();
      var border = GetButtonBorder();
      var cubeModel = new ComplexCuboidModel();

      cubeModel.Add(Vector3.zero, new CuboidModel(size, size, height, this.model.plateMaterial));
      cubeModel.Add(new Vector3(border, border, -height),
        new CuboidModel(size - 2 * border, size - 2 * border, height, this.model.buttonMaterial));

      return cubeModel;
    }

    private CuboidModel CreatePedestalModel()
    {
      return new CuboidModel(model.size - model.border, model.height, model.size - model.border,
        model.pedestalMaterial);
    }

    public void Generate()
    {
      GenerateModel();
      teleporter = gameObject.AddComponent<PlayerTeleporter>();
      teleporter.destination = destination;
      SetupInteraction();
      SetupUIElement();
    }

    private void GenerateModel()
    {
      name = "TeleportButton";

      if (model.hasPedestal)
      {
        pedestal = ModelFactory.CreateCuboid(CreatePedestalModel());
        pedestal.transform.SetParent(transform, false);
        pedestal.transform.localPosition = Vector3.zero;
        pedestal.name = "Pedestal";
      }
      else
      {
        model.height = 0;
      }

      button = ModelFactory.CreateModel(CreateButtonModel());
      button.transform.SetParent(transform, false);
      button.transform.localPosition =
        new Vector3(model.border / 2, model.height + GetButtonHeight(), model.border / 2);

      if (model.hasPedestal)
      {
        button.transform.Rotate(Vector3.right, 90);
      }

      button.name = "Button";
    }

    private void SetupInteraction()
    {
      var col = button.AddComponent<BoxCollider>();
      var size = GetButtonSize();
      var height = GetButtonHeight();
      var border = GetButtonBorder();

      col.size = new Vector3(size, size, border * 2);
      col.center = new Vector3(size / 2f, size / 2f, -border);
      button.AddComponent<Button>();

      var hand = new CustomEvents.UnityEventHand();

      hand.AddListener(h => { ButtonPress(); });
      button.AddComponent<UIElement>().onHandClick = hand;
    }

    public void ButtonPress()
    {
      OnTeleportStart?.Invoke();
      teleporter.TeleportPlayer();
      OnTeleportEnd?.Invoke();
    }

    private void SetupUIElement()
    {
      if (string.IsNullOrEmpty(text) && image == null)
      {
        return;
      }

      if (!string.IsNullOrEmpty(text))
      {
        SetupText(text);
      }
      else if (image != null)
      {
        SetupImage(image);
      }
    }

    private void SetupText(string newText)
    {
      var canvas = CreateAndPositionCanvas();
      var to = new GameObject("Text");
      to.transform.SetParent(canvas.transform, false);
      var txt = to.AddComponent<Text>();

      txt.text = newText;
      txt.alignment = TextAnchor.MiddleCenter;
      txt.resizeTextForBestFit = true;

      var f = Resources.Load<Font>("Fonts/Glacial-Indifference/GlacialIndifference-Regular");
      if (f == null)
      {
        f = Font.CreateDynamicFontFromOSFont("Arial", 14);
      }

      txt.font = f;
      uiElement = to;
    }

    private Canvas CreateAndPositionCanvas()
    {
      var co = new GameObject("Canvas");
      var canvas = co.AddComponent<Canvas>();
      var rt = (RectTransform) canvas.transform;

      // rt.sizeDelta = new Vector2(512, 512);
      co.transform.localScale = new Vector3((GetButtonSize() - 2 * GetButtonBorder()) / rt.rect.width,
        (GetButtonSize() - 2 * GetButtonBorder()) / rt.rect.height);
      co.transform.SetParent(button.transform, false);
      co.transform.localPosition = new Vector3(GetButtonSize() / 2, GetButtonSize() / 2,
        -(GetButtonHeight() + GetButtonBorder() / 100f));

      return canvas;
    }

    private void SetupImage(Sprite img)
    {
      var canvas = CreateAndPositionCanvas();
      var io = new GameObject("Image");
      var newImage = io.AddComponent<Image>();

      newImage.sprite = img;
      io.transform.SetParent(canvas.transform, false);
      uiElement = io;
    }
  }
}