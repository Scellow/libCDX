using System;
using CDX.Graphics.Scene2D.UI;

namespace CDX.Graphics.Scene2D
{
    public class Stage : InputAdapter
    {
        private static bool debug;
        
        private Viewport viewport;
        private readonly IBatch batch;
        private bool ownsBatch;
        private Group   root;
        private readonly Actor[] pointerOverActors = new Actor[20];
        private readonly       bool[] pointerTouched = new bool[20];
        private readonly int[] pointerScreenX = new int[20];
        private readonly int[] pointerScreenY = new int[20];
        private int         mouseScreenX, mouseScreenY;
        private Actor       mouseOverActor;
        private Actor       keyboardFocus, scrollFocus;
        //private readonly SnapshotArray<TouchFocus> touchFocuses = new SnapshotArray(true, 4, TouchFocus.class);
        private bool actionsRequestRendering = true;

        //private ShapeRenderer debugShapes;
        private bool       debugInvisible, debugAll, debugUnderMouse, debugParentUnderMouse;
        private Debug         debugTableUnderMouse = Debug.none;
        private Color debugColor = new Color(0, 1, 0, 0.85f);

        public Stage (Viewport viewport, IBatch batch = null) {
            if (viewport == null) throw new Exception("viewport cannot be null.");
            
            this.viewport = viewport;

            if (batch == null)
            {
                // todo: make spritebatch : ibatch
                ownsBatch = true;
            }
            else
                this.batch    = batch;

            //root = new Group();
            //root.setStage(this);

            viewport.update(Gdx.graphics.getWidth(), Gdx.graphics.getHeight(), true);
        }
        
        
        public void draw () {
            Camera camera = viewport.getCamera();
            camera.update();

            //if (!root.isVisible()) return;

            var batch = this.batch;
            batch.setProjectionMatrix(camera.combined);
            batch.begin();
            //root.draw(batch, 1);
            batch.end();

            if (debug) drawDebug();
        }
        
        
        private void drawDebug () {
          // todo: implement debug stuff
        }


    }
}