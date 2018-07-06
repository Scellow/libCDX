# libCDX
libGDX port to C# using .NET Core


# What's missing

- Audio
- Net
- Scene2D
- Controller Support

# Demo

```c#
    public class Sandbox : ApplicationAdapter
    {
        private SpriteBatch   _batch;
        private Texture       _texture;

        public override void create()
        {
            _batch = new SpriteBatch();
            _texture = Texture.loadFromFile("badlogic.jpg");
        }

        public override void render()
        {
            GL.Viewport(0,0, Gdx.graphics.getBackBufferWidth(), Gdx.graphics.getHeight());
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.ClearColor(0.5f,0,0.25f, 1);
            
            _batch.begin();
            _batch.draw(_texture, 0, 0);
            _batch.end();
        }
    }
```

![image](https://raw.githubusercontent.com/Scellow/libCDX/master/wiki/screenshot_0.png)
