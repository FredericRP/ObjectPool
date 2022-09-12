using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace FredericRP.ObjectPooling
{
  [CustomEditor(typeof(ObjectPool))]
  public class ObjectPoolInspector : UnityEditor.Editor
  {
    public override void OnInspectorGUI()
    {
      ObjectPool pool = target as ObjectPool;

      // New : set ID of ObjectPool
      //EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
      EditorGUILayout.PropertyField(serializedObject.FindProperty("id"));
      //EditorGUILayout.EndHorizontal();
      //
      List<ObjectPool.PoolGameObjectInfo> poolObjectList = pool.PoolGameObjectInfoList;
      Color previousColor = GUI.color;
      //*
      //Rect defaultRect = EditorGUILayout.GetControlRect(false, GUILayout.ExpandWidth(true));

      // EditorGUILayout.GetControlRect adds a new Layout in the grid and prevent using its width simpluy so we just use the total view and substract usual extra space
      float totalWidth = EditorGUIUtility.currentViewWidth - 24;
      float idWidth;
      float prefabWidth = 44;
      float minMaxIntWidth = 38;
      float sliderWidth;
      float parentWidth;
      float buttonWidth = 20;
      float buttonsWidth = 4 * buttonWidth;
      // min max slider use an int field of size 38, for id, parent and slider we are left with: (total width - 2x38) / 3
      idWidth = (totalWidth - 2 * minMaxIntWidth - prefabWidth - buttonsWidth - 9 * EditorGUIUtility.standardVerticalSpacing) / 3;
      parentWidth = idWidth;
      sliderWidth = idWidth;

      EditorGUILayout.BeginHorizontal();
      GUILayout.Label("object id", EditorStyles.boldLabel, GUILayout.Width(idWidth));
      GUILayout.Label("prefab", EditorStyles.boldLabel, GUILayout.Width(prefabWidth));
      GUILayout.Label("default", EditorStyles.boldLabel, GUILayout.Width(minMaxIntWidth + sliderWidth / 2));
      GUIStyle rightLabel = new GUIStyle(EditorStyles.boldLabel);
      rightLabel.alignment = TextAnchor.MiddleRight;
      GUILayout.Label("max count", rightLabel, GUILayout.Width(minMaxIntWidth + sliderWidth / 2));
      GUILayout.Label("def. parent", EditorStyles.boldLabel, GUILayout.Width(parentWidth));
      //GUILayout.Label("- - - -", GUILayout.Width(buttonsWidth));
      EditorGUILayout.EndHorizontal();
      //EditorGUI.DrawRect(new Rect(0, 0, idWidth, EditorGUIUtility.singleLineHeight), Color.green);
      //EditorGUI.DrawRect(new Rect(18, 20, totalWidth, EditorGUIUtility.singleLineHeight), Color.cyan);
      //EditorGUI.DrawRect(new Rect(18 + EditorGUIUtility.standardVerticalSpacing + idWidth, EditorGUIUtility.singleLineHeight + 4, prefabWidth, EditorGUIUtility.singleLineHeight), Color.yellow);

      EditorGUI.BeginChangeCheck();

      if (poolObjectList.Count > 0)
      {
        for (int i = 0; i < poolObjectList.Count; i++)
        {
          ObjectPool.PoolGameObjectInfo data = poolObjectList[i];
          EditorGUILayout.BeginHorizontal();

          // PoolObject : name, prefab, bufferCount, defaultParent
          data.id = EditorGUILayout.TextField(data.id, GUILayout.Width(idWidth));
          //data.tag = EditorGUILayout.TagField(data.tag);
          if (data.prefab == null)
            GUI.color = Color.red;
          data.prefab = (GameObject)EditorGUILayout.ObjectField(data.prefab, typeof(GameObject), false, GUILayout.Width(prefabWidth));
          GUI.color = previousColor;

          data.bufferCount = EditorGUILayout.IntField(data.bufferCount, GUILayout.Width(minMaxIntWidth));
          float count = data.bufferCount;
          float max = data.maxCount;
          EditorGUILayout.MinMaxSlider(ref count, ref max, 0, ObjectPool.MAX_BUFFER, GUILayout.Width(sliderWidth));
          data.bufferCount = (int)count;
          data.maxCount = (int)max;
          data.maxCount = EditorGUILayout.IntField(data.maxCount, GUILayout.Width(minMaxIntWidth));

          //GUILayout.Label("on");
          if (data.defaultParent == null)
            GUI.color = Color.red;
          data.defaultParent = (Transform)EditorGUILayout.ObjectField(data.defaultParent, typeof(Transform), true, GUILayout.Width(parentWidth));
          GUI.color = previousColor;

          GUI.enabled = (i > 0);
          if (GUILayout.Button("\u25B2", EditorStyles.miniButtonLeft, GUILayout.Width(buttonWidth)))
          {
            // Switch with previous property
            ObjectPool.PoolGameObjectInfo pgoInfo = poolObjectList[i];
            poolObjectList.RemoveAt(i);
            poolObjectList.Insert(i - 1, pgoInfo);

          }
          GUI.enabled = (i < poolObjectList.Count - 1);
          if (GUILayout.Button("\u25BC", EditorStyles.miniButtonMid, GUILayout.Width(buttonWidth)))
          {
            // Switch with next property
            ObjectPool.PoolGameObjectInfo pgoInfo = poolObjectList[i];
            poolObjectList.RemoveAt(i);
            poolObjectList.Insert(i + 1, pgoInfo);
          }
          GUI.enabled = true;
          GUI.color = Color.green;
          if (GUILayout.Button("+", EditorStyles.miniButtonMid, GUILayout.Width(buttonWidth)))
          {
            poolObjectList.Insert(i + 1, new ObjectPool.PoolGameObjectInfo());
          }
          GUI.color = Color.red;
          if (GUILayout.Button("X", EditorStyles.miniButtonRight, GUILayout.Width(buttonWidth)))
          {
            // Remove property
            poolObjectList.RemoveAt(i);
          }
          GUI.color = previousColor;
          EditorGUILayout.EndHorizontal();

        }
      }
      else
      {
        GUI.color = Color.green;
        if (GUILayout.Button("+", EditorStyles.miniButton, GUILayout.Width(buttonWidth)))
        {
          poolObjectList.Add(new ObjectPool.PoolGameObjectInfo());
        }
        GUI.color = previousColor;
      }

      if (EditorGUI.EndChangeCheck())
      {
        EditorUtility.SetDirty(pool);
      }
    }
  }
}