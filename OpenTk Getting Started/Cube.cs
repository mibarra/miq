#region --- License ---
/* Copyright (c) 2006, 2007 Stefanos Apostolopoulos
 * See license.txt for license info
 */
#endregion

using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

using System.Runtime.InteropServices;
using OpenTK;

namespace Examples.Shapes
{

    public static class Utilities
    {
        public static int ColorToRgba32(Color c)
        {
            return ((c.A << 24) | (c.B << 16) | (c.G << 8) | c.R);
        }
    }

    public abstract class Shape
    {
        private Vector3[] vertices, normals;
        private Vector2[] texcoords;
        private int[] indices;
        private int[] colors;

        public Vector3[] Vertices
        {
            get { return vertices; }
            protected set
            {
                vertices = value;
            }
        }

        public Vector3[] Normals
        {
            get { return normals; }
            protected set
            {
                normals = value;
            }
        }

        public Vector2[] Texcoords
        {
            get { return texcoords; }
            protected set
            {
                texcoords = value;
            }
        }

        public int[] Indices
        {
            get { return indices; }
            protected set
            {
                indices = value;
            }
        }

        public int[] Colors
        {
            get { return colors; }
            protected set
            {
                colors = value;
            }
        }
    }

    public class Cube : Shape
    {
        public Cube()
        {
            Vertices = new Vector3[]
            {
                new Vector3(-1.0f, -1.0f,  1.0f),
                new Vector3( 1.0f, -1.0f,  1.0f),
                new Vector3( 1.0f,  1.0f,  1.0f),
                new Vector3(-1.0f,  1.0f,  1.0f),
                new Vector3(-1.0f, -1.0f, -1.0f),
                new Vector3( 1.0f, -1.0f, -1.0f), 
                new Vector3( 1.0f,  1.0f, -1.0f),
                new Vector3(-1.0f,  1.0f, -1.0f)
            };

            Indices = new int[]
            {
                // front face
                0, 1, 2, 2, 3, 0,
                // top face
                3, 2, 6, 6, 7, 3,
                // back face
                7, 6, 5, 5, 4, 7,
                // left face
                4, 0, 3, 3, 7, 4,
                // bottom face
                0, 1, 5, 5, 4, 0,
                // right face
                1, 5, 6, 6, 2, 1,
            };

            Normals = new Vector3[]
            {
                new Vector3(-1.0f, -1.0f,  1.0f),
                new Vector3( 1.0f, -1.0f,  1.0f),
                new Vector3( 1.0f,  1.0f,  1.0f),
                new Vector3(-1.0f,  1.0f,  1.0f),
                new Vector3(-1.0f, -1.0f, -1.0f),
                new Vector3( 1.0f, -1.0f, -1.0f),
                new Vector3( 1.0f,  1.0f, -1.0f),
                new Vector3(-1.0f,  1.0f, -1.0f),
            };

            Colors = new int[]
            {
                Utilities.ColorToRgba32(Color.DarkRed),
                Utilities.ColorToRgba32(Color.DarkRed),
                Utilities.ColorToRgba32(Color.Gold),
                Utilities.ColorToRgba32(Color.Gold),
                Utilities.ColorToRgba32(Color.DarkRed),
                Utilities.ColorToRgba32(Color.DarkRed),
                Utilities.ColorToRgba32(Color.Gold),
                Utilities.ColorToRgba32(Color.Gold),
            };

        //    Texcoords = new Vector2[]
        //    {
        //        new Vector2(0, 1),
        //        new Vector2(1, 1),
        //        new Vector2(1, 0),
        //        new Vector2(0, 0),
        //        new Vector2(0, 1),
        //        new Vector2(1, 1),
        //        new Vector2(1, 0),
        //        new Vector2(0, 0),
        //    };
        }
    }
}