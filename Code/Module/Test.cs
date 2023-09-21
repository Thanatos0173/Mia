using System;
using System.Runtime.InteropServices;

class Program
{
    static void Main()
    {
        KeySender.SendVirtualKeyPress(KeySender.VK_U);

        Console.WriteLine("Virtual key press sent.");
        Console.ReadLine();
    }
}

class KeySender
{
    [DllImport("user32.dll", SetLastError = true)]
    public static extern uint SendInput(uint nInputs, INPUT[] pInputs, int cbSize);

    public const ushort VK_U = 0x55;

    public static void SendVirtualKeyPress(ushort keyCode)
    {
        INPUT[] inputs = new INPUT[1];

        inputs[0].type = 1;
        inputs[0].u.ki.wVk = keyCode;
        inputs[0].u.ki.dwFlags = 0;

        SendInput((uint)inputs.Length, inputs, Marshal.SizeOf(typeof(INPUT)));
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct INPUTUNION
    {
        [FieldOffset(0)]
        public KEYBDINPUT ki;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct INPUT
    {
        public int type;
        public INPUTUNION u;
    }

    public struct KEYBDINPUT
    {
        public ushort wVk;
        public ushort wScan;
        public uint dwFlags;
        public uint time;
        public IntPtr dwExtraInfo;
    }
}
