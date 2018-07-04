using System;

namespace CDX.GLFWBackend
{
    internal class Input : IInput, IDisposable
    {
        private readonly Window          window;
        private          InputProcessor  inputProcessor;
        private readonly InputEventQueue eventQueue = new InputEventQueue();

        private int    mouseX, mouseY;
        private int    mousePressed;
        private int    deltaX, deltaY;
        private bool   _justTouched;
        private int    pressedKeys;
        private bool   keyJustPressed;
        private bool[] justPressedKeys = new bool[256];
        private char   lastCharacter;


        public Input(Window window)
        {
            this.window = window;
            windowHandleChanged(window.getWindowHandle());
        }

        internal void resetPollingStates()
        {
            _justTouched   = false;
            keyJustPressed = false;
            for (int i = 0; i < justPressedKeys.Length; i++)
            {
                justPressedKeys[i] = false;
            }

            eventQueue.setProcessor(null);
            eventQueue.drain();
        }

        public void windowHandleChanged(global::GLFW.GLFW.Window windowHandle)
        {
            resetPollingStates();
            GLFW.GLFW.SetKeyCallback(window.getWindowHandle(), keyCallback);
            GLFW.GLFW.SetCharCallback(window.getWindowHandle(), charCallback);
            GLFW.GLFW.SetScrollCallback(window.getWindowHandle(), scrollCallback);
            GLFW.GLFW.SetCursorPosCallback(window.getWindowHandle(), cursorPosCallback);
            GLFW.GLFW.SetMouseButtonCallback(window.getWindowHandle(), mouseButtonCallback);
        }

        private void keyCallback(global::GLFW.GLFW.Window window1, global::GLFW.GLFW.KeyCode key, int scancode, global::GLFW.GLFW.InputState state, global::GLFW.GLFW.KeyMods mods)
        {
            switch (state)
            {
                case GLFW.GLFW.InputState.Press:
                    var gdxkey = getGdxKeyCode(key);
                    eventQueue.keyDown(gdxkey);
                    pressedKeys++;
                    keyJustPressed                = true;
                    justPressedKeys[(int) gdxkey] = true;
                    window.getGraphics().requestRendering();
                    lastCharacter = (char) 0;
                    char character = characterForKeyCode(gdxkey);
                    if (character != 0) charCallback(window1, (uint) character);
                    break;
                case GLFW.GLFW.InputState.Release:
                    pressedKeys--;
                    window.getGraphics().requestRendering();
                    eventQueue.keyUp(getGdxKeyCode(key));
                    break;
                case GLFW.GLFW.InputState.Repeat:
                    pressedKeys--;
                    window.getGraphics().requestRendering();
                    eventQueue.keyUp(getGdxKeyCode(key));
                    break;
            }
        }

        private void charCallback(global::GLFW.GLFW.Window window1, uint codepoint)
        {
            if ((codepoint & 0xff00) == 0xf700) return;
            lastCharacter = (char) codepoint;
            window.getGraphics().requestRendering();
            eventQueue.keyTyped((char) codepoint);
        }

        private void scrollCallback(global::GLFW.GLFW.Window window1, double xpos, double ypos)
        {
            window.getGraphics().requestRendering();
            eventQueue.scrolled((int) -Math.Sign(ypos));
        }

        private int logicalMouseY;
        private int logicalMouseX;

        private void cursorPosCallback(global::GLFW.GLFW.Window window1, double xpos, double ypos)
        {
            deltaX = (int) xpos - logicalMouseX;
            deltaY = (int) ypos - logicalMouseY;
            mouseX = logicalMouseX = (int) xpos;
            mouseY = logicalMouseY = (int) ypos;

            if (window.getConfig().hdpiMode == HdpiMode.Pixels)
            {
                float xScale = window.getGraphics().getBackBufferWidth() / (float) window.getGraphics().getLogicalWidth();
                float yScale = window.getGraphics().getBackBufferHeight() / (float) window.getGraphics().getLogicalHeight();
                deltaX = (int) (deltaX * xScale);
                deltaY = (int) (deltaY * yScale);
                mouseX = (int) (mouseX * xScale);
                mouseY = (int) (mouseY * yScale);
            }

            window.getGraphics().requestRendering();
            if (mousePressed > 0)
            {
                eventQueue.touchDragged(mouseX, mouseY, 0);
            }
            else
            {
                eventQueue.mouseMoved(mouseX, mouseY);
            }
        }

        private void mouseButtonCallback(global::GLFW.GLFW.Window window1, global::GLFW.GLFW.MouseButton button, global::GLFW.GLFW.InputState state, global::GLFW.GLFW.KeyMods mods)
        {
            var gdxButton = toGdxButton(button);
            if (gdxButton == Buttons.UKNOWN) return;

            if (state == GLFW.GLFW.InputState.Press)
            {
                mousePressed++;
                _justTouched = true;
                window.getGraphics().requestRendering();
                eventQueue.touchDown(mouseX, mouseY, 0, gdxButton);
            }
            else
            {
                mousePressed = Math.Max(0, mousePressed - 1);
                window.getGraphics().requestRendering();
                eventQueue.touchUp(mouseX, mouseY, 0, gdxButton);
            }
        }

        internal void update()
        {
            eventQueue.setProcessor(inputProcessor);
            eventQueue.drain();
        }

        internal void prepareNext()
        {
            _justTouched = false;

            if (keyJustPressed)
            {
                keyJustPressed = false;
                for (int i = 0; i < justPressedKeys.Length; i++)
                {
                    justPressedKeys[i] = false;
                }
            }

            deltaX = 0;
            deltaY = 0;
        }


        public int getX()
        {
            return mouseX;
        }


        public int getX(int pointer)
        {
            return pointer == 0 ? mouseX : 0;
        }


        public int getDeltaX()
        {
            return deltaX;
        }


        public int getDeltaX(int pointer)
        {
            return pointer == 0 ? deltaX : 0;
        }


        public int getY()
        {
            return mouseY;
        }


        public int getY(int pointer)
        {
            return pointer == 0 ? mouseY : 0;
        }


        public int getDeltaY()
        {
            return deltaY;
        }


        public int getDeltaY(int pointer)
        {
            return pointer == 0 ? deltaY : 0;
        }


        public bool isTouched()
        {
            return GLFW.GLFW.GetMouseButton(window.getWindowHandle(), GLFW.GLFW.MouseButton.Button1) ||
                   GLFW.GLFW.GetMouseButton(window.getWindowHandle(), GLFW.GLFW.MouseButton.Button2) ||
                   GLFW.GLFW.GetMouseButton(window.getWindowHandle(), GLFW.GLFW.MouseButton.Button3) ||
                   GLFW.GLFW.GetMouseButton(window.getWindowHandle(), GLFW.GLFW.MouseButton.Button4) ||
                   GLFW.GLFW.GetMouseButton(window.getWindowHandle(), GLFW.GLFW.MouseButton.Button5);
        }


        public bool justTouched()
        {
            return _justTouched;
        }


        public bool isTouched(int pointer)
        {
            return pointer == 0 ? isTouched() : false;
        }

        public bool isButtonPressed(Buttons button)
        {
            return GLFW.GLFW.GetMouseButton(window.getWindowHandle(), (global::GLFW.GLFW.MouseButton) button);
        }

        public bool isKeyPressed(Keys key)
        {
            if (key == Keys.ANY_KEY) return pressedKeys > 0;
            if (key == Keys.SYM)
            {
                return GLFW.GLFW.GetKey(window.getWindowHandle(), (int) GLFW.GLFW.KeyCode.LeftSuper) ||
                       GLFW.GLFW.GetKey(window.getWindowHandle(), (int) GLFW.GLFW.KeyCode.RightSuper);
            }

            return GLFW.GLFW.GetKey(window.getWindowHandle(), getGlfwKeyCode(key));
        }


        public bool isKeyJustPressed(Keys key)
        {
            if (key == Keys.ANY_KEY)
            {
                return keyJustPressed;
            }

            if (key < 0 || (int) key > 256)
            {
                return false;
            }

            return justPressedKeys[(int) key];
        }

        public long getCurrentEventTime()
        {
            // queue sets its event time for each event dequeued/processed
            return eventQueue.getCurrentEventTime();
        }


        public void setInputProcessor(InputProcessor processor)
        {
            inputProcessor = processor;
        }


        public InputProcessor getInputProcessor()
        {
            return inputProcessor;
        }


        public void setCursorCatched(bool catched)
        {
            GLFW.GLFW.SetInputMode(window.getWindowHandle(), GLFW.GLFW.InputMode.Cursor, catched ? GLFW.GLFW.CursorMode.Disabled : GLFW.GLFW.CursorMode.Normal);
        }


        public bool isCursorCatched()
        {
            return GLFW.GLFW.GetInputMode(window.getWindowHandle(), GLFW.GLFW.InputMode.Cursor) == (int) GLFW.GLFW.CursorMode.Disabled;
        }


        public void setCursorPosition(int x, int y)
        {
            if (window.getConfig().hdpiMode == HdpiMode.Pixels)
            {
                float xScale = window.getGraphics().getLogicalWidth() / (float) window.getGraphics().getBackBufferWidth();
                float yScale = window.getGraphics().getLogicalHeight() / (float) window.getGraphics().getBackBufferHeight();
                x = (int) (x * xScale);
                y = (int) (y * yScale);
            }

            GLFW.GLFW.SetCursorPos(window.getWindowHandle(), x, y);
        }

        static char characterForKeyCode(Keys key)
        {
            // Map certain key codes to character codes.
            switch (key)
            {
                case Keys.BACKSPACE:
                    return (char) 8;
                case Keys.TAB:
                    return '\t';
                case Keys.FORWARD_DEL:
                    return (char) 127;
                case Keys.ENTER:
                    return '\n';
            }

            return (char) 0;
        }

        public static Keys getGdxKeyCode(global::GLFW.GLFW.KeyCode lwjglKeyCode)
        {
            switch (lwjglKeyCode)
            {
                case GLFW.GLFW.KeyCode.Space:
                    return Keys.SPACE;
                case GLFW.GLFW.KeyCode.Apostrophe:
                    return Keys.APOSTROPHE;
                case GLFW.GLFW.KeyCode.Comma:
                    return Keys.COMMA;
                case GLFW.GLFW.KeyCode.Minus:
                    return Keys.MINUS;
                case GLFW.GLFW.KeyCode.Period:
                    return Keys.PERIOD;
                case GLFW.GLFW.KeyCode.Slash:
                    return Keys.SLASH;
                case GLFW.GLFW.KeyCode.Alpha0:
                    return Keys.NUM_0;
                case GLFW.GLFW.KeyCode.Alpha1:
                    return Keys.NUM_1;
                case GLFW.GLFW.KeyCode.Alpha2:
                    return Keys.NUM_2;
                case GLFW.GLFW.KeyCode.Alpha3:
                    return Keys.NUM_3;
                case GLFW.GLFW.KeyCode.Alpha4:
                    return Keys.NUM_4;
                case GLFW.GLFW.KeyCode.Alpha5:
                    return Keys.NUM_5;
                case GLFW.GLFW.KeyCode.Alpha6:
                    return Keys.NUM_6;
                case GLFW.GLFW.KeyCode.Alpha7:
                    return Keys.NUM_7;
                case GLFW.GLFW.KeyCode.Alpha8:
                    return Keys.NUM_8;
                case GLFW.GLFW.KeyCode.Alpha9:
                    return Keys.NUM_9;
                case GLFW.GLFW.KeyCode.SemiColon:
                    return Keys.SEMICOLON;
                case GLFW.GLFW.KeyCode.Equal:
                    return Keys.EQUALS;
                case GLFW.GLFW.KeyCode.A:
                    return Keys.A;
                case GLFW.GLFW.KeyCode.B:
                    return Keys.B;
                case GLFW.GLFW.KeyCode.C:
                    return Keys.C;
                case GLFW.GLFW.KeyCode.D:
                    return Keys.D;
                case GLFW.GLFW.KeyCode.E:
                    return Keys.E;
                case GLFW.GLFW.KeyCode.F:
                    return Keys.F;
                case GLFW.GLFW.KeyCode.G:
                    return Keys.G;
                case GLFW.GLFW.KeyCode.H:
                    return Keys.H;
                case GLFW.GLFW.KeyCode.I:
                    return Keys.I;
                case GLFW.GLFW.KeyCode.J:
                    return Keys.J;
                case GLFW.GLFW.KeyCode.K:
                    return Keys.K;
                case GLFW.GLFW.KeyCode.L:
                    return Keys.L;
                case GLFW.GLFW.KeyCode.M:
                    return Keys.M;
                case GLFW.GLFW.KeyCode.N:
                    return Keys.N;
                case GLFW.GLFW.KeyCode.O:
                    return Keys.O;
                case GLFW.GLFW.KeyCode.P:
                    return Keys.P;
                case GLFW.GLFW.KeyCode.Q:
                    return Keys.Q;
                case GLFW.GLFW.KeyCode.R:
                    return Keys.R;
                case GLFW.GLFW.KeyCode.S:
                    return Keys.S;
                case GLFW.GLFW.KeyCode.T:
                    return Keys.T;
                case GLFW.GLFW.KeyCode.U:
                    return Keys.U;
                case GLFW.GLFW.KeyCode.V:
                    return Keys.V;
                case GLFW.GLFW.KeyCode.W:
                    return Keys.W;
                case GLFW.GLFW.KeyCode.X:
                    return Keys.X;
                case GLFW.GLFW.KeyCode.Y:
                    return Keys.Y;
                case GLFW.GLFW.KeyCode.Z:
                    return Keys.Z;
                case GLFW.GLFW.KeyCode.LeftBracket:
                    return Keys.LEFT_BRACKET;
                case GLFW.GLFW.KeyCode.Backslash:
                    return Keys.BACKSLASH;
                case GLFW.GLFW.KeyCode.RightBracket:
                    return Keys.RIGHT_BRACKET;
                case GLFW.GLFW.KeyCode.GraveAccent:
                    return Keys.GRAVE;
                case GLFW.GLFW.KeyCode.World1:
                case GLFW.GLFW.KeyCode.World2:
                    return Keys.UNKNOWN;
                case GLFW.GLFW.KeyCode.Escape:
                    return Keys.ESCAPE;
                case GLFW.GLFW.KeyCode.Enter:
                    return Keys.ENTER;
                case GLFW.GLFW.KeyCode.Tab:
                    return Keys.TAB;
                case GLFW.GLFW.KeyCode.Backspace:
                    return Keys.BACKSPACE;
                case GLFW.GLFW.KeyCode.Insert:
                    return Keys.INSERT;
                case GLFW.GLFW.KeyCode.Delete:
                    return Keys.FORWARD_DEL;
                case GLFW.GLFW.KeyCode.Right:
                    return Keys.RIGHT;
                case GLFW.GLFW.KeyCode.Left:
                    return Keys.LEFT;
                case GLFW.GLFW.KeyCode.Down:
                    return Keys.DOWN;
                case GLFW.GLFW.KeyCode.Up:
                    return Keys.UP;
                case GLFW.GLFW.KeyCode.PageUp:
                    return Keys.PAGE_UP;
                case GLFW.GLFW.KeyCode.PageDown:
                    return Keys.PAGE_DOWN;
                case GLFW.GLFW.KeyCode.Home:
                    return Keys.HOME;
                case GLFW.GLFW.KeyCode.End:
                    return Keys.END;
                case GLFW.GLFW.KeyCode.CapsLock:
                case GLFW.GLFW.KeyCode.ScrollLock:
                case GLFW.GLFW.KeyCode.NumLock:
                case GLFW.GLFW.KeyCode.PrintScreen:
                case GLFW.GLFW.KeyCode.Pause:
                    return Keys.UNKNOWN;
                case GLFW.GLFW.KeyCode.F1:
                    return Keys.F1;
                case GLFW.GLFW.KeyCode.F2:
                    return Keys.F2;
                case GLFW.GLFW.KeyCode.F3:
                    return Keys.F3;
                case GLFW.GLFW.KeyCode.F4:
                    return Keys.F4;
                case GLFW.GLFW.KeyCode.F5:
                    return Keys.F5;
                case GLFW.GLFW.KeyCode.F6:
                    return Keys.F6;
                case GLFW.GLFW.KeyCode.F7:
                    return Keys.F7;
                case GLFW.GLFW.KeyCode.F8:
                    return Keys.F8;
                case GLFW.GLFW.KeyCode.F9:
                    return Keys.F9;
                case GLFW.GLFW.KeyCode.F10:
                    return Keys.F10;
                case GLFW.GLFW.KeyCode.F11:
                    return Keys.F11;
                case GLFW.GLFW.KeyCode.F12:
                    return Keys.F12;
                case GLFW.GLFW.KeyCode.F13:
                case GLFW.GLFW.KeyCode.F14:
                case GLFW.GLFW.KeyCode.F15:
                case GLFW.GLFW.KeyCode.F16:
                case GLFW.GLFW.KeyCode.F17:
                case GLFW.GLFW.KeyCode.F18:
                case GLFW.GLFW.KeyCode.F19:
                case GLFW.GLFW.KeyCode.F20:
                case GLFW.GLFW.KeyCode.F21:
                case GLFW.GLFW.KeyCode.F22:
                case GLFW.GLFW.KeyCode.F23:
                case GLFW.GLFW.KeyCode.F24:
                case GLFW.GLFW.KeyCode.F25:
                    return Keys.UNKNOWN;
                case GLFW.GLFW.KeyCode.Numpad0:
                    return Keys.NUMPAD_0;
                case GLFW.GLFW.KeyCode.Numpad1:
                    return Keys.NUMPAD_1;
                case GLFW.GLFW.KeyCode.Numpad2:
                    return Keys.NUMPAD_2;
                case GLFW.GLFW.KeyCode.Numpad3:
                    return Keys.NUMPAD_3;
                case GLFW.GLFW.KeyCode.Numpad4:
                    return Keys.NUMPAD_4;
                case GLFW.GLFW.KeyCode.Numpad5:
                    return Keys.NUMPAD_5;
                case GLFW.GLFW.KeyCode.Numpad6:
                    return Keys.NUMPAD_6;
                case GLFW.GLFW.KeyCode.Numpad7:
                    return Keys.NUMPAD_7;
                case GLFW.GLFW.KeyCode.Numpad8:
                    return Keys.NUMPAD_8;
                case GLFW.GLFW.KeyCode.Numpad9:
                    return Keys.NUMPAD_9;
                case GLFW.GLFW.KeyCode.NumpadDecimal:
                    return Keys.PERIOD;
                case GLFW.GLFW.KeyCode.NumpadDivide:
                    return Keys.SLASH;
                case GLFW.GLFW.KeyCode.NumpadMultiply:
                    return Keys.STAR;
                case GLFW.GLFW.KeyCode.NumpadSubtract:
                    return Keys.MINUS;
                case GLFW.GLFW.KeyCode.NumpadAdd:
                    return Keys.PLUS;
                case GLFW.GLFW.KeyCode.NumpadEnter:
                    return Keys.ENTER;
                case GLFW.GLFW.KeyCode.NumpadEqual:
                    return Keys.EQUALS;
                case GLFW.GLFW.KeyCode.LeftShift:
                    return Keys.SHIFT_LEFT;
                case GLFW.GLFW.KeyCode.LeftControl:
                    return Keys.CONTROL_LEFT;
                case GLFW.GLFW.KeyCode.LeftAlt:
                    return Keys.ALT_LEFT;
                case GLFW.GLFW.KeyCode.LeftSuper:
                    return Keys.SYM;
                case GLFW.GLFW.KeyCode.RightShift:
                    return Keys.SHIFT_RIGHT;
                case GLFW.GLFW.KeyCode.RightControl:
                    return Keys.CONTROL_RIGHT;
                case GLFW.GLFW.KeyCode.RightAlt:
                    return Keys.ALT_RIGHT;
                case GLFW.GLFW.KeyCode.RightSuper:
                    return Keys.SYM;
                case GLFW.GLFW.KeyCode.Menu:
                    return Keys.MENU;
                default:
                    return Keys.UNKNOWN;
            }
        }

        public static int getGlfwKeyCode(Keys gdxKeyCode)
        {
            switch (gdxKeyCode)
            {
                case Keys.SPACE:
                    return (int) GLFW.GLFW.KeyCode.Space;
                case Keys.APOSTROPHE:
                    return (int) GLFW.GLFW.KeyCode.Apostrophe;
                case Keys.COMMA:
                    return (int) GLFW.GLFW.KeyCode.Comma;
                case Keys.PERIOD:
                    return (int) GLFW.GLFW.KeyCode.Period;
                case Keys.NUM_0:
                    return (int) GLFW.GLFW.KeyCode.Alpha0;
                case Keys.NUM_1:
                    return (int) GLFW.GLFW.KeyCode.Alpha1;
                case Keys.NUM_2:
                    return (int) GLFW.GLFW.KeyCode.Alpha2;
                case Keys.NUM_3:
                    return (int) GLFW.GLFW.KeyCode.Alpha3;
                case Keys.NUM_4:
                    return (int) GLFW.GLFW.KeyCode.Alpha4;
                case Keys.NUM_5:
                    return (int) GLFW.GLFW.KeyCode.Alpha5;
                case Keys.NUM_6:
                    return (int) GLFW.GLFW.KeyCode.Alpha6;
                case Keys.NUM_7:
                    return (int) GLFW.GLFW.KeyCode.Alpha7;
                case Keys.NUM_8:
                    return (int) GLFW.GLFW.KeyCode.Alpha8;
                case Keys.NUM_9:
                    return (int) GLFW.GLFW.KeyCode.Alpha9;
                case Keys.SEMICOLON:
                    return (int) GLFW.GLFW.KeyCode.SemiColon;
                case Keys.EQUALS:
                    return (int) GLFW.GLFW.KeyCode.Equal;
                case Keys.A:
                    return (int) GLFW.GLFW.KeyCode.A;
                case Keys.B:
                    return (int) GLFW.GLFW.KeyCode.B;
                case Keys.C:
                    return (int) GLFW.GLFW.KeyCode.C;
                case Keys.D:
                    return (int) GLFW.GLFW.KeyCode.D;
                case Keys.E:
                    return (int) GLFW.GLFW.KeyCode.E;
                case Keys.F:
                    return (int) GLFW.GLFW.KeyCode.F;
                case Keys.G:
                    return (int) GLFW.GLFW.KeyCode.G;
                case Keys.H:
                    return (int) GLFW.GLFW.KeyCode.H;
                case Keys.I:
                    return (int) GLFW.GLFW.KeyCode.I;
                case Keys.J:
                    return (int) GLFW.GLFW.KeyCode.J;
                case Keys.K:
                    return (int) GLFW.GLFW.KeyCode.K;
                case Keys.L:
                    return (int) GLFW.GLFW.KeyCode.L;
                case Keys.M:
                    return (int) GLFW.GLFW.KeyCode.M;
                case Keys.N:
                    return (int) GLFW.GLFW.KeyCode.N;
                case Keys.O:
                    return (int) GLFW.GLFW.KeyCode.O;
                case Keys.P:
                    return (int) GLFW.GLFW.KeyCode.P;
                case Keys.Q:
                    return (int) GLFW.GLFW.KeyCode.Q;
                case Keys.R:
                    return (int) GLFW.GLFW.KeyCode.R;
                case Keys.S:
                    return (int) GLFW.GLFW.KeyCode.S;
                case Keys.T:
                    return (int) GLFW.GLFW.KeyCode.T;
                case Keys.U:
                    return (int) GLFW.GLFW.KeyCode.U;
                case Keys.V:
                    return (int) GLFW.GLFW.KeyCode.V;
                case Keys.W:
                    return (int) GLFW.GLFW.KeyCode.W;
                case Keys.X:
                    return (int) GLFW.GLFW.KeyCode.X;
                case Keys.Y:
                    return (int) GLFW.GLFW.KeyCode.Y;
                case Keys.Z:
                    return (int) GLFW.GLFW.KeyCode.Z;
                case Keys.LEFT_BRACKET:
                    return (int) GLFW.GLFW.KeyCode.LeftBracket;
                case Keys.BACKSLASH:
                    return (int) GLFW.GLFW.KeyCode.Backslash;
                case Keys.RIGHT_BRACKET:
                    return (int) GLFW.GLFW.KeyCode.RightBracket;
                case Keys.GRAVE:
                    return (int) GLFW.GLFW.KeyCode.GraveAccent;
                case Keys.ESCAPE:
                    return (int) GLFW.GLFW.KeyCode.Escape;
                case Keys.ENTER:
                    return (int) GLFW.GLFW.KeyCode.Enter;
                case Keys.TAB:
                    return (int) GLFW.GLFW.KeyCode.Tab;
                case Keys.BACKSPACE:
                    return (int) GLFW.GLFW.KeyCode.Backspace;
                case Keys.INSERT:
                    return (int) GLFW.GLFW.KeyCode.Insert;
                case Keys.FORWARD_DEL:
                    return (int) GLFW.GLFW.KeyCode.Delete;
                case Keys.RIGHT:
                    return (int) GLFW.GLFW.KeyCode.Right;
                case Keys.LEFT:
                    return (int) GLFW.GLFW.KeyCode.Left;
                case Keys.DOWN:
                    return (int) GLFW.GLFW.KeyCode.Down;
                case Keys.UP:
                    return (int) GLFW.GLFW.KeyCode.Up;
                case Keys.PAGE_UP:
                    return (int) GLFW.GLFW.KeyCode.PageUp;
                case Keys.PAGE_DOWN:
                    return (int) GLFW.GLFW.KeyCode.PageDown;
                case Keys.HOME:
                    return (int) GLFW.GLFW.KeyCode.Home;
                case Keys.END:
                    return (int) GLFW.GLFW.KeyCode.End;
                case Keys.F1:
                    return (int) GLFW.GLFW.KeyCode.F1;
                case Keys.F2:
                    return (int) GLFW.GLFW.KeyCode.F2;
                case Keys.F3:
                    return (int) GLFW.GLFW.KeyCode.F3;
                case Keys.F4:
                    return (int) GLFW.GLFW.KeyCode.F4;
                case Keys.F5:
                    return (int) GLFW.GLFW.KeyCode.F5;
                case Keys.F6:
                    return (int) GLFW.GLFW.KeyCode.F6;
                case Keys.F7:
                    return (int) GLFW.GLFW.KeyCode.F7;
                case Keys.F8:
                    return (int) GLFW.GLFW.KeyCode.F8;
                case Keys.F9:
                    return (int) GLFW.GLFW.KeyCode.F9;
                case Keys.F10:
                    return (int) GLFW.GLFW.KeyCode.F10;
                case Keys.F11:
                    return (int) GLFW.GLFW.KeyCode.F11;
                case Keys.F12:
                    return (int) GLFW.GLFW.KeyCode.F12;
                case Keys.NUMPAD_0:
                    return (int) GLFW.GLFW.KeyCode.Numpad0;
                case Keys.NUMPAD_1:
                    return (int) GLFW.GLFW.KeyCode.Numpad1;
                case Keys.NUMPAD_2:
                    return (int) GLFW.GLFW.KeyCode.Numpad2;
                case Keys.NUMPAD_3:
                    return (int) GLFW.GLFW.KeyCode.Numpad3;
                case Keys.NUMPAD_4:
                    return (int) GLFW.GLFW.KeyCode.Numpad4;
                case Keys.NUMPAD_5:
                    return (int) GLFW.GLFW.KeyCode.Numpad5;
                case Keys.NUMPAD_6:
                    return (int) GLFW.GLFW.KeyCode.Numpad6;
                case Keys.NUMPAD_7:
                    return (int) GLFW.GLFW.KeyCode.Numpad7;
                case Keys.NUMPAD_8:
                    return (int) GLFW.GLFW.KeyCode.Numpad8;
                case Keys.NUMPAD_9:
                    return (int) GLFW.GLFW.KeyCode.Numpad9;
                case Keys.SLASH:
                    return (int) GLFW.GLFW.KeyCode.NumpadDivide;
                case Keys.STAR:
                    return (int) GLFW.GLFW.KeyCode.NumpadMultiply;
                case Keys.MINUS:
                    return (int) GLFW.GLFW.KeyCode.NumpadSubtract;
                case Keys.PLUS:
                    return (int) GLFW.GLFW.KeyCode.NumpadAdd;
                case Keys.SHIFT_LEFT:
                    return (int) GLFW.GLFW.KeyCode.LeftShift;
                case Keys.CONTROL_LEFT:
                    return (int) GLFW.GLFW.KeyCode.LeftControl;
                case Keys.ALT_LEFT:
                    return (int) GLFW.GLFW.KeyCode.LeftAlt;
                case Keys.SYM:
                    return (int) GLFW.GLFW.KeyCode.LeftSuper;
                case Keys.SHIFT_RIGHT:
                    return (int) GLFW.GLFW.KeyCode.RightShift;
                case Keys.CONTROL_RIGHT:
                    return (int) GLFW.GLFW.KeyCode.RightControl;
                case Keys.ALT_RIGHT:
                    return (int) GLFW.GLFW.KeyCode.RightAlt;
                case Keys.MENU:
                    return (int) GLFW.GLFW.KeyCode.Menu;
                default:
                    return 0;
            }
        }

        private static Buttons toGdxButton(global::GLFW.GLFW.MouseButton button)
        {
            if (button == GLFW.GLFW.MouseButton.ButtonLeft) return Buttons.LEFT;
            if (button == GLFW.GLFW.MouseButton.ButtonRight) return Buttons.RIGHT;
            if (button == GLFW.GLFW.MouseButton.ButtonMiddle) return Buttons.MIDDLE;
            //if (button == GLFW.GLFW.MouseButton.Ba) return Buttons.BACK;
            //if (button == 4) return Buttons.FORWARD;
            return Buttons.UKNOWN;
        }


        public void Dispose()
        {
        }

        // --------------------------------------------------------------------------
        // -------------------------- Nothing to see below this line except for stubs
        // --------------------------------------------------------------------------

        public void setCatchBackKey(bool catchBack)
        {
        }


        public bool isCatchBackKey()
        {
            return false;
        }


        public void setCatchMenuKey(bool catchMenu)
        {
        }


        public bool isCatchMenuKey()
        {
            return false;
        }


        public float getAccelerometerX()
        {
            return 0;
        }


        public float getAccelerometerY()
        {
            return 0;
        }


        public float getAccelerometerZ()
        {
            return 0;
        }


        public bool isPeripheralAvailable(Peripheral peripheral)
        {
            return peripheral == Peripheral.HardwareKeyboard;
        }


        public int getRotation()
        {
            return 0;
        }


        public Orientation getNativeOrientation()
        {
            return Orientation.Landscape;
        }


        public void setOnscreenKeyboardVisible(bool visible)
        {
        }


        public void vibrate(int milliseconds)
        {
        }


        public void vibrate(long[] pattern, int repeat)
        {
        }


        public void cancelVibrate()
        {
        }


        public float getAzimuth()
        {
            return 0;
        }


        public float getPitch()
        {
            return 0;
        }


        public float getRoll()
        {
            return 0;
        }


        public void getRotationMatrix(float[] matrix)
        {
        }


        public float getGyroscopeX()
        {
            return 0;
        }


        public float getGyroscopeY()
        {
            return 0;
        }


        public float getGyroscopeZ()
        {
            return 0;
        }
    }
}