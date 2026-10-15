using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using static R2API.DamageAPI;

namespace Dancer
{
    public static class Content
    {
        public static class Bodies
        {
            public static GameObject DancerBody => Dancer.Modules.Survivors.Dancer.characterPrefab;
        }

        public static class DamageTypes
        {
            public static ModdedDamageType Ribbon;
        }
    }
}
