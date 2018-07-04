using System;
using System.Collections.Generic;
using CDX.Utils;

namespace CDX
{
    public class InputEventQueue : InputProcessor
    {
        private const int KEY_DOWN      = 0;
        private const int KEY_UP        = 1;
        private const int KEY_TYPED     = 2;
        private const int TOUCH_DOWN    = 3;
        private const int TOUCH_UP      = 4;
        private const int TOUCH_DRAGGED = 5;
        private const int MOUSE_MOVED   = 6;
        private const int SCROLLED      = 7;

        private          InputProcessor processor;
        private readonly List<int>      queue           = new List<int>();
        private readonly List<int>      processingQueue = new List<int>();
        private          long           currentEventTime;

        public InputEventQueue()
        {
        }

        public InputEventQueue(InputProcessor processor)
        {
            this.processor = processor;
        }

        public void setProcessor(InputProcessor processor)
        {
            this.processor = processor;
        }

        public InputProcessor getProcessor()
        {
            return processor;
        }

        public void drain()
        {
            var q = processingQueue;
            lock (this)
            {
                if (processor == null)
                {
                    queue.Clear();
                    return;
                }

                q.AddRange(queue);
                queue.Clear();
            }

            InputProcessor localProcessor = processor;
            for (int i = 0, n = q.Count; i < n;)
            {
                currentEventTime = (long) q[i++] << 32 | q[i++] & 0xFFFFFFFFL;
                switch (q[i++])
                {
                    case KEY_DOWN:
                        localProcessor.keyDown((Keys) q[i++]);
                        break;
                    case KEY_UP:
                        localProcessor.keyUp((Keys) q[i++]);
                        break;
                    case KEY_TYPED:
                        localProcessor.keyTyped((char) q[i++]);
                        break;
                    case TOUCH_DOWN:
                        localProcessor.touchDown(q[i++], q[i++], q[i++], (Buttons) q[i++]);
                        break;
                    case TOUCH_UP:
                        localProcessor.touchUp(q[i++], q[i++], q[i++], (Buttons) q[i++]);
                        break;
                    case TOUCH_DRAGGED:
                        localProcessor.touchDragged(q[i++], q[i++], q[i++]);
                        break;
                    case MOUSE_MOVED:
                        localProcessor.mouseMoved(q[i++], q[i++]);
                        break;
                    case SCROLLED:
                        localProcessor.scrolled(q[i++]);
                        break;
                }
            }

            q.Clear();
        }

        private void queueTime()
        {
            long time = TimeUtils.nanoTime();
            queue.Add((int) (time >> 32));
            queue.Add((int) time);
        }

        public bool keyDown(Keys keycode)
        {
            queueTime();
            queue.Add(KEY_DOWN);
            queue.Add((int) keycode);
            return false;
        }

        public bool keyUp(Keys keycode)
        {
            queueTime();
            queue.Add(KEY_UP);
            queue.Add((int) keycode);
            return false;
        }

        public bool keyTyped(char character)
        {
            queueTime();
            queue.Add(KEY_TYPED);
            queue.Add(character);
            return false;
        }

        public bool touchDown(int screenX, int screenY, int pointer, Buttons button)
        {
            queueTime();
            queue.Add(TOUCH_DOWN);
            queue.Add(screenX);
            queue.Add(screenY);
            queue.Add(pointer);
            queue.Add((int) button);
            return false;
        }

        public bool touchUp(int screenX, int screenY, int pointer, Buttons button)
        {
            queueTime();
            queue.Add(TOUCH_UP);
            queue.Add(screenX);
            queue.Add(screenY);
            queue.Add(pointer);
            queue.Add((int) button);
            return false;
        }

        public bool touchDragged(int screenX, int screenY, int pointer)
        {
            queueTime();
            queue.Add(TOUCH_DRAGGED);
            queue.Add(screenX);
            queue.Add(screenY);
            queue.Add(pointer);
            return false;
        }

        public bool mouseMoved(int screenX, int screenY)
        {
            queueTime();
            queue.Add(MOUSE_MOVED);
            queue.Add(screenX);
            queue.Add(screenY);
            return false;
        }

        public bool scrolled(int amount)
        {
            queueTime();
            queue.Add(SCROLLED);
            queue.Add(amount);
            return false;
        }

        public long getCurrentEventTime()
        {
            return currentEventTime;
        }
    }
}