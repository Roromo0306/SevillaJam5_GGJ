using UnityEngine;
using TMPro;

public class TMP_ShakeWords : MonoBehaviour
{
    public float intensity = 1.5f;

    TMP_Text textMesh;

    void Awake()
    {
        textMesh = GetComponent<TMP_Text>();
    }

    void Update()
    {
        textMesh.ForceMeshUpdate();
        TMP_TextInfo textInfo = textMesh.textInfo;

        for (int l = 0; l < textInfo.linkCount; l++)
        {
            TMP_LinkInfo linkInfo = textInfo.linkInfo[l];

            if (linkInfo.GetLinkID() != "shake")
                continue;

            for (int i = 0; i < linkInfo.linkTextLength; i++)
            {
                int charIndex = linkInfo.linkTextfirstCharacterIndex + i;
                TMP_CharacterInfo charInfo = textInfo.characterInfo[charIndex];

                if (!charInfo.isVisible) continue;

                int materialIndex = charInfo.materialReferenceIndex;
                int vertexIndex = charInfo.vertexIndex;

                Vector3[] vertices = textInfo.meshInfo[materialIndex].vertices;

                Vector3 offset = new Vector3(
                    Random.Range(-intensity, intensity),
                    Random.Range(-intensity, intensity),
                    0
                );

                vertices[vertexIndex + 0] += offset;
                vertices[vertexIndex + 1] += offset;
                vertices[vertexIndex + 2] += offset;
                vertices[vertexIndex + 3] += offset;
            }
        }

        for (int i = 0; i < textInfo.meshInfo.Length; i++)
        {
            textInfo.meshInfo[i].mesh.vertices = textInfo.meshInfo[i].vertices;
            textMesh.UpdateGeometry(textInfo.meshInfo[i].mesh, i);
        }
    }
}

