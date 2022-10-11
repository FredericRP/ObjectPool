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
      // New : set ID of ObjectPool
      EditorGUILayout.PropertyField(serializedObject.FindProperty("id"));
      //
      SerializedProperty poolObjectListProperty = serializedObject.FindProperty("poolObjectList");
      Color previousColor = GUI.color;

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
      idWidth = (totalWidth - 2 * minMaxIntWidth - prefabWidth - buttonsWidth) / 3 - 3 * EditorGUIUtility.standardVerticalSpacing;
      parentWidth = idWidth;
      sliderWidth = idWidth;

      EditorGUILayout.BeginHorizontal();
      GUIStyle headerStyle = EditorStyles.miniBoldLabel;
      GUILayout.Label("object id", headerStyle, GUILayout.Width(idWidth));
      GUILayout.Label("prefab", headerStyle, GUILayout.Width(prefabWidth));
      GUILayout.Label("default", headerStyle, GUILayout.Width(minMaxIntWidth + sliderWidth / 2 + EditorGUIUtility.standardVerticalSpacing));
      GUIStyle rightLabel = new GUIStyle(headerStyle);
      rightLabel.alignment = TextAnchor.MiddleRight;
      GUILayout.Label("max count", rightLabel, GUILayout.Width(minMaxIntWidth + sliderWidth / 2));
      GUILayout.Label("def. parent", headerStyle, GUILayout.Width(parentWidth));
      EditorGUILayout.EndHorizontal();

      EditorGUI.BeginChangeCheck();

      if (poolObjectListProperty.isArray && poolObjectListProperty.arraySize > 0)
      {
        for (int i = 0; i < poolObjectListProperty.arraySize; i++)
        {
          SerializedProperty dataProperty = poolObjectListProperty.GetArrayElementAtIndex(i);
          EditorGUILayout.BeginHorizontal();

          // PoolObject : name, prefab, bufferCount, defaultParent
          EditorGUILayout.PropertyField(dataProperty.FindPropertyRelative("id"), GUIContent.none, true, GUILayout.Width(idWidth));
          SerializedProperty prefabProperty = dataProperty.FindPropertyRelative("prefab");
          if (prefabProperty.objectReferenceValue == null)
            GUI.color = Color.red;
          EditorGUILayout.PropertyField(prefabProperty, GUIContent.none, true, GUILayout.Width(prefabWidth));
          GUI.color = previousColor;

          SerializedProperty bufferCountProperty = dataProperty.FindPropertyRelative("bufferCount");
          SerializedProperty maxCountProperty = dataProperty.FindPropertyRelative("maxCount");
          EditorGUILayout.PropertyField(bufferCountProperty, GUIContent.none, true, GUILayout.Width(minMaxIntWidth));
          float count = bufferCountProperty.intValue;
          float max = maxCountProperty.intValue;
          EditorGUILayout.MinMaxSlider(ref count, ref max, 0, ObjectPool.MAX_BUFFER, GUILayout.Width(sliderWidth));
          bufferCountProperty.intValue = (int)count;
          maxCountProperty.intValue = (int)max;
          EditorGUILayout.PropertyField(maxCountProperty, GUIContent.none, true, GUILayout.Width(minMaxIntWidth));

          SerializedProperty defaultParentProperty = dataProperty.FindPropertyRelative("defaultParent");
          if (defaultParentProperty.objectReferenceValue == null)
            GUI.color = Color.red;
          EditorGUILayout.PropertyField(defaultParentProperty, GUIContent.none, true, GUILayout.Width(parentWidth));
          GUI.color = previousColor;

          GUI.enabled = (i > 0);
          if (GUILayout.Button("\u25B2", EditorStyles.miniButtonLeft, GUILayout.Width(buttonWidth)))
          {
            // Switch with previous property
            poolObjectListProperty.MoveArrayElement(i, i - 1);
          }
          GUI.enabled = (i < poolObjectListProperty.arraySize - 1);
          if (GUILayout.Button("\u25BC", EditorStyles.miniButtonMid, GUILayout.Width(buttonWidth)))
          {
            // Switch with next property
            poolObjectListProperty.MoveArrayElement(i, i + 1);
          }
          GUI.enabled = true;
          GUI.color = Color.green;
          if (GUILayout.Button("+", EditorStyles.miniButtonMid, GUILayout.Width(buttonWidth)))
          {
            poolObjectListProperty.InsertArrayElementAtIndex(i + 1);
          }
          GUI.color = Color.red;
          if (GUILayout.Button("X", EditorStyles.miniButtonRight, GUILayout.Width(buttonWidth)))
          {
            // Remove property
            poolObjectListProperty.DeleteArrayElementAtIndex(i);
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
          poolObjectListProperty.InsertArrayElementAtIndex(0);
          //poolObjectList.Add(new ObjectPool.PoolGameObjectInfo());
        }
        GUI.color = previousColor;
      }

      if (EditorGUI.EndChangeCheck())
      {
        serializedObject.ApplyModifiedProperties();
      }
    }
  }
}