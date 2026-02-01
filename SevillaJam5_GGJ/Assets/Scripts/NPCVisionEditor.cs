using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(NPCVision))]
public class NPCVisionEditor : Editor
{
    void OnSceneGUI()
    {
        NPCVision npcVision = (NPCVision)target;

        // Permite mover la posición del gizmo usando un handle
        EditorGUI.BeginChangeCheck();
        Vector3 newPosition = Handles.PositionHandle(npcVision.transform.position, Quaternion.identity);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(npcVision.transform, "Move NPCVision Gizmo");
            npcVision.transform.position = newPosition;
        }

        // Opcional: dibujar el radio directamente en la escena
        Handles.color = Color.yellow;
        Handles.DrawWireDisc(npcVision.transform.position, Vector3.forward, npcVision.radius);
    }
}
