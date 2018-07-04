namespace CDX
{
    public interface ILifecycleListener
    {
        void pause();

        void resume();

        void dispose();
    }
}