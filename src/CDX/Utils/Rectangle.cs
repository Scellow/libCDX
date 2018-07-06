namespace CDX.Utils
{
    public struct Rectangle
    {
        public float x,     y;
        public float width, height;
        
        public Rectangle (float x, float y, float width, float height) {
            this.x      = x;
            this.y      = y;
            this.width  = width;
            this.height = height;
        }
    }
}