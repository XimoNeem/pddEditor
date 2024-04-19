
namespace PDDEditor
{
    namespace SceneManagment 
    {
        public static class PDDEditorScenes
        {
            public const string Menu = "MainMenu";
            public const string Main = "Main";
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
        }
    }

    namespace Items
    {
        public static class PDDItems
        {
            public const string Node = "Assets/Prefabs/Node.prefab";
        }
    }

    namespace Types
    {
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
}
