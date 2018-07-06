using CDX.Graphics.G3D.Models;
using OpenTK;

namespace CDX.Graphics.G3D
{
    public class Renderable
    {
        public          Matrix4  worldTransform;
        public readonly MeshPart meshPart = new MeshPart();

        public Material material;
        public Environment environment;

        public Matrix4[] bones;
        public Shader    shader;

        public object userData;


        public Renderable set(Renderable renderable)
        {
            worldTransform = renderable.worldTransform;
            material       = renderable.material;
            meshPart.set(renderable.meshPart);
            bones = renderable.bones;
            environment = renderable.environment;
            shader   = renderable.shader;
            userData = renderable.userData;
            return this;
        }
    }
}