namespace CDX.Graphics
{
    public interface Shader
    {
        void init();

        int compareTo(Shader other); // TODO: probably better to add some weight value to sort on

        bool canRender(Renderable instance);

        void begin(Camera camera, RenderContext context);

        void render(Renderable renderable);

        void end();
    }
}