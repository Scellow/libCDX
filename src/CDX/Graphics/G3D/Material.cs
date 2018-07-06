namespace CDX.Graphics.G3D
{
    public class Material : Attributes
    {
        private static int    counter = 0;
        public         string id;

        public Material() : this("mtl" + (++counter))

        {
        }

        public Material(string id)
        {
            this.id = id;
        }

        public Material(Material copyFrom) : this(copyFrom.id, copyFrom)
        {
        }

        public Material(string id, Material copyFrom) : this(id)
        {
            foreach (var attr in copyFrom)
                set(attr.copy());
        }

        public Material copy()
        {
            return new Material(this);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode() + 3 * id.GetHashCode();
        }

        public override bool Equals(object other)
        {
            return (other is Material) && ((other == this) || ((((Material) other).id == (id)) && base.Equals(other)));
        }
    }
}