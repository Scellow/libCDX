using System;
using System.Collections.Generic;
using CDX.Graphics.G3D.Models;
using CDX.Utils;
using OpenTK;

namespace CDX.Graphics.G3D
{
    public class ModelInstance : RenderableProvider
    {
        public static bool defaultShareKeyframes = true;

        public readonly List<Material> materials = new List<Material>();

        public readonly List<Node> nodes = new List<Node>();

        //public readonly List<Animation> animations = new Array();
        public readonly Model   model;
        public          Matrix4 transform;
        public          object  userData;

        public ModelInstance(Model model, Matrix4 transform, params string[] rootNodeIds)
        {
            this.model     = model;
            this.transform = transform == null ? Matrix4.Identity : transform;

            if (rootNodeIds == null)
                copyNodes(model.nodes);
            else
                copyNodes(model.nodes, rootNodeIds);
            //copyAnimations(model.animations, defaultShareKeyframes);
            calculateTransforms();
        }
        
        public void calculateTransforms () {
            int n = nodes.Count;
            for (int i = 0; i < n; i++) {
                nodes[i].calculateTransforms(true);
            }
            for (int i = 0; i < n; i++) {
                nodes[i].calculateBoneTransforms(true);
            }
        }

        private void copyNodes(IList<Node> nodes)
        {
            for (int i = 0, n = nodes.Count; i < n; ++i)
            {
                Node node = nodes[i];
                this.nodes.Add(node.copy());
            }

            invalidate();
        }
        
        private void copyNodes (IList<Node> nodes, params string[] nodeIds) {
            for (int i = 0, n = nodes.Count; i < n; ++i) {
                Node node = nodes[i];
                foreach (string nodeId in nodeIds) {
                    if (nodeId == (node.id)) {
                        this.nodes.Add(node.copy());
                        break;
                    }
                }
            }
            invalidate();
        }

        private void invalidate()
        {
            for (int i = 0, n = nodes.Count; i < n; ++i)
            {
                invalidate(nodes[i]);
            }
        }

        // todo: maybe idendity issues with indexOf
        private void invalidate(Node node)
        {
            for (int i = 0, n = node.parts.Count; i < n; ++i)
            {
                NodePart                part     = node.parts[i];
                var bindPose = part.invBoneBindTransforms;
                if (bindPose != null)
                {
                    for (int j = 0; j < bindPose.Count; ++j)
                    {
                        // tood: figure out that ordered dic thing, key should be accessable by index
                        //bindPose.Keys[j] = getNode(bindPose.Keys[j].id);
                    }
                }

                if (!materials.Contains(part.material))
                {
                    int midx = materials.IndexOf(part.material);
                    if (midx < 0)
                        materials.Add(part.material = part.material.copy());
                    else
                        part.material = materials[midx];
                }
            }

            for (int i = 0, n = node.getChildCount(); i < n; ++i)
            {
                invalidate(node.getChild(i));
            }
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

        public void getRenderables(IList<Renderable> renderables, Pool<Renderable> pool)
        {
            foreach (Node node in nodes) {
                getRenderables(node, renderables, pool);
            }
        }
        
        
        protected void getRenderables (Node node, IList<Renderable> renderables, Pool<Renderable> pool) {
            if (node.parts.Count > 0) {
                foreach (NodePart nodePart in node.parts) {
                    if (nodePart.enabled) renderables.Add(getRenderable(pool.obtain(), node, nodePart));
                }
            }

            foreach (Node child in node.getChildren()) {
                getRenderables(child, renderables, pool);
            }
        }
        
        public Renderable getRenderable (Renderable @out, Node node, NodePart nodePart) {
            nodePart.setRenderable(@out);
            if (nodePart.bones == null && transform != default(Matrix4))
                @out.worldTransform = (transform)*(node.globalTransform);
            else if (transform != default(Matrix4))
                @out.worldTransform = (transform);
            else
                @out.worldTransform = Matrix4.Identity;
            @out.userData = userData;
            return @out;
        }
    }
}