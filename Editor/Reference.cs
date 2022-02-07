using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrefabChildReference
{
    [Serializable]
    public class Reference
    {
        [PrefabReference]
        public UnityEngine.GameObject referencedObject;
        public string nameOverride;
    }
}