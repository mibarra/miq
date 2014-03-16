using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Miq.Games
{
    class GameEntry
    {
        [STAThread]
        static void Main()
        {
            using (GameOfLifeInterface game = new GameOfLifeInterface(200, 50))
            {
                game.Run();
            }
        }
    }

    abstract class Simple2DGame : GameWindow
    {
        internal Simple2DGame(int width, int height, string title) : base(width, height,  OpenTK.Graphics.GraphicsMode.Default, title)
        {
            VSync = VSyncMode.On;
        }

        abstract protected void Init();
        abstract protected void Update(double elapsedSeconds);
        abstract protected void Render(double elapsedSeconds);

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            GL.ClearColor(1f, 1f, 1f, 0.0f);
            GL.Enable(EnableCap.DepthTest);

            Init();
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);
            Update(e.Time);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            Render(e.Time);
            SwapBuffers();
        }

        struct PerspectiveProjectionParameters
        {
            public readonly float FieldOfViewY;
            public readonly float AspectRatio;
            public readonly float NearZ;
            public readonly float FarZ;

            public PerspectiveProjectionParameters(float fieldOfViewY, float aspectRatio, float nearZ, float farZ)
            {
                FieldOfViewY = fieldOfViewY;
                AspectRatio = aspectRatio;
                NearZ = nearZ;
                FarZ = farZ;
            }

            public PerspectiveProjectionParameters(float aspectRatio)
            {
                FieldOfViewY = (float)Math.PI / 4.0f;
                AspectRatio = aspectRatio;
                NearZ = 1.0f;
                FarZ = 64.0f;
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            GL.Viewport(ClientRectangle);
            SetPerspectiveProjection(new PerspectiveProjectionParameters((float)Width / (float)Height));
        }

        private void SetPerspectiveProjection(PerspectiveProjectionParameters parms)
        {
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            GL.Ortho(0.0, Width, 0.0, Height, -100f, 100f);
        }

        protected void Rectangle(float x, float y, float width, float height)
        {
            GL.Color3(0, 0, 0);
            GL.Begin(BeginMode.Quads);

            GL.Vertex3(x, y, 0);
            GL.Vertex3(x, y + height, 0);
            GL.Vertex3(x + width, y + height, 0);
            GL.Vertex3(x + width, y, 0);

            GL.End();
        }
    }

    public class CumulativeMovingAverage
    {
        public CumulativeMovingAverage()
        {
            Average = -1;
            Changed = true;
        }

        public long Average { get; private set; }
        public long Samples { get; private set; }
        public bool Changed { get; private set; }

        public void Add(long n)
        {
            Samples++;
            long next = Average + (n - Average) / Samples;
            if (next != Average)
            {
                Average = next;
                Changed = true;
            }
            else
            {
                Changed = false;
            }
        }
    }

    class GameOfLifeInterface : Simple2DGame
    {
        Stopwatch stopwatch = new Stopwatch();
        CumulativeMovingAverage average = new CumulativeMovingAverage();
        private GameOfLife Board;
        
        public GameOfLifeInterface(int width, int height) : base(width, height, "Game of Life")
        {
            Board = new GameOfLife(Width, Height);
        }

        protected override void Init()
        {
            Board.Randomize();
            
            GL.EnableClientState(ArrayCap.VertexArray);
            GL.DisableClientState(ArrayCap.TextureCoordArray);

            GL.GenBuffers(1, out VBOid);
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBOid);

            float DotWidth = (float)Width / Board.Width;
            float DotHeight = (float)Height / Board.Height;

            Vertices = new Vector3[(Width + 1) * (Height + 1)];
            for (int i = 0; i <= Width; i++)
            {
                for (int j = 0; j <= Height; j++)
                {
                    Vertices[j * (Width+1) + i] = new Vector3(i * DotWidth, j * DotHeight, 0);
                }
            }
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(Vertices.Length * Vector3.SizeInBytes), Vertices, BufferUsageHint.StaticDraw);

            print();
        }

        private void print()
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBOid);

            //Print contents
            IntPtr ptr = GL.MapBuffer(BufferTarget.ArrayBuffer, BufferAccess.ReadOnly);
            unsafe
            {
                float* f = (float*)ptr;
                for (int i = 0; i < Vertices.Length; i++)
                {
                    Console.WriteLine("{0}: ({1}, {2}) ", i, f[i * 3], f[i * 3 + 1]);
                }
            }
            GL.UnmapBuffer(BufferTarget.ArrayBuffer);
        }

        public Vector3[] Vertices;
        int VBOid;

        protected override void Update(double elapsedSeconds)
        {
            stopwatch.Restart();
            Board.Update();
            stopwatch.Stop();
            average.Add(stopwatch.ElapsedMilliseconds);
            if (average.Changed)
            {
                Console.WriteLine(average.Average);
            }


            if (Keyboard[Key.Escape])
                Exit();

            if (Keyboard[Key.Space])
                Board.Randomize();
        }

        protected override void Render(Double elapsedSeconds)
        {
            List<uint> Indices = new List<uint>();

            for (int x = 0; x < Board.Width; x++)
                for (int y = 0; y < Board.Height; y++)
                    if (Board[x, y])
                    {
                        uint index = (uint)x + (uint)y * ((uint)Width + 1);
                        Indices.Add(index);
                        Indices.Add(index + 1);
                        Indices.Add(index + 1 + ((uint)Board.Width + 1));
                        Indices.Add(index     + ((uint)Board.Width + 1));
                    }

            GL.Color3(0, 0, 0);

            IndicesArray = Indices.ToArray();
            GL.DrawElements(BeginMode.Quads, IndicesArray.Length, DrawElementsType.UnsignedInt, IndicesArray);
        }

        public uint[] IndicesArray;
    }

    public class GameOfLife
    {
        public GameOfLife(int width, int height)
        {
            Width = width;
            Height = height;
            Board = new Cell[width, height];
            Random = new Random();
        }

        public bool this[int x, int y]
        {
            get
            {
                return Board[x, y].state;
            }
        }

        public void Update()
        {
            var temp = (Cell[,])Board.Clone();
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    bool alive = temp[x, y].state;
                    int neighboors = temp[x, y].neighbors;
                    
                    if (alive)
                    {
                        if (neighboors != 2 && neighboors != 3)
                        {
                            ClearCell(x, y);
                        }
                    }
                    else
                    {
                        if (neighboors == 3)
	                    {
                            SetCell(x, y);		 
	                    }
                    }
                }
            }
        }

        int WrapWidth(int x)
        {
            if (x < 0) return x + Width;
            if (x >= Width) return x - Width;
            return x;
        }

        int WrapHeight(int y)
        {
            if (y < 0) return y + Height;
            if (y >= Height)  return y - Height;
            return y;
        }

        public void Randomize()
        {
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    if (GetRandomCellState())
                    {
                        SetCell(x, y);
                    }
                }
            }
        }

        public int Width { get; private set; }
        public int Height { get; private set; }

        #region Private

        struct Cell
        {
            public bool state;
            public byte neighbors;
        }

        Cell[,] Board { get; set; }
        Random Random { get; set; }

        private bool GetRandomCellState()
        {
            return Random.NextDouble() < 0.05;
        }

        private void SetCell(int x, int y)
        {
            Board[x, y].state = true;

            int left = WrapWidth(x - 1);
            int rigth = WrapWidth(x + 1);
            int above = WrapHeight(y - 1);
            int below = WrapHeight(y + 1);

            Board[left, above].neighbors++;
            Board[x, above].neighbors++;
            Board[rigth, above].neighbors++;
            Board[rigth, y].neighbors++;
            Board[rigth, below].neighbors++;
            Board[x, below].neighbors++;
            Board[left, below].neighbors++;
            Board[left, y].neighbors++;
        }

        private void ClearCell(int x, int y)
        {
            Board[x, y].state = false;

            int left = WrapWidth(x - 1);
            int rigth = WrapWidth(x + 1);
            int above = WrapHeight(y - 1);
            int below = WrapHeight(y + 1);

            Board[left, above].neighbors--;
            Board[x, above].neighbors--;
            Board[rigth, above].neighbors--;
            Board[rigth, y].neighbors--;
            Board[rigth, below].neighbors--;
            Board[x, below].neighbors--;
            Board[left, below].neighbors--;
            Board[left, y].neighbors--;
        }

        #endregion
    }
}