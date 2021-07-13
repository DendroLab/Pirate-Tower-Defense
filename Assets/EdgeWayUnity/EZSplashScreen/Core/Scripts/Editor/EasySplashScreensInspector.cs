using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace EdgeWay.Unity.EZSplashScreen
{
    [CustomEditor(typeof(EZSplashScreens))]
    public class EasySplashScreensInspector : Editor
    {
        EZSplashScreens targetScript;

        GUIStyle header;
        GUIStyle subHeader;
 

        SerializedProperty autoPlay, destroyAfterCompletion, enableEsc, backgroundColor, fadeOutBackgroundTime, splashScreens;

        private List<bool> foldoutSplashScreens = new List<bool>();
        private List<bool> foldoutSplashScreensEvents = new List<bool>();

        private void OnEnable()
        {
            targetScript = (EZSplashScreens)target;


            // get properties
            autoPlay = serializedObject.FindProperty("autoPlay");
            destroyAfterCompletion = serializedObject.FindProperty("destroyAfterCompletion");
            enableEsc = serializedObject.FindProperty("enableEsc");
            backgroundColor = serializedObject.FindProperty("backgroundColor");
            fadeOutBackgroundTime = serializedObject.FindProperty("fadeOutBackgroundTime");
            splashScreens = serializedObject.FindProperty("splashScreens");

            foldoutSplashScreens.Clear();
            foldoutSplashScreensEvents.Clear();
            for (int x = 0; x < splashScreens.arraySize; x++)
            {
                foldoutSplashScreens.Add(false);
                foldoutSplashScreensEvents.Add(false);
            }


            SetGUIStyles();

        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.BeginVertical();
            DrawHeader("Splash screen settings");


            EditorGUILayout.PropertyField(autoPlay, new GUIContent("AutoPlay"), true);
            DrawSpace();
            EditorGUILayout.PropertyField(destroyAfterCompletion, new GUIContent("Destroy after completion"), true);
            DrawSpace();
            EditorGUILayout.PropertyField(enableEsc, new GUIContent("Enable Esc"), true);
            DrawSpace();
            EditorGUILayout.PropertyField(backgroundColor, new GUIContent("Background Color"), true);
            DrawSpace();
            EditorGUILayout.PropertyField(fadeOutBackgroundTime, new GUIContent("Fade Out Background Time"), true);

            DrawHeader("Splash Screens");
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Add or remove splash screens", subHeader);

            GUIStyle btn = EditorStyles.miniButton;
            btn.fontSize = 14;
            btn.fontStyle = FontStyle.Bold;
            if (GUILayout.Button("+",btn))
            {
                EZSplashScreens.SplashScreen sp = new EZSplashScreens.SplashScreen();

                splashScreens.arraySize += 1;
                SerializedProperty splash = splashScreens.GetArrayElementAtIndex(splashScreens.arraySize-1);
                splash.FindPropertyRelative("aspectRatio").enumValueIndex = 1;
                splash.FindPropertyRelative("initialDelay").floatValue = 0.5f;
                splash.FindPropertyRelative("fadeInTime").floatValue = 1;
                splash.FindPropertyRelative("displayTime").floatValue = 2;
                splash.FindPropertyRelative("fadeOutTime").floatValue = 1;

                //     foldoutSplashScreens = new bool[splashScreens.arraySize];
                //  for (int x=0;x<splashScreens.arraySize;x++)
                //   {
                //       foldoutSplashScreens[x] = true;
                //   }

                foldoutSplashScreens.Add(true);
                foldoutSplashScreensEvents.Add(false);
            }
            if (GUILayout.Button("-",btn))
            {
                if (targetScript.splashScreens.Count > 0)
                {
                    splashScreens.arraySize += -1;
                    foldoutSplashScreens.RemoveAt(foldoutSplashScreens.Count - 1);
                    foldoutSplashScreensEvents.RemoveAt(foldoutSplashScreensEvents.Count - 1);
                    //foldoutSplashScreens = new bool[splashScreens.arraySize];
                    //for (int x = 0; x < splashScreens.arraySize; x++)
                    //{
                    //    foldoutSplashScreens[x] = true;
                    //}
                    //foldoutSplashScreensEvents = new bool[splashScreens.arraySize];
                }
            }
            EditorGUILayout.EndHorizontal();

            DrawSplashScreenItems();
            EditorGUILayout.EndVertical();
            serializedObject.ApplyModifiedProperties();
        }





        private void DrawSplashScreenItems()
        {

            EditorGUILayout.BeginVertical();
            for (int x = 0; x < splashScreens.arraySize; x++)
            {
                GUIStyle style = EditorStyles.foldout;
                style.fontStyle = FontStyle.Bold;
                style.fontSize = 14;
                foldoutSplashScreens[x] = EditorGUILayout.Foldout(foldoutSplashScreens[x], "Splash Screen " + (x + 1).ToString(), style);
                if (foldoutSplashScreens[x])
                {
                    SerializedProperty splash = splashScreens.GetArrayElementAtIndex(x);
                    EditorGUILayout.PropertyField(splash.FindPropertyRelative("splashImage"), new GUIContent("Splash Image / Logo"), true);

                    EditorGUILayout.PropertyField(splash.FindPropertyRelative("aspectRatio"), new GUIContent("Aspect Ratio"), true);
                    EditorGUILayout.PropertyField(splash.FindPropertyRelative("initialDelay"), new GUIContent("Initial Delay"), true);
                    EditorGUILayout.PropertyField(splash.FindPropertyRelative("fadeInTime"), new GUIContent("Fade In Time"), true);
                    EditorGUILayout.PropertyField(splash.FindPropertyRelative("displayTime"), new GUIContent("Display Time"), true);
                    EditorGUILayout.PropertyField(splash.FindPropertyRelative("fadeOutTime"), new GUIContent("Fade Out Time"), true);

                    GUIStyle style1 = EditorStyles.foldout;
                    style.fontStyle = FontStyle.Bold;
                    style.fontSize = 12;
                    foldoutSplashScreensEvents[x] = EditorGUILayout.Foldout(foldoutSplashScreensEvents[x], "Events", style1);
                    if (foldoutSplashScreensEvents[x])
                    {
                        EditorGUILayout.LabelField("Called after splash has faded in before display time", subHeader);
                        EditorGUILayout.PropertyField(splash.FindPropertyRelative("OnFadedIn"), new GUIContent("OnFadedIn"), true);
                        EditorGUILayout.LabelField("Called after splash has completed", subHeader);
                        EditorGUILayout.PropertyField(splash.FindPropertyRelative("OnComplete"), new GUIContent("OnComplete"), true);
                    }
                }
                DrawSpace();
            }



            EditorGUILayout.EndVertical();

        }


        private void SetGUIStyles()
        {
            header = new GUIStyle();
            header.fontSize = 16;
            header.normal.textColor = Color.white;
            header.font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;

            subHeader = new GUIStyle();
            subHeader.fontSize = 12;
            subHeader.normal.textColor = new Color32(255, 255, 255, 255);
            subHeader.font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;

        }

        private void DrawHeader(string txt)
        {
            DrawSpace();
            EditorGUILayout.LabelField(txt, header);
            DrawLine();
            DrawSpace();
        }
        private void DrawSpace()
        {
            EditorGUILayout.Space();
        }
        private void DrawLine()
        {
            Rect rect = EditorGUILayout.GetControlRect(false, 1);
            rect.height = 1;
            EditorGUI.DrawRect(rect, new Color(0.5f, 0.5f, 0.5f, 1));
        }


    }
}





