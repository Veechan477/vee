using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Numerics;
using VfxEditor.Parsing;

namespace VfxEditor.AvfxFormat {
    public class UiFloat3 : IAvfxUiBase {
        public readonly UiParsedFloat3 Parsed;
        private readonly List<AvfxBase> Literals;

        public UiFloat3( string name, AvfxFloat l1, AvfxFloat l2, AvfxFloat l3 ) {
            Literals = new() { l1, l2, l3 };
            Parsed = new( name, l1.Parsed, l2.Parsed, l3.Parsed );
        }

        public void Draw( string id ) {
            // Unassigned
            AvfxBase.AssignedCopyPaste( Literals[0], $"{Parsed.Name}_1" );
            AvfxBase.AssignedCopyPaste( Literals[1], $"{Parsed.Name}_2" );
            AvfxBase.AssignedCopyPaste( Literals[2], $"{Parsed.Name}_3" );
            if( AvfxBase.DrawAddButton( Literals, Parsed.Name, id ) ) return;

            Parsed.Draw( id, CommandManager.Avfx );

            AvfxBase.DrawRemoveContextMenu( Literals, Parsed.Name, id );
        }
    }
}
