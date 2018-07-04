using System;
using OpenTK;

namespace CDX.Graphics
{
    public abstract class Camera
    {
        public Vector3 position = new Vector3();

        public Vector3 direction = new Vector3(0, 0, -1);

        public Vector3 up = new Vector3(0, 1, 0);

        public Matrix4 projection = Matrix4.Identity;

        public Matrix4 view = Matrix4.Identity;

        public Matrix4 combined = Matrix4.Identity;

        public Matrix4 invProjectionView = Matrix4.Identity;

        public float near = 1;

        public float far = 100;

        public float viewportWidth = 0;

        public float viewportHeight = 0;

        public abstract void update();

        public abstract void update(bool updateFrustum);

        public void lookAt(Vector3 target)
        {
            var tmp = target - position;
            tmp.Normalize();

            if (tmp != Vector3.Zero)
            {
                float dot = Vector3.Dot(tmp, up); // up and direction must ALWAYS be orthonormal vectors
                if (Math.Abs(dot - 1) < 0.000000001f)
                {
                    // Collinear
                    up = direction * -1;
                }
                else if (Math.Abs(dot + 1) < 0.000000001f)
                {
                    // Collinear opposite
                    up = direction;
                }

                direction = tmp;
                normalizeUp();
            }
        }

        public void lookAt(float x, float y, float z)
        {
            lookAt(new Vector3(x, y, z));
        }

        public void normalizeUp()
        {
            up = Vector3.Cross(direction, up);
            up.Normalize();
        }
    }

    public class OrthographicCamera : Camera
    {
        public float zoom = 1;

        public OrthographicCamera()
        {
            this.near = 0;
        }

        public OrthographicCamera(float viewportWidth, float viewportHeight)
        {
            this.viewportWidth  = viewportWidth;
            this.viewportHeight = viewportHeight;
            this.near           = 0;
            update();
        }

        public override void update()
        {
            update(true);
        }

        public override void update(bool updateFrustum)
        {
            projection = Matrix4.CreateOrthographicOffCenter(zoom * -viewportWidth / 2, zoom * (viewportWidth / 2), zoom * -(viewportHeight / 2), zoom * viewportHeight / 2, near, far);
            
            view = Matrix4.LookAt(position, position + direction, up);
            combined = projection * view;

            if (updateFrustum)
            {
                invProjectionView = combined.Inverted();
                
                // todo: update frustrum
            }
        }
    }
}