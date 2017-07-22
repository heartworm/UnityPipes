using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomRotationPlacer : ItemPlacer {

    public GameObject[] prefabs;
    public float maxItemsPerLength;

    public override void PlaceItems(Pipe pipe) {

        int maxItems = Mathf.RoundToInt(maxItemsPerLength * pipe.CurveArcLength);
        int items = Random.Range(1, maxItems);
        

        if (items > 0) {
            float itemAngleSpacing = pipe.CurveArcAngleDeg / items;
            float initAngleSpacing = itemAngleSpacing / 2;
            for (int i = 0; i < items; i++) {
                GameObject randomPrefab = prefabs[Random.Range(0, prefabs.Length)];
                float pipeRotation = Random.Range(0f, 360f);
                float curveRotation = initAngleSpacing + (itemAngleSpacing * i);
                GenerateItem(randomPrefab, pipe, pipeRotation, curveRotation);
            }
        }
    }

    private void GenerateItem(GameObject prefab, Pipe pipe, float pipeRotation, float curveRotation) {
        GameObject item = Instantiate(prefab, pipe.transform);

        Vector3 pipeMouthRel = Vector3.up * pipe.CurveRadius;
        item.transform.Translate(pipeMouthRel);
        //Vector3 curveAxis = pipe.transform.TransformDirection(Vector3.back);
        Vector3 curveAxis = -pipe.transform.forward; 
        //Vector3 pipeAxis = pipe.transform.TransformDirection(Vector3.right);
        Vector3 pipeAxis = pipe.transform.right;
        item.transform.RotateAround(pipe.transform.TransformPoint(pipeMouthRel), pipeAxis, pipeRotation);
        item.transform.RotateAround(pipe.transform.position, curveAxis, curveRotation);
    }
}
