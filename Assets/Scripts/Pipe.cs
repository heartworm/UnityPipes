using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]

public class Pipe : MonoBehaviour {

    public float curveSegmentArcLength;
    public float minCurveRadius, maxCurveRadius;
    public int  minCurveSegmentCount, maxCurveSegmentCount;
    public float pipeRadius;
    public int pipeSegmentCount;


    private float curveRadius;
    public float CurveRadius {
        get { return curveRadius; }
    }

    private int curveSegmentCount;
    public int CurveSegmentCount {
        get { return curveSegmentCount; }
    }

    public float CurveArcLength {
        get { return curveSegmentCount * curveSegmentArcLength; }
    }
    public float CurveArcAngleDeg {
        get { return (CurveArcLength / curveRadius) * Mathf.Rad2Deg; }
    }

    private float pipeAngleCovered;


    private Mesh mesh;
    private Vector2[] uv;
    private Vector3[] vertices;
    private int[] triangles;

    private Vector3 GetPointOnTorus(float curveAngle, float pipeAngle) { //both in radians
        curveAngle = curveAngle % (2 * Mathf.PI);
        pipeAngle = pipeAngle % (2 * Mathf.PI);


        Vector3 p;
        p.y = (curveRadius + pipeRadius * Mathf.Cos(pipeAngle)) * Mathf.Cos(curveAngle);
        p.x = (curveRadius + pipeRadius * Mathf.Cos(pipeAngle)) * Mathf.Sin(curveAngle);
        p.z = pipeRadius * Mathf.Sin(pipeAngle);
        return p;
    }

    private void SetVertices() {
        vertices = new Vector3[pipeSegmentCount * curveSegmentCount * 4];

        for (int curveSegment = 0; curveSegment < curveSegmentCount; curveSegment++) {
            CreateQuadRing(curveSegment);
        }

        mesh.vertices = vertices;
    }

    private void SetTriangles() {
        triangles = new int[pipeSegmentCount * curveSegmentCount * 6];

        for (int curveSegment = 0; curveSegment < curveSegmentCount; curveSegment++) {
            int curveStartInd = curveSegment * pipeSegmentCount * 6;
            int curveLookupInd = curveSegment * pipeSegmentCount * 4;
            for (int pipeSegment = 0; pipeSegment < pipeSegmentCount; pipeSegment++) {
                int startInd = curveStartInd + pipeSegment * 6;
                int lookupInd = curveLookupInd + pipeSegment * 4;

                triangles[startInd + 0] = lookupInd + 0;
                triangles[startInd + 1] = lookupInd + 1;
                triangles[startInd + 2] = lookupInd + 3;
                triangles[startInd + 3] = lookupInd + 3;
                triangles[startInd + 4] = lookupInd + 1;
                triangles[startInd + 5] = lookupInd + 2;
            }
        }

        mesh.triangles = triangles;
    }

    private void CreateQuadRing(int curveSegment) {
        float pipeAngleStep = (2f * Mathf.PI) / pipeSegmentCount;
        float curveAngleStep = curveSegmentArcLength / curveRadius;

        pipeAngleCovered = curveAngleStep * curveSegmentCount;

        float curveAngle = curveSegment * curveAngleStep;
        float curveAngleNext = curveAngle + curveAngleStep;

        for (int pipeSegment = 0; pipeSegment < pipeSegmentCount; pipeSegment++) {
            int startInd = (curveSegment * pipeSegmentCount + pipeSegment) * 4;
            float pipeAngle = pipeSegment * pipeAngleStep;
            float pipeAngleNext = pipeAngle + pipeAngleStep;
            vertices[startInd] = GetPointOnTorus(curveAngle, pipeAngle);
            vertices[startInd + 1] = GetPointOnTorus(curveAngleNext, pipeAngle);
            vertices[startInd + 2] = GetPointOnTorus(curveAngleNext, pipeAngleNext);
            vertices[startInd + 3] = GetPointOnTorus(curveAngle, pipeAngleNext);
        }

    }


    private void Awake() {
        curveSegmentCount = Random.Range(minCurveSegmentCount, maxCurveSegmentCount);
        curveRadius = Random.Range(minCurveRadius, maxCurveRadius);

        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        mesh.name = "Pipe";
        SetVertices();
        SetTriangles();
        SetUVCoordinates();
        mesh.RecalculateNormals();
    }
    
    public void AlignWith(Pipe otherPipe) {
        transform.SetParent(otherPipe.transform, false);
        
        //rotate the pipe so they don't overlap
        transform.localEulerAngles = new Vector3(0, 0, -otherPipe.pipeAngleCovered * Mathf.Rad2Deg);

        //align mouths due to differing curve radii
        transform.Translate(0, otherPipe.curveRadius - curveRadius, 0);

        //rotate the pipe so that they dont lie in one plane, but so meshes are aligned (mouth is a polygon)
        float rotationAmount = Random.Range(0, pipeSegmentCount) * (360f / pipeSegmentCount);

        // rotate in reference to the axis connecting the two mouths, let unity handle the math. 
        Vector3 otherPipeMouth = transform.TransformPoint(0, curveRadius, 0);
        Vector3 rotationAxis = transform.TransformDirection(1, 0, 0);
        transform.RotateAround(otherPipeMouth, rotationAxis, rotationAmount);

        transform.SetParent(otherPipe.transform.parent, true);
    }
    
    private void SetUVCoordinates() {
        uv = new Vector2[vertices.Length];
        for (int i = 0; i < vertices.Length; i += 4) {
            uv[i] = new Vector2(0, 0);
            uv[i+1] = new Vector2(0, 1);
            uv[i+2] = new Vector2(1, 1);
            uv[i+3] = new Vector2(1, 0);
        }
        mesh.uv = uv;
    }


    //private void OnDrawGizmos() {    //    float pipeAngleStep = (2 * Mathf.PI) / pipeSegmentCount;
    //    float curveAngleStep = (2 * Mathf.PI) / curveSegmentCount;

    //    for (int curveSegment = 0; curveSegment < curveSegmentCount; curveSegment++) {
    //        for (int pipeSegment = 0; pipeSegment < pipeSegmentCount; pipeSegment++) {
    //            Gizmos.color = new Color(1f, (float)pipeSegment / pipeSegmentCount,
    //                (float)curveSegment / curveSegmentCount);
    //            Vector3 point = GetPointOnTorus(curveSegment * curveAngleStep, pipeSegment * pipeAngleStep);
    //            Gizmos.DrawSphere(point, 0.1f);
    //        }
    //    }

    //}

}
