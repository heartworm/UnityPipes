using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PipeSystem : MonoBehaviour {

    public Pipe pipePrefab;
    public int pipeCount;
    public ItemPlacer[] itemPlacers;
    public int noObstaclePipeCount;
    

    private List<Pipe> pipes = new List<Pipe>();
    private Pipe currentPipe;

    private bool firstPipe = true;
    private float currentPipeOffset = 0;

    private void Awake() {
        for (int i = 0; i < pipeCount; i++) {
            GeneratePipe();
        }
        AlignSystem();
        transform.Rotate(0, 0, pipes[0].CurveArcAngleDeg);
        currentPipe = pipes[1];
    }
    
    private void GeneratePipe() {
        Pipe pipe = Instantiate<Pipe>(pipePrefab);
        pipe.transform.SetParent(transform, false);
        if (pipes.Count > 0) {
            pipe.AlignWith(pipes[pipes.Count - 1]);
        }
        pipes.Add(pipe);



        if (noObstaclePipeCount == 0) {
            itemPlacers[Random.Range(0, itemPlacers.Length)].PlaceItems(pipe);
        } else {
            noObstaclePipeCount--;
        }
    }

    private void PopPipe() {
        Destroy(pipes[0].gameObject);
        pipes.RemoveAt(0);
        GeneratePipe();

        currentPipe = pipes[1];
        currentPipeOffset = 0;

        transform.position -= currentPipe.transform.TransformPoint(0, currentPipe.CurveRadius, 0);
    }

    private void AlignSystem() {
        transform.Translate(0, -pipes[0].CurveRadius, 0);
    }

    public void MoveSystem(float arcLength) {
        do {
            arcLength = RotateCurrentSegment(arcLength);
            if (arcLength != 0) {
                PopPipe();
            }
        } while (arcLength != 0);
    }

    private float RotateCurrentSegment(float arcLength) {
        currentPipe = pipes[1];
        
        float remainingPipeLength = currentPipe.CurveArcLength - currentPipeOffset;

        Vector3 worldAxis = currentPipe.transform.TransformDirection(Vector3.forward);
        Vector3 worldPivot = currentPipe.transform.position;

        if (arcLength > remainingPipeLength) {
            float rotateAmt = (remainingPipeLength / currentPipe.CurveRadius) * Mathf.Rad2Deg;
            transform.RotateAround(worldPivot, worldAxis, rotateAmt);
            return arcLength - remainingPipeLength;
        } else {
            float rotateAmt = (arcLength / currentPipe.CurveRadius) * Mathf.Rad2Deg;
            transform.RotateAround(worldPivot, worldAxis, rotateAmt);
            currentPipeOffset += arcLength;
            return 0;
        }
    }
    
    

}
