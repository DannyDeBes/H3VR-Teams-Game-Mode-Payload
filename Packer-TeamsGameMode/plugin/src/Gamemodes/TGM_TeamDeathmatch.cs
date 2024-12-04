using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TeamGameMode
{
    [Serializable]
    public class TGM_TeamDeathmatch : TGM_Gamemode
    {
        public TGM_Area[] teamAreas;

        public override void Setup()
        {
            base.Setup();

        }
    }
}
