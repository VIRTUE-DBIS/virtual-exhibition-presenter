using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Unibas.DBIS.VREP.UI
{
    public class FadeController : MonoBehaviour
    {


        private Camera _camera;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            var go = GameObject.FindWithTag("MainCamera");
            if (go.GetComponent<Camera>() != null)
            {
                _camera = go.GetComponent<Camera>();
                AttachCanvas();
            }
        }

        public static FadeController Instance;

        private Image img;

        public float Speed = 1.5f;

        private void AttachCanvas()
        {
            var co = new GameObject("FadeCanvas");
            var canvas = co.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceCamera;
            canvas.worldCamera = _camera;
            canvas.planeDistance = 1f;
            co.transform.SetParent(_camera.transform);
            var io = new GameObject("FadeImage");
            io.transform.SetParent(co.transform);
            img = io.AddComponent<Image>();
            img.color = new Color(0,0,0,0);// invisble black
            img.rectTransform.anchorMin = Vector2.zero;
            img.rectTransform.anchorMax = Vector2.one;
            img.rectTransform.anchoredPosition3D = Vector3.zero;
            img.rectTransform.sizeDelta = img.canvas.pixelRect.size;
            Debug.Log("Setup FadeController");
        }

        private bool fading = false;

        private float alpha = 0f;

        private bool fadeIn = true;
        
        private void Update()
        {
            if (fading)
            {
                Debug.Log("Fadining: in:"+fadeIn+", alph="+alpha);
                if (fadeIn)
                {
                    if (alpha < 1)
                    {
                        alpha += Time.deltaTime * Speed;
                        if (alpha > 1)
                        {
                            alpha = 1f;
                            fading = false;
                        }

                        img.color = new Color(0, 0, 0, alpha);
                    }
                }

                if (!fadeIn)
                {
                    if (alpha > 0)
                    {
                        alpha -= Time.deltaTime * Speed;
                        if (alpha < 0)
                        {
                            alpha = 0f;
                            fading = false;
                        }
                        img.color = new Color(0,0,0,alpha);
                    }
                }
            }
        }

        public void FadeInOut(float waitTime = 1f)
        {
            FadeToBlack();
            StartCoroutine(fadeTime(waitTime));

        }

        private IEnumerator fadeTime(float waitTime)
        {
            yield return new WaitForSecondsRealtime(waitTime);
            Debug.Log("Timer out");
            FadeToTransparent();
        }


        public void FadeToBlack()
        {
            fading = true;
            fadeIn = true;
        }


        public void FadeToTransparent()
        {
            fading = true;
            fadeIn = false;
        }
        
    }
}