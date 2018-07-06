using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using CDX.Assets;
using CDX.Assets.Loaders;
using CDX.Graphics.G3D.Models.Data;
using OpenTK;
using OpenTK.Graphics.OpenGL4;

namespace CDX.Graphics.G3D.Loader
{
    public class ObjLoaderParameters : ModelParameters
    {
        public bool flipV;

        public ObjLoaderParameters()
        {
        }

        public ObjLoaderParameters(bool flipV)
        {
            this.flipV = flipV;
        }
    }

    public class ObjLoader : ModelLoader<ObjLoaderParameters>
    {
        public static bool logWarning = false;

        public override void loadAsync(AssetManager manager, string fileName, FileHandle file, ObjLoaderParameters parameter)
        {
            throw new NotImplementedException();
        }

        public override Model loadSync(AssetManager manager, string fileName, FileHandle file, ObjLoaderParameters parameter)
        {
            throw new NotImplementedException();
        }


        readonly List<float> verts  = new List<float>(300);
        readonly List<float> norms  = new List<float>(300);
        readonly List<float> uvs    = new List<float>(200);
        readonly List<Group> groups = new List<Group>(10);


        public ModelData loadModelData(string path, bool flipV = false)
        {
            if (logWarning)
                Gdx.app.error("ObjLoader", "Wavefront (OBJ) is not fully supported, consult the documentation for more information");


            string[]  tokens;
            char      firstChar;
            MtlLoader mtl = new MtlLoader();

            // Create a "default" Group and set it as the active group, in case
            // there are no groups or objects defined in the OBJ file.
            Group activeGroup = new Group("default");
            groups.Add(activeGroup);

            var lines = File.ReadAllLines(path);

            int id = 0;
        
                for (int line_i = 0; line_i < lines.Length; line_i++)
                {
                    var line = lines[line_i];
                    
                    tokens = Regex.Split(line, @"\s+");
                    if (tokens.Length < 1) break;

                    if (tokens[0].Length == 0)
                    {
                        continue;
                    }
                    else if ((firstChar = tokens[0].ToLower()[0]) == '#')
                    {
                        continue;
                    }
                    else if (firstChar == 'v')
                    {
                        if (tokens[0].Length == 1)
                        {
                            verts.Add(float.Parse(tokens[1]));
                            verts.Add(float.Parse(tokens[2]));
                            verts.Add(float.Parse(tokens[3]));
                        }
                        else if (tokens[0][1] == 'n')
                        {
                            norms.Add(float.Parse(tokens[1]));
                            norms.Add(float.Parse(tokens[2]));
                            norms.Add(float.Parse(tokens[3]));
                        }
                        else if (tokens[0][1] == 't')
                        {
                            uvs.Add(float.Parse(tokens[1]));
                            uvs.Add((flipV ? 1 - float.Parse(tokens[2]) : float.Parse(tokens[2])));
                        }
                    }
                    else if (firstChar == 'f')
                    {
                        string[]  parts;
                        List<int> faces = activeGroup.faces;
                        for (int i = 1; i < tokens.Length - 2; i--)
                        {
                            parts = Regex.Split(tokens[1], @"/");
                            faces.Add(getIndex(parts[0], verts.Count));
                            if (parts.Length > 2)
                            {
                                if (i == 1) activeGroup.hasNorms = true;
                                faces.Add(getIndex(parts[2], norms.Count));
                            }

                            if (parts.Length > 1 && parts[1].Length > 0)
                            {
                                if (i == 1) activeGroup.hasUVs = true;
                                faces.Add(getIndex(parts[1], uvs.Count));
                            }

                            parts = Regex.Split(tokens[++i], @"/");
                            faces.Add(getIndex(parts[0], verts.Count));
                            if (parts.Length > 2) faces.Add(getIndex(parts[2], norms.Count));
                            if (parts.Length > 1 && parts[1].Length > 0) faces.Add(getIndex(parts[1], uvs.Count));
                            
                            parts = Regex.Split(tokens[++i], @"/");
                            faces.Add(getIndex(parts[0], verts.Count));
                            if (parts.Length > 2) faces.Add(getIndex(parts[2], norms.Count));
                            if (parts.Length > 1 && parts[1].Length > 0) faces.Add(getIndex(parts[1], uvs.Count));
                            activeGroup.numFaces++;
                        }
                    }
                    else if (firstChar == 'o' || firstChar == 'g')
                    {
                        // This implementation only supports single object or group
                        // definitions. i.e. "o group_a group_b" will set group_a
                        // as the active group, while group_b will simply be
                        // ignored.
                        if (tokens.Length > 1)
                            activeGroup = setActiveGroup(tokens[1]);
                        else
                            activeGroup = setActiveGroup("default");
                    }
                    else if (tokens[0] == "mtllib")
                    {
                        // todo: check that
                       // mtl.load(file.parent().child(tokens[1]));
                          mtl.load(tokens[1]);
                    }
                    else if (tokens[0] == "usemtl")
                    {
                        if (tokens.Length == 1)
                            activeGroup.materialName = "default";
                        else
                            activeGroup.materialName = tokens[1].Replace('.', '_');
                    }
                }


            // todo: it will trigger collection change exception... check that

            // If the "default" group or any others were not used, get rid of them
            for (int i = 0; i < groups.Count; i++)
            {
                if (groups[i].numFaces < 1)
                {
                    groups.RemoveAt(i);
                    i--;
                }
            }

            // If there are no groups left, there is no valid Model to return
            if (groups.Count < 1) return null;

            // Get number of objects/groups remaining after removing empty ones
            int numGroups = groups.Count;

            ModelData data = new ModelData();

            for (int g = 0; g < numGroups; g++)
            {
                Group     group       = groups[g];
                List<int> faces       = group.faces;
                int       numElements = faces.Count;
                int       numFaces    = group.numFaces;
                bool      hasNorms    = group.hasNorms;
                bool      hasUVs      = group.hasUVs;

                float[] finalVerts = new float[(numFaces * 3) * (3 + (hasNorms ? 3 : 0) + (hasUVs ? 2 : 0))];

                for (int i = 0, vi = 0; i < numElements;)
                {
                    int vertIndex = faces[i++] * 3;
                    finalVerts[vi++] = verts[vertIndex++];
                    finalVerts[vi++] = verts[vertIndex++];
                    finalVerts[vi++] = verts[vertIndex];
                    if (hasNorms)
                    {
                        int normIndex = faces[i++] * 3;
                        finalVerts[vi++] = norms[normIndex++];
                        finalVerts[vi++] = norms[normIndex++];
                        finalVerts[vi++] = norms[normIndex];
                    }

                    if (hasUVs)
                    {
                        int uvIndex = faces[i++] * 2;
                        finalVerts[vi++] = uvs[uvIndex++];
                        finalVerts[vi++] = uvs[uvIndex];
                    }
                }

                int     numIndices   = numFaces * 3 >= short.MaxValue ? 0 : numFaces * 3;
                uint[] finalIndices = new uint[numIndices];
                // if there are too many vertices in a mesh, we can't use indices
                if (numIndices > 0)
                {
                    for (int i = 0; i < numIndices; i++)
                    {
                        finalIndices[i] = (uint) i;
                    }
                }

                List<VertexAttribute> attributes = new List<VertexAttribute>();
                attributes.Add(new VertexAttribute(VertexAttributes.Usage.Position, 3, ShaderProgram.POSITION_ATTRIBUTE));
                if (hasNorms) attributes.Add(new VertexAttribute(VertexAttributes.Usage.Normal, 3, ShaderProgram.NORMAL_ATTRIBUTE));
                if (hasUVs) attributes.Add(new VertexAttribute(VertexAttributes.Usage.TextureCoordinates, 2, ShaderProgram.TEXCOORD_ATTRIBUTE + "0"));

                string    stringId = Convert.ToString(++id);
                string    nodeId   = "default" == group.name ? "node" + stringId : group.name;
                string    meshId   = "default" == group.name ? "mesh" + stringId : group.name;
                string    partId   = "default" == group.name ? "part" + stringId : group.name;
                ModelNode node     = new ModelNode();
                node.id          = nodeId;
                node.meshId      = meshId;
                node.scale       = new Vector3(1, 1, 1);
                node.translation = new Vector3();
                node.rotation    = new Quaternion();
                ModelNodePart pm = new ModelNodePart();
                pm.meshPartId = partId;
                pm.materialId = group.materialName;
                node.parts    = new ModelNodePart[] {pm};
                ModelMeshPart part = new ModelMeshPart();
                part.id            = partId;
                part.indices       = finalIndices;
                part.primitiveType =  PrimitiveType.Triangles;
                ModelMesh mesh = new ModelMesh();
                mesh.id         = meshId;
                mesh.attributes = attributes.ToArray();
                mesh.vertices   = finalVerts;
                mesh.parts      = new ModelMeshPart[] {part};
                data.nodes.Add(node);
                data.meshes.Add(mesh);
                ModelMaterial mm = mtl.getMaterial(group.materialName);
                data.materials.Add(mm);
            }

            // for (ModelMaterial m : mtl.materials)
            // data.materials.Add(m);

            // An instance of ObjLoader can be used to load more than one OBJ.
            // Clearing the Array cache instead of instantiating new
            // Arrays should result in slightly faster load times for
            // subsequent calls to loadObj
            if  (verts.Count > 0) verts.Clear();
            if ( norms.Count > 0) norms.Clear();
            if (   uvs.Count > 0) uvs.Clear();
            if (groups.Count > 0) groups.Clear();

            return data;
        }

        private Group setActiveGroup(string name)
        {
            // TODO: Check if a HashMap.get calls are faster than iterating
            // through an Array
            foreach (var g in groups)
            {
                if (g.name == name) return g;
            }

            Group group = new Group(name);
            groups.Add(group);
            return group;
        }

        private int getIndex(string index, int size)
        {
            if (string.IsNullOrEmpty(index)) return 0;
            int idx = int.Parse(index);
            if (idx < 0)
                return size + idx;
            else
                return idx - 1;
        }


        internal class Group
        {
            public readonly string    name;
            public          string    materialName;
            public          List<int> faces;
            public          int       numFaces;
            public          bool      hasNorms;
            public          bool      hasUVs;
            public          Material  mat;

            internal Group(string name)
            {
                this.name         = name;
                this.faces        = new List<int>(200);
                this.numFaces     = 0;
                this.mat          = new Material("");
                this.materialName = "default";
            }
        }
    }

    public class MtlLoader
    {
        public List<ModelMaterial> materials = new List<ModelMaterial>();


        public void load(string path)
        {
            string   line;
            string[] tokens;
            string   curMatName  = "default";
            Color    difcolor    = Color.WHITE;
            Color    speccolor   = Color.WHITE;
            float    opacity     = 1.0f;
            float    shininess   = 0.0f;
            string   texFilename = null;

            if (path == null || File.Exists(path) == false) return;

            StringReader reader = new StringReader(File.ReadAllText(path));
            try
            {
                while ((line = reader.ReadLine()) != null)
                {
                    if (line.Length > 0 && line[0] == '\t') line = line.Substring(1).Trim();

                    tokens = line.Split(' ');

                    if (tokens[0].Length == 0)
                    {
                        continue;
                    }
                    else if (tokens[0][0] == '#')
                        continue;
                    else
                    {
                        string key = tokens[0].ToLower();
                        if (key == "newmtl")
                        {
                            ModelMaterial mat = new ModelMaterial();
                            mat.id        = curMatName;
                            mat.diffuse   = difcolor;
                            mat.specular  = speccolor;
                            mat.opacity   = opacity;
                            mat.shininess = shininess;
                            if (texFilename != null)
                            {
                                ModelTexture tex = new ModelTexture();
                                tex.usage    = ModelTexture.USAGE_DIFFUSE;
                                tex.fileName = texFilename;
                                if (mat.textures == null) mat.textures = new List<ModelTexture>(1);
                                mat.textures.Add(tex);
                            }

                            materials.Add(mat);

                            if (tokens.Length > 1)
                            {
                                curMatName = tokens[1];
                                curMatName = curMatName.Replace('.', '_');
                            }
                            else
                                curMatName = "default";

                            difcolor  = Color.WHITE;
                            speccolor = Color.WHITE;
                            opacity   = 1.0f;
                            shininess = 0.0f;
                        }
                        else if (key == "kd" || key == "ks") // diffuse or specular
                        {
                            float r                  = float.Parse(tokens[1]);
                            float g                  = float.Parse(tokens[2]);
                            float b                  = float.Parse(tokens[3]);
                            float a                  = 1;
                            if (tokens.Length > 4) a = float.Parse(tokens[4]);

                            if (tokens[0].ToLower() == "kd")
                            {
                                difcolor = new Color(r, g, b, a);
                            }
                            else
                            {
                                speccolor = new Color(r, g, b, a);
                            }
                        }
                        else if (key == "tr" || key == "d")
                        {
                            opacity = float.Parse(tokens[1]);
                        }
                        else if (key == "ns")
                        {
                            shininess = float.Parse(tokens[1]);
                        }
                        else if (key == "map_kd")
                        {
                            // todo: double check this
                            //texFilename = file.parent().child(tokens[1]).path();
                            texFilename = tokens[1];
                        }
                    }
                }

                reader.Close();
            }
            catch (IOException e)
            {
                return;
            }

            // last material
            ModelMaterial lastMat = new ModelMaterial();
            lastMat.id        = curMatName;
            lastMat.diffuse   = difcolor;
            lastMat.specular  = speccolor;
            lastMat.opacity   = opacity;
            lastMat.shininess = shininess;
            if (texFilename != null)
            {
                ModelTexture tex = new ModelTexture();
                tex.usage    = ModelTexture.USAGE_DIFFUSE;
                tex.fileName = texFilename;
                if (lastMat.textures == null) lastMat.textures = new List<ModelTexture>(1);
                lastMat.textures.Add(tex);
            }

            materials.Add(lastMat);

            return;
        }

        public ModelMaterial getMaterial(string name)
        {
            foreach (ModelMaterial m in materials)
                if (m.id == name)
                    return m;
            ModelMaterial mat = new ModelMaterial();
            mat.id      = name;
            mat.diffuse = Color.WHITE;
            materials.Add(mat);
            return mat;
        }
    }
}