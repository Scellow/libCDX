using System;
using System.Collections.Generic;
using CDX.Graphics.GLUtils;
using CDX.Utils;
using OpenTK.Graphics.OpenGL4;

namespace CDX.Graphics.Scene2D.Utils
{
    public class ScissorStack
    {
        private static List<Rectangle> scissors = new List<Rectangle>();
        
        public static bool pushScissors (Rectangle scissor) {
            fix(scissor);

            if (scissors.Count == 0) {
                if (scissor.width < 1 || scissor.height < 1) return false;
                GL.Enable(EnableCap.ScissorTest);
            } else {
                // merge scissors
                Rectangle parent = scissors[scissors.Count - 1];
                float     minX   = Math.Max(parent.x, scissor.x);
                float     maxX   = Math.Min(parent.x + parent.width, scissor.x + scissor.width);
                if (maxX - minX < 1) return false;

                float minY = Math.Max(parent.y, scissor.y);
                float maxY = Math.Min(parent.y + parent.height, scissor.y + scissor.height);
                if (maxY - minY < 1) return false;

                scissor.x      = minX;
                scissor.y      = minY;
                scissor.width  = maxX - minX;
                scissor.height = Math.Max(1, maxY - minY);
            }
            scissors.Add(scissor);
            HdpiUtils.glScissor((int)scissor.x, (int)scissor.y, (int)scissor.width, (int)scissor.height);
            return true;
        }
        
        public static Rectangle popScissors ()
        {
            var li = scissors.Count - 1;
            
            Rectangle old = scissors[li];
            scissors.RemoveAt(li);
            
            if (scissors.Count == 0)
                GL.Disable(EnableCap.ScissorTest);
            else {
                Rectangle scissor = scissors[scissors.Count - 1];
                HdpiUtils.glScissor((int)scissor.x, (int)scissor.y, (int)scissor.width, (int)scissor.height);
            }
            return old;
        }
        private static void fix (Rectangle rect) {
            rect.x      = (float) Math.Round(rect.x);
            rect.y      = (float) Math.Round(rect.y);
            rect.width  = (float) Math.Round(rect.width);
            rect.height = (float) Math.Round(rect.height);
            if (rect.width < 0) {
                rect.width =  -rect.width;
                rect.x     -= rect.width;
            }
            if (rect.height < 0) {
                rect.height =  -rect.height;
                rect.y      -= rect.height;
            }
        }
    }
}