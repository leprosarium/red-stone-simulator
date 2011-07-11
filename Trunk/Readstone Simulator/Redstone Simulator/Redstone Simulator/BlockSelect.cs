﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Imaging;

namespace Redstone_Simulator
{
    public partial class BlockSelect : UserControl
    {
        const int ConstScale = 10;
        Bitmap bar;
        int selected = 0; public int Selected { get { return selected; } }
        public Block SelectedBlock { get { return sArray[selected][0]; } }
        float scale = 5;
        public float BlockScale { get { return scale; } set { scale = value; } }
        Block[][] sArray = PickBlocks;
      
        public BlockSelect()
        {
            makeBar();      
            this.DoubleBuffered = true;
            InitializeComponent();
        }
       
        void makeBar()
        {
            bar = new Bitmap((int)((sArray.Length * 10) * scale), (int)(scale * 10));
            Graphics g = Graphics.FromImage(bar);
            
            g.Clear(BlockColors.cGrid);
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            g.ScaleTransform(scale, scale);
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            g.FillRectangle(BlockColors.bHilite, (selected * 9), 0, 10, 10);
 
            for (int i = 0; i < sArray.Length; i++)
            {
                
                Rectangle r = new Rectangle(i * 9 + 1, 1, 8, 8);
                BlockDrawSettings b;
                int j = 0;
                if (sArray[i][j].isBlock)
                {
                    j++;
                    b = new BlockDrawSettings(sArray[i][j],  true);
                    b.OnBlock = true;
                }
                else
                    b = new BlockDrawSettings(sArray[i][j],  true);

                if (sArray[i][j+1].isBlock) b.Fog = true;
                BlockImages.gDrawBlock(g, r, b);
            }
            g.Dispose();
            this.MinimumSize = bar.Size;
            this.MaximumSize = bar.Size;
        }
        private void BlockSelect_Load(object sender, EventArgs e)
        {
            
        }

        public  void moveSelect(int select)
        {
            selected += select;
            if (selected  > sArray.Length-1)
                selected = sArray.Length-1;
            if(selected < 0)
                selected = 0;
            makeBar();
            this.Refresh();
            this.Invalidate();
        }

        public static Block[][] PickBlocks
        {
            get
            {
                Block[][] tmp = new Block[][] 
                 {
                    new Block[] { Block.AIR,Block.AIR,Block.AIR },
                                new Block[]  { Block.BLOCK,Block.AIR,Block.AIR },
                                new Block[]  { Block.BLOCK,Block.BLOCK,Block.AIR },
                                new Block[]  { Block.WIRE,Block.AIR,Block.AIR },
                                new Block[]  { Block.TORCH,Block.AIR,Block.AIR },
                                new Block[]  { Block.BLOCK,Block.WIRE,Block.AIR },
                               new Block[]   { Block.BLOCK,Block.TORCH,Block.AIR },
                               new Block[]   { Block.WIRE,Block.BLOCK,Block.AIR },
                                new Block[]  { Block.TORCH,Block.BLOCK,Block.AIR },
                                new Block[]  { Block.WIRE,Block.TORCH,Block.AIR },
                                new Block[]  { Block.WIRE,Block.BLOCK,Block.WIRE },
                               new Block[]   { Block.LEVER,Block.AIR,Block.AIR },
                                new Block[]  { Block.BUTTON,Block.AIR,Block.AIR },
                                new Block[]  { Block.PREASUREPAD,Block.AIR,Block.AIR },
                               // new Blocks[]  { Blocks.DOORA,Blocks.AIR,Blocks.AIR }  ,
                                 new Block[]  { Block.REPEATER,Block.AIR,Block.AIR }
                 };
                for (int i = 0; i < tmp.Length; i++)
                    for (int j = 0; j < tmp[i].Length; j++)
                    {
                        if (tmp[i][j].isControl) tmp[i][j].Charge = 0;
                        else
                            tmp[i][j].Charge = 16;
                        tmp[i][j].Place = Direction.NORTH;


                    }

                return tmp;
            }
        }

        private void BlockSelect_Paint(object sender, PaintEventArgs e)
        {
      
            Graphics g = e.Graphics;
            g.DrawImage(bar, 0, 0);
        }
       

        private void BlockSelect_MouseClick(object sender, MouseEventArgs e)
        {
            switch (e.Button)
            {
                case System.Windows.Forms.MouseButtons.Left:
                   // int pX = (e.X-center) / (int)scale;
                    int pX = (e.X) / (int)scale;
                    if (pX < 0) return;
                    if (pX % 9 == 0) return;
                    pX /= 9;
                    if (pX >= sArray.Length) return;
                    else
                    {
                        selected = pX;
                        makeBar();
                        this.Refresh();
                    }
                    break;

            }
        }

        private void BlockSelect_MouseEnter(object sender, EventArgs e)
        {
           if (!this.Focused && this.Visible) this.Focus();
        }

      
    }
}
