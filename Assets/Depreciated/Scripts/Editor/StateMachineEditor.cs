using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(StateMachine), true)]
public class StateMachineEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        StateMachine machine = (StateMachine)target;
        if (machine.currentState == null)
        {
            EditorGUILayout.LabelField("Current State", "nothing lol");
        }
        else
        {
            EditorGUILayout.LabelField("Current State", machine.currentState.stateName);
            EditorGUILayout.FloatField(machine.currentState.age);
        }
    }
}