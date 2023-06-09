using ImGuiNET;
using VfxEditor.FileManager;
using VfxEditor.NodeLibrary;
using VfxEditor.Select.Vfx;
using VfxEditor.Utils;

namespace VfxEditor.AvfxFormat {
    public class AvfxManager : FileManagerWindow<AvfxDocument, AvfxFile, WorkspaceMetaAvfx> {
        public readonly AvfxNodeLibrary NodeLibrary;

        public AvfxManager() : base( "VFXEditor", "Vfx", "avfx", "Docs", "VFX" ) {
            SourceSelect = new VfxSelectDialog( "File Select [LOADED]", this, true );
            ReplaceSelect = new VfxSelectDialog( "File Select [REPLACED]", this, false );
            NodeLibrary = new( Plugin.Configuration.VFXNodeLibraryItems, Plugin.Configuration.WriteLocation );
        }

        protected override AvfxDocument GetNewDocument() => new( this, LocalPath );

        protected override AvfxDocument GetWorkspaceDocument( WorkspaceMetaAvfx data, string localPath ) => 
            new( this, LocalPath, WorkspaceUtils.ResolveWorkspacePath( data.RelativeLocation, localPath ), data );

        protected override void DrawMenu() {
            if( CurrentFile == null ) return;

            if( ImGui.BeginMenu( "Edit##Menu" ) ) {
                GetCopyManager().Draw();
                GetCommandManager().Draw();

                if( ImGui.BeginMenu( "Templates##Menu" ) ) {
                    if( ImGui.MenuItem( "Blank##Menu" ) ) ActiveDocument?.OpenTemplate( @"default_vfx.avfx" );
                    if( ImGui.MenuItem( "Weapon##Menu" ) ) ActiveDocument?.OpenTemplate( @"default_weapon.avfx" );
                    ImGui.EndMenu();
                }

                if( ImGui.MenuItem( "Clean up##Menu" ) ) CurrentFile.Cleanup();

                ImGui.EndMenu();
            }
        }

        public override void DrawBody() {
            NodeLibrary.Draw();
            base.DrawBody();
        }

        public void Import( string path ) => ActiveDocument.Import( path );

        public void ShowExportDialog( AvfxNode node ) => ActiveDocument.ShowExportDialog( node );
    }
}
