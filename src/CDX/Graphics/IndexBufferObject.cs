using System;
using System.IO;
using OpenTK.Graphics.OpenGL4;

namespace CDX.Graphics
{
    public class IndexBufferObject
    {
        private int bufferHandle;
        private bool isDirect;
        private bool isDirty = true;
        private bool isBound = false;
        private BufferUsageHint usage;
        private uint[] indicies;
        
        public IndexBufferObject(bool isStatic, int maxIndices)
        {
            this.indicies = new uint[maxIndices];
            
            this.isDirect = true;

            usage = isStatic ? BufferUsageHint.StaticDraw : BufferUsageHint.DynamicDraw;
            
            bufferHandle = GL.GenBuffer();
        }
        
        public int getNumMaxVertices ()
        {
            return indicies.Length;
        }
        
        public void setIndices (uint[] indices) {
            isDirty = true;
            this.indicies = indices;

            // todo: double check
            if (isBound) {
                GL.BufferData(
                    BufferTarget.ElementArrayBuffer,
                    (IntPtr)(indicies.Length * sizeof(uint)),
                    indicies, usage);
                isDirty = false;
            }
        }
        
        public void bind () {
            if (bufferHandle == 0) throw new Exception("IndexBufferObject cannot be used after it has been disposed.");

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, bufferHandle);
            if (isDirty) 
            {
                GL.BufferData(
                    BufferTarget.ElementArrayBuffer,
                    (IntPtr)(indicies.Length * sizeof(uint)),
                    indicies, usage);
                isDirty = false;
            }
            isBound = true;
        }
        
        public void unbind () {
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
            isBound = false;
        }
        
        public void invalidate () {
            bufferHandle = GL.GenBuffer();
            isDirty = true;
        }
        
        public void dispose () {
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
            GL.DeleteBuffer(bufferHandle);
            bufferHandle = 0;
        }

        public uint[] getIndices()
        {
            return indicies;
        }

        public uint[] getBuffer()
        {
            return indicies;
        }
    }
}