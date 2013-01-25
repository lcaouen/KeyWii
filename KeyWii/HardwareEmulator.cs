using System;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Windows.Forms;
using System.IO;
using KeyWii;

namespace Hardware
{
    class HardwareEmulator
    {
        ////////////////
        // Constantes //
        ////////////////

        const int INPUT_MOUSE = 0;
        const int INPUT_KEYBOARD = 1;
        const int INPUT_HARDWARE = 2;

        const uint KEYEVENTF_EXTENDEDKEY = 0x0001;
        const uint KEYEVENTF_KEYUP = 0x0002;
        const uint KEYEVENTF_UNICODE = 0x0004;
        const uint KEYEVENTF_SCANCODE = 0x0008;
        
        const uint XBUTTON1 = 0x0001;
        const uint XBUTTON2 = 0x0002;
        
        const uint MOUSEEVENTF_MOVE = 0x0001;
        const uint MOUSEEVENTF_LEFTDOWN = 0x0002;
        const uint MOUSEEVENTF_LEFTUP = 0x0004;
        const uint MOUSEEVENTF_RIGHTDOWN = 0x0008;
        const uint MOUSEEVENTF_RIGHTUP = 0x0010;
        const uint MOUSEEVENTF_MIDDLEDOWN = 0x0020;
        const uint MOUSEEVENTF_MIDDLEUP = 0x0040;
        const uint MOUSEEVENTF_XDOWN = 0x0080;
        const uint MOUSEEVENTF_XUP = 0x0100;
        const uint MOUSEEVENTF_WHEEL = 0x0800;
        const uint MOUSEEVENTF_VIRTUALDESK = 0x4000;
        const uint MOUSEEVENTF_ABSOLUTE = 0x8000;

        const uint JOYSTICK_STATE_V1 = 0x53544143;
        const int PPJOY_AXIS_MIN = 1;
        const int PPJOY_AXIS_MAX = 32767;

        ////////////////
        // Structures //
        ////////////////

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        struct JOYSTICK_STATE
        {
            public uint     Signature;	/* Signature to identify packet to PPJoy IOCTL */
            public byte     NumAnalog;	/* Num of analog values we pass */
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public int[]    Analog;		/* Analog values */
            public byte     NumDigital;	/* Num of digital values we pass */
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
            public byte[]   Digital;	/* Digital values */
        }

        [StructLayout(LayoutKind.Explicit)]
        struct INPUT
        {
            [FieldOffset(0)]
            public int type;
            [FieldOffset(4)]
            public MOUSEINPUT mi;
            [FieldOffset(4)]
            public KEYBDINPUT ki;
            [FieldOffset(4)]
            public HARDWAREINPUT hi;
        }
        
        [StructLayout(LayoutKind.Sequential)]
        struct MOUSEINPUT
        {
            public int dx;
            public int dy;
            public uint mouseData;
            public uint dwFlags;
            public uint time;
            public IntPtr dwExtraInfo;
        }
        
        [StructLayout(LayoutKind.Sequential)]
        struct KEYBDINPUT
        {
            public ushort wVk;
            public ushort wScan;
            public uint dwFlags;
            public uint time;
            public IntPtr dwExtraInfo;
        }
        
        [StructLayout(LayoutKind.Sequential)]
        struct HARDWAREINPUT
        {
            public uint uMsg;
            public ushort wParamL;
            public ushort wParamH;
        }

        
        /////////////
        // PInvoke //
        /////////////

        [DllImport("Kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern IntPtr CreateFile(
           string fileName,
           [MarshalAs(UnmanagedType.U4)] FileAccess fileAccess,
           [MarshalAs(UnmanagedType.U4)] FileShare fileShare,
           IntPtr securityAttributes,
           [MarshalAs(UnmanagedType.U4)] FileMode creationDisposition,
           int flags,
           IntPtr template);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool CloseHandle(IntPtr hObject);

        [DllImport("kernel32.dll", ExactSpelling = true, SetLastError = true, CharSet = CharSet.Auto)]
        static extern bool DeviceIoControl(IntPtr hDevice, uint dwIoControlCode,
        IntPtr lpInBuffer, uint nInBufferSize,
        IntPtr lpOutBuffer, uint nOutBufferSize,
        out uint lpBytesReturned, IntPtr lpOverlapped);
        
        [DllImport("user32.dll", SetLastError = true)]
        static extern uint SendInput(uint nInputs, INPUT[] pInputs, int cbSize);

        [DllImport("user32.dll")]
        static extern uint MapVirtualKey(uint uCode, uint uMapType);

        [DllImport("user32.dll")]
        static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, int dwExtraInfo);


        /////////////
        // Membres //
        /////////////

        IntPtr m_JoyHandle = IntPtr.Zero;
        JOYSTICK_STATE m_JoyState = new JOYSTICK_STATE();


        //////////////
        // Methodes //
        //////////////

        public HardwareEmulator(int nMotes)
        {
            string strDeviceName = "\\\\.\\PPJoyIOCTL" + nMotes.ToString();

            /* Open a handle to the control device for the first virtual joystick. */
            /* Virtual joystick devices are names PPJoyIOCTL1 to PPJoyIOCTL16. */
            m_JoyHandle = CreateFile(strDeviceName, FileAccess.Write, FileShare.Write, IntPtr.Zero, FileMode.Open, 0, IntPtr.Zero);

            if (m_JoyHandle.ToInt32() == -1)
            {
                m_JoyHandle = IntPtr.Zero;
                Debug.WriteLine("No Joystick");
            }

            m_JoyState.Signature = JOYSTICK_STATE_V1;
            m_JoyState.NumAnalog = 8;     /* Number of analog values */
            m_JoyState.NumDigital = 16;	/* Number of digital values */
            m_JoyState.Analog = new int[8];	/* Analog values */
            m_JoyState.Digital = new byte[16];	/* Digital values */
        }

        ~HardwareEmulator()
        {
            if (m_JoyHandle != IntPtr.Zero) CloseHandle(m_JoyHandle);
        }

        public void MoveMouse(int in_dx, int in_dy)
        {
            MOUSEINPUT m = new MOUSEINPUT();
            m.dx = in_dx;
            m.dy = in_dy;
            m.dwFlags = MOUSEEVENTF_MOVE;

            INPUT input = new INPUT();
            input.type = INPUT_MOUSE;
            input.mi = m;

            INPUT[] lstInput = new INPUT[] { input };
            int iSize = Marshal.SizeOf(input);

            uint result = SendInput(1, lstInput, iSize);
            if (result != 1) throw new Exception("Could not move the mouse.");
        }

        public void SendKey(uint in_wScanCode, bool in_bPress)
        {
            KEYBDINPUT k = new KEYBDINPUT();
            k.wVk = 0;
            k.dwFlags = KEYEVENTF_SCANCODE;
            k.time = 0;
            k.dwExtraInfo = IntPtr.Zero;

            if ((in_wScanCode & 0xFF00) == 0xE000)
            { // extended key?
                k.dwFlags |= KEYEVENTF_EXTENDEDKEY;
            }

            if (in_bPress)
            { // press?
                k.wScan = (ushort)(in_wScanCode & 0xFF);
            }
            else
            { // release?
                k.wScan = (ushort) in_wScanCode;
                k.dwFlags |= KEYEVENTF_KEYUP;
            }

            INPUT input = new INPUT();
            input.type = INPUT_KEYBOARD;
            input.ki = k;

            INPUT[] lstInput = new INPUT[] { input };
            int iSize = Marshal.SizeOf(input);

            uint result = SendInput(1, lstInput, iSize);
            if (result != 1) throw new Exception("Could not send key: " + in_wScanCode);
        }

        public uint KeyCodeToScanCode(Keys in_KeyCode)
        {
            uint iScanCode = MapVirtualKey((uint)in_KeyCode, 0);
            switch (in_KeyCode)
            {
                case Keys.Up: iScanCode = 0xE048; break;
                case Keys.Insert:
                case Keys.Delete:
                case Keys.Home:
                case Keys.End:
                case Keys.Next:  // Page down
                case Keys.Prior: // Page up
                case Keys.Left:
                case Keys.Right:
                case Keys.Down:
                    iScanCode |= 0xE000; // Add extended bit
                    break;
            }
            return iScanCode;
        }

        public void ResetJoy()
        {
            for (int iAnalog = 0; iAnalog < 8; iAnalog++)
            {
                m_JoyState.Analog[iAnalog] = (PPJOY_AXIS_MIN + PPJOY_AXIS_MAX) / 2;
            }

            for (int iDigital = 0; iDigital < 8; iDigital++)
            {
                m_JoyState.Digital[iDigital] = (byte) 0;
            }
        }

        public void SetJoyState(Key in_Key, float in_dValue)
        {
            int[] Analog = m_JoyState.Analog;

            if (in_Key.m_eJoyType == Key.eJoyType.eAnalog)
            {
                Analog[in_Key.m_iJoyButton] = (int)((PPJOY_AXIS_MAX - PPJOY_AXIS_MIN) * (in_dValue + 0.5));
            }
        }

        public void SetJoyState(Key in_Key, bool in_bState)
        {
            byte[] Digital = m_JoyState.Digital;

            if (in_Key.m_eJoyType == Key.eJoyType.eDigital)
            {
                if (in_bState) Digital[in_Key.m_iJoyButton] = 1;
                else Digital[in_Key.m_iJoyButton] = 0;
            }
        }

        public void SendJoy()
        {
            if (m_JoyHandle == IntPtr.Zero) return;

            uint iRet = 0;
            uint val = 2228224;
            
            IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf(m_JoyState));
            try
            {
                Marshal.StructureToPtr(m_JoyState, ptr, false);
                if (!DeviceIoControl(m_JoyHandle, val, ptr, (uint)Marshal.SizeOf(m_JoyState), IntPtr.Zero, 0, out iRet, IntPtr.Zero))
                {
                    throw new Exception("Error SendJoy");
                }
            }
            finally
            {
                // Free the unmanaged memory.
                Marshal.FreeHGlobal(ptr);
            }
        }
    }
}
