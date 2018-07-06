using System;
using System.Collections.Generic;
using CDX.Utils;
using OpenTK;
using OpenTK.Graphics.OpenGL4;

namespace CDX.Graphics
{
    public class Mesh : IDisposable
    {
        public enum VertexDataType
        {
            VertexArray,
            VertexBufferObject,
            VertexBufferObjectSubData,
            VertexBufferObjectWithVAO
        }

        static readonly List<Mesh> meshes = new List<Mesh>();

        private VertexBufferObjectWithVAO vertices;
        private IndexBufferObject         indices;

        readonly bool isVertexArray;
        bool          autoBind = true;

        public Mesh(VertexDataType type, bool isStatic, int maxVertices, int maxIndices, params VertexAttribute[] attributes)
            : this(type, isStatic, maxVertices, maxIndices, new VertexAttributes(attributes))
        {
        }

        public Mesh(VertexDataType type, bool isStatic, int maxVertices, int maxIndices, VertexAttributes attributes)
        {
            // todo: handle other type

            vertices      = new VertexBufferObjectWithVAO(isStatic, maxVertices, attributes);
            indices       = new IndexBufferObject(isStatic, maxIndices);
            isVertexArray = false;
        }

        public Mesh(bool isStatic, int maxVertices, int maxIndices, VertexAttributes attributes)
        {
            vertices      = new VertexBufferObjectWithVAO(isStatic, maxVertices, attributes);
            indices       = new IndexBufferObject(isStatic, maxIndices);
            isVertexArray = false;
        }


        public Mesh setVertices(float[] vertices)
        {
            this.vertices.setVertices(vertices);
            return this;
        }

        public Mesh setVertices(float[] vertices, int offset, int count)
        {
            this.vertices.setVertices(vertices, offset, count);
            return this;
        }

        public Mesh setIndices(uint[] indices)
        {
            this.indices.setIndices(indices);
            return this;
        }

        public void render(ShaderProgram shader, PrimitiveType primitiveType, int offset, int count)
        {
            render(shader, primitiveType, offset, count, autoBind);
        }

        public void render(ShaderProgram shader, PrimitiveType primitiveType, int offset, int count, bool autoBind)
        {
            if (count == 0) return;

            if (autoBind)
                bind(shader);


            if (isVertexArray)
            {
                // Since we target GL 4.1+ we don't need this
                throw new Exception("Not supported");
                /*
                //if (indices.getNumIndices() > 0)
                if (indices.getIndices().Length > 0)
                {
                    //ShortBuffer buffer      = indices.getBuffer();
                    //int         oldPosition = buffer.position();
                    //int         oldLimit    = buffer.limit();
                    //buffer.position(offset);
                    //buffer.limit(offset + count);
                    GL.DrawElements(primitiveType, count, DrawElementsType.UnsignedInt, indices.getIndices());
                    //buffer.position(oldPosition);
                    //buffer.limit(oldLimit);
                }
                else
                {
                    GL.DrawArrays(primitiveType, offset, count);
                }
                */
            }
            else
            {
                //if (indices.getNumIndices() > 0)
                if (indices.getIndices().Length > 0)
                {
                    //if (count + offset > indices.getNumMaxIndices())
                    if (count + offset > indices.getIndices().Length)
                    {
                        throw new Exception("Mesh attempting to access memory outside of the index buffer (count: "
                                            + count + ", offset: " + offset + ", max: " + indices.getIndices().Length + ")");
                    }

                    GL.DrawElements(primitiveType, count, DrawElementsType.UnsignedInt, offset * 2);
                }
                else
                {
                    GL.DrawArrays(primitiveType, offset, count);
                }
            }

            if (autoBind) unbind(shader);
        }

        public void bind(ShaderProgram shader, int[] locations = null)
        {
            vertices.bind(shader, locations);
            //if (indices.getNumIndices() > 0) indices.bind();
            if (indices.getIndices().Length > 0) indices.bind();
        }

        public void unbind(ShaderProgram shader, int[] locations = null)
        {
            vertices.unbind(shader, locations);
            //if (indices.getNumIndices() > 0) indices.unbind();
            if (indices.getIndices().Length > 0) indices.unbind();
        }

        public BoundingBox calculateBoundingBox(ref BoundingBox bb, int offset, int count)
        {
            bb.inf();
            return extendBoundingBox(ref bb, offset, count);
        }

        public BoundingBox extendBoundingBox(ref BoundingBox bb, int offset, int count)
        {
            return extendBoundingBox(ref bb, offset, count, default(Matrix4));
        }

        public BoundingBox extendBoundingBox(ref BoundingBox bb, int offset, int count, Matrix4 transform)
        {
            // todo: i'm not sure about the difference between getNum and getNumMax..
            int numIndices  = getNumIndices();
            int numVertices = getNumVertices();
            int max         = numIndices == 0 ? numVertices : numIndices;
            if (offset < 0 || count < 1 || offset + count > max)
                throw new Exception("Invalid part specified ( offset=" + offset + ", count=" + count + ", max=" + max + " )");
            var verts      = vertices.getBuffer();
            var index      = indices.getBuffer();
            var posAttrib  = getVertexAttribute(VertexAttributes.Usage.Position);
            var posoff     = posAttrib.offset / 4;
            var vertexSize = vertices.getAttributes().vertexSize / 4;
            var end        = offset + count;

            
            switch (posAttrib.numComponents) {
                case 1:
                    if (numIndices > 0) {
                        for (int i = offset; i < end; i++) {
                            var idx = index[i] * vertexSize + posoff;
                            var tmpV = new Vector3(verts[idx], 0, 0);
                            if (transform != null) tmpV = Vector3.TransformPosition(tmpV, transform);// tmpV.mul(transform);
                                bb.ext(tmpV);
                        }
                    } else {
                        for (int i = offset; i < end; i++) {
                            int idx = i * vertexSize + posoff;
                            var tmpV = new Vector3(verts[idx], 0, 0);
                            if (transform != null) tmpV = Vector3.TransformPosition(tmpV, transform);// tmpV.mul(transform);
                                bb.ext(tmpV);
                        }
                    }
                    break;
                case 2:
                    if (numIndices > 0) {
                        for (int i = offset; i < end; i++) {
                            var idx = index[i] * vertexSize + posoff;
                            var tmpV = new Vector3(verts[idx], verts[idx+1], 0);
                            if (transform != null) tmpV = Vector3.TransformPosition(tmpV, transform);// tmpV.mul(transform);
                                bb.ext(tmpV);
                        }
                    } else {
                        for (int i = offset; i < end; i++) {
                            int idx = i * vertexSize + posoff;
                            var tmpV = new Vector3(verts[idx], verts[idx+1], 0);
                            if (transform != null) tmpV = Vector3.TransformPosition(tmpV, transform);// tmpV.mul(transform);
                                bb.ext(tmpV);
                        }
                    }
                    break;
                case 3:
                    if (numIndices > 0) {
                        for (int i = offset; i < end; i++) {
                            var idx = index[i] * vertexSize + posoff;
                            var tmpV = new Vector3(verts[idx], verts[idx+1], verts[idx+2]);
                            if (transform != null) tmpV = Vector3.TransformPosition(tmpV, transform);// tmpV.mul(transform);
                                bb.ext(tmpV);
                        }
                    } else {
                        for (int i = offset; i < end; i++) {
                            int idx = i * vertexSize + posoff;
                            var tmpV = new Vector3(verts[idx], verts[idx+1], verts[idx+2]);
                            if (transform != null) tmpV = Vector3.TransformPosition(tmpV, transform);// tmpV.mul(transform);
                                bb.ext(tmpV);
                        }
                    }
                    break;
            }
            
            
            return bb;
        }

        public VertexAttribute getVertexAttribute(int usage)
        {
            VertexAttributes attributes = vertices.getAttributes();
            int              len        = attributes.size();
            for (int i = 0; i < len; i++)
                if (attributes[i].usage == usage)
                    return attributes[i];

            return null;
        }
        
        public VertexAttributes getVertexAttributes () {
            return vertices.getAttributes();
        }


        public float[] getVerticesBuffer()
        {
            return vertices.getBuffer();
        }
        public uint[] getIndicesBuffer()
        {
            return indices.getBuffer();
        }
        
        private int getNumVertices()
        {
            return vertices.getNumMaxVertices();
        }

        private int getNumIndices()
        {
            return indices.getNumMaxVertices();
        }

        public void Dispose()
        {
            vertices.dispose();
            indices.dispose();
        }
    }
}