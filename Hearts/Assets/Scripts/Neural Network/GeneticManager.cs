using System.Collections;
using UnityEngine;

public class GeneticManager : MonoBehaviour
{
    [SerializeField, Range(0,1f)]
    private float timeFrame;
    
    [SerializeField, Range(0,1f)]
    private float mutationChance;

    [SerializeField, Range(0,1f)]
    private float MutationStrength;

    [SerializeField] private NeuralNetwork neuralNetwork;
   
}
