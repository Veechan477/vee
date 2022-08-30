using VFXEditor.FileManager;
using VFXEditor.Select.TMB;

namespace VFXEditor.TmbFormat {
    public partial class TmbManager : FileManager<TmbDocument, WorkspaceMetaTmb, TmbFile> {
        public static TMBSelectDialog SourceSelect { get; private set; }
        public static TMBSelectDialog ReplaceSelect { get; private set; }

        public static void Setup() {
            SourceSelect = new TMBSelectDialog(
                "Tmb Select [SOURCE]",
                VfxEditor.Configuration.RecentSelectsTMB,
                true,
                SetSourceGlobal
            );

            ReplaceSelect = new TMBSelectDialog(
                "Tmb Select [TARGET]",
                VfxEditor.Configuration.RecentSelectsTMB,
                false,
                SetReplaceGlobal
            );
        }

        public static void SetSourceGlobal( SelectResult result ) {
            VfxEditor.TmbManager?.SetSource( result );
            VfxEditor.Configuration.AddRecent( VfxEditor.Configuration.RecentSelectsTMB, result );
        }

        public static void SetReplaceGlobal( SelectResult result ) {
            VfxEditor.TmbManager?.SetReplace( result );
            VfxEditor.Configuration.AddRecent( VfxEditor.Configuration.RecentSelectsTMB, result );
        }

        public static readonly string PenumbraPath = "Tmb";

        public TmbManager() : base( title: "Tmb Editor", id: "Tmb", tempFilePrefix: "TmbTemp", extension: "tmb", penumbaPath: PenumbraPath ) { }

        protected override TmbDocument GetNewDocument() => new( LocalPath );

        protected override TmbDocument GetImportedDocument( string localPath, WorkspaceMetaTmb data ) => new( LocalPath, localPath, data.Source, data.Replace );

        protected override void DrawMenu() { }

        public override void Dispose() {
            base.Dispose();
            SourceSelect.Hide();
            ReplaceSelect.Hide();
        }

        public override void DrawBody() {
            SourceSelect.Draw();
            ReplaceSelect.Draw();
            base.DrawBody();
        }
    }
}
