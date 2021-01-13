using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImGuiNET;

namespace VFXEditor.UI.VFX
{
    public abstract class UIBase
    {
        public bool Assigned = true;
        public List<UIBase> Attributes = new List<UIBase>();

        public virtual void Init() {
            Assigned = true;
            Attributes = new List<UIBase>();
        }

        public abstract void Draw(string parentId);

        public void DrawAttrs(string parentId)
        {
            DrawList(Attributes, parentId);
        }
        public void DrawList(List<UIBase> items, string parentId)
        {
            foreach(var item in items )
            {
                if( item.Assigned )
                {
                    item.Draw( parentId );
                }
            }
            foreach( var item in items )
            {
                if( !item.Assigned )
                {
                    item.Draw( parentId );
                }
            }
        }
    }
}