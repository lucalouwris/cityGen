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
    private void OnEnable()
    {
        _visualizer = FindAnyObjectByType<FieldVisualizer>();
        _splineContainer = GetComponent<SplineContainer>();
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

        _fieldPositionIndex = new NativeArray<int>(5, Allocator.Persistent);
        for (int i = 0; i < _fieldPositionIndex.Length; i++)
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

    private void Update()
    {
        if(maxItterations > currentItteration)
        {
            if(roadComplete)
                return;
            roadComplete = true;
            MergeSplines();
        }
        
        // Generate directions for those points.
        // Add points to spline
        for (int i = 0; i < _fieldPositionIndex.Length; i++)
        {
            
            Debug.Log("Adding newKnot");
            //if(_splineContainer.Splines[i].Last().TangentOut)
            BezierKnot newKnot = new BezierKnot
            {
                Position = new float3(_visualizer._fieldPositions[_fieldPositionIndex[i]], 0),
                TangentOut = new float3(_visualizer._fieldDirections[_fieldPositionIndex[i]].xy,0),
            };
            _splineContainer.Splines[i].Add(newKnot);
        }
        
        // Add random chance to split
        // Move positions some random amount in range in the direction.
        for (int i = 0; i < _fieldPositionIndex.Length; i++)
        {
            _fieldPositionIndex[i] = FindNextIndex(i);
        }

        // Pick points were the center is close to the collision if you draw vector lines in the in/out direction.
    }

    private int FindNextIndex(int index)
    {
        float2 position = _visualizer._fieldPositions[_fieldPositionIndex[index]];
        float2 tangentOut =_visualizer._fieldDirections[_fieldPositionIndex[index]].xy;

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
