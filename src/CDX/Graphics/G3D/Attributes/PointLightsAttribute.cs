using System.Collections.Generic;
using CDX.Graphics.G3D.Environements;

namespace CDX.Graphics.G3D
{
    public class PointLightsAttribute : Attribute
    {
        public static readonly string Alias = "pointLights";
        public static readonly long   Type  = register(Alias);

        public static bool @is(long mask)
        {
            return (mask & Type) == mask;
        }

        public readonly List<PointLight> lights;

        public PointLightsAttribute() : base(Type)
        {
            lights = new List<PointLight>(1);
        }

        public PointLightsAttribute(PointLightsAttribute copyFrom) : this()
        {
            lights.AddRange(copyFrom.lights);
        }

        public override Attribute copy()
        {
            return new PointLightsAttribute(this);
        }

        public override int GetHashCode()
        {
            int result = base.GetHashCode();
            foreach (var light in lights)
            result = 1231 * result + (light == null ? 0 : light.GetHashCode());
            return result;
        }

        public override int CompareTo(Attribute o)
        {
            if (type != o.type) return type < o.type ? -1 : 1;
            return 0; // FIXME implement comparing
        }
    }
}