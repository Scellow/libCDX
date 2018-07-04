namespace CDX
{
    public class InputAdapter : InputProcessor
    {
        public virtual bool keyDown(Keys keycode)
        {
            return false;
        }

        public virtual bool keyUp(Keys keycode)
        {
            return false;
        }

        public virtual bool keyTyped(char character)
        {
            return false;
        }

        public virtual bool touchDown(int screenX, int screenY, int pointer, Buttons button)
        {
            return false;
        }

        public virtual bool touchUp(int screenX, int screenY, int pointer, Buttons button)
        {
            return false;
        }

        public virtual bool touchDragged(int screenX, int screenY, int pointer)
        {
            return false;
        }

        public virtual bool mouseMoved(int screenX, int screenY)
        {
            return false;
        }

        public virtual bool scrolled(int amount)
        {
            return false;
        }
    }
}