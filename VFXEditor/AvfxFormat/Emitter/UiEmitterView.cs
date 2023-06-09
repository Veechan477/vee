using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VfxEditor.AvfxFormat {
    public class UiEmitterView : UiNodeDropdownView<AvfxEmitter> {
        public UiEmitterView( AvfxFile file, UiNodeGroup<AvfxEmitter> group ) : base( file, group, "Emitter", true, true, "default_emitter.vfxedit" ) { }

        public override void OnSelect( AvfxEmitter item ) { }

        public override AvfxEmitter Read( BinaryReader reader, int size ) {
            var item = new AvfxEmitter( File.NodeGroupSet );
            item.Read( reader, size );
            return item;
        }
    }
}