using System.Collections.Generic;
using CDX.Graphics.G3D.Environements;

namespace CDX.Graphics.G3D
{
    public class SpotLightsAttribute : Attribute
    {
        public readonly static string Alias = "spotLights";
        public readonly static long   Type  = register(Alias);

        public static bool @is (long mask) {
            return (mask & Type) == mask;
        }

        public readonly List<SpotLight> lights;

        public SpotLightsAttribute () : base(Type){
            lights = new List<SpotLight>(1);
        }

        public SpotLightsAttribute (SpotLightsAttribute copyFrom) :this(){
            lights.AddRange(copyFrom.lights);
        }
        public override Attribute copy()
        {
            return new SpotLightsAttribute(this);
        }

        public override int GetHashCode()
        {
            int result = base.GetHashCode();
            foreach (SpotLight light in lights)
            result = 1237 * result + (light == null ? 0 : light.GetHashCode());
            return result;
        }

        public override int CompareTo(Attribute o)
        {
            if (type != o.type) return type < o.type ? -1 : 1;
            return 0; // FIXME implement comparing
        }
    }
}