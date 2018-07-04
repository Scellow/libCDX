namespace CDX
{
    public abstract class ApplicationAdapter : IApplicationListener
    {
        public virtual void create()
        {
        }

        public virtual void resize(int width, int height)
        {
        }

        public virtual void render()
        {
        }

        public virtual void pause()
        {
        }

        public virtual void resume()
        {
        }

        public virtual void dispose()
        {
        }
    }
}