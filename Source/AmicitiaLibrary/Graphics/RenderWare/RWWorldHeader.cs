using System;
using System.IO;
using System.Numerics;

namespace AmicitiaLibrary.Graphics.RenderWare
{
    public class RwWorldHeader : RwNode
    {
        public int RootIsWorldSector { get; set; }

        public int Ambient { get; set; }

        public int Specular { get; set; }

        public int Diffuse { get; set; }

        public int TriangleCount { get; set; }

        public int VertexCount { get; set; }

        public int PlaneSectorCount { get; set; }

        public int WorldSectorCount { get; set; }

        public int ColSectorSize { get; set; }

        public RwWorldFormatFlags Format { get; set; }

        public Vector3 Min { get; set; }

        public Vector3 Max { get; set; }

        public RwWorldHeader( RwNode parent ) : base( RwNodeId.RwStructNode, parent )
        {
        }

        internal RwWorldHeader( RwNodeFactory.RwNodeHeader header, BinaryReader reader ) : base( header )
        {
            ReadBody( reader );
        }

        protected internal override void ReadBody( BinaryReader reader )
        {
            RootIsWorldSector = reader.ReadInt32();
            Ambient = reader.ReadInt32();
            Specular = reader.ReadInt32();
            Diffuse = reader.ReadInt32();
            TriangleCount = reader.ReadInt32();
            VertexCount = reader.ReadInt32();
            PlaneSectorCount = reader.ReadInt32();
            WorldSectorCount = reader.ReadInt32();
            ColSectorSize = reader.ReadInt32();
            Format = ( RwWorldFormatFlags )reader.ReadInt32();
            Min = new Vector3( reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle() );
            Max = new Vector3( reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle() );
        }

        protected internal override void WriteBody( BinaryWriter writer )
        {
            writer.Write( RootIsWorldSector );
            writer.Write( Ambient );
            writer.Write( Specular );
            writer.Write( Diffuse );
            writer.Write( TriangleCount );
            writer.Write( VertexCount );
            writer.Write( PlaneSectorCount );
            writer.Write( ColSectorSize );
            writer.Write( (int)Format );
            writer.Write( Min.X );
            writer.Write( Min.Y );
            writer.Write( Min.Z );
            writer.Write( Max.X );
            writer.Write( Max.Y );
            writer.Write( Max.Z );
        }
    }

    public enum RwWorldFormatFlags
    {
        TriStrip = 1,
        Positions = 2,
        Textured = 4,
        Prelit = 8,
        Normals = 0x10,
        Light = 0x20,
        ModulateMaterialColor = 0x40,
        Textured2 = 0x80,
        Native = 0x01000000,
        SectorsOverlap = 0x40000000
    }
}