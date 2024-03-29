using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using NUnit.Framework;
using LibNbt.Tags;
using LibNbt;
using System.IO;
using NUnit.Framework;
using LibNbt.Tags;
using LibNbt;
using System.IO; 

namespace MonoRedstoneSim
{
	 // This class creates the tiny images used for pictures. I guess I could make this a super class and then inherit them all
    // ..  Sigh.  Hard to go from C to obj
    // Nono, this must be its own class.  I want one class to hold the images needed, this way we don't have thousands 
    // of single yellow squares .
    // Change the functions here on how the images look.  They are rotated in redstoneObj so
    // be sure each of these are facing north
    class redstoneBmp
    {
        const int blockMax = 256; // can only be 256 types of blocks
        const int tileHeight = 20;
        const int tileWidth = 20;
        Bitmap[][] img=null;

        // I'll make this vector drawing some day, right now its hardwired to 20,20.
       // public void redstoneBmp()
       // {
       //     makeImages();
        // }
        public redstoneBmp()
        {
            makeImages();
        }
        

        void debugSaveImages(string prefix, Bitmap[] iBmp)
        {
            for (int i = 0; i < iBmp.Length; i++)
            {
                iBmp[i].Save(prefix + "-" + i.ToString() + ".png", ImageFormat.Png);
            }

        }
        void makeImages()
        {            
            //Create graphics contucts and other values
            Bitmap tmpBmp = new Bitmap(tileWidth, tileHeight, PixelFormat.Format32bppArgb);
            RectangleF centerCircle = new RectangleF(
                (tileWidth / 2) - (tileWidth / 4),
                (tileHeight / 2) - (tileHeight / 4),
                (tileWidth / 4)*2,
                (tileHeight / 4)*2);
            RectangleF centerLine = new RectangleF(
                (tileWidth / 2) - (tileWidth / 8),
                (tileHeight / 2) - (tileHeight / 8),
                (tileWidth / 8)*2,
                tileHeight);

            Bitmap t;
            
            Bitmap[] nb = null;
            Graphics g;

            // Yea yea, using a temp var as global.  byte me.
            // Sigh, so is it better to pass Graphics and Bitmap to functions
            // or just make a global scope for it?  meh.  Only running all of these
            // once
            g = Graphics.FromImage(tmpBmp);
            g.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceOver;

            // Lets make the nice Yellow Block and lets fill all the blocks with it in the first space.
            // I am assuming that all the blocks put out by MCsim is all dirt blocks unless specified.
            // Otherwise, its going to be annoying to illerate though all of them, next version:P
            g.FillRectangle(Brushes.Yellow, 0, 0, tmpBmp.Width, tmpBmp.Width);
            for(int idx = 0; idx < blockMax; idx++)
                if(Enum.IsDefined(typeof(blockType),(byte)idx))
                {
                    blockType cBlockType = (blockType)Enum.Parse(typeof(blockType),idx.ToString());
                    switch(cBlockType)
                    {
                        case blockType.StoneButton:
                            nb = new Bitmap[4];
                            //North,West,South,East
                            for(int i = 0; i<4;i++)
                            {
                                g.Clear(Color.White);
                                g.FillRectangle(Brushes.Gray, 4, 14, 14, 19);
                                g.RotateTransform(i * 90);
                                nb[i] = new Bitmap(tmpBmp);
                            }
                            //Why the hell did notch do it like this?
                            //South,North,West,East
                            t=nb[0]; nb[0] = nb[2]; nb[2] = nb[1]; nb[1] = t;
                            img[idx] = nb;
                            
                            break;
                        case blockType.RedstoneTorchOn:
                            //North,East,South,West
                            nb = new Bitmap[5];
                            g.Clear(Color.White);
                            g.FillRectangle(Brushes.Gray, centerLine);
                            g.FillEllipse(Brushes.Red, centerCircle);
                            for(int i = 0; i<4;i++)
                            {
                                nb[i] = new Bitmap(tmpBmp);
                                tmpBmp.RotateFlip(RotateFlipType.Rotate90FlipNone);
                            }
                            // Floor Torch
                            g.Clear(Color.White);
                            g.FillEllipse(Brushes.Red, centerCircle);
                            nb[4] = new Bitmap(tmpBmp);

                            //Why the hell did notch do it like this?
                            //South,North,West,East,Floor
                            t =nb[0]; 
                            nb[0] = nb[2]; 
                            nb[2] = nb[3]; 
                            nb[3] = nb[1];
                            nb[1] = t;
                            img[idx] = nb;
                            debugSaveImages("RedstoneTorchOn", nb);
                            break;
                        case blockType.RedstoneTorchOff:
                            //South,North,West,East,Floor
                            nb = new Bitmap[5];
                            g.Clear(Color.White);
                            g.FillRectangle(Brushes.Gray, centerLine);
                            g.FillEllipse(Brushes.Black, centerCircle);
                            for (int i = 0; i < 4; i++)
                            {
                                nb[i] = new Bitmap(tmpBmp);
                                tmpBmp.RotateFlip(RotateFlipType.Rotate90FlipNone);
                            }
                            // Floor Torch
                            g.Clear(Color.White);
                            g.FillEllipse(Brushes.Black, centerCircle);
                            nb[4] = new Bitmap(tmpBmp);
                             t =nb[0]; 
                            nb[0] = nb[2]; 
                            nb[2] = nb[3]; 
                            nb[3] = nb[1];
                            nb[1] = t;
                            img[idx] = nb;
                            debugSaveImages("RedstoneTorchOff", nb);
                            break;
                        case blockType.RedstoneWire:
                            nb = new Bitmap[1];
                            Pen thickLine = new Pen(Color.Red,5);
                            g.Clear(Color.White);
                            g.DrawLine(thickLine,10,0,10,20);
                            g.DrawLine(thickLine,0,10,20,10);
                            nb[0] = new Bitmap(tmpBmp);
                            img[idx] = nb;
                            nb = null;
                            break;
                        case blockType.Air:
                            nb = new Bitmap[1];
                            g.Clear(Color.White);
                            nb[0] = new Bitmap(tmpBmp);
                            img[idx] = nb;
                            nb = null;
                            break;
                        default:
                            nb = new Bitmap[1];
                            g.Clear(Color.Yellow);
                            nb[0] = new Bitmap(tmpBmp);
                            img[idx] = nb;
                            nb = null;
                            break;
                    }
                }
                else
                {
                    // block not defined, going to make it white with a black X in it
                    Font boxFont = new Font("courier", 10, FontStyle.Regular);
                    StringFormat strCenterBox = new StringFormat();
                    strCenterBox.Alignment = StringAlignment.Center;
                    strCenterBox.LineAlignment = StringAlignment.Center;
                    Bitmap[] b = new Bitmap[1];
                    RectangleF tmpBmpRect = new Rectangle(5, 5, 15, 15);

                    g.Clear(Color.White);
                    g.DrawString(idx.ToString(), SystemFonts.DefaultFont, Brushes.Black, tmpBmpRect);
 
                    b[0] = new Bitmap(tmpBmp);
                    img[idx] = b;
                }
     

        }

        public Bitmap[][] getFullSet
        {
            get { return img; }
        }
        public gBlockTypeStruct getSet(blockType iType)
        {
            return new gBlockTypeStruct(iType,img[(int)iType]);
        }

        // This way we can fine the exact block and will image it, if its out of range it will display the 255 block
        // if it is in range but no defiend it wil display the block with the block number
       
       
    }

   
    // remember [FlagsAttribute]  I learned for masks
    // Only definding blocks that WILL BE USED IN THE PROGRAM.
    // Again, might change, but right now I want to draw something on the damn screen.
    // So only the stones that can have torches and wires attached are going to be defined, except for sand, lava and gravel
    // as they are affected by gravity and thats a whole another nightmare
    enum blockType : byte
    {
        // Blank Air, duh
        Air                 =   0,
        // Blocks you can stick switches and redstone on
        Stone               =   1,
        Grass               =   2,
        Dirt                =   3,
        Cobblestone         =   4,
        WoodenPlank         =   5,
        GoldOre             =   14,
        IronOre             =   15,
        CoalOre             =   16,
        Wood                =   17,
        GoldBrick           =   41,
        IronBrick           =   42,
        BrickBlock          =   45,
        Obsidian            =   49,
        DiamondOre          =   56,
        DiamondBlock        =   57,
        RedstoneOre         =   73,
        ClayBlock           =   82,
        // Things that need to know what direction they are facing
        Torch               =   50,     // Not redstone, but might impment so people can see at night or in a cave.
        RedstoneTorchOff     =  75,
        RedstoneTorchOn    =    76,
        StoneButton         =   77,
        Lever               =   69,
        RedstoneRepeaterOff =   93,
        RedstoneRepeaterOn  =   94,
        // Redstone Wire, have to make bitmaps of evey formation as its determed in game than in grid
        RedstoneWire        =   55,
        // PresurePlate
        StonePressurePlate  =   70,
        WoodenPressurePlate =   72,
        Undefined           =   0xFF    
        
    };
    enum dataWood : byte { Normal,RedWood,Birch };
    enum dataFire : byte { Placed,Eternal = 15 };
    enum dataLeaves : byte { Normal,RedWood,Birch } ;
    enum dataCoal : byte { Coal, Charcoal } ;
    enum dataSaplings : byte { Fresh,NewTree = 0xF };
    // I will put in more, but this is mainly a redstone simulator so I will stop here and
    // just put in the stuff I NEED

    // Same with redstone and normal Torches
    enum dataTorches : byte { South,North,West,East,Floor } ;
    // Mine cart, last 4 is when they go into a circle
    enum dataMineCart : byte { EastWest,NorthSouth,UpSouth,UpNorth,UpEast,UpWest,NorthEast,SouthEast,SouthWest,NorthWest }
    // Side of a block its attached
    enum dataLadders : byte { East=2,West,North,South } ; 
    // Leavers, the power bit is set if the switch is on. Directons is the faceing of the leaver, not the mount
    enum dataLeaver : byte { South=1,North,West,East,PointWestWhenOff,PointSouthWhenOff,Power = 8 }
    // Doors, location of the hinge on the block
    enum dataDoors : byte { NorthEast,SouthEast,SouthWest,NorthWest,CounterClockWise = 4, TopHalf = 8 };
    // Buttons, shouldn't need to worry about the pressed atrubute, just make sure its not set
    enum dataButton : byte { South,North,West,East,Pressed = 8 } ;
    // Humm, be useful to have sign posts, next version
    // Now for the copderestance (bad french:P), the whole reason I got back into minecraft
    // One, two, three and four are the tick wait for it, its the upper two bits
    // the lower two bits show the facing of the thing
    enum dataRedstoneRepeater : byte { East,South,West,North,One=0,Two=4,Three=8,Four=12 };

    
    // This is the basic redstone object that is used to hold the values of a block
    // Eventualy it will also have the simulator invovled, but for right now its 
    // a hollow class till I figure that nightmare out.
	
	struct gBlockStruct
    {
		public gBlockStruct(byte iType, byte data) 
        {
            if (Enum.IsDefined(typeof(blockType), iType))
            {
                blockType b = (blockType)Enum.Parse(typeof(blockType), iType.ToString());
                gBlockStruct(b, data);
            }
            else
                    gBlockStruct(blockType.Undefined, data);
        }
		public gBlockStruct(blockType iType, byte data, int x, int y, int z) { bType = iType; Data = data; X=x; Y=y; Z=z; }
        public gBlockStruct(blockType iType, byte data) { bType = iType;  Data = data; X=0; Y=0; Z=0; }
		public byte Data;
        public blockType bType;
		public int X,Y,Z;
    }
    class redstoneObj
    {
        Bitmap[][] _bmp;
		gBlockStruct[] _blocks;
        blockType _bType;

        // Constructors, MUST have the bmp to get refrences to images.
        public redstoneObj(gBlockTypeStruct bData)
        {
            bmp = bData.bmp;
            bType = bData.bType;
			_block = null;
			
        }

        public Bitmap getBitmap(int data)
        {
            if (bType == blockType.RedstoneTorchOn)
                data += 1;
            if (bType == blockType.RedstoneTorchOff)
                data += 1;
            if (data < bmp.Length)
                return bmp[data];
            else
                return bmp[0];
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

                _block = new gBlockStruct[rBlocks.Length];
				
				int xStride = rXmax;
                int zStride = xStride * rYmax;
				int zLast = rData.Length - zStride;
                int yLast = zStride - xStride;
				
                for (int i = 0; i < rBlocks.Length; i++)
                {
					_blocks[i] = new gBlockStruct(rBlocks[i],rData[i]);
					_blocks[i].Z = i % (rZmax * rYmax);
					_blocks[i].Y = (i % rYmax) + (_blocks[i].Z * rZmax * rYmax);
					_blocks[i].X = 0;
					
					
					
                   int z =  i/zStride;
                    int y = (i - z * zStride)/xStride;
                    int x = i - (y * xStride) - (z * zStride);
                    rGrid[i] = new redstoneObj(redBmp.getSet(rBlocks[i]));
                    rGrid[i].X = x; rGrid[i].Y = y; rGrid[i].Z = z;
                }

                
            }

        }

    }
}

