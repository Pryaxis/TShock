#define TILE_BITPACK
using System;
using System.Runtime.InteropServices;

namespace Terraria
{
    public class Tile
    {
        public bool active
        {
            get
            {
                return this.Tiles.Datas[this.X, this.Y].active;
            }
            set
            {
                this.Tiles.Datas[this.X, this.Y].active = value;
            }
        }
        public bool checkingLiquid
        {
            get
            {
                return this.Tiles.Datas[this.X, this.Y].checkingLiquid;
            }
            set
            {
                this.Tiles.Datas[this.X, this.Y].checkingLiquid = value;
            }
        }
        public byte frameNumber
        {
            get
            {
                return this.Tiles.Datas[this.X, this.Y].frameNumber;
            }
            set
            {
                this.Tiles.Datas[this.X, this.Y].frameNumber = value;
            }
        }
        public short frameX
        {
            get
            {
                return this.Tiles.Datas[this.X, this.Y].frameX;
            }
            set
            {
                this.Tiles.Datas[this.X, this.Y].frameX = value;
            }
        }
        public short frameY
        {
            get
            {
                return this.Tiles.Datas[this.X, this.Y].frameY;
            }
            set
            {
                this.Tiles.Datas[this.X, this.Y].frameY = value;
            }
        }
        public bool lava
        {
            get
            {
                return this.Tiles.Datas[this.X, this.Y].lava;
            }
            set
            {
                this.Tiles.Datas[this.X, this.Y].lava = value;
            }
        }
        public byte liquid
        {
            get
            {
                return this.Tiles.Datas[this.X, this.Y].liquid;
            }
            set
            {
                this.Tiles.Datas[this.X, this.Y].liquid = value;
            }
        }
        public byte type
        {
            get
            {
                return this.Tiles.Datas[this.X, this.Y].type;
            }
            set
            {
                this.Tiles.Datas[this.X, this.Y].type = value;
            }
        }
        public byte wall
        {
            get
            {
                return this.Tiles.Datas[this.X, this.Y].wall;
            }
            set
            {
                this.Tiles.Datas[this.X, this.Y].wall = value;
            }
        }
        public bool skipLiquid
        {
            get
            {
                return this.Tiles.Datas[this.X, this.Y].skipLiquid;
            }
            set
            {
                this.Tiles.Datas[this.X, this.Y].skipLiquid = value;
            }
        }
        public bool lighted
        {
            get
            {
                return this.Tiles.Datas[this.X, this.Y].lighted;
            }
            set
            {
                this.Tiles.Datas[this.X, this.Y].lighted = value;
            }
        }
        public bool wire
        {
            get { return this.Tiles.Datas[this.X, this.Y].wire; }
            set { this.Tiles.Datas[this.X, this.Y].wire = value; }
        }
        public byte wallFrameNumber
        {
            get { return this.Tiles.Datas[this.X, this.Y].wallFrameNumber; }
            set { this.Tiles.Datas[this.X, this.Y].wallFrameNumber = value; }
        }
        public byte wallFrameX
        {
            get { return this.Tiles.Datas[this.X, this.Y].wallFrameX; }
            set { this.Tiles.Datas[this.X, this.Y].wallFrameX = value; }
        }
        public byte wallFrameY
        {
            get { return this.Tiles.Datas[this.X, this.Y].wallFrameY; }
            set { this.Tiles.Datas[this.X, this.Y].wallFrameY = value; }
        }
        private readonly TileCollection Tiles;
        private readonly int X;
        private readonly int Y;
        public TileData Data
        {
            get
            {
                return this.Tiles.Datas[this.X, this.Y];
            }
            set
            {
                this.Tiles.Datas[this.X, this.Y] = value;
            }
        }
        public Tile(TileCollection tiles, int x, int y)
        {
            this.Tiles = tiles;
            this.X = x;
            this.Y = y;
        }
        public object Clone()
        {
            return base.MemberwiseClone();
        }
        public bool isTheSameAs(Tile compTile)
        {
            if (this.active != compTile.active)
            {
                return false;
            }
            if (this.active)
            {
                if (this.type != compTile.type)
                {
                    return false;
                }
                if (Main.tileFrameImportant[(int)this.type])
                {
                    if (this.frameX != compTile.frameX)
                    {
                        return false;
                    }
                    if (this.frameY != compTile.frameY)
                    {
                        return false;
                    }
                }
            }
            return this.wall == compTile.wall && this.liquid == compTile.liquid && this.lava == compTile.lava && this.wire == compTile.wire;
        }
    }

    public class TileCollection
    {
        internal TileData[,] Datas;
        public Tile this[int x, int y]
        {
            get
            {
                return new Tile(this, x, y);
            }
        }
        internal void SetSize(int x, int y)
        {
            this.Datas = new TileData[x, y];
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct TileData
    {
        public byte liquid;
        public byte type;
        public byte wall;
        public byte wallFrameNumber;
        public byte wallFrameX;
        public byte wallFrameY;
        public short frameX;
        public short frameY;

#if TILE_BITPACK
        public bool active
        {
            get { return (byte) (this.Flags & TileFlag.Active) != 0; }
            set { this.SetFlag(TileFlag.Active, value); }
        }

        public bool checkingLiquid
        {
            get { return (byte) (this.Flags & TileFlag.CheckingLiquid) != 0; }
            set { this.SetFlag(TileFlag.CheckingLiquid, value); }
        }

        public byte frameNumber
        {
            get { return (byte) (this.Flags & (TileFlag) 3); }
            set { this.Flags = ((this.Flags & (TileFlag) 252) | (TileFlag) value); }
        }

        //Perhaps this is causing statue/mannequin issues.
        /*
        public short frameX
        {
            get
            {
                int num = this.frame >> 8;
                return (short) ((num != 255) ? ((short) (num << 1)) : -1);
            }
            set { this.frame = (ushort) (value >> 1 << 8 | (int) (this.frame & 255)); }
        }

        public short frameY
        {
            get
            {
                int num = (int) (this.frame & 255);
                return (short) ((num != 255) ? ((short) (num << 1)) : -1);
            }
            set { this.frame = (ushort) (value >> 1 | (int) (this.frame & 65280)); }
        }*/

        public bool lava
        {
            get { return (byte) (this.Flags & TileFlag.Lava) != 0; }
            set { this.SetFlag(TileFlag.Lava, value); }
        }

        public bool lighted
        {
            get { return (byte) (this.Flags & TileFlag.Lighted) != 0; }
            set { this.SetFlag(TileFlag.Lighted, value); }
        }

        public bool skipLiquid
        {
            get { return (byte) (this.Flags & TileFlag.SkipLiquid) != 0; }
            set { this.SetFlag(TileFlag.SkipLiquid, value); }
        }

        public bool wire
        {
            get { return (byte) (this.Flags & TileFlag.Wire) != 0; }
            set { this.SetFlag(TileFlag.Wire, value); }
        }

        private void SetFlag(TileFlag flag, bool set)
        {
            if (set)
            {
                this.Flags |= flag;
                return;
            }
            this.Flags &= ~flag;
        }

        private TileFlag Flags;
        //private ushort frame;
#else

        public bool active;
        public bool checkingLiquid;
        public byte frameNumber;
        public short frameX;
        public short frameY;
        public bool lava;
        public bool lighted;
        public bool skipLiquid;
        public bool wire;
#endif
    }

    public enum TileFlag : byte
    {
        Unknown,
        Reserved1,
        Wire = 4,
        Active = 8,
        SkipLiquid = 16,
        Lighted = 32,
        CheckingLiquid = 64,
        Lava = 128
    }
}
/*using System;
namespace Terraria
{
	public class Tile
	{
		public bool active;
		public byte type;
		public byte wall;
		public byte wallFrameX;
		public byte wallFrameY;
		public byte wallFrameNumber;
		public bool wire;
		public byte liquid;
		public bool checkingLiquid;
		public bool skipLiquid;
		public bool lava;
		public byte frameNumber;
		public short frameX;
		public short frameY;
		public object Clone()
		{
			return base.MemberwiseClone();
		}
		public bool isTheSameAs(Tile compTile)
		{
			if (this.active != compTile.active)
			{
				return false;
			}
			if (this.active)
			{
				if (this.type != compTile.type)
				{
					return false;
				}
				if (Main.tileFrameImportant[(int)this.type])
				{
					if (this.frameX != compTile.frameX)
					{
						return false;
					}
					if (this.frameY != compTile.frameY)
					{
						return false;
					}
				}
			}
			return this.wall == compTile.wall && this.liquid == compTile.liquid && this.lava == compTile.lava && this.wire == compTile.wire;
		}
	}
}*/
