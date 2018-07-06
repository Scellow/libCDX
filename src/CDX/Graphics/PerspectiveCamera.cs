using System;
using OpenTK;

namespace CDX.Graphics
{
    public class PerspectiveCamera : Camera
    {
        public float fieldOfView = 67;

        public PerspectiveCamera()
        {
        }

        public PerspectiveCamera(float fieldOfViewY, float viewportWidth, float viewportHeight)
        {
            this.fieldOfView    = fieldOfViewY;
            this.viewportWidth  = viewportWidth;
            this.viewportHeight = viewportHeight;
            update();
        }

        public override void update()
        {
            update(true);
        }

        public override void update(bool updateFrustum)
        {
            float aspect = viewportWidth / viewportHeight;
            projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(fieldOfView), aspect, Math.Abs(near), Math.Abs(far));
            view       = Matrix4.LookAt(position, position + direction, up);

            combined = projection * combined * view;

            if (updateFrustum)
            {
                invProjectionView = Matrix4.Invert(combined);
                
                // tood: frustrum
                //frustum.update(invProjectionView);
            }
        }
    }
}