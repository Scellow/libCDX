using System.Collections;
using System.Collections.Generic;

namespace CDX.Graphics.G3D.Utils
{
    public interface RenderableSorter
    {
        void sort(Camera camera, List<Renderable> renderables);
    }
}