using System;
using System.Collections.Generic;
using CDX.Utils;
using OpenTK;
using OpenTK.Graphics.OpenGL4;

namespace CDX.Graphics.G3D.Models.Data
{
    public class ModelData
    {
        public          string               id;
        public readonly short[]              version    = new short[2];
        public readonly List<ModelMesh>      meshes     = new List<ModelMesh>();
        public readonly List<ModelMaterial>  materials  = new List<ModelMaterial>();
        public readonly List<ModelNode>      nodes      = new List<ModelNode>();
        public readonly List<ModelAnimation> animations = new List<ModelAnimation>();

        public void addMesh(ModelMesh mesh)
        {
            foreach (var other in meshes)
            {
                if (other.id == mesh.id) throw new Exception("Mesh with id '" + other.id + "' already in model");
            }
        }
    }

    public class ModelAnimation
    {
        public string                   id;
        public List<ModelNodeAnimation> nodeAnimations = new List<ModelNodeAnimation>();
    }

    public class ModelNodeAnimation
    {
        public string                              nodeId;
        public List<ModelNodeKeyframe<Vector3>>    translation;
        public List<ModelNodeKeyframe<Quaternion>> rotation;
        public List<ModelNodeKeyframe<Vector3>>    scaling;
    }

    public class ModelNodeKeyframe<T>
    {
        public float keytime;
        public T     value = default(T);
    }

    public class ModelNode
    {
        public string          id;
        public Vector3         translation;
        public Quaternion      rotation;
        public Vector3         scale;
        public string          meshId;
        public ModelNodePart[] parts;
        public ModelNode[]     children;
    }

    public class ModelNodePart
    {
        public String                      materialId;
        public String                      meshPartId;
        public OrderedDictionary<string, Matrix4> bones;
        public int[,]                      uvMapping;
    }

    public class ModelMaterial
    {
        public enum MaterialType
        {
            Lambert,
            Phong
        }

        public String id;

        public MaterialType type;

        public Color ambient;
        public Color diffuse;
        public Color specular;
        public Color emissive;
        public Color reflection;

        public float shininess;
        public float opacity = 1.0f;

        public List<ModelTexture> textures;
    }

    public class ModelTexture
    {
        public const int USAGE_UNKNOWN      = 0;
        public const int USAGE_NONE         = 1;
        public const int USAGE_DIFFUSE      = 2;
        public const int USAGE_EMISSIVE     = 3;
        public const int USAGE_AMBIENT      = 4;
        public const int USAGE_SPECULAR     = 5;
        public const int USAGE_SHININESS    = 6;
        public const int USAGE_NORMAL       = 7;
        public const int USAGE_BUMP         = 8;
        public const int USAGE_TRANSPARENCY = 9;
        public const int USAGE_REFLECTION   = 10;

        public string  id;
        public string  fileName;
        public Vector2 uvTranslation;
        public Vector2 uvScaling;
        public int     usage;
    }

    public class ModelMesh
    {
        public string            id;
        public VertexAttribute[] attributes;
        public float[]           vertices;
        public ModelMeshPart[]   parts;
    }

    public class ModelMeshPart
    {
        public string  id;
        public uint[] indices;
        public PrimitiveType     primitiveType;
    }
}