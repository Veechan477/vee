using ImGuiNET;
using Lumina.Excel.GeneratedSheets;
using System;
using System.Linq;
using VfxEditor.Select.Shared;

namespace VfxEditor.Select.Vfx.Housing {
    public class HousingTab : SelectTab<HousingRow, ParseAvfxFromFile> {
        public HousingTab( SelectDialog dialog, string name ) : base( dialog, name, "Vfx-Housing" ) { }

        // ===== LOADING =====

        public override void LoadData() {
            var indoorSheet = Plugin.DataManager.GetExcelSheet<HousingFurniture>().Where( x => x.ModelKey > 0 );
            foreach( var item in indoorSheet ) Items.Add( new HousingRow( item ) );

            var outdoorSheet = Plugin.DataManager.GetExcelSheet<HousingYardObject>().Where( x => x.ModelKey > 0 );
            foreach( var item in outdoorSheet ) Items.Add( new HousingRow( item ) );
        }

        public override void LoadSelection( HousingRow item, out ParseAvfxFromFile loaded ) => ParseAvfxFromFile.ReadFile( item.SgbPath, out loaded );

        // ===== DRAWING ======

        protected override void OnSelect() => LoadIcon( Selected.Icon );

        protected override void DrawSelected( string parentId ) {
            SelectTabUtils.DrawIcon( Icon );
            ImGui.Text( "SGB:" );
            ImGui.SameLine();
            SelectTabUtils.DisplayPath( Selected.SgbPath );

            Dialog.DrawPath( "VFX", Loaded.VfxPaths, parentId, SelectResultType.GameItem, Selected.Name, true );
        }

        protected override string GetName( HousingRow item ) => item.Name;
    }
}