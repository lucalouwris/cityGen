using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadGenerator : MonoBehaviour
{
    private int currentItteration, maxItterations;
    private bool roadComplete = true;
    private FieldVisualizer _visualizer;
    private void OnEnable()
    {
        _visualizer = FindAnyObjectByType<FieldVisualizer>();
        _visualizer.generationCompleted.AddListener(RecalculateNetwork);
    }

    private void RecalculateNetwork()
    {
        // Clear road network
        currentItteration = 0;
        roadComplete = false;
        // Generate 5 random points
    }

    private void OnDisable()
    {
        _visualizer.generationCompleted.RemoveListener(RecalculateNetwork);
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

        // Add random chance to split
        // Move positions some random amount in range in the direction.
        
        // Pick points were the center is close to the collision if you draw vector lines in the in/out direction.
    }

    private void MergeSplines()
    {
        // Foreach bezierknot check if any in proximity, merge points.
    }
}
