using System;
using System.Collections.Generic;
using System.Globalization;
using CDX.Utils;
using OpenTK;

namespace CDX.Graphics.G3D.Models
{
    public class Node
    {
        public string     id;
        public bool       inheritTransform = true;
        public bool       isAnimated;
        public Vector3    translation     = new Vector3();
        public Quaternion rotation        = new Quaternion(0, 0, 0, 1);
        public Vector3    scale           = new Vector3(1, 1, 1);
        public Matrix4    localTransform  = Matrix4.Identity;
        public Matrix4    globalTransform = Matrix4.Identity;

        public List<NodePart> parts = new List<NodePart>(2);

        protected        Node       parent;
        private readonly List<Node> children = new List<Node>(2);

        
        public Matrix4 calculateLocalTransform () {
            if (!isAnimated)
            {
                localTransform = Matrix4.CreateScale(scale) * Matrix4.CreateRotationX(rotation.X)
                    *Matrix4.CreateRotationY(rotation.Y)
                    * Matrix4.CreateRotationZ(rotation.Z)
                    * Matrix4.CreateTranslation(translation);
            }
            return localTransform;
        }
        
        public Matrix4 calculateWorldTransform () {
            if (inheritTransform && parent != null)
                globalTransform = (parent.globalTransform) * (localTransform);
            else
                globalTransform = (localTransform);
            return globalTransform;
        }
        
        public void calculateTransforms (bool recursive) {
            calculateLocalTransform();
            calculateWorldTransform();

            if (recursive) {
                foreach (Node child in children) {
                    child.calculateTransforms(true);
                }
            }
        }

        public void calculateBoneTransforms(bool recursive)
        {
            // todo: figure out the arraymap thing
            /*
            foreach (NodePart part in parts)
            {
                if (part.invBoneBindTransforms == null || part.bones == null || part.invBoneBindTransforms.Count != part.bones.Length)
                    continue;
                int n = part.invBoneBindTransforms.Count;
                for (int i = 0; i < n; i++)
                    part.bones[i].set(part.invBoneBindTransforms.Keys[i].globalTransform).mul(part.invBoneBindTransforms.Values[i]);
            }

            if (recursive)
            {
                foreach (Node child in children)
                {
                    child.calculateBoneTransforms(true);
                }
            }
            */
        }
        
        
        
        
        
        
        
        
        
        public void addChild<T>(T child) where T : Node
        {
            insertChild(-1, child);
        }

        private int insertChild<T>(int index, T child) where T : Node
        {
            {
                for (Node p = this; p != null; p = p.getParent())
                {
                    if (p == child) throw new Exception("Cannot add a parent as a child");
                }
            }
            {
                Node p = child.getParent();
                if (p != null && !p.removeChild(child)) throw new Exception("Could not remove child from its current parent");
                if (index < 0 || index >= children.Count)
                {
                    index = children.Count;
                    children.Add(child);
                }
                else
                    children.Insert(index, child);

                child.parent = this;
            }
            return index;
        }

        public bool removeChild<T>(T child) where T : Node
        {
            if (!children.Remove(child)) return false;
            child.parent = null;
            return true;
        }

        public Node getParent()
        {
            return parent;
        }

        public void detach()
        {
            if (parent != null)
            {
                parent.removeChild(this);
                parent = null;
            }
        }

        public Node copy()
        {
            return new Node().set(this);
        }

        public IList<Node> getChildren()
        {
            return children;
        }


        protected Node set(Node other)
        {
            detach();
            id               = other.id;
            isAnimated       = other.isAnimated;
            inheritTransform = other.inheritTransform;
            translation      = (other.translation);
            rotation         = (other.rotation);
            scale            = (other.scale);
            localTransform   = (other.localTransform);
            globalTransform  = (other.globalTransform);
            parts.Clear();
            foreach (NodePart nodePart in other.parts)
            {
                parts.Add(nodePart.copy());
            }

            children.Clear();
            foreach (Node child in other.getChildren())
            {
                addChild(child.copy());
            }

            return this;
        }

        public int getChildCount()
        {
            return children.Count;
        }

        public Node getChild(int index)
        {
            return children[index];
        }

        public static Node getNode(IList<Node> nodes, string id, bool recursive, bool ignoreCase)
        {
            int  n = nodes.Count;
            Node node;
            if (ignoreCase)
            {
                for (int i = 0; i < n; i++)
                {
                    node = nodes[i];
                    if (node.id.Equals(id, StringComparison.InvariantCultureIgnoreCase))
                        return node;
                }
            }
            else
            {
                for (int i = 0; i < n; i++)
                {
                    node = nodes[i];
                    if (node.id == (id))
                        return node;
                }
            }

            if (recursive)
            {
                for (int i = 0; i < n; i++)
                {
                    node = getNode(nodes[i].children, id, true, ignoreCase);
                    if (node != null)
                        return node;
                }
            }

            return null;
        }
    }

    public class NodePart
    {
        public MeshPart                         meshPart;
        public Material                         material;
        public OrderedDictionary<Node, Matrix4> invBoneBindTransforms;
        public Matrix4[]                        bones;
        public bool                             enabled = true;

        public NodePart()
        {
        }

        public NodePart(MeshPart meshPart, Material material)
        {
            this.meshPart = meshPart;
            this.material = material;
        }

        // FIXME add copy constructor and override #equals.

        /** Convenience method to set the material, mesh, meshPartOffset, meshPartSize, primitiveType and bones members of the specified
         * Renderable. The other member of the provided {@link Renderable} remain untouched. Note that the material, mesh and bones
         * members are referenced, not copied. Any changes made to those objects will be reflected in both the NodePart and Renderable
         * object.
         * @param out The Renderable of which to set the members to the values of this NodePart. */
        public Renderable setRenderable(Renderable @out)
        {
            @out.material = material;
            @out.meshPart.set(meshPart);
            @out.bones = bones;
            return @out;
        }

        public NodePart copy()
        {
            return new NodePart().set(this);
        }

        protected NodePart set(NodePart other)
        {
            meshPart = new MeshPart(other.meshPart);
            material = other.material;
            enabled  = other.enabled;
            if (other.invBoneBindTransforms == null)
            {
                invBoneBindTransforms = null;
                bones                 = null;
            }
            else
            {
                if (invBoneBindTransforms == null)
                    invBoneBindTransforms = new OrderedDictionary<Node, Matrix4>();
                else
                    invBoneBindTransforms.Clear();

                foreach (var kp in other.invBoneBindTransforms)
                {
                    invBoneBindTransforms.Add(kp);
                }

                if (bones == null || bones.Length != invBoneBindTransforms.Count)
                    bones = new Matrix4[invBoneBindTransforms.Count];

                for (int i = 0; i < bones.Length; i++)
                {
                    if (bones[i] == default(Matrix4))
                        bones[i] = Matrix4.Identity;
                }
            }

            return this;
        }
    }
}