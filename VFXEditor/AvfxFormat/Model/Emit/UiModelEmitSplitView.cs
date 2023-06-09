using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VfxEditor.AvfxFormat {
    public class UiModelEmitSplitView : UiItemSplitView<UiEmitVertex> {
        public UiModelEmitSplitView( List<UiEmitVertex> items ) : base( items ) { }

        public override void Disable( UiEmitVertex item ) { }

        public override void Enable( UiEmitVertex item ) { }

        public override UiEmitVertex CreateNewAvfx() => new UiEmitVertex( new AvfxEmitVertex(), new AvfxVertexNumber() );
    }
}
