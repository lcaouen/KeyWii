using System;

namespace KeyWii
{
    class Key
    {
        public enum eJoyType {
            eUndefined,
            eAnalog,
            eDigital
        };

        public Key(int in_iJoyButton, eJoyType in_eJoyType)
        {
            m_iJoyButton = in_iJoyButton;
            m_eJoyType = in_eJoyType;
        }

        public uint m_iKeyVal = 0;
        public String m_strKeyName = "";
        public bool m_bKeyState = false;
        public int m_iJoyButton = -1;
        public eJoyType m_eJoyType = eJoyType.eUndefined;
    }
}
