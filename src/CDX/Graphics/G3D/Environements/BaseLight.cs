namespace CDX.Graphics.G3D.Environements
{
    public abstract class BaseLight<T>
    {
        public Color color = new Color(0, 0, 0, 1);

        public BaseLight<T> setColor(float r, float g, float b, float a)
        {
            this.color = new Color(r, g, b, a);
            return this;
        }

        public BaseLight<T> setColor(Color color)
        {
            this.color = color;
            return this;
        }
    }
}