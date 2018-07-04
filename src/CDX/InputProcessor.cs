namespace CDX
{
    public interface InputProcessor
    {
        bool keyDown(Keys keycode);

        bool keyUp(Keys keycode);

        bool keyTyped(char character);

        bool touchDown(int screenX, int screenY, int pointer, Buttons button);

        bool touchUp(int screenX, int screenY, int pointer, Buttons button);

        bool touchDragged(int screenX, int screenY, int pointer);

        bool mouseMoved(int screenX, int screenY);

        bool scrolled(int amount);
    }
}