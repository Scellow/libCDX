using OpenTK;

namespace CDX.Graphics.G3D.Environements
{
    public class PointLight : BaseLight<PointLight>
    {
        public Vector3 position = new Vector3();
        public float   intensity;

        public PointLight set(float r, float g, float b, float x, float y, float z, float intensity)
        {
            this.color     = new Color(r, g, b, 1f);
            this.position  = new Vector3(x, y, z);
            this.intensity = intensity;
            return this;
        }

        public PointLight set(PointLight copyFrom)
        {
            return set(copyFrom.color, copyFrom.position, copyFrom.intensity);
        }

        public PointLight set(Color color, Vector3 position, float intensity)
        {
            this.color     = (color);
            this.position  = (position);
            this.intensity = intensity;
            return this;
        }
    }
}