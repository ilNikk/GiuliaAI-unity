using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using UniGLTF;
using VRM;
using System.Linq;

using HANA_ClipDatas;

/*!
[2021] [kuniyan]
Please read included license

 */

namespace HanaClipBuilder
{
    public class HANA_Tool_ClipBuilder : EditorWindow
    {
        GameObject baseObj;
        VRMBlendShapeProxy baseBSProxy;
        BlendShapeAvatar baseBSAvator;
        SkinnedMeshRenderer renderer;
        string relativePath;
        
        enum SelectAvatar
        {
            VRoid,
            Other
        }

        SelectAvatar selectAvatar = SelectAvatar.VRoid;
        public static GUIContent[] tabSelectAvatar
        {
            get
            {
                return System.Enum.GetNames(typeof(SelectAvatar)).
                    Select(x => new GUIContent(x)).ToArray();
            }
        }

        enum SelectData
        {
            ARKit,
            OVR
        }

        SelectData selectData = SelectData.ARKit;
        public static GUIContent[] tabSelectData
        {
            get
            {
                return System.Enum.GetNames(typeof(SelectData)).
                    Select(x => new GUIContent(x)).ToArray();
            }
        }

        [MenuItem("HANA_Tool/ClipBuilder", false, 62)]
        static void CreateWindow()
        {
            GetWindow<HANA_Tool_ClipBuilder>("HANA_Tool_ClipBuilder");
        }

        void OnGUI()
        {
            using (var checkScope = new EditorGUI.ChangeCheckScope())
            {
                baseObj = EditorGUILayout.ObjectField("avatar",
                    baseObj, typeof(GameObject), true) as GameObject;

                if (checkScope.changed)
                {
                    if (baseObj == null)
                    {
                        ClearThis();
                    }
                    else
                    {
                        baseBSProxy = baseObj.GetComponent<VRMBlendShapeProxy>();

                        if (baseBSProxy == null)
                        {
                            EditorUtility.DisplayDialog("Error", "No VRMBlendShapeProxy component on GameObject", "OK");
                            ClearThis();
                            return;
                        }
                        else
                        {
                            baseBSAvator = baseBSProxy.BlendShapeAvatar;

                            if(!GetSkinnedMeshRenderer())
                            {
                                ClearThis();
                                return;
                            }
                        }
                    }
                }
            }

            GUILayout.Label("------------------------------------------------------------------------------------------------------------------------");

            GUILayout.Label("You can apply default settings for VRoid models");
            selectAvatar = (SelectAvatar)GUILayout.
            Toolbar((int)selectAvatar, tabSelectAvatar, "LargeButton", GUI.ToolbarButtonSize.Fixed);

            GUILayout.Label("------------------------------------------------------------------------------------------------------------------------");

            GUILayout.Label("ARKit Facial or MetaQuest(OVR)");
            selectData = (SelectData)GUILayout.
            Toolbar((int)selectData, tabSelectData, "LargeButton", GUI.ToolbarButtonSize.Fixed);
            GUILayout.Label("------------------------------------------------------------------------------------------------------------------------");

            GUILayout.Label("Warning: This modifies the avatar's prefab (For VRM specification support)");

            using (new EditorGUI.DisabledScope((baseBSAvator == null) || (renderer == null)))
            {
                if (GUILayout.Button("Clip Build"))
                {
                    //update prefab
                    string prefabPath = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(baseObj);
                    PrefabUtility.ApplyPrefabInstance(baseObj, InteractionMode.UserAction);

                    if (ReadSetBlendShapeClips(baseBSAvator))
                    {
                        EditorUtility.DisplayDialog("Log", "Finished to clip build", "OK");
                    }
                    else
                    {
                        EditorUtility.DisplayDialog("Log", "Failed to add clips", "OK");
                    }
                }
            }
        }

        private bool ReadSetBlendShapeClips(BlendShapeAvatar baseBSAvator)
        {
            Dictionary<string, string> clipNameTable;
            Dictionary<string, float> clipWeightTable;


            if (selectData == SelectData.ARKit)
            {
                if (selectAvatar == SelectAvatar.VRoid)
                {
                    clipNameTable = HANA_ClipData.clipNameTable_VRoid;
                }
                else
                {
                    clipNameTable = HANA_ClipData.clipNameTable_def;
                }
                clipWeightTable = HANA_ClipData.clipWeightTable_def;
            }
            else
            {
                if (selectAvatar == SelectAvatar.VRoid)
                {
                    clipNameTable = HANA_ClipData.clipNameTable_quest_VRoid;
                }
                else
                {
                    clipNameTable = HANA_ClipData.clipNameTable_quest;
                }
                clipWeightTable = HANA_ClipData.clipWeightTable_quest;
            }

            var sharedMesh = renderer.sharedMesh;
            string[] newClipNames = clipNameTable.Keys.ToArray();
            foreach(string clipName in newClipNames)
            {
                bool isSameClip = false;
                foreach(BlendShapeClip clip in baseBSAvator.Clips)
                {
                    if(clip.BlendShapeName == clipName)
                    {
                        isSameClip = true;
                        break;
                    }
                }

                if(isSameClip)
                {
                    continue;
                }

                BlendShapeClip newClip = ScriptableObject.CreateInstance<BlendShapeClip>();
                newClip.BlendShapeName = clipName;

                int sameNameNum = 0;
                bool findSameName = false;
                for (sameNameNum = 0; sameNameNum < sharedMesh.blendShapeCount; sameNameNum++)
                {
                    if( (clipNameTable[clipName] == sharedMesh.GetBlendShapeName(sameNameNum)) ||
                        (clipName == sharedMesh.GetBlendShapeName(sameNameNum)))
                    {
                        findSameName = true;
                        break;
                    }
                }

                //for OVR, If the table does not have the same name, substitute it for the one for ARKit.
                if ((!findSameName) && (sameNameNum == sharedMesh.blendShapeCount))
                {
                    if (selectData == SelectData.OVR)
                    {
                        for (sameNameNum = 0; sameNameNum < sharedMesh.blendShapeCount; sameNameNum++)
                        {
                            if ((HANA_ClipData.clipNameTable_quest_arkit[clipName] == sharedMesh.GetBlendShapeName(sameNameNum)) ||
                                (clipName == sharedMesh.GetBlendShapeName(sameNameNum)))
                            {
                                findSameName = true;
                                break;
                            }
                        }
                    }
                }

                BlendShapeBinding[] newbinds = new BlendShapeBinding[1];
                BlendShapeBinding newbind = new BlendShapeBinding();
                newbind.RelativePath = relativePath;
                if (sameNameNum < sharedMesh.blendShapeCount)
                {
                    newbind.Index = sameNameNum;
                    if (clipWeightTable.ContainsKey(clipName))
                    {
                        newbind.Weight = clipWeightTable[clipName];
                    }
                    else
                    {
                        newbind.Weight = 0;
                    }
                }
                newbinds[0] = newbind;

                newClip.Values = newbinds;

                string targetPath = UnityPath.FromAsset(baseBSAvator).Parent.Child("BlendShape." + clipName + ".asset").Value;
                AssetDatabase.CreateAsset(newClip, targetPath);
                AssetDatabase.ImportAsset(targetPath);

                baseBSAvator.Clips.Add(newClip);

                EditorUtility.SetDirty(baseBSAvator);
                AssetDatabase.SaveAssets();
            }

            return true;
        }

        private bool GetSkinnedMeshRenderer()
        {
            if (baseBSAvator == null)
            {
                EditorUtility.DisplayDialog("Error", "No BlendShapeAvatar found on VRMBlendShapeProxy", "OK");
                ClearThis();
                return false;
            }
            else if ((baseBSAvator.Clips == null) || (baseBSAvator.Clips.Count == 0))
            {
                EditorUtility.DisplayDialog("Error", "No BlendShapeClip found on BlendShapeAvatar", "OK");
                ClearThis();
                return false;
            }
            else
            {
                string meshObjName = null;
                foreach (BlendShapeClip clip in baseBSAvator.Clips)
                {
                    if ((clip.Values != null) && (clip.Values.Length != 0))
                    {
                        meshObjName = clip.Values[0].RelativePath;
                        if (meshObjName != null)
                        {
                            break;
                        }
                    }
                }
                if (meshObjName == null)
                {
                    EditorUtility.DisplayDialog("Error", "Faild to get Meshes name from BlendShapeClip", "OK");
                    ClearThis();
                }
                else
                {
                    Transform baseMeshTransform = baseObj.transform.Find(meshObjName);
                    if (baseMeshTransform == null)
                    {
                        EditorUtility.DisplayDialog("Error", "No Mesh Object found", "OK");
                        ClearThis();
                    }
                    else
                    {
                        renderer = baseMeshTransform.GetComponent<SkinnedMeshRenderer>();
                        if (renderer == null)
                        {
                            EditorUtility.DisplayDialog("Error", "No SkinnedMeshRenderer component found on Object", "OK");
                            ClearThis();
                        }
                        else
                        {
                            var sharedMesh = renderer.sharedMesh;
                            if (sharedMesh == null)
                            {
                                EditorUtility.DisplayDialog("Error", "No Mesh Object found on SkinnedMeshRenderer component", "OK");
                                ClearThis();
                            }
                            else
                            {
                                relativePath = meshObjName;
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }

        private void ClearThis()
        {
            baseObj = null;
            baseBSProxy = null;
            baseBSAvator = null;
            renderer = null;
            relativePath = null;
        }
    }
}

