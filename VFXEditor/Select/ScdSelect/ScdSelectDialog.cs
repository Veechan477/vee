using ImGuiNET;
using System;
using System.Collections.Generic;

namespace VfxEditor.Select.ScdSelect {
    public class ScdSelectDialog : SelectDialog {
        private readonly List<SelectTab> GameTabs;

        public ScdSelectDialog(
                string id,
                List<SelectResult> recentList,
                bool showLocal,
                Action<SelectResult> onSelect
            ) : base( id, "scd", recentList, showLocal, onSelect ) {

            GameTabs = new List<SelectTab>( new SelectTab[]{
                new ScdMountSelect( "Mount", this ),
                new ScdOrchestrionSelect( "Orchestrion", this ),
                new ScdZoneSelect( "Zone", this ),
                new ScdBgmSelect( "BGM", this ),
                new ScdBgmQuestSelect( "Quest BGM", this )
            } );
        }

        protected override List<SelectTab> GetTabs() => GameTabs;
    }
}
