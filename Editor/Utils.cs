using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace PrefabChildReference
{
    public class Utils
    {
        public static List<int> GetPathFromRoot(Transform transform)
        {
            var path = new List<int>();

            BuildPathRecursive(path, transform);
            path.Reverse();
            return path;

            void BuildPathRecursive(List<int> list, Transform child)
            {
                if (!child.parent)
                {
                    return;
                }
                list.Add(child.GetSiblingIndex());
                BuildPathRecursive(list, child.transform.parent);
            }
        }

        public static List<int> GetPath(Transform root, Transform transform)
        {
            var path = new List<int>();

            BuildPathRecursive(path, root, transform);
            path.Reverse();
            return path;

            void BuildPathRecursive(List<int> list, Transform parent, Transform child)
            {
                if (!child.parent || child == parent)
                {
                    return;
                }
                list.Add(child.GetSiblingIndex());
                BuildPathRecursive(list, parent, child.transform.parent);
            }
        }
    }
}