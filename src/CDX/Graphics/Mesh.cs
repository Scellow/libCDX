using System;
using System.Collections.Generic;
using OpenTK.Graphics.OpenGL4;

namespace CDX.Graphics
{
    public class Mesh
    {
        public enum VertexDataType {
            VertexArray, VertexBufferObject, VertexBufferObjectSubData, VertexBufferObjectWithVAO
        }
        
        static readonly List<Mesh> meshes = new List<Mesh>();

        private VertexBufferObjectWithVAO vertices;
        private IndexBufferObject indices;
        
        readonly bool isVertexArray;
        bool autoBind = true;
        
        public Mesh (VertexDataType type, bool isStatic, int maxVertices, int maxIndices, params VertexAttribute[] attributes)
            :this(type, isStatic, maxVertices, maxIndices, new VertexAttributes(attributes)){
           
        }
        
        public Mesh(VertexDataType type, bool isStatic, int maxVertices, int maxIndices, VertexAttributes attributes)
        {            
            // todo: handle other type
            
            vertices = new VertexBufferObjectWithVAO(isStatic, maxVertices, attributes);
            indices = new IndexBufferObject(isStatic, maxIndices);
            isVertexArray = false;
        }
        
        public Mesh (bool isStatic, int maxVertices, int maxIndices, VertexAttributes attributes) {
            vertices      = new VertexBufferObjectWithVAO(isStatic, maxVertices, attributes);
            indices       = new IndexBufferObject(isStatic, maxIndices);
            isVertexArray = false;
        }        
        
        
        public Mesh setVertices (float[] vertices) {
            this.vertices.setVertices(vertices);
            return this;
        }
        
        public Mesh setVertices (float[] vertices, int offset, int count) {
            this.vertices.setVertices(vertices, offset, count);

            return this;
        }
        
        public Mesh setIndices (uint[] indices) {
            this.indices.setIndices(indices);
            return this;
        }
        
        public void render (ShaderProgram shader, PrimitiveType primitiveType, int offset, int count) {
            render(shader, primitiveType, offset, count, autoBind);
        }
        
        public void render (ShaderProgram shader, PrimitiveType primitiveType, int offset, int count, bool autoBind) {
            if (count == 0) return;

            if (autoBind)
                bind(shader);
            
            // todo: i really need to find a c# alternative to Buffer and correctly implement it, so i can handle that shit correctly
/*
            if (isVertexArray)
            {
                if (indices.getNumIndices() > 0)
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
            }
            else
*/
            {

                //if (indices.getNumIndices() > 0)
                if(indices.getIndices().Length > 0)
                {
                    /*
                    if (count + offset > indices.getNumMaxIndices())
                    {
                        throw new Exception("Mesh attempting to access memory outside of the index buffer (count: "
                                            + count + ", offset: " + offset + ", max: " + indices.getNumMaxIndices() + ")");
                    }
                    */

                    GL.DrawElements(primitiveType, count, DrawElementsType.UnsignedInt, offset * 2);
                }
                else
                {
                    GL.DrawArrays(primitiveType, offset, count);
                }
            }

            if (autoBind) unbind(shader);
        }
        
        public void bind (ShaderProgram shader, int[] locations = null) {
            vertices.bind(shader, locations);
            //if (indices.getNumIndices() > 0) indices.bind();
            if (indices.getIndices().Length > 0) indices.bind();
        }
        public void unbind (ShaderProgram shader, int[] locations = null) {
            vertices.unbind(shader, locations);
            //if (indices.getNumIndices() > 0) indices.unbind();
            if (indices.getIndices().Length > 0) indices.unbind();
        }
        
    }

}