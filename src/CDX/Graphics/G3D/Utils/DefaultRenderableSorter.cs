using System;
using System.Collections;
using System.Collections.Generic;
using CDX.Utils;
using OpenTK;

namespace CDX.Graphics.G3D.Utils
{
    public class DefaultRenderableSorter : RenderableSorter, IComparer<Renderable>
    {
        private Camera camera;

        public void sort(Camera camera, List<Renderable> renderables)
        {
            this.camera = camera;
            renderables.Sort(this);
        }


        // todo: check this
        private Vector3 getTranslation(Matrix4 worldTransform, Vector3 center, out Vector3 output)
        {
            if (center == Vector3.Zero)
                output = worldTransform.ExtractTranslation();
            else if (!worldTransform.hasRotationOrScaling())
                output = worldTransform.ExtractTranslation() + center;
            else
                output = Vector3.TransformPosition(center, worldTransform);
            return output;
        }

        public int Compare(Renderable o1, Renderable o2)
        {
           var b1 = o1.material.has(BlendingAttribute.Type) && ((BlendingAttribute)o1.material.get(BlendingAttribute.Type)).blended;
           var b2 = o2.material.has(BlendingAttribute.Type) && ((BlendingAttribute)o2.material.get(BlendingAttribute.Type)).blended;
            if (b1 != b2) return b1 ? 1 : -1;
            // FIXME implement better sorting algorithm
            // final boolean same = o1.shader == o2.shader && o1.mesh == o2.mesh && (o1.lights == null) == (o2.lights == null) &&
            // o1.material.equals(o2.material);
            getTranslation(o1.worldTransform, o1.meshPart.center, out var tmpV1);
            getTranslation(o2.worldTransform, o2.meshPart.center, out var tmpV2);
            float dst    = (int)(1000f * camera.position.dst2(tmpV1)) - (int)(1000f * camera.position.dst2(tmpV2));
            int   result = dst < 0 ? -1 : (dst > 0 ? 1 : 0);
            return b1 ? -result : result;
        }
    }
}