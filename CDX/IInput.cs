using System;
using System.Collections.Generic;

namespace CDX
{
    public enum Buttons
    {
        UKNOWN  = -1,
        LEFT    = 0,
        RIGHT   = 1,
        MIDDLE  = 2,
        BACK    = 3,
        FORWARD = 4
    }

    public enum Keys
    {
       ANY_KEY             = -1,
       NUM_0               = 7,
       NUM_1               = 8,
       NUM_2               = 9,
       NUM_3               = 10,
       NUM_4               = 11,
       NUM_5               = 12,
       NUM_6               = 13,
       NUM_7               = 14,
       NUM_8               = 15,
       NUM_9               = 16,
       A                   = 29,
       ALT_LEFT            = 57,
       ALT_RIGHT           = 58,
       APOSTROPHE          = 75,
       AT                  = 77,
       B                   = 30,
       BACK                = 4,
       BACKSLASH           = 73,
       C                   = 31,
       CALL                = 5,
       CAMERA              = 27,
       CLEAR               = 28,
       COMMA               = 55,
       D                   = 32,
       DEL                 = 67,
       BACKSPACE           = 67,
       FORWARD_DEL         = 112,
       DPAD_CENTER         = 23,
       DPAD_DOWN           = 20,
       DPAD_LEFT           = 21,
       DPAD_RIGHT          = 22,
       DPAD_UP             = 19,
       CENTER              = 23,
       DOWN                = 20,
       LEFT                = 21,
       RIGHT               = 22,
       UP                  = 19,
       E                   = 33,
       ENDCALL             = 6,
       ENTER               = 66,
       ENVELOPE            = 65,
       EQUALS              = 70,
       EXPLORER            = 64,
       F                   = 34,
       FOCUS               = 80,
       G                   = 35,
       GRAVE               = 68,
       H                   = 36,
       HEADSETHOOK         = 79,
       HOME                = 3,
       I                   = 37,
       J                   = 38,
       K                   = 39,
       L                   = 40,
       LEFT_BRACKET        = 71,
       M                   = 41,
       MEDIA_FAST_FORWARD  = 90,
       MEDIA_NEXT          = 87,
       MEDIA_PLAY_PAUSE    = 85,
       MEDIA_PREVIOUS      = 88,
       MEDIA_REWIND        = 89,
       MEDIA_STOP          = 86,
       MENU                = 82,
       MINUS               = 69,
       MUTE                = 91,
       N                   = 42,
       NOTIFICATION        = 83,
       NUM                 = 78,
       O                   = 43,
       P                   = 44,
       PERIOD              = 56,
       PLUS                = 81,
       POUND               = 18,
       POWER               = 26,
       Q                   = 45,
       R                   = 46,
       RIGHT_BRACKET       = 72,
       S                   = 47,
       SEARCH              = 84,
       SEMICOLON           = 74,
       SHIFT_LEFT          = 59,
       SHIFT_RIGHT         = 60,
       SLASH               = 76,
       SOFT_LEFT           = 1,
       SOFT_RIGHT          = 2,
       SPACE               = 62,
       STAR                = 17,
       SYM                 = 63,
       T                   = 48,
       TAB                 = 61,
       U                   = 49,
       UNKNOWN             = 0,
       V                   = 50,
       VOLUME_DOWN         = 25,
       VOLUME_UP           = 24,
       W                   = 51,
       X                   = 52,
       Y                   = 53,
       Z                   = 54,
       META_ALT_LEFT_ON    = 16,
       META_ALT_ON         = 2,
       META_ALT_RIGHT_ON   = 32,
       META_SHIFT_LEFT_ON  = 64,
       META_SHIFT_ON       = 1,
       META_SHIFT_RIGHT_ON = 128,
       META_SYM_ON         = 4,
       CONTROL_LEFT        = 129,
       CONTROL_RIGHT       = 130,
       ESCAPE              = 131,
       END                 = 132,
       INSERT              = 133,
       PAGE_UP             = 92,
       PAGE_DOWN           = 93,
       PICTSYMBOLS         = 94,
       SWITCH_CHARSET      = 95,
       BUTTON_CIRCLE       = 255,
       BUTTON_A            = 96,
       BUTTON_B            = 97,
       BUTTON_C            = 98,
       BUTTON_X            = 99,
       BUTTON_Y            = 100,
       BUTTON_Z            = 101,
       BUTTON_L1           = 102,
       BUTTON_R1           = 103,
       BUTTON_L2           = 104,
       BUTTON_R2           = 105,
       BUTTON_THUMBL       = 106,
       BUTTON_THUMBR       = 107,
       BUTTON_START        = 108,
       BUTTON_SELECT       = 109,
       BUTTON_MODE         = 110,

        NUMPAD_0 = 144,
        NUMPAD_1 = 145,
        NUMPAD_2 = 146,
        NUMPAD_3 = 147,
        NUMPAD_4 = 148,
        NUMPAD_5 = 149,
        NUMPAD_6 = 150,
        NUMPAD_7 = 151,
        NUMPAD_8 = 152,
        NUMPAD_9 = 153,

        // public const int BACKTICK = 0;
        // public const int TILDE = 0;
        // public const int UNDERSCORE = 0;
        // public const int DOT = 0;
        // public const int BREAK = 0;
        // public const int PIPE = 0;
        // public const int EXCLAMATION = 0;
        // public const int QUESTIONMARK = 0;
        
        // ` | VK_BACKTICK
        // ~ | VK_TILDE
        // : | VK_COLON
        // _ | VK_UNDERSCORE
        // . | VK_DOT
        // (break) | VK_BREAK
        // | | VK_PIPE
        // ! | VK_EXCLAMATION
        // ? | VK_QUESTION
        COLON = 243,
        F1    = 244,
        F2    = 245,
        F3    = 246,
        F4    = 247,
        F5    = 248,
        F6    = 249,
        F7    = 250,
        F8    = 251,
        F9    = 252,
        F10   = 253,
        F11   = 254,
        F12   = 255,
    }

    public class KeysUtils
    {
        public static string toString(Keys keycode)
        {
            if (keycode < 0) throw new Exception("keycode cannot be negative, keycode: " + keycode);
            if ((int)keycode > 255) throw new Exception("keycode cannot be greater than 255, keycode: " + keycode);
            switch (keycode)
            {
                // META* variables should not be used with this method.
                case Keys.UNKNOWN:
                    return "Unknown";
                case Keys.SOFT_LEFT:
                    return "Soft Left";
                case Keys.SOFT_RIGHT:
                    return "Soft Right";
                case Keys.HOME:
                    return "Home";
                case Keys.BACK:
                    return "Back";
                case Keys.CALL:
                    return "Call";
                case Keys.ENDCALL:
                    return "End Call";
                case Keys.NUM_0:
                    return "0";
                case Keys.NUM_1:
                    return "1";
                case Keys.NUM_2:
                    return "2";
                case Keys.NUM_3:
                    return "3";
                case Keys.NUM_4:
                    return "4";
                case Keys.NUM_5:
                    return "5";
                case Keys.NUM_6:
                    return "6";
                case Keys.NUM_7:
                    return "7";
                case Keys.NUM_8:
                    return "8";
                case Keys.NUM_9:
                    return "9";
                case Keys.STAR:
                    return "*";
                case Keys.POUND:
                    return "#";
                case Keys.UP:
                    return "Up";
                case Keys.DOWN:
                    return "Down";
                case Keys.LEFT:
                    return "Left";
                case Keys.RIGHT:
                    return "Right";
                case Keys.CENTER:
                    return "Center";
                case Keys.VOLUME_UP:
                    return "Volume Up";
                case Keys.VOLUME_DOWN:
                    return "Volume Down";
                case Keys.POWER:
                    return "Power";
                case Keys.CAMERA:
                    return "Camera";
                case Keys.CLEAR:
                    return "Clear";
                case Keys.A:
                    return "A";
                case Keys.B:
                    return "B";
                case Keys.C:
                    return "C";
                case Keys.D:
                    return "D";
                case Keys.E:
                    return "E";
                case Keys.F:
                    return "F";
                case Keys.G:
                    return "G";
                case Keys.H:
                    return "H";
                case Keys.I:
                    return "I";
                case Keys.J:
                    return "J";
                case Keys.K:
                    return "K";
                case Keys.L:
                    return "L";
                case Keys.M:
                    return "M";
                case Keys.N:
                    return "N";
                case Keys.O:
                    return "O";
                case Keys.P:
                    return "P";
                case Keys.Q:
                    return "Q";
                case Keys.R:
                    return "R";
                case Keys.S:
                    return "S";
                case Keys.T:
                    return "T";
                case Keys.U:
                    return "U";
                case Keys.V:
                    return "V";
                case Keys.W:
                    return "W";
                case Keys.X:
                    return "X";
                case Keys.Y:
                    return "Y";
                case Keys.Z:
                    return "Z";
                case Keys.COMMA:
                    return ",";
                case Keys.PERIOD:
                    return ".";
                case Keys.ALT_LEFT:
                    return "L-Alt";
                case Keys.ALT_RIGHT:
                    return "R-Alt";
                case Keys.SHIFT_LEFT:
                    return "L-Shift";
                case Keys.SHIFT_RIGHT:
                    return "R-Shift";
                case Keys.TAB:
                    return "Tab";
                case Keys.SPACE:
                    return "Space";
                case Keys.SYM:
                    return "SYM";
                case Keys.EXPLORER:
                    return "Explorer";
                case Keys.ENVELOPE:
                    return "Envelope";
                case Keys.ENTER:
                    return "Enter";
                case Keys.DEL:
                    return "Delete"; // also BACKSPACE
                case Keys.GRAVE:
                    return "`";
                case Keys.MINUS:
                    return "-";
                case Keys.EQUALS:
                    return "=";
                case Keys.LEFT_BRACKET:
                    return "[";
                case Keys.RIGHT_BRACKET:
                    return "]";
                case Keys.BACKSLASH:
                    return "\\";
                case Keys.SEMICOLON:
                    return ";";
                case Keys.APOSTROPHE:
                    return "'";
                case Keys.SLASH:
                    return "/";
                case Keys.AT:
                    return "@";
                case Keys.NUM:
                    return "Num";
                case Keys.HEADSETHOOK:
                    return "Headset Hook";
                case Keys.FOCUS:
                    return "Focus";
                case Keys.PLUS:
                    return "Plus";
                case Keys.MENU:
                    return "Menu";
                case Keys.NOTIFICATION:
                    return "Notification";
                case Keys.SEARCH:
                    return "Search";
                case Keys.MEDIA_PLAY_PAUSE:
                    return "Play/Pause";
                case Keys.MEDIA_STOP:
                    return "Stop Media";
                case Keys.MEDIA_NEXT:
                    return "Next Media";
                case Keys.MEDIA_PREVIOUS:
                    return "Prev Media";
                case Keys.MEDIA_REWIND:
                    return "Rewind";
                case Keys.MEDIA_FAST_FORWARD:
                    return "Fast Forward";
                case Keys.MUTE:
                    return "Mute";
                case Keys.PAGE_UP:
                    return "Page Up";
                case Keys.PAGE_DOWN:
                    return "Page Down";
                case Keys.PICTSYMBOLS:
                    return "PICTSYMBOLS";
                case Keys.SWITCH_CHARSET:
                    return "SWITCH_CHARSET";
                case Keys.BUTTON_A:
                    return "A Button";
                case Keys.BUTTON_B:
                    return "B Button";
                case Keys.BUTTON_C:
                    return "C Button";
                case Keys.BUTTON_X:
                    return "X Button";
                case Keys.BUTTON_Y:
                    return "Y Button";
                case Keys.BUTTON_Z:
                    return "Z Button";
                case Keys.BUTTON_L1:
                    return "L1 Button";
                case Keys.BUTTON_R1:
                    return "R1 Button";
                case Keys.BUTTON_L2:
                    return "L2 Button";
                case Keys.BUTTON_R2:
                    return "R2 Button";
                case Keys.BUTTON_THUMBL:
                    return "Left Thumb";
                case Keys.BUTTON_THUMBR:
                    return "Right Thumb";
                case Keys.BUTTON_START:
                    return "Start";
                case Keys.BUTTON_SELECT:
                    return "Select";
                case Keys.BUTTON_MODE:
                    return "Button Mode";
                case Keys.FORWARD_DEL:
                    return "Forward Delete";
                case Keys.CONTROL_LEFT:
                    return "L-Ctrl";
                case Keys.CONTROL_RIGHT:
                    return "R-Ctrl";
                case Keys.ESCAPE:
                    return "Escape";
                case Keys.END:
                    return "End";
                case Keys.INSERT:
                    return "Insert";
                case Keys.NUMPAD_0:
                    return "Numpad 0";
                case Keys.NUMPAD_1:
                    return "Numpad 1";
                case Keys.NUMPAD_2:
                    return "Numpad 2";
                case Keys.NUMPAD_3:
                    return "Numpad 3";
                case Keys.NUMPAD_4:
                    return "Numpad 4";
                case Keys.NUMPAD_5:
                    return "Numpad 5";
                case Keys.NUMPAD_6:
                    return "Numpad 6";
                case Keys.NUMPAD_7:
                    return "Numpad 7";
                case Keys.NUMPAD_8:
                    return "Numpad 8";
                case Keys.NUMPAD_9:
                    return "Numpad 9";
                case Keys.COLON:
                    return ":";
                case Keys.F1:
                    return "F1";
                case Keys.F2:
                    return "F2";
                case Keys.F3:
                    return "F3";
                case Keys.F4:
                    return "F4";
                case Keys.F5:
                    return "F5";
                case Keys.F6:
                    return "F6";
                case Keys.F7:
                    return "F7";
                case Keys.F8:
                    return "F8";
                case Keys.F9:
                    return "F9";
                case Keys.F10:
                    return "F10";
                case Keys.F11:
                    return "F11";
                case Keys.F12:
                    return "F12";
                // BUTTON_CIRCLE unhandled, as it conflicts with the more likely to be pressed F12
                default:
                    // key name not found
                    return null;
            }
        }

        private static Dictionary<string, Keys> keyNames;

        public static Keys valueOf(string keyname)
        {
            if (keyNames == null) initializeKeyNames();
            if (keyNames.TryGetValue(keyname, out var i)) return  i;
            return Keys.ANY_KEY;
        }

        private static void initializeKeyNames()
        {
            keyNames = new Dictionary<string, Keys>();
            for (int i = 0; i < 256; i++)
            {
                var name = toString((Keys) i);
                if (name != null) keyNames.Add(name, (Keys) i);
            }
        }
    }

    public enum Peripheral
    {
        HardwareKeyboard,
        OnscreenKeyboard,
        MultitouchScreen,
        Accelerometer,
        Compass,
        Vibrator,
        Gyroscope,
        RotationVector
    }

    public enum Orientation
    {
        Landscape,
        Portrait
    }

    public interface IInput
    {
        float getAccelerometerX();

        float getAccelerometerY();

        float getAccelerometerZ();

        float getGyroscopeX();

        float getGyroscopeY();

        float getGyroscopeZ();

        int getX();

        int getX(int pointer);

        int getDeltaX();

        int getDeltaX(int pointer);

        int getY();

        int getY(int pointer);

        int getDeltaY();

        int getDeltaY(int pointer);

        bool isTouched();

        bool justTouched();

        bool isTouched(int pointer);

        bool isButtonPressed(Buttons button);

        bool isKeyPressed(Keys key);

        bool isKeyJustPressed(Keys key);

        //void getTextInput(TextInputListener listener, String title, String text, String hint);

        void setOnscreenKeyboardVisible(bool visible);

        void vibrate(int milliseconds);

        void vibrate(long[] pattern, int repeat);

        void cancelVibrate();

        float getAzimuth();

        float getPitch();

        float getRoll();

        void getRotationMatrix(float[] matrix);

        long getCurrentEventTime();

        void setCatchBackKey(bool catchBack);

        bool isCatchBackKey();

        void setCatchMenuKey(bool catchMenu);

        bool isCatchMenuKey();

        void setInputProcessor(InputProcessor processor);

        InputProcessor getInputProcessor();

        bool isPeripheralAvailable(Peripheral peripheral);

        int getRotation();

        Orientation getNativeOrientation();

        void setCursorCatched(bool catched);

        bool isCursorCatched();

        void setCursorPosition(int x, int y);
    }
}