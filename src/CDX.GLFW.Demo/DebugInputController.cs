namespace CDX.GLFWBackend.Demo
{
    class DebugInputController : InputAdapter
    {
        public override bool keyDown(Keys keycode)
        {
            Gdx.app.log("InputController", $"KeyDown: {keycode}");
            return base.keyDown(keycode);
        }

        public override bool keyUp(Keys keycode)
        {
            Gdx.app.log("InputController", $"KeyUp: {keycode}");
            return base.keyDown(keycode);
        }

        public override bool keyTyped(char character)
        {
            Gdx.app.log("InputController", $"KeyTyped: {character}");
            return base.keyTyped(character);
        }

        public override bool touchDown(int screenX, int screenY, int pointer, Buttons button)
        {
            Gdx.app.log("InputController", $"TouchDown: {button} {screenX}:{screenY}");
            return base.touchDown(screenX, screenY, pointer, button);
        }

        public override bool touchUp(int screenX, int screenY, int pointer, Buttons button)
        {
            Gdx.app.log("InputController", $"TouchUp: {button} {screenX}:{screenY}");
            return base.touchDown(screenX, screenY, pointer, button);
        }
    }
}