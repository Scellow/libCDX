using OpenTK;

namespace CDX.Graphics.G3D.Environements
{
    public class DirectionalLight : BaseLight<DirectionalLight>
    {
        public Vector3 direction = new Vector3();

        public DirectionalLight setDirection(float directionX, float directionY, float directionZ)
        {
            this.direction = new Vector3(directionX, directionY, directionZ);
            return this;
        }

        public DirectionalLight setDirection(Vector3 direction)
        {
            this.direction = direction;
            return this;
        }

        public DirectionalLight set(DirectionalLight copyFrom)
        {
            return set(copyFrom.color, copyFrom.direction);
        }

        public DirectionalLight set(Color color, Vector3 direction)
        {
            this.color     = color;
            this.direction = Vector3.Normalize(direction);
            return this;
        }

        public DirectionalLight set(float r, float g, float b, Vector3 direction)
        {
            this.color     = new Color(r, g, b, 1f);
            this.direction = Vector3.Normalize(direction);
            return this;
        }

        public DirectionalLight set(Color color, float dirX, float dirY, float dirZ)
        {
            this.color     = color;
            this.direction = Vector3.Normalize(new Vector3(dirX, dirY, dirZ));
            return this;
        }

        public DirectionalLight set(float r, float g, float b, float dirX, float dirY, float dirZ)
        {
            this.color     = new Color(r, g, b, 1f);
            this.direction = Vector3.Normalize(new Vector3(dirX, dirY, dirZ));
            return this;
        }

        public override bool Equals(object obj)
        {
            return (obj is DirectionalLight) ? equals((DirectionalLight) obj) : false;
        }

        public bool equals(DirectionalLight other)
        {
            return (other != null) && ((other == this) || ((color == (other.color) && direction == (other.direction))));
        }
    }
}