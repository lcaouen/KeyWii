using System;

namespace KeyWii
{
    class KeyConfig
    {
        public bool bMouse = false;
        public bool bJoystick = false;

        public Key A = new Key(0, Key.eJoyType.eDigital);
        public Key B = new Key(1, Key.eJoyType.eDigital);
        public Key Minus = new Key(2, Key.eJoyType.eDigital);
        public Key Home = new Key(3, Key.eJoyType.eDigital);
        public Key Plus = new Key(4, Key.eJoyType.eDigital);
        public Key One = new Key(5, Key.eJoyType.eDigital);
        public Key Two = new Key(6, Key.eJoyType.eDigital);
        public Key Up = new Key(7, Key.eJoyType.eDigital);
        public Key Down = new Key(8, Key.eJoyType.eDigital);
        public Key Left = new Key(9, Key.eJoyType.eDigital);
        public Key Right = new Key(10, Key.eJoyType.eDigital);
        public Key C = new Key(11, Key.eJoyType.eDigital);
        public Key Z = new Key(12, Key.eJoyType.eDigital);
        public Key NunchukUp = new Key(1, Key.eJoyType.eAnalog);
        public Key NunchukDown = new Key(1, Key.eJoyType.eAnalog);
        public Key NunchukLeft = new Key(0, Key.eJoyType.eAnalog);
        public Key NunchukRight = new Key(0, Key.eJoyType.eAnalog);
    }
}
