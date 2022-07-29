using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
#if UNITY_2020_2_OR_NEWER
using UnityEditor.AssetImporters;
#else
using UnityEditor.Experimental.AssetImporters;
#endif
using UnityEditor.ProjectWindowCallback;
using UnityEngine;

namespace PrefabChildReference
{
    //If importQueueOffset is less than 500, even though reimport would be caused by the changes in a prefab, you would still get old prefab version thus outdated data
    [ScriptedImporter(1, extension, 1000)]
    public class PrefabChildReferenceImporter : ScriptedImporter
    {
        private const string extension = "cprefab";
        private const string extensionWithDot = "." + extension;

        public Reference[] references = Array.Empty<Reference>();
        public bool useObjectAsRoot;

        [MenuItem("Assets/Create/Prefab Child Reference")]
        public static void CreateCPrefab()
        {
            var path = AssetDatabase.GetAssetPath(Selection.activeObject);
            if (path == "")
            {
                path = "Assets";
            }
            else if (Path.GetExtension(path) != "")
            {
                path = path.Replace(Path.GetFileName(AssetDatabase.GetAssetPath(Selection.activeObject)), "");
            }
            var assetPathAndName = AssetDatabase.GenerateUniqueAssetPath($"{path}/NewPrefabChildReference{extensionWithDot}");

            var endAction = ScriptableObject.CreateInstance<CreateEmptyAsset>();
            ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0, endAction, assetPathAndName, null, null);
        }

        [OnOpenAsset]
        public static bool PreventCPrefabDoubleClick(int instanceID, int line)
        {
            return Path.GetExtension(AssetDatabase.GetAssetPath(EditorUtility.InstanceIDToObject(instanceID))).ToLower() == extensionWithDot;
        }

        public override void OnImportAsset(AssetImportContext ctx)
        {
            var fileName = Path.GetFileNameWithoutExtension(ctx.assetPath);

            var prefabInstances = new Dictionary<GameObject, GameObject>();
            var refInstances = new List<GameObject>();
            foreach (var reference in references)
            {
                var refObj = reference.referencedObject;
                if (!refObj)
                {
                    Debug.LogWarning($"Empty reference in {fileName}{extensionWithDot}. Skipping");
                    continue;
                }

                var refRoot = refObj.transform.root.gameObject;
                if (!prefabInstances.TryGetValue(refRoot, out var rootInstance))
                {
                    prefabInstances[refRoot] = rootInstance = Instantiate(refRoot);
                    rootInstance.name = refRoot.name;
                }

                var refPath = Utils.GetPathFromRoot(refObj.transform);
                var refInstance = rootInstance;
                foreach (var i in refPath)
                {
                    refInstance = refInstance.transform.GetChild(i).gameObject;
                }
                if (!string.IsNullOrWhiteSpace(reference.nameOverride))
                {
                    refInstance.name = reference.nameOverride;
                }
                refInstances.Add(refInstance);
            }

            GameObject root;
            if (refInstances.Count == 1 && useObjectAsRoot)
            {
                root = refInstances[0];
                root.transform.SetParent(null);
            }
            else
            {
                root = new GameObject(fileName);
                foreach (var refInstance in refInstances)
                {
                    refInstance.transform.SetParent(root.transform);
                }
            }
            ctx.AddObjectToAsset(fileName, root);

            foreach (var row in prefabInstances)
            {
                var referenceAssetPath = AssetDatabase.GetAssetPath(row.Key);
                ctx.DependsOnSourceAsset(referenceAssetPath);
                if (!refInstances.Contains(row.Value))
                {
                    DestroyImmediate(row.Value);
                }
            }
        }

        private class CreateEmptyAsset : EndNameEditAction
        {
            public override void Action(int instanceId, string pathName, string resourceFile)
            {
                File.WriteAllText(pathName, "");
                AssetDatabase.ImportAsset(pathName);
                Selection.activeObject = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(pathName);
                CleanUp();
            }
        }
    }
}
