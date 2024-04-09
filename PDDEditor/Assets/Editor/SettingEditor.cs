using PDDEditor.Types;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
[CustomEditor(typeof(ToggleSetting))]
public class SettingEditor : Editor
{
  /*  private ItemSetting Target;
    private void OnEnable()
    {
        Target = (ItemSetting)target;
    }
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        Target.Type = (SettingType)EditorGUILayout.EnumPopup("MyType", Target.Type);
        switch (Target.Type)
        {
            case SettingType.Toggle:
                Target.ToggledObject = EditorGUILayout.ObjectField("Toggled object", Target.ToggledObject); break;
        }
        serializedObject.ApplyModifiedProperties();
    }*/
}
