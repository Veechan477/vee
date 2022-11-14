using System;
using System.Collections.Generic;
using System.IO;
using VfxEditor.Parsing;

namespace VfxEditor.AvfxFormat2 {
    public class AvfxEmitVertexes : AvfxBase {
        public readonly List<AvfxEmitVertex> EmitVertexes = new();

        public AvfxEmitVertexes() : base( "VEmt" ) { }

        public override void ReadContents( BinaryReader reader, int size ) {
            for( var i = 0; i < size / 28; i++ ) EmitVertexes.Add( new AvfxEmitVertex( reader ) );
        }

        protected override void RecurseChildrenAssigned( bool assigned ) { }

        protected override void WriteContents( BinaryWriter writer ) {
            foreach( var vert in EmitVertexes ) vert.Write( writer );
        }
    }

    public class AvfxEmitVertex {
        public readonly ParsedFloat3 Position = new( "Position" );
        public readonly ParsedFloat3 Normal = new( "Normal" );
        public readonly ParsedIntColor Color = new( "Color" );

        public AvfxEmitVertex() { }

        public AvfxEmitVertex( BinaryReader reader ) {
            Position.Read( reader, 0 );
            Normal.Read( reader, 0 );
            Color.Read( reader, 0 );
        }

        public void Write( BinaryWriter writer ) {
            Position.Write( writer );
            Normal.Write( writer );
            Color.Write( writer );
        }
    }
}