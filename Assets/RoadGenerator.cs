using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;
using Random = UnityEngine.Random;

[RequireComponent(typeof(SplineContainer))]
public class RoadGenerator : MonoBehaviour
{
    [SerializeField]
    private int currentItteration, maxItterations;
    private bool roadComplete = true;
    private FieldVisualizer _visualizer;
    
    private NativeArray<int> _fieldPositionIndex;
    private SplineContainer _splineContainer;
    
    [SerializeField]
    private int seed;

    [SerializeField] private float maxAngle;

    private void OnEnable()
    {
        _visualizer = FindAnyObjectByType<FieldVisualizer>();
        _splineContainer = GetComponent<SplineContainer>();
        roadComplete = true;
    }

    private void OnValidate()
    {
        if (_visualizer == null || _splineContainer == null)
            return;
        RecalculateNetwork();
    }

    public void RecalculateNetwork()
    {
        Random.InitState(seed);
        // Clear road network
        currentItteration = 0;
        roadComplete = false;
        // Generate 5 random points
        if (_fieldPositionIndex.IsCreated)
            _fieldPositionIndex.Dispose();

        _fieldPositionIndex = new NativeArray<int>(2, Allocator.Persistent);
        foreach (var VARIABLE in _splineContainer.Splines)
        {
            _splineContainer.RemoveSpline(VARIABLE);
        }
        
        for (int i = _fieldPositionIndex.Length - 1; i >= 0; i--)
        {
            Debug.Log("Adding SPline");
            _splineContainer.AddSpline();
            _fieldPositionIndex[i] = Random.Range(0, _visualizer._fieldPositions.Length);
        }
    }

    private void OnDisable()
    {
        _visualizer.generationCompleted.RemoveListener(RecalculateNetwork);
        
        if (_fieldPositionIndex.IsCreated)
            _fieldPositionIndex.Dispose();
    }

    [ContextMenu("Add next step")]
    private void Update()
    { 
        if(roadComplete)
            return;
        if(currentItteration > maxItterations)
        {
            roadComplete = true;
            MergeSplines();
            return;
        }
        
        // Generate directions for those points.
        // Add points to splines
        for (int i = 0; i < _fieldPositionIndex.Length; i++)
        {
            // Grab new tangent
            float3 tangent = new float3(_visualizer._fieldDirections[_fieldPositionIndex[i]].xy,0);
            float3 position = new float3(_visualizer._fieldPositions[_fieldPositionIndex[i]], 0);
            // if dot product of last point was less then 0 flip tangents
            if (_splineContainer.Splines[i].Count <= 0)
            {
                _splineContainer.Splines[i].Add(AddNewKnot(position, tangent * -1, tangent));
                continue;
            }
            if(math.dot(tangent, _splineContainer.Splines[i].Last().TangentOut) < 0)
                _splineContainer.Splines[i].Add(AddNewKnot(position, tangent, tangent *-1));
            else
                _splineContainer.Splines[i].Add(AddNewKnot(position, tangent * -1, tangent));
        }
        
        // Add random chance to split
        // Move positions some random amount in range in the direction.
        for (int i = 0; i < _fieldPositionIndex.Length; i++)
        {
            _fieldPositionIndex[i] = FindNextIndex(i, _splineContainer.Splines[i].Last().TangentOut);
        }

        // Pick points were the center is close to the collision if you draw vector lines in the in/out direction.
    }

    private BezierKnot AddNewKnot(float3 position, float3 tangentIn, float3 tangentOut)
    {
        BezierKnot knot = new BezierKnot();
        knot.Position = position;
        knot.TangentIn = tangentIn;
        knot.TangentOut = tangentOut;
        
        return knot;
    }

    private int FindNextIndex(int index, float3 tangent)
    {
        float2 position = _visualizer._fieldPositions[_fieldPositionIndex[index]];
        float2 tangentOut = tangent.xy;
        
        position += tangentOut * (_visualizer.resolution * Random.Range(1, 4));

        int newIndex = -1;
        float minDistance = float.MaxValue;
        for (int i = 0; i < _visualizer._fieldPositions.Length; i++)
        {
            float distance = math.distancesq(_visualizer._fieldPositions[i], position);
            if (distance < minDistance)
            {
                newIndex = i;
                minDistance = distance;
            }
        }

        return newIndex;
    }

    private void MergeSplines()
    {
        // Foreach bezierknot check if any in proximity, merge points.
    }
}
