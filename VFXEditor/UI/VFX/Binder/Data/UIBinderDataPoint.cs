using AVFXLib.Models;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VFXEditor.UI.Vfx
{
    public class UIBinderDataPoint : UIData {
        public AVFXBinderDataPoint Data;

        public UIBinderDataPoint(AVFXBinderDataPoint data)
        {
            Data = data;
            //==================
            Tabs.Add(new UICurve(data.SpringStrength, "Spring Strength"));
        }
    }
}
