using ImGuiNET;
using System.Numerics;

namespace VfxEditor.Ui {
    public partial class ToolsDialog : GenericDialog {
        private readonly ToolsDialogResourceTab ResourceTab;
        private readonly ToolsDialogUtilitiesTab UtilitiesTab;

        public ToolsDialog() : base( "Tools", false, 300, 400 ) {
            ResourceTab = new ToolsDialogResourceTab();
            UtilitiesTab = new ToolsDialogUtilitiesTab();
        }

        public override void DrawBody() {
            if( ImGui.BeginTabBar( "##ToolsTabs", ImGuiTabBarFlags.NoCloseWithMiddleMouseButton ) ) {
                if( ImGui.BeginTabItem( "Resources##ToolsTabs" ) ) {
                    ResourceTab.Draw();
                    ImGui.EndTabItem();
                }
                if( ImGui.BeginTabItem( "Utilities##ToolsTabs" ) ) {
                    UtilitiesTab.Draw();
                    ImGui.EndTabItem();
                }
                ImGui.EndTabBar();
            }
        }
    }
}