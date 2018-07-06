using OpenTK;

namespace CDX.Graphics.G3D.Environements
{
    public class SpotLight : BaseLight<SpotLight>
    {
        public Vector3 position  = new Vector3();
        public Vector3 direction = new Vector3();
        public float   intensity;
        public float   cutoffAngle;
        public float   exponent;


        public SpotLight setPosition(float positionX, float positionY, float positionZ)
        {
            this.position = new Vector3(positionX, positionY, positionZ);
            return this;
        }

        public SpotLight setPosition(Vector3 position)
        {
            this.position = (position);
            return this;
        }

        public SpotLight setDirection(float directionX, float directionY, float directionZ)
        {
            this.direction = new Vector3(directionX, directionY, directionZ);
            return this;
        }

        public SpotLight setDirection(Vector3 direction)
        {
            this.direction = (direction);
            return this;
        }

        public SpotLight setIntensity(float intensity)
        {
            this.intensity = intensity;
            return this;
        }

        public SpotLight setCutoffAngle(float cutoffAngle)
        {
            this.cutoffAngle = cutoffAngle;
            return this;
        }

        public SpotLight setExponent(float exponent)
        {
            this.exponent = exponent;
            return this;
        }

        public SpotLight set(SpotLight copyFrom)
        {
            return set(copyFrom.color, copyFrom.position, copyFrom.direction, copyFrom.intensity, copyFrom.cutoffAngle, copyFrom.exponent);
        }

        public SpotLight set(Color color, Vector3 position, Vector3 direction, float intensity,
            float cutoffAngle, float exponent) {
            if (color != null) this.color = (color);
            if (position != null) this.position  = (position);
            if (direction != null) this.direction = Vector3.Normalize(direction);
            this.intensity   = intensity;
            this.cutoffAngle = cutoffAngle;
            this.exponent    = exponent;
            return this;
        }

        public SpotLight set(float r,         float g,           float b, Vector3 position, Vector3 direction,
            float                  intensity, float cutoffAngle, float exponent) {
            this.color = new Color(r, g, b, 1f);
            if (position != null) this.position = (position);
            if (direction != null) this.direction = Vector3.Normalize(direction);;
            this.intensity   = intensity;
            this.cutoffAngle = cutoffAngle;
            this.exponent    = exponent;
            return this;
        }

        public SpotLight set(Color color, float posX, float posY, float posZ,      float dirX,
            float                                     dirY, float dirZ, float intensity, float cutoffAngle, float exponent) {
            if (color != null) this.color= (color);
            this.position = new Vector3 (posX, posY, posZ);
            this.direction = Vector3.Normalize(new Vector3(dirX, dirY, dirZ));
            this.intensity   = intensity;
            this.cutoffAngle = cutoffAngle;
            this.exponent    = exponent;
            return this;
        }

        public SpotLight set(float r,    float g,    float b,    float posX,      float posY,        float posZ,
            float                  dirX, float dirY, float dirZ, float intensity, float cutoffAngle, float exponent) {
            this.color = new Color(r, g, b, 1f);
            this.position = new Vector3(posX, posY, posZ);
            this.direction = Vector3.Normalize(new Vector3(dirX, dirY, dirZ));
            this.intensity   = intensity;
            this.cutoffAngle = cutoffAngle;
            this.exponent    = exponent;
            return this;
        }

        public SpotLight setTarget(Vector3 target)
        {
            direction = Vector3.Normalize(target - position);
            return this;
        }
    }
}