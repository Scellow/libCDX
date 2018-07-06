using System;
using System.Collections.Generic;
using System.IO;
using CDX.Utils;
using OpenTK.Graphics.OpenGL4;

namespace CDX.Graphics
{
    public class VertexBufferObjectWithVAO
    {
        private readonly VertexAttributes attributes;
        private int bufferHandle;
        private bool isStatic;
        private BufferUsageHint usage;
        private bool isDirty = false;
        private bool isBound = false;
        private int vaoHandle = -1;
        
        private float[] vertices;
        
        private List<int> cachedLocations = new List<int>();

        public VertexBufferObjectWithVAO(bool isStatic, int numVerticies, VertexAttributes attributes)
        {
            this.isStatic = isStatic;
            this.attributes = attributes;
            this.vertices = new float[numVerticies];

            bufferHandle = GL.GenBuffer();
            usage = isStatic ? BufferUsageHint.StaticDraw : BufferUsageHint.DynamicDraw;

            createVAO();
        }

        public VertexAttributes getAttributes () {
            return attributes;
        }
        
        public int getNumMaxVertices () 
        {
            return vertices.Length / attributes.vertexSize;
        }
        
        private void bufferChanged () {
            if (isBound) {
                GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(vertices.Length * sizeof(float)), vertices, usage);
                isDirty = false;
            }
        }
        
        public void setVertices (float[] vertices) {
            isDirty = true;
            this.vertices = vertices;
            bufferChanged();
        }
        
        public void setVertices (float[] vertices, int offset, int count) {
            isDirty = true;

            this.vertices = vertices;
            //for (int i = offset; i < offset + count; i++)
            //{
            //    var ri = i / attributes.vertexSize;
            //    var v = vertices[i];
            //    this.vertices[ri] = v;
            //}
            

            //for (int i = 0; i < count; i++)
            //{
            //    var v = vertices[i];
            //    this.vertices[offset + i] = v;
            //}
            
            //for (int i = offset; i < offset + count; i++)
            //{
            //    this.vertices[i] = vertices[i];
            //}
            
            bufferChanged();
        }
        
        public void bind (ShaderProgram shader, int[] locations = null) {
            GL.BindVertexArray(vaoHandle);

            bindAttributes(shader, locations);

            //if our data has changed upload it:
            bindData();

            isBound = true;
        }
        
        private void bindAttributes (ShaderProgram shader, int[] locations) {
            var stillValid = this.cachedLocations.Count != 0;
            var numAttributes = attributes.size();

            if (stillValid) {
                if (locations == null) {
                    for (int i = 0; stillValid && i < numAttributes; i++) {
                        VertexAttribute attribute = attributes[i];
                        int location = shader.getAttributeLocation(attribute.alias);
                        stillValid = location == this.cachedLocations[i];
                    }
                } else {
                    stillValid = locations.Length == this.cachedLocations.Count;
                    for (int i = 0; stillValid && i < numAttributes; i++) {
                        stillValid = locations[i] == this.cachedLocations[i];
                    }
                }
            }

            if (!stillValid) {
                GL.BindBuffer(BufferTarget.ArrayBuffer, bufferHandle);
                unbindAttributes(shader);
                this.cachedLocations.Clear();

                for (int i = 0; i < numAttributes; i++) {
                    VertexAttribute attribute = attributes[i];
                    if (locations == null) {
                        this.cachedLocations.Add(shader.getAttributeLocation(attribute.alias));
                    } else {
                        this.cachedLocations.Add(locations[i]);
                    }

                    int location = this.cachedLocations[i];
                    if (location < 0) {
                        continue;
                    }

                    shader.enableVertexAttribute(location);
                    shader.setVertexAttribute(location, attribute.numComponents, attribute.type, attribute.normalized, attributes.vertexSize, attribute.offset);
                }
            }
        }

        private void unbindAttributes (ShaderProgram shaderProgram) {
            if (cachedLocations.Count == 0) {
                return;
            }
            int numAttributes = attributes.size();
            for (int i = 0; i < numAttributes; i++) {
                int location = cachedLocations[i];
                if (location < 0) {
                    continue;
                }
                shaderProgram.disableVertexAttribute(location);
            }
        }

        private void bindData ()
        {
            if (isDirty) {
                GL.BindBuffer(BufferTarget.ArrayBuffer, bufferHandle);
                GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(vertices.Length * sizeof(float)), vertices, usage);
                isDirty = false;
            }
        }

        public void unbind (ShaderProgram shader, int[] locations = null) 
        {
            GL.BindVertexArray(0);
            isBound = false;
        }
        
        public void invalidate () {
            bufferHandle = GL.GenBuffer();
            createVAO();
            isDirty = true;
        }

        public void dispose()
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.DeleteBuffer(bufferHandle);
            bufferHandle = 0;
            deleteVAO();
        }
        
        private void createVAO()
        {
            vaoHandle = GL.GenVertexArray();
        }

        private void deleteVAO()
        {
            if (vaoHandle != -1)
            {
                GL.DeleteVertexArray(vaoHandle);
                vaoHandle = -1;
            }
        }

        public float[] getBuffer()
        {
            return vertices;
        }
    }
}