using System;
using Unibas.DBIS.DynamicModelling;
using Unibas.DBIS.DynamicModelling.Models;
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
            public float Size;
            public float Border;
            public float Height;
            public Material PedestalMaterial;
            public Material PlateMaterial;
            public Material ButtonMaterial;

            public TeleportButtonModel(float size, float border, float height, Material pedestalMaterial, Material plateMaterial, Material buttonMaterial)
            {
                Size = size;
                Border = border;
                Height = height;
                PedestalMaterial = pedestalMaterial;
                PlateMaterial = plateMaterial;
                ButtonMaterial = buttonMaterial;
            }
        }

        public Vector3 Destination;
        
        [SerializeField]
        public TeleportButtonModel Model;

        public string Text;
        public Sprite Image;


        public GameObject UIElement;

        public GameObject Pedestal;
        public GameObject Button;

        public PlayerTeleporter Teleporter;


        public static SteamVRTeleportButton Create(GameObject parent, Vector3 position, Vector3 destination,
            TeleportButtonModel model, string text)
        {
            var go = new GameObject("TeleportButton");
            SteamVRTeleportButton tp = go.AddComponent<SteamVRTeleportButton>();
            tp.transform.SetParent(parent.transform, false);
            tp.transform.localPosition = position;
            tp.Destination = destination;
            tp.Model = model;
            tp.Text = text;
            
            
            return tp;
        }

        public static SteamVRTeleportButton Create(GameObject parent, Vector3 position, Vector3 destination,
            TeleportButtonModel model, Sprite image)
        {
            var go = new GameObject("TeleportButton");
            SteamVRTeleportButton tp = go.AddComponent<SteamVRTeleportButton>();
            tp.transform.SetParent(parent.transform, false);
            tp.transform.localPosition = position;
            tp.Destination = destination;
            tp.Model = model;
            tp.Image = image;
            
            
            return tp;
        }

        private void Start()
        {
            Generate();
        }

        private float GetButtonSize()
        {
            return Model.Size - 2*Model.Border;
        }

        private float GetButtonHeight()
        {
            return Model.Border/2f;
        }

        private float GetButtonBorder()
        {
            return Model.Border/2f;
        }

        private ComplexCuboidModel CreateButtonModel()
        {
            var size = GetButtonSize();
            var height = GetButtonHeight();
            var border = GetButtonBorder();
            ComplexCuboidModel model = new ComplexCuboidModel();
            model.Add(Vector3.zero, new CuboidModel(size, size, height, Model.PlateMaterial));
            model.Add(new Vector3(border, border, -height),
                new CuboidModel(size - 2 * border, size - 2 * border, height, Model.ButtonMaterial));
            return model;
        }

        private CuboidModel CreatePedestalModel()
        {
            return new CuboidModel(Model.Size -Model.Border, Model.Height, Model.Size - Model.Border, Model.PedestalMaterial);
        }

        public void Generate()
        {
            GenerateModel();
            Teleporter = gameObject.AddComponent<PlayerTeleporter>();
            Teleporter.Destination = Destination;
            SetupInteraction();
            SetupUIElement();
        }

        private void GenerateModel()
        {
            name = "TeleportButton";

            Pedestal = ModelFactory.CreateCuboid(CreatePedestalModel());
            Pedestal.transform.SetParent(transform, false);
            Pedestal.transform.localPosition = Vector3.zero;
            Pedestal.name = "Pedestal";

            Button = ModelFactory.CreateModel(CreateButtonModel());
            Button.transform.SetParent(transform,false);
            Button.transform.localPosition = new Vector3(Model.Border/2, Model.Height + GetButtonHeight(), Model.Border/2);
            Button.transform.Rotate(Vector3.right, 90);
            Button.name = "Button";
        }

        private void SetupInteraction()
        {
            BoxCollider col = Button.AddComponent<BoxCollider>();
            var size = GetButtonSize();
            var height = GetButtonHeight();
            var border = GetButtonBorder();
            col.size = new Vector3(size, size, border * 2);
            col.center = new Vector3(size / 2f, size / 2f, -border);
            Button.AddComponent<Button>();
            var hand = new CustomEvents.UnityEventHand();
            hand.AddListener(h => { Teleporter.TeleportPlayer(); });
            Button.AddComponent<UIElement>().onHandClick = hand;
        }

        private void SetupUIElement()
        {
            if (string.IsNullOrEmpty(Text) && Image == null)
            {
                return;
            }
            if (!string.IsNullOrEmpty(Text))
            {
                SetupText(Text);
            }else if (Image != null)
            {
                SetupImage(Image);
            }

        }

        private void SetupText(string text)
        {
            var canvas = CreateAndPositionCanvas();
            var to = new GameObject("Text");
            to.transform.SetParent(canvas.transform, false);
            var txt = to.AddComponent<Text>();
            txt.text = text;
            txt.alignment = TextAnchor.MiddleCenter;
            txt.resizeTextForBestFit = true;
            var f = Resources.Load<Font>("Fonts/Glacial-Indifference/GlacialIndifference-Regular") ;
            if (f == null)
            {
                f = Font.CreateDynamicFontFromOSFont("Arial", 14);
            }
            txt.font = f;
            UIElement = to;
        }

        private Canvas CreateAndPositionCanvas()
        {
            var co = new GameObject("Canvas");
            var canvas = co.AddComponent<Canvas>();
            var rt = (RectTransform) canvas.transform;
            //rt.sizeDelta = new Vector2(512,512);
            co.transform.localScale = new Vector3((GetButtonSize()-2*GetButtonBorder())/rt.rect.width, (GetButtonSize()-2*GetButtonBorder())/rt.rect.height);
            co.transform.SetParent(Button.transform, false);
            co.transform.localPosition = new Vector3(GetButtonSize()/2, GetButtonSize()/2, -(GetButtonHeight()+GetButtonBorder()/100f));
            return canvas;
        }


        private void SetupImage(Sprite img)
        {
            var canvas = CreateAndPositionCanvas();
            var io =new GameObject("Image");
            var image = io.AddComponent<Image>();
            image.sprite = img;
            io.transform.SetParent(canvas.transform, false);
            UIElement = io;
        }
    }
}