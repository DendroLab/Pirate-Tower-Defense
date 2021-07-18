using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace EdgeWay.Unity.EZSplashScreen
{
    public class EZSplashScreens : MonoBehaviour
    {
        private static EZSplashScreens _instance;
        public static EZSplashScreens Instance { get { return _instance; } }

        private GameObject splashCanvas;
        private GameObject splashBackground;
        private CanvasGroup cgBackground;
        private GameObject splashSprite;
        private CanvasGroup cgSplashSprite;

        [HideInInspector]
        public GameObject _splashPrefab;

        private int currentSplashScreenIndex;
        private int noOfSplashScreens;

        private float initialDelayCTR;
        private float fadeInCTR;
        private float displayCTR;
        private float fadeOutCTR;
        private float finishedSplashScreensCTR;

        public enum AspectRatio
        {
            StretchToFill,
            Center
        }

        [Tooltip("Auto play splash screen sequence")]
        public bool autoPlay = true;

        [Tooltip("Destroy splash screen sequence object after completion")]
        public bool destroyAfterCompletion = true;

        [Tooltip("If enabled will jump right to splash screen sequence completed on ESC key")]
        public bool enableEsc = false;

        [Tooltip("Background color for splash screen sequence")]
        public Color backgroundColor;

        [Tooltip("Upon completion of splash screens fade out background (Seconds)")]
        public float fadeOutBackgroundTime = 0.5f;

     
        public List<SplashScreen> splashScreens;



        public void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(this.gameObject);
            }
            else
            {
                _instance = this;
            }
            DontDestroyOnLoad(this.gameObject);

           GameObject splashPrefab= Instantiate(_splashPrefab, gameObject.transform);

            currentSplashScreenIndex = 0;
            noOfSplashScreens = splashScreens.Count;
            // get canvas
            splashCanvas = splashPrefab.GetComponentInChildren<Canvas>().gameObject;
            splashCanvas.GetComponent<Canvas>().sortingOrder = 32767; // set max sort order to make sure in front

            // get background
            splashBackground = splashCanvas.transform.GetChild(0).gameObject;
            splashBackground.GetComponent<Image>().color = backgroundColor;
            cgBackground = splashBackground.GetComponent<CanvasGroup>();
            cgBackground.alpha = 0;

            // get splash sprite
            splashSprite = splashBackground.transform.GetChild(0).gameObject;
            cgSplashSprite = splashSprite.GetComponent<CanvasGroup>();

            initialDelayCTR = -1;
            fadeInCTR = -1;
            displayCTR = -1;
            fadeOutCTR = -1;
            finishedSplashScreensCTR = -1;

            splashCanvas.SetActive(false);

        }



        // Start is called before the first frame update
        void Start()
        {
     
            if (autoPlay)
            {

                if (Instance.noOfSplashScreens > 0)
                {
                    Instance.splashCanvas.SetActive(true);
                    Instance.cgBackground.alpha = 1;
                    Instance.FireSplashScreen(Instance.currentSplashScreenIndex);
                }
                else
                {
                    Debug.LogError("You have no splash screens defined");
                }

            }
        }

        // Update is called once per frame
        void Update()
        {
            // detect for esc key to skip splash screen sequence
            if (enableEsc)
            {
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    initialDelayCTR = -1;
                    fadeInCTR = -1;
                    displayCTR = -1;
                    fadeOutCTR = -1;
                    finishedSplashScreensCTR = fadeOutBackgroundTime;
                }
            }

            // initial delay
            if (initialDelayCTR!=-1)
            {
                initialDelayCTR += -Time.deltaTime;
                if (initialDelayCTR<0)
                {
                    initialDelayCTR = -1;
                    fadeInCTR = splashScreens[currentSplashScreenIndex].fadeInTime;
                }
            }

            // fade in
            if (fadeInCTR!=-1)
            {
                fadeInCTR += -Time.deltaTime;
                cgSplashSprite.alpha += Time.deltaTime / splashScreens[currentSplashScreenIndex].fadeInTime;
                if (fadeInCTR<0)
                {
                    splashScreens[currentSplashScreenIndex].OnFadedIn?.Invoke();
                    fadeInCTR = -1;
                    displayCTR = splashScreens[currentSplashScreenIndex].displayTime;
                    

                }           
            }

            // display
            if (displayCTR!=-1)
            {
                displayCTR += -Time.deltaTime;
                if (displayCTR<0)
                {
                    displayCTR = -1;
                    fadeOutCTR = splashScreens[currentSplashScreenIndex].fadeOutTime;

                }
            }

            // fade out
            if (fadeOutCTR!=-1)
            {
                fadeOutCTR += -Time.deltaTime;
                cgSplashSprite.alpha +=- Time.deltaTime / splashScreens[currentSplashScreenIndex].fadeOutTime;
                if (fadeOutCTR<0)
                {
                    splashScreens[currentSplashScreenIndex].OnComplete?.Invoke();
                    fadeOutCTR = -1;
                    currentSplashScreenIndex += 1;
                    if (currentSplashScreenIndex>noOfSplashScreens-1)
                    {
                        finishedSplashScreensCTR = fadeOutBackgroundTime;
                    }
                    else
                    {
                        // next splash screen
                        FireSplashScreen(currentSplashScreenIndex);
                    }

                }
            }

            // finshed splash screens fade out background
            if (finishedSplashScreensCTR!=-1)
            {
                finishedSplashScreensCTR += -Time.deltaTime;
                cgBackground.alpha += -Time.deltaTime / fadeOutBackgroundTime;
                if (finishedSplashScreensCTR<0)
                {
                    finishedSplashScreensCTR = -1;
                    FinishedSplashScreens();
                }
            }



      


        }

        public void FireSplashScreen(int n)
        {

            currentSplashScreenIndex = n;
            initialDelayCTR = splashScreens[n].initialDelay;
            splashSprite.GetComponent<Image>().sprite = splashScreens[n].splashImage;
            cgSplashSprite.alpha = 0;

          
            if (splashScreens[n].aspectRatio == SplashScreen.AspectRatio.Center)
            {
                if (splashScreens[n].splashImage != null)
                {
                    if (Screen.height > Screen.width)
                    {
                        float r = (float)splashScreens[n].splashImage.texture.width / (float)splashScreens[n].splashImage.texture.height;
                        splashSprite.GetComponent<RectTransform>().sizeDelta = new Vector2(Screen.width / 2, ((float)Screen.width / 2) / r);
                    }
                    else
                    {
                        float r = (float)splashScreens[n].splashImage.texture.width / (float)splashScreens[n].splashImage.texture.height;
                        splashSprite.GetComponent<RectTransform>().sizeDelta = new Vector2((Screen.height / 2) * r, Screen.height / 2);
                    }
                }
            }
            if (splashScreens[n].aspectRatio == SplashScreen.AspectRatio.StretchToFill)
            {
                splashSprite.GetComponent<RectTransform>().sizeDelta = new Vector2(Screen.width, Screen.height);
            }

        }


        // Finished splash screens
        private void FinishedSplashScreens()
        {
            if (destroyAfterCompletion)
            {
                Destroy(gameObject, 0.1f);
            }
            else
            {
                splashCanvas.SetActive(false);
                autoPlay = false;
                currentSplashScreenIndex = 0;
            }
        }


        // ====== Static Methods ======
        public static void StartSplashScreens()
        {
            if (Instance.autoPlay)
            {
                Debug.LogError("Auto Play is checked, uncheck to start from script");
            }
            else
            {
                if (Instance.noOfSplashScreens > 0)
                {
                    Instance.splashCanvas.SetActive(true);
                    Instance.cgBackground.alpha = 1;
                    Instance.FireSplashScreen(Instance.currentSplashScreenIndex);
                }
                else
                {
                    Debug.LogError("You have no splash screens defined");
                }
            }
        }


        #region classes
        [System.Serializable]
        public class SplashScreen
        {

           [Tooltip("Drag your image/logo as a sprite here")]
            public Sprite splashImage = null;
            public enum AspectRatio
            {
                StretchToFill,
                Center
            }

            public AspectRatio aspectRatio = AspectRatio.Center;
        
            [Tooltip("Intial delay before fade in (Seconds)")]
            public float initialDelay = 0.5f;
            [Tooltip("Fade in time (Seconds)")]
            public float fadeInTime = 1;
            [Tooltip("Time splash remains on screen befor fade out (Seconds)")]
            public float displayTime = 2;
            [Tooltip("Fade out time (Seconds)")]
            public float fadeOutTime = 1;


            public UnityEngine.Events.UnityEvent OnFadedIn;
            public UnityEngine.Events.UnityEvent OnComplete;
        }

      
        #endregion
    }
}
