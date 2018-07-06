using System.Collections.Generic;
using CDX.Graphics;
using CDX.Graphics.G3D;
using CDX.Graphics.G3D.Loader;
using OpenTK;
using OpenTK.Graphics.OpenGL4;

namespace CDX.GLFWBackend.Demo
{
    public class TestModel : ApplicationAdapter
    {
        private ModelBatch          _batch;
        private Model               _model;
        private List<ModelInstance> _modelInstances = new List<ModelInstance>();
        private PerspectiveCamera   _camera;

        public override void create()
        {
            Gdx.app.log("Game", "Create");


            _batch           = new ModelBatch();
            _camera          = new PerspectiveCamera(67, Gdx.graphics.getWidth(), Gdx.graphics.getHeight());
            _camera.position = new Vector3(10f, 10f, 100f);
            _camera.lookAt(0f, 0f, 0f);
            _camera.near = 1;
            _camera.far  = 300;
            _camera.update();

            var loader = new ObjLoader();
            var data   = loader.loadModelData("brown_wall.obj");
            _model = new Model(data);

            for (int i = -10; i < 10; i++)
            {
                var modelInstance = new ModelInstance(_model, Matrix4.Identity, null);
                _modelInstances.Add(modelInstance);
            }
            Gdx.app.debug("TestModel", $"V: {Gdx.graphics.getWidth()}:{Gdx.graphics.getHeight()}  Models: {_modelInstances.Count}");
        }

        public override void render()
        {
            GL.Viewport(0, 0, Gdx.graphics.getBackBufferWidth(), Gdx.graphics.getHeight());
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.ClearColor(0.5f, 0, 0.25f, 1);

            _batch.begin(_camera);

            foreach (var modelInstance in _modelInstances)
            {
                _batch.render(modelInstance);
            }

            _batch.end();
        }
    }
}