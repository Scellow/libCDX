using System;
using System.Collections.Generic;
using System.Linq;
using CDX.Graphics.G3D.Models;
using CDX.Graphics.G3D.Models.Data;
using CDX.Graphics.G3D.Utils;
using CDX.Utils;
using OpenTK;
using OpenTK.Graphics.OpenGL4;
using ModelData = CDX.Graphics.G3D.Models.Data.ModelData;

namespace CDX.Graphics.G3D
{
    public class Model : IDisposable
    {
        public readonly List<Material> materials = new List<Material>();

        public readonly List<Node> nodes = new List<Node>();

        //public readonly List<Animation> animations = new List<Animation>();
        public readonly List<Mesh>        meshes      = new List<Mesh>();
        public readonly List<MeshPart>    meshParts   = new List<MeshPart>();
        public readonly List<IDisposable> disposables = new List<IDisposable>();


        public Model()
        {
        }

        public Model(ModelData modelData) : this(modelData, new FileTextureProvider())
        {
        }

        public Model(ModelData modelData, TextureProvider textureProvider)
        {
            load(modelData, textureProvider);
        }

        protected void load(ModelData modelData, TextureProvider textureProvider)
        {
            loadMeshes(modelData.meshes);
            loadMaterials(modelData.materials, textureProvider);
            loadNodes(modelData.nodes);
            //loadAnimations(modelData.animations);
            calculateTransforms();
        }

        private Dictionary<NodePart, OrderedDictionary<string, Matrix4>> nodePartBones = new Dictionary<NodePart, OrderedDictionary<string, Matrix4>>();

        protected void loadNodes(IList<ModelNode> modelNodes)
        {
            nodePartBones.Clear();
            foreach (ModelNode node in modelNodes) {
                nodes.Add(loadNode(node));
            }
            foreach (var e in nodePartBones)
            {
                if (e.Key.invBoneBindTransforms == null)
                    e.Key.invBoneBindTransforms = new OrderedDictionary<Node, Matrix4>();
                e.Key.invBoneBindTransforms.Clear();
                foreach (var b in e.Value)
                    e.Key.invBoneBindTransforms.Add(getNode(b.Key), Matrix4.Invert(b.Value));
            }
        }

        protected Node loadNode (ModelNode modelNode) {
            Node node = new Node();
            node.id = modelNode.id;

            if (modelNode.translation != null) node.translation = (modelNode.translation);
            if (modelNode.rotation != null) node.rotation = (modelNode.rotation);
            if (modelNode.scale != null) node.scale = (modelNode.scale);
            // FIXME create temporary maps for faster lookup?
            if (modelNode.parts != null) {
                foreach (ModelNodePart modelNodePart in modelNode.parts) {
                    MeshPart meshPart     = null;
                    Material meshMaterial = null;

                    if (modelNodePart.meshPartId != null) {
                        foreach (MeshPart part in meshParts) {
                            if (modelNodePart.meshPartId == (part.id)) {
                                meshPart = part;
                                break;
                            }
                        }
                    }

                    if (modelNodePart.materialId != null) {
                        foreach(Material material in materials) {
                            if (modelNodePart.materialId == (material.id)) {
                                meshMaterial = material;
                                break;
                            }
                        }
                    }

                    if (meshPart == null || meshMaterial == null) throw new Exception("Invalid node: " + node.id);

                    if (meshPart != null && meshMaterial != null) {
                        NodePart nodePart = new NodePart();
                        nodePart.meshPart = meshPart;
                        nodePart.material = meshMaterial;
                        node.parts.Add(nodePart);
                        if (modelNodePart.bones != null) nodePartBones.Add(nodePart, modelNodePart.bones);
                    }
                }
            }

            if (modelNode.children != null) {
                foreach (ModelNode child in modelNode.children) {
                    node.addChild(loadNode(child));
                }
            }

            return node;
        }

        private void loadMeshes(List<ModelMesh> meshes)
        {
            foreach (ModelMesh mesh in meshes)
            {
                convertMesh(mesh);
            }
        }

        protected void convertMesh(ModelMesh modelMesh)
        {
            var numIndices = 0;
            foreach (var part in modelMesh.parts)
            {
                numIndices += part.indices.Length;
            }

            var attributes  = new VertexAttributes(modelMesh.attributes);
            var numVertices = modelMesh.vertices.Length / (attributes.vertexSize / 4);
            numVertices = modelMesh.vertices.Length;
            var mesh = new Mesh(true, numVertices, numIndices, attributes);
            
            meshes.Add(mesh);
            disposables.Add(mesh);

            // src, dst, num, offset
            //mesh.setVertices(new float[modelMesh.vertices.Length]);
            //Array.Copy(modelMesh.vertices, mesh.getVerticesBuffer(), modelMesh.vertices.Length);
            mesh.setVertices(modelMesh.vertices);
            
            var list   = new List<uint>();
            var offset = 0;
            foreach (var part in modelMesh.parts)
            {
                var meshPart = new MeshPart();
                meshPart.id            = part.id;
                meshPart.primitiveType = part.primitiveType;
                meshPart.offset        = offset;
                meshPart.size          = part.indices.Length;
                meshPart.mesh          = mesh;
                //mesh.getIndicesBuffer().put(part.indices);

                // todo: optimize
                list.AddRange(part.indices.ToList());

                offset += meshPart.size;


                meshParts.Add(meshPart);
            }

            mesh.setIndices(list.ToArray());

            foreach (var part in meshParts)
                part.update();
        }

        private void loadMaterials(List<ModelMaterial> modelMaterials, TextureProvider textureProvider)
        {
            foreach (ModelMaterial mtl in modelMaterials)
            {
                this.materials.Add(convertMaterial(mtl, textureProvider));
            }
        }

        protected Material convertMaterial(ModelMaterial mtl, TextureProvider textureProvider)
        {
            Material result = new Material();
            result.id = mtl.id;
            if (mtl.ambient != default(Color)) result.set(new ColorAttribute(ColorAttribute.Ambient, mtl.ambient));
            if (mtl.diffuse != default(Color)) result.set(new ColorAttribute(ColorAttribute.Diffuse, mtl.diffuse));
            if (mtl.specular != default(Color)) result.set(new ColorAttribute(ColorAttribute.Specular, mtl.specular));
            if (mtl.emissive != default(Color)) result.set(new ColorAttribute(ColorAttribute.Emissive, mtl.emissive));
            if (mtl.reflection != default(Color)) result.set(new ColorAttribute(ColorAttribute.Reflection, mtl.reflection));
            if (mtl.shininess > 0f) result.set(new FloatAttribute(FloatAttribute.Shininess, mtl.shininess));
            if (mtl.opacity != 1.0f) result.set(new BlendingAttribute(true, BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha, mtl.opacity));

            var textures = new Dictionary<string, Texture>();

            // FIXME uvScaling/uvTranslation totally ignored
            if (mtl.textures != null)
            {
                foreach (ModelTexture tex in mtl.textures)
                {
                    Texture texture;
                    if (textures.ContainsKey(tex.fileName))
                    {
                        texture = textures[tex.fileName];
                    }
                    else
                    {
                        texture = textureProvider.load(tex.fileName);
                        textures.Add(tex.fileName, texture);
                        disposables.Add(texture);
                    }

                    var descriptor = new TextureDescriptor(texture);
                    descriptor.minFilter = texture.getMinFilter();
                    descriptor.magFilter = texture.getMagFilter();
                    descriptor.uWrap     = texture.getUWrap();
                    descriptor.vWrap     = texture.getVWrap();

                    float offsetU = tex.uvTranslation == null ? 0f : tex.uvTranslation.X;
                    float offsetV = tex.uvTranslation == null ? 0f : tex.uvTranslation.Y;
                    float scaleU  = tex.uvScaling == null ? 1f : tex.uvScaling.X;
                    float scaleV  = tex.uvScaling == null ? 1f : tex.uvScaling.Y;

                    switch (tex.usage)
                    {
                        case ModelTexture.USAGE_DIFFUSE:
                            result.set(new TextureAttribute(TextureAttribute.Diffuse, descriptor, offsetU, offsetV, scaleU, scaleV));
                            break;
                        case ModelTexture.USAGE_SPECULAR:
                            result.set(new TextureAttribute(TextureAttribute.Specular, descriptor, offsetU, offsetV, scaleU, scaleV));
                            break;
                        case ModelTexture.USAGE_BUMP:
                            result.set(new TextureAttribute(TextureAttribute.Bump, descriptor, offsetU, offsetV, scaleU, scaleV));
                            break;
                        case ModelTexture.USAGE_NORMAL:
                            result.set(new TextureAttribute(TextureAttribute.Normal, descriptor, offsetU, offsetV, scaleU, scaleV));
                            break;
                        case ModelTexture.USAGE_AMBIENT:
                            result.set(new TextureAttribute(TextureAttribute.Ambient, descriptor, offsetU, offsetV, scaleU, scaleV));
                            break;
                        case ModelTexture.USAGE_EMISSIVE:
                            result.set(new TextureAttribute(TextureAttribute.Emissive, descriptor, offsetU, offsetV, scaleU, scaleV));
                            break;
                        case ModelTexture.USAGE_REFLECTION:
                            result.set(new TextureAttribute(TextureAttribute.Reflection, descriptor, offsetU, offsetV, scaleU, scaleV));
                            break;
                    }
                }
            }

            return result;
        }

        public void calculateTransforms()
        {
            
            int n = nodes.Count;
            for (int i = 0; i < n; i++) {
                nodes[i].calculateTransforms(true);
            }
            for (int i = 0; i < n; i++) {
                nodes[i].calculateBoneTransforms(true);
            }
            
        }

        public void Dispose()
        {
        }
        
        
        public Node getNode (string id) {
            return getNode(id, true);
        }
        public Node getNode (string id, bool recursive) {
            return getNode(id, recursive, false);
        }
        public Node getNode (string id, bool recursive, bool ignoreCase) {
            return Node.getNode(nodes, id, recursive, ignoreCase);
        }
    }
}