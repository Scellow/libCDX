using CDX.Graphics.GLUtils;
using OpenTK;

namespace CDX.Graphics
{
    public abstract class Viewport
    {
        private Camera camera;
        private float  worldWidth, worldHeight;
        private int    screenX,    screenY, screenWidth, screenHeight;

        public void apply()
        {
            apply(false);
        }

        public void apply(bool centerCamera)
        {
            HdpiUtils.glViewport(screenX, screenY, screenWidth, screenHeight);
            camera.viewportWidth  = worldWidth;
            camera.viewportHeight = worldHeight;
            if (centerCamera) camera.position = new Vector3(worldWidth / 2, worldHeight / 2, 0);
            camera.update();
        }

        public void update(int screenWidth, int screenHeight)
        {
            update(screenWidth, screenHeight, false);
        }

        public void update(int screenWidth, int screenHeight, bool centerCamera)
        {
            apply(centerCamera);
        }

        public Vector2 unproject(Vector2 screenCoords)
        {
            var tmp = new Vector3(screenCoords.X, screenCoords.Y, 1);
            camera.unproject(tmp, screenX, screenY, screenWidth, screenHeight);
            screenCoords.X = tmp.X;
            screenCoords.Y = tmp.Y;
            return screenCoords;
        }

        public Vector2 project(Vector2 worldCoords)
        {
            var tmp = new Vector3(worldCoords.X, worldCoords.Y, 1);
            camera.project(tmp, screenX, screenY, screenWidth, screenHeight);
            worldCoords.X = tmp.X;
            worldCoords.Y = tmp.Y;
            return worldCoords;
        }

        public Vector3 unproject(Vector3 screenCoords)
        {
            camera.unproject(screenCoords, screenX, screenY, screenWidth, screenHeight);
            return screenCoords;
        }

        public Vector3 project(Vector3 worldCoords)
        {
            camera.project(worldCoords, screenX, screenY, screenWidth, screenHeight);
            return worldCoords;
        }

        public Camera getCamera()
        {
            return camera;
        }
    }
}