using UnityEngine;

namespace Main
{
    public class GSC : GenericSingletonClass<GSC>
    {
        public static PlayerController playerController;
        public static CameraController cameraController;
        public static InputController inputController;
    }
}

