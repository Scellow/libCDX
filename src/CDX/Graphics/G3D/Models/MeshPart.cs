using CDX.Utils;
using OpenTK;
using OpenTK.Graphics.OpenGL4;

namespace CDX.Graphics.G3D.Models
{
    public class MeshPart
    {
        public string id;

        public PrimitiveType primitiveType;
        public int           offset;
        public int           size;
        public Mesh          mesh;
        public Vector3       center      = new Vector3();
        public Vector3       halfExtents = new Vector3();

        public float radius = -1;

        public MeshPart()
        {
        }


        public MeshPart(string id, Mesh mesh, int offset, int size, PrimitiveType type)
        {
            set(id, mesh, offset, size, type);
        }

        public MeshPart(MeshPart copyFrom)
        {
            set(copyFrom);
        }

        public MeshPart set(MeshPart other)
        {
            this.id            = other.id;
            this.mesh          = other.mesh;
            this.offset        = other.offset;
            this.size          = other.size;
            this.primitiveType = other.primitiveType;
            this.center        = other.center;
            this.halfExtents   = other.halfExtents;
            this.radius        = other.radius;
            return this;
        }

        public MeshPart set(string id, Mesh mesh, int offset, int size, PrimitiveType type)
        {
            this.id            = id;
            this.mesh          = mesh;
            this.offset        = offset;
            this.size          = size;
            this.primitiveType = type;
            this.center        = new Vector3(0, 0, 0);
            this.halfExtents   = new Vector3(0, 0, 0);
            this.radius        = -1f;
            return this;
        }

        public void update()
        {
            var bounds = default(BoundingBox);
            mesh.calculateBoundingBox(ref bounds, offset, size);
            center = bounds.getCenter();
            halfExtents = bounds.getDimension() * 0.5f;
            radius = halfExtents.Length;
        }

        public bool equals(MeshPart other)
        {
            return other == this
                   || (other != null && other.mesh == mesh && other.primitiveType == primitiveType && other.offset == offset && other.size == size);
        }

        public override bool Equals(object arg0)
        {
            if (arg0 == null) return false;
            if (arg0 == this) return true;
            if (!(arg0 is MeshPart)) return false;
            return equals((MeshPart) arg0);
        }

        public void render(ShaderProgram shader, bool autoBind)
        {
            mesh.render(shader, primitiveType, offset, size, autoBind);
        }

        public void render(ShaderProgram shader)
        {
            mesh.render(shader, primitiveType, offset, size);
        }
    }
}