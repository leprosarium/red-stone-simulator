using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using NUnit.Framework;
using LibNbt.Tags;
using LibNbt;
using System.IO;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System.Drawing.Imaging;


namespace Mincraft_Simulator
{
    public partial class Form1 : Form
    {
        bool paintIt = false;
        redstoneBmp redBmp;
        redstoneObj[] rGrid;
        int rXmax;
        int rYmax;
        int rZmax;
        int currentZ = 0;
        string rMaterials;
        byte[] rBlocks;
        byte[] rData;
        bool loaded = false;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //NbtFile test = new NbtFile("C:\\Users\\Paul Bruner\\Desktop\\Ol Drive\\alu-new-sixteen-bits.schematic",true);
           // test.LoadFile();

            redBmp = new redstoneBmp();

        }

        private void AssertNbtBigFile(NbtFile file)
        {
           // NbtList enities;
            //NbtList tileenities;
            
            // See TestFiles/bigtest.nbt.txt to see the expected format
  

            NbtCompound root = file.RootTag;
            if (root.Name == "Schematic")
            {
                redBmp = new redstoneBmp();
                // I don't know why but I keep getting cofused with the height, width, length stuff
                // maybe its because I deal with to many 2d objects.
                rYmax = root.Query<NbtShort>("/Schematic/Width").Value;
                rZmax = root.Query<NbtShort>("/Schematic/Height").Value;
                rXmax = root.Query<NbtShort>("/Schematic/Length").Value;
                rMaterials = root.Query<NbtString>("/Schematic/Materials").Value;
                rBlocks = root.Query<NbtByteArray>("/Schematic/Blocks").Value;
                rData = root.Query<NbtByteArray>("/Schematic/Data").Value;
                rGrid = new redstoneObj[rBlocks.Length]; 

                // Ok, lets get this link list a starting
                for (int i = 0; i < rBlocks.Length; i++)
                {
                    // Put the normal code here to fill the data for the blocks, and data
                    // skipping for now as its more important to get the link list working
                }

                // Just so I can get a good visual on it going to precaculate the numbers
                int xStride = rXmax;
                int zStride = xStride * rYmax;
                
                // Precaculate the last line in a floor and the last floor
                int zLast = rData.Length - zStride;
                int yLast = zStride - xStride;

                // Lets try something diffrent and do all bounds checking

                for (int i = 0; i < rBlocks.Length; i++)
                {
                    
                    int z =  i/zStride;
                    int y = (i - z * zStride)/xStride;
                    int x = i - (y * xStride) - (z * zStride);
                    rGrid[i] = new redstoneObj(redBmp.getSet(rBlocks[i]));
                    rGrid[i].X = x; rGrid[i].Y = y; rGrid[i].Z = z;
                }
            }

        }


        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
          
            openFileDialog.Filter = "Schematic Files .schematic | *.schematic";
            openFileDialog.Title = "Select a Schematic";
            openFileDialog.InitialDirectory = System.Environment.GetFolderPath(Environment.SpecialFolder.Personal);

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                NbtFile test = new NbtFile(openFileDialog.FileName, true);
                test.LoadFile();
                AssertNbtBigFile(test);
                paintIt = true;
                this.Width = rXmax * 20;
                this.Height = rYmax * 20;
                this.Refresh();

                Console.WriteLine("CLICK!");
            }
            
        }

        private void upToolStripMenuItem_Click(object sender, EventArgs e)
        {
            currentZ += 1;
            if (currentZ > rZmax) currentZ = rZmax;
            this.Refresh();
        }

        private void downToolStripMenuItem_Click(object sender, EventArgs e)
        {
            currentZ -= 1;
            if (currentZ < 0) currentZ = 0;
            this.Refresh();
        }

        private Bitmap CreateGrid(int zLevel)
        {
            // Just so I can get a good visual on it going to precaculate the numbers
            int xStride = rXmax;
            int zStride = xStride * rYmax;

            int bmpWidth = rXmax * 20; // With of the square + 2 for the lin left and rigg
            int bmpLength = rYmax * 20;
            Bitmap buffer = new Bitmap(bmpWidth, bmpLength, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            Graphics g = Graphics.FromImage(buffer);

            g.Clear(Color.White);

            for (int i = 0; i < rBlocks.Length; i++)
            {
                redstoneObj temp = rGrid[i];
                if (temp.Z == zLevel)
                {
                    g.DrawImage(temp.getBitmap(rData[i]), temp.X * 20, temp.Y * 20);

                    int nZ = zLevel + 1 > rZmax ? rZmax : zLevel + 1;
              //      if (rGrid[i + zStride].bType == blockType.RedstoneWire)
                //        g.DrawImage(rGrid[i + zStride].getBitmap(rData[i + zStride]), temp.X * 20, temp.Y * 20);
                    g.DrawRectangle(Pens.Black, temp.X * 20, temp.Y * 20, 20, 20);
                }

            }
            return buffer;
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            int yoffset = 50;
            if (paintIt)
            {
                
               
                e.Graphics.DrawImage(CreateGrid(currentZ), 0, 0);

            }
        }

        private void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 'w')
            {
                currentZ += 1;
                if (currentZ > rZmax) currentZ = rZmax;
                this.Refresh();
            }
            if (e.KeyChar == 's')
            {
                currentZ -= 1;
                if (currentZ < 0) currentZ = 0;
                this.Refresh();
            }
        }

        private void glControl_Load(object sender, EventArgs e)
        {
            int w = glControl.Width;
            int h = glControl.Height;
            loaded = true;
            GL.ClearColor(Color.SkyBlue);
            // SetupViewport();
            ResizeScreen(w, h);
            cubeText= LoadTexture("Cube.bmp");
            buildList();

            
            GL.Enable(EnableCap.Texture2D);
            GL.ShadeModel(ShadingModel.Smooth);
            GL.ClearColor(Color.Black);
            GL.Enable(EnableCap.DepthTest);
            GL.DepthFunc(DepthFunction.Lequal);
            GL.Enable(EnableCap.Light0);								// Quick And Dirty Lighting (Assumes Light0 Is Set Up)
            GL.Enable(EnableCap.Lighting);								// Enable Lighting
            GL.Enable(EnableCap.ColorMaterial);							// Enable Material Coloring
            GL.Hint(HintTarget.PerspectiveCorrectionHint, HintMode.Nicest);
            glControl.Refresh();
        }
        private void ResizeScreen(int w, int h)
        {
            GL.Viewport(0, 0, w, h); // Use all of the glControl painting area
            GL.MatrixMode(MatrixMode.Projection);
            //GL.PushMatrix();
            //OpenTK.Graphics.OpenGL.
            GL.LoadIdentity();
            GL.Ortho(-w/64, w/64, -h/64, h/64, -1, 100); // Bottom-left corner pixel has coordinate (0, 0)
            //GL.Viewport(0, 0, w, h); // Use all of the glControl painting area

            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();

        }
        private void DrawTutorial()
        {
            int w = glControl.Width;
            int h = glControl.Height;
            int xloop;
            int yloop;
            
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.BindTexture(TextureTarget.Texture2D, cubeText);
            for (yloop = 1; yloop < 6; yloop++)							// Loop Through The Y Plane
            {
                for (xloop = 0; xloop < yloop; xloop++)					// Loop Through The X Plane
                {
                    GL.LoadIdentity();                                  // Reset The View
                    
                    // Position The Cubes On The Screen
			        GL.Translate(1.4f+(xloop*2.8f)-(yloop)*1.4f,((6.0f-yloop)*2.4f)-7.0f,-20.0f);
                    GL.Rotate(45.0f-(2.0f*yloop)+xrot,1.0f,0.0f,0.0f);		// Tilt The Cubes Up And Down
			        GL.Rotate(45.0f+yrot,0.0f,1.0f,0.0f);				// Spin Cubes Left And Right
                    GL.Color3(boxcol[yloop - 1]);
                    GL.CallList(box);
                    GL.Color3(topcol[yloop - 1]);					// Select The Top Color
                    GL.CallList(top);
                }
            }
        }
        
        float[][]  boxcol = new float[5][] {
            new float[3] {1.0f,0.0f,0.0f },
            new float[3] {1.0f,0.5f,0.0f },
            new float[3] {1.0f,1.0f,0.0f },
            new float[3] {0.0f,1.0f,0.0f },
            new float[3] {0.0f,1.0f,1.0f },    
        };
      
           float[][]  topcol = new float[5][] {
            new float[3] {.5f,0.0f,0.0f },
            new float[3] {0.5f,0.25f,0.0f },
            new float[3] {0.5f,0.5f,0.0f },
            new float[3] {0.0f,0.5f,0.0f},
            new float[3] {0.0f,0.5f,0.5f },    
           };

        int box;
        int top;
        
        float xrot=0;
        float yrot=0;
        int cubeText;
        private void buildList()
        {
 
            box = GL.GenLists(2);
            GL.NewList(box, ListMode.Compile);
            GL.Begin(BeginMode.Quads);							// Start Drawing Quads
            // Bottom Face
                GL.TexCoord2(1.0f, 1.0f); GL.Vertex3(-1.0f, -1.0f, -1.0f);	// Top Right Of The Texture and Quad
                GL.TexCoord2(0.0f, 1.0f); GL.Vertex3(1.0f, -1.0f, -1.0f);	// Top Left Of The Texture and Quad
                GL.TexCoord2(0.0f, 0.0f); GL.Vertex3(1.0f, -1.0f, 1.0f);	// Bottom Left Of The Texture and Quad
                GL.TexCoord2(1.0f, 0.0f); GL.Vertex3(-1.0f, -1.0f, 1.0f);	// Bottom Right Of The Texture and Quad
            // Front Face
                GL.TexCoord2(0.0f, 0.0f); GL.Vertex3(-1.0f, -1.0f, 1.0f);	// Bottom Left Of The Texture and Quad
                GL.TexCoord2(1.0f, 0.0f); GL.Vertex3(1.0f, -1.0f, 1.0f);	// Bottom Right Of The Texture and Quad
                GL.TexCoord2(1.0f, 1.0f); GL.Vertex3(1.0f, 1.0f, 1.0f);	// Top Right Of The Texture and Quad
                GL.TexCoord2(0.0f, 1.0f); GL.Vertex3(-1.0f, 1.0f, 1.0f);	// Top Left Of The Texture and Quad
            // Back Face
                GL.TexCoord2(1.0f, 0.0f); GL.Vertex3(-1.0f, -1.0f, -1.0f);	// Bottom Right Of The Texture and Quad
                GL.TexCoord2(1.0f, 1.0f); GL.Vertex3(-1.0f, 1.0f, -1.0f);	// Top Right Of The Texture and Quad
                GL.TexCoord2(0.0f, 1.0f); GL.Vertex3(1.0f, 1.0f, -1.0f);	// Top Left Of The Texture and Quad
                GL.TexCoord2(0.0f, 0.0f); GL.Vertex3(1.0f, -1.0f, -1.0f);	// Bottom Left Of The Texture and Quad
            // Right face
                GL.TexCoord2(1.0f, 0.0f); GL.Vertex3(1.0f, -1.0f, -1.0f);	// Bottom Right Of The Texture and Quad
                GL.TexCoord2(1.0f, 1.0f); GL.Vertex3(1.0f, 1.0f, -1.0f);	// Top Right Of The Texture and Quad
                GL.TexCoord2(0.0f, 1.0f); GL.Vertex3(1.0f, 1.0f, 1.0f);	// Top Left Of The Texture and Quad
                GL.TexCoord2(0.0f, 0.0f); GL.Vertex3(1.0f, -1.0f, 1.0f);	// Bottom Left Of The Texture and Quad
            // Left Face
                GL.TexCoord2(0.0f, 0.0f); GL.Vertex3(-1.0f, -1.0f, -1.0f);	// Bottom Left Of The Texture and Quad
                GL.TexCoord2(1.0f, 0.0f); GL.Vertex3(-1.0f, -1.0f, 1.0f);	// Bottom Right Of The Texture and Quad
                GL.TexCoord2(1.0f, 1.0f); GL.Vertex3(-1.0f, 1.0f, 1.0f);	// Top Right Of The Texture and Quad
                GL.TexCoord2(0.0f, 1.0f); GL.Vertex3(-1.0f, 1.0f, -1.0f);	// Top Left Of The Texture and Quad
            GL.End();
            GL.EndList();

            top=box+1;
            GL.NewList(top, ListMode.Compile);
            GL.Begin(BeginMode.Quads);							// Start Drawing Quad
			// Top Face
			    GL.TexCoord2(0.0f, 1.0f); GL.Vertex3(-1.0f,  1.0f, -1.0f);	// Top Left Of The Texture and Quad
			    GL.TexCoord2(0.0f, 0.0f); GL.Vertex3(-1.0f,  1.0f,  1.0f);	// Bottom Left Of The Texture and Quad
			    GL.TexCoord2(1.0f, 0.0f); GL.Vertex3( 1.0f,  1.0f,  1.0f);	// Bottom Right Of The Texture and Quad
			    GL.TexCoord2(1.0f, 1.0f); GL.Vertex3( 1.0f,  1.0f, -1.0f);	// Top Right Of The Texture and Quad
		    GL.End();
            GL.EndList();
        }
        private void glControl_Paint(object sender, PaintEventArgs e)
        {
            if (!loaded) // Play nice
                return;

            /*
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();
            GL.Color3(Color.Yellow);
            GL.Begin(BeginMode.Triangles);
                GL.Vertex2(10, 20);
                GL.Vertex2(100, 20);
                GL.Vertex2(100, 50);
            GL.End();
            */
            DrawTutorial();
 
            glControl.SwapBuffers();
        }
        private int LoadTexture(string filename)
        {
            if (String.IsNullOrEmpty(filename))
                throw new ArgumentException(filename);
 
            int id = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, id);
 
            //Bitmap bmp = new Bitmap(filename);
            Bitmap bmp = CreateBoxTexture();
            BitmapData bmp_data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
 
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, bmp_data.Width, bmp_data.Height, 0,
            OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, bmp_data.Scan0);
 
            bmp.UnlockBits(bmp_data);
 
            // We haven't uploaded mipmaps, so disable mipmapping (otherwise the texture will not appear).
            // On newer video cards, we can use GL.GenerateMipmaps() or GL.Ext.GenerateMipmaps() to create
            // mipmaps automatically. In that case, use TextureMinFilter.LinearMipmapLinear to enable them.
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
 
            return id;
        }


        private Bitmap CreateBoxTexture()
        {
            Bitmap tmp = new Bitmap(100, 100, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            Graphics g = Graphics.FromImage(tmp);
            g.Clear(Color.Yellow);
            g.DrawRectangle(new Pen(Brushes.Black, 10f), 0, 0, 100, 100);
            g.Dispose();
            return tmp;
        }
        private void SetupViewport()
        {
            int w = glControl.Width;
            int h = glControl.Height;
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            GL.Ortho(0, w, 0, h, -1, 1); // Bottom-left corner pixel has coordinate (0, 0)
            GL.Viewport(0, 0, w, h); // Use all of the glControl painting area
        }
        private void TutorialInit()
        {

        }

        private void glControl_KeyPress(object sender, KeyPressEventArgs e)
        {
        
        }
        bool tempKey = false;
        private void glControl_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void glControl_KeyUp(object sender, KeyEventArgs e)
        {
            if (Keys.Up == e.KeyCode)
            {
                xrot -= 0.2f;
            }

            if (Keys.Left == e.KeyCode)							// Left Arrow Being Pressed?
            {
                yrot -= 0.2f;							// If So Spin Cubes Left
            }
            if (Keys.Right == e.KeyCode)							// Right Arrow Being Pressed?
            {
                yrot += 0.2f;							// If So Spin Cubes Right
            }
            if (Keys.Down == e.KeyCode)							// Down Arrow Being Pressed?
            {
                xrot += 0.2f;							// If So Tilt Cubes Down
            }
            glControl.Refresh();
        }
    }
}
