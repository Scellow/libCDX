namespace CDX
{
    public interface IApplicationListener
    {
        void create();

        void resize(int width, int height);

        void render();

        void pause();

        void resume();

        void dispose();
    }
}