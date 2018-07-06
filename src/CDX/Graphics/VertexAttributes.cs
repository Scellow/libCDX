using System;

namespace CDX.Graphics
{
    public class VertexAttributes
    {
        public static class Usage {
            public static readonly int Position = 1;
            public static readonly int ColorUnpacked = 2;
            public static readonly int ColorPacked = 4;
            public static readonly int Normal = 8;
            public static readonly int TextureCoordinates = 16;
            public static readonly int Generic = 32;
            public static readonly int BoneWeight = 64;
            public static readonly int Tangent = 128;
            public static readonly int BiNormal = 256;
        }
        private readonly VertexAttribute[] attributes;
        public readonly int vertexSize;
        private long mask = -1;
        
        public VertexAttribute this[int index]    // Indexer declaration  
        {
            get => attributes[index];
            set => attributes[index] = value;
        }  
        
        public VertexAttributes (params VertexAttribute[] attributes) {
            if (attributes.Length == 0) throw new Exception("attributes must be >= 1");

            VertexAttribute[] list = new VertexAttribute[attributes.Length];
            for (int i = 0; i < attributes.Length; i++)
                list[i] = attributes[i];

            this.attributes = list;
            vertexSize = calculateOffsets();
        }
        
        private int calculateOffsets () {
            int count = 0;
            for (int i = 0; i < attributes.Length; i++) {
                VertexAttribute attribute = attributes[i];
                attribute.offset = count;
                count += attribute.getSizeInBytes();
            }

            return count;
        }
        
        public int size () {
            return attributes.Length;
        }
       
        public long getMask () {
            if (mask == -1) {
                long result = 0;
                for (int i = 0; i < attributes.Length; i++) {
                    result |= attributes[i].usage;
                }
                mask = result;
            }
            return mask;
        }
        
        public long getMaskWithSizePacked () {
            return getMask() | ((long)attributes.Length << 32);
        }
    }
}