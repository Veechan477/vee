using AVFXLib.AVFX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVFXLib.Models
{
    public class AVFXEffectorDataCameraQuake : AVFXEffectorData
    {
        public AVFXCurve Attenuation = new("Att");
        public AVFXCurve RadiusOut = new("RdO");
        public AVFXCurve RadiusIn = new("RdI");
        public AVFXCurve3Axis Rotation = new("Rot");
        public AVFXCurve3Axis Position = new("Pos");
        readonly List<Base> Attributes;

        public AVFXEffectorDataCameraQuake() : base("Data")
        {
            Attributes = new List<Base>(new Base[]{
                Attenuation,
                RadiusOut,
                RadiusIn,
                Rotation,
                Position
            });
        }

        public override void Read(AVFXNode node)
        {
            Assigned = true;
            ReadAVFX(Attributes, node);
        }

        public override void ToDefault()
        {
            Assigned = true;
            SetUnAssigned(Attributes);
        }

        public override AVFXNode ToAVFX()
        {
            var dataAvfx = new AVFXNode("Data");
            PutAVFX(dataAvfx, Attributes);
            return dataAvfx;
        }
    }
}
