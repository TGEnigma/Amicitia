﻿using System.Drawing;
using System.IO;
using System.Numerics;
using AmicitiaLibrary.Utilities;

namespace AmicitiaLibrary.Graphics.RenderWare
{
    public class RwAtomicSectorHeader : RwNode
    {
        public int MaterialIdBase { get; set; }

        public int TriangleCount { get; set; }

        public int VertexCount { get; set; }

        public Vector3 Min { get; set; }

        public Vector3 Max { get; set; }

        public int Unused1 { get; set; }

        public int Unused2 { get; set; }

        public Vector3[] Positions { get; set; }

        public Vector3[] Normals { get; set; }

        public Color[] Colors { get; set; }

        public Vector2[][] TextureCoordinateChannels { get; set; }

        public RwTriangle[] Triangles { get; set; }

        public RwAtomicSectorHeader( RwNode parent ) : base( RwNodeId.RwStructNode, parent )
        {
        }

        internal RwAtomicSectorHeader( RwNodeFactory.RwNodeHeader header, BinaryReader reader ) : base( header )
        {
            MaterialIdBase = reader.ReadInt32();
            TriangleCount = reader.ReadInt32();
            VertexCount = reader.ReadInt32();
            Min = new Vector3( reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle() );
            Max = new Vector3( reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle() );
            Unused1 = reader.ReadInt32();
            Unused2 = reader.ReadInt32();

            var world = FindParent< RwWorld >( RwNodeId.RwWorldNode );
            if ( world == null )
                throw new System.Exception();

            var format = world.Header.Format;

            Positions = reader.ReadVector3Array( VertexCount );

            if ( format.HasFlag( RwWorldFormatFlags.Normals ) )
            {
                Normals = new Vector3[VertexCount];
                for ( int i = 0; i < VertexCount; i++ )
                {
                    Normals[i] = new Vector3( reader.ReadSByte() / 128f, reader.ReadSByte() / 128f, reader.ReadSByte() / 128f );
                    reader.BaseStream.Position += 1; // padding
                }
            }

            if ( format.HasFlag( RwWorldFormatFlags.Prelit ) )
            {
                Colors = reader.ReadColorArray( VertexCount );
            }

            if ( format.HasFlag( RwWorldFormatFlags.Textured ) )
            {
                TextureCoordinateChannels = new Vector2[1][];
                TextureCoordinateChannels[ 0 ] = reader.ReadVector2Array( VertexCount );
            }
            else if ( format.HasFlag( RwWorldFormatFlags.Textured2 ) )
            {
                TextureCoordinateChannels = new Vector2[2][];
                TextureCoordinateChannels[0] = reader.ReadVector2Array( VertexCount );
                TextureCoordinateChannels[1] = reader.ReadVector2Array( VertexCount );
            }

            Triangles = new RwTriangle[TriangleCount];
            for ( int i = 0; i < Triangles.Length; i++ )
            {
                var v1 = reader.ReadUInt16();
                var v2 = reader.ReadUInt16();
                var v3 = reader.ReadUInt16();
                var materialId = reader.ReadInt16();

                Triangles[ i ] = new RwTriangle( v1, v2, v3, materialId );
            }
        }

        protected internal override void WriteBody( BinaryWriter writer )
        {
            writer.Write( MaterialIdBase );
            writer.Write( TriangleCount );
            writer.Write( VertexCount );
            writer.Write( Min );
            writer.Write( Max );
            writer.Write( Unused1 );
            writer.Write( Unused2 );
            writer.Write( Positions );

            if ( Normals != null )
            {
                foreach ( var normal in Normals )
                {
                    writer.Write( ( sbyte ) ( normal.X * 128f ) );
                    writer.Write( ( sbyte ) ( normal.Y * 128f ) );
                    writer.Write( ( sbyte ) ( normal.Z * 128f ) );
                    writer.Write( ( byte ) 0 );
                }
            }

            if ( Colors != null )
                writer.Write( Colors );

            if ( TextureCoordinateChannels != null )
            {
                foreach ( var channel in TextureCoordinateChannels )
                {
                    writer.Write( channel );
                }
            }

            foreach ( var triangle in Triangles )
            {
                writer.Write( triangle.A );
                writer.Write( triangle.B );
                writer.Write( triangle.C );
                writer.Write( triangle.MatId );
            }
        }
    }
}