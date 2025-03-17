using UnityEngine;

namespace Main
{
    public class GSC : GenericSingletonClass<GSC>
    {
        public static MainController main;
        public static PlayerController playerController;
        public static CameraController cameraController;
        public static InputController inputController;
        public static UIController uiController;
    }
}

