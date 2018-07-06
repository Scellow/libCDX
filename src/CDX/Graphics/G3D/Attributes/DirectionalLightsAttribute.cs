using System.Collections.Generic;
using CDX.Graphics.G3D.Environements;

namespace CDX.Graphics.G3D
{
    public class DirectionalLightsAttribute : Attribute
    {
        public static readonly string Alias = "directionalLights";
        public static readonly long   Type  = register(Alias);

        public static bool @is (long mask) {
            return (mask & Type) == mask;
        }

        public readonly List<DirectionalLight> lights;

        public DirectionalLightsAttribute ():base(Type) {
            lights = new List<DirectionalLight>(1);
        }

        public DirectionalLightsAttribute (DirectionalLightsAttribute copyFrom) :this() {
            lights.AddRange(copyFrom.lights);
        }

        public override Attribute copy()
        {
            return new DirectionalLightsAttribute(this);
        }

        public override int GetHashCode()
        {
            int result = base.GetHashCode();
            foreach (DirectionalLight light in lights)
            result = 1229 * result + (light == null ? 0 : light.GetHashCode());
            return result;
        }

        public override int CompareTo(Attribute o)
        {
            if (type != o.type) return type < o.type ? -1 : 1;
            return 0; // FIXME implement comparing
        }
    }
}