using ImGuiNET;
using System;
using System.Collections.Generic;

namespace VfxEditor.ScdFormat.Music {
    public class ScdAudioEntrySplitView : ScdSimpleSplitView<ScdAudioEntry> {
        public ScdAudioEntrySplitView( List<ScdAudioEntry> items ) : base( "Audio", items, false ) { }

        protected override void DrawLeftItem( ScdAudioEntry item, int idx, string id ) {
            if( item.DataLength == 0 ) return;
            if( ImGui.Selectable( $"{ItemName} {idx}{id}{idx}", item == Selected ) ) Selected = item;
        }
    }
}
