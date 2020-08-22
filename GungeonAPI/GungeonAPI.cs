using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GungeonAPI
{
    public static class GungeonAPI
    {
        public static void Init()
        {
            Tools.Init();
            StaticReferences.Init();
            FakePrefabHooks.Init();
            ShrineFactory.Init();
            DungeonHandler.Init();
        }
    }
}
