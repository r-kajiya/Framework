#if UNITY_EDITOR

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;


namespace FrameworkEditor
{
    public static class GenerateBlendTree
    {
        [MenuItem("Assets/Create/BlendTree")]
        static void CreateBlendTree()
        {
            const string fileName = "New Blend Tree.asset";
            string path = EditorHelper.GetCurrentDirectory() + "/" + fileName;

            BlendTree blendTree = new BlendTree();
            
            AssetDatabase.CreateAsset(blendTree, path);
            AssetDatabase.Refresh();
            
            Debug.Log("BlendTreeを作成しました。" + path);
        }
    }
}

#endif

