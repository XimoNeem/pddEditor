
using PDDEditor.Assets;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace PDDEditor
{
    namespace SceneManagment
    {
        public static class PDDEditorScenes
        {
            public const string Menu = "MainMenu";
            public const string Main = "Main";
        }

        public class PDDScene
        {
            public string Name;
            public string Path;
            public string PreviewURL;
        }

        [System.Serializable]
        public class PDDSceneData
        {
            public string Name;
            public List<PDDNodeData> ItemsList;
        }
    }

    namespace UI
    {
        public static class PDDEditorWindows
        {
            public const string MainMenu = "Assets/Prefabs/UI/MainMenuCanvas.prefab";
            public const string MainTopBar = "Assets/Prefabs/UI/TopBar.prefab";
            public const string MainButtomBar = "Assets/Prefabs/UI/ButtomBar.prefab";
            public const string ItemsList = "Assets/Prefabs/UI/ItemsList.prefab";
            public const string ObjectSettings = "Assets/Prefabs/UI/ObjectSettings.prefab";
            public const string ScreenShot = "Assets/Prefabs/UI/ScreenShotWindow.prefab";
            public const string FilePicker = "Assets/Prefabs/UI/FilePicker.prefab";
            public const string ColorPicker = "ColorPicker";
            public const string ImportPreview = "ImportPreview";
            public const string SceneSettings = "SceneSettings";
            public const string EditorSettings = "EditorSettings";
            public const string DebugLayer = "DebugLayer";
            public const string PopupWindow = "PopupWindow";
            public const string AssetsManager = "AssetsManager";
        }

        public class PopupElement
        {
            
        }

        public class PopupHeader
        {
            public string Text;

            public PopupHeader(string text)
            {
                Text = text ?? throw new ArgumentNullException(nameof(text));
            }
        }

        public class PopupButton : PopupElement
        {
            public string Name;
            public Color Color;
            public Action ClickAction;

            public PopupButton(string name, Color color, Action clickAction)
            {
                Name = name ?? throw new ArgumentNullException(nameof(name));
                Color = color;
                ClickAction = clickAction ?? throw new ArgumentNullException(nameof(clickAction));
            }
        }

        public class PopupInput : PopupElement
        {
            public string PlaceHolderText;
            public Action<string> Action;

            public PopupInput(string placeHolderText, Action<string> action)
            {
                PlaceHolderText = placeHolderText ?? throw new ArgumentNullException(nameof(placeHolderText));
                Action = action ?? throw new ArgumentNullException(nameof(action));
            }
        }
    }

    namespace Types
    {
        public static class PDDObjectTypes
        {
            public static string[] GetTypes()
            {
                return Context.Instance.EditorBase.EditorTypes.GetTypes();
            }
        }
        public enum ObjectType
        {
            StaticModel,
            SplineModel,
            RoadSigns,
            RoadMarkings,
            TrafficLightObject,
            Vehicle,
            AnimatedModel,
            Vegetation
        }

        public enum SettingType
        {
            Toggle,
            Texture
        }

        public enum VehicleType
        {
            Car,

        }

        public enum WheelType
        {
            FrontRight,
            FrontLeft,
            BackRight,
            BackLeft
        }
        public enum VehiclelightType
        {
            Headlight,
            RunningLight,
            ParkingLight,
            TurnSignal
        }

        public enum LightPosition
        {
            Left,
            Right
        }
    }

    namespace Paths
    {
        public static class PDDEditorPaths
        {
            public const string EditorSettings = "Editor/editor.json";
            public const string EditorTypes = "Editor/types.json";
            public const string Node = "Assets/Prefabs/Node.prefab";
            public const string PreviewImageName = "scene.png";
            public const string SceneDataName = "data.json";
            public const string AssetsPath = "/PDDAssets";
            public const string ScenesPath = "/PDDScenes";
        }
    }

    namespace Assets
    {
        public class PDDAssetData
        {
            public string Path; // Путь к ассету
            public string ImagePath; // Путь к превью ассета
            public string Name; // Имя ассета

            public PDDAssetData(string path, string imagePath, string name)
            {
                Path = path;
                ImagePath = imagePath;
                Name = name;
            }
        }

        [System.Serializable]
        public class PDDNodeData
        {
            public Vector3 Position;
            public Vector3 Rotation;
            public Vector3 Scale;
            public string AssetPath;

            public ToggleSetting[] ToggleSettings;
        }
    }
}
