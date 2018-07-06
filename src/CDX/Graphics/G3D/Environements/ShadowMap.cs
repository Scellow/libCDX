using OpenTK;

namespace CDX.Graphics.G3D.Environements
{
    public interface ShadowMap {
        Matrix4 getProjViewTrans ();

        TextureDescriptor getDepthMap ();
    }

}