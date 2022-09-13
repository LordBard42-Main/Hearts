using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
//SOURC: https://towardsdatascience.com/building-a-neural-network-framework-in-c-16ef56ce1fef
public class NeuralNetwork : IComparable<NeuralNetwork>
{
    private int[] layers;//layers    
    private float[][] neurons;//neurons    
    private float[][] biases;//biasses    
    private float[][][] weights;//weights    
    private int[] activations;//layers
    public float fitness = 0;//fitness
    public NeuralNetwork(int[] layers)
    {

        //Initializes layers
        this.layers = new int[layers.Length];
        for (int i = 0; i < layers.Length; i++)
        {
            this.layers[i] = layers[i];
        }



        InitNeurons();
        InitBiases();
        InitWeights();
    }

    //Inputs >===> Outputs
    public float[] FeedForward(float[] inputs)
    {
        //Set Layer 0 Nerons to your inptus
        for (int i = 0; i <inputs.Length; i++)
        {
            neurons[0][1] = inputs[i];
        }

        //Iterate through your layers
        for(int i = 1; i < layers.Length; i++)
        {
            //Specify the previous layer index
            int prevLayer = i - 1;
            //Iterate through all the nerouns in a given layer
            for(int j = 0; j < neurons[i].Length; j++)
            {
                float value = 0f;
                //Iterate through the nerons of the previousLayer
                for(int k = 0; k < neurons[prevLayer].Length; k++)
                {
                    //Multiply weights and Nerons of the previous layer, add them together which achieves a weighted sum
                    value += weights[prevLayer][j][k] * neurons[prevLayer][k];
                }
                neurons[i][j] = activate(value + biases[i][j]);
            }
        }
        return neurons[neurons.Length - 1];
    }

    //Moves through The Networks
    public float activate(float value)
    {
        return (float)Math.Tanh(value);
    }

    //Create Empty array to store the Neurons
    private void InitNeurons()
    {
        //Use list so you can add an undetermined amount of neurons...
        //Than convert back to array...
        List<float[]> neuronList = new List<float[]>();
        for(int i = 0; i < layers.Length; i++)
        {
            neuronList.Add(new float[layers[i]]);
        }
        neurons = neuronList.ToArray();
    }
    //Initializes and populates array with Biases
    private void InitBiases()
    {
        List<float[]> biasList = new List<float[]>();
        for(int i = 0; i < layers.Length; i++)
        {
            float[] bias = new float[layers[i]];
            for(int j = 0; j < layers[i]; j++)
            {
                bias[j] = UnityEngine.Random.Range(-0.5f, 0.5f);
            }
            biasList.Add(bias);
        }
        biases = biasList.ToArray();
    }
    //initializes random array for the weights being held in the network.
    private void InitWeights()
    {
        //Creates 3 Dimensional Array of unknown y,z lengths
        List<float[][]> weightsList = new List<float[][]>();
        for(int i = 1; i < layers.Length; i++)
        { //This iterates Through the layer
            List<float[]> layerWeightsList = new List<float[]>();
            int neuronsInPreviousLayer = layers[i - 1];
            for(int j = 0; j < neurons[i].Length;j++)
            {//This iterates through the neurons in "i" layer
                float[] neuronWeigths = new float[neuronsInPreviousLayer];
                for(int k = 0; k < neuronsInPreviousLayer; k++)
                {//This iterates throughn nerons in "i-1" layer
                    neuronWeigths[k] = UnityEngine.Random.Range(-.5f, .5f);
                }
                layerWeightsList.Add(neuronWeigths);
            }
            weightsList.Add(layerWeightsList.ToArray());
        }
        weights = weightsList.ToArray();
    }

    //Compare two Network Performances
    public int CompareTo(NeuralNetwork other)
    {
        if (other == null)
            return 1;

        if (fitness > other.fitness)
            return 1;
        else if (fitness < other.fitness)
            return -1;
        else
            return 0;
        
    }

    //Loads Weigths and Biases from file
    public void Load(string path)
    {
        //This Block Opens a Stream Reader and Loads the file into our ListLines[]
        TextReader tr = new StreamReader(path);
        int NumberOfLines = (int)new FileInfo(path).Length;
        string[] ListLines = new string[NumberOfLines];
        int index = 1;
        for(int i = 1; i < NumberOfLines; i++)
        {
            ListLines[i] = tr.ReadLine();
        }
        tr.Close();


        //This block will load biases and weights from List Lines
        if(new FileInfo(path).Length > 0)
        {
            for (int i = 0; i < biases.Length; i++)
            {
                for (int j = 0; j < biases[i].Length; j++)
                {
                    biases[i][j] = float.Parse(ListLines[index]);
                    index++;
                }
            }
            for (int i = 0; i < weights.Length; i++)
            {
                for (int j = 0; j < weights[i].Length; j++)
                {
                    for (int k = 0; k < weights[i][j].Length; k++)
                    {
                        weights[i][j][k] = float.Parse(ListLines[index]);
                        index++;
                    }
                }
            }
        }

    }

    //Genetic Implementation of a Neural Network
    public void Mutate(int chance, float val)
    {
        for (int i = 0; i < biases.Length; i++)
        {
            for (int j = 0; j < biases[i].Length; j++)
            {
                biases[i][j] = (UnityEngine.Random.Range(0f, chance) <= 5) ? biases[i][j] += UnityEngine.Random.Range(-val, val) : biases[i][j];
            }
        }

        for (int i = 0; i < weights.Length; i++)
        {
            for (int j = 0; j < weights[i].Length; j++)
            {
                for (int k = 0; k < weights[i][j].Length; k++)
                {
                    weights[i][j][k] = (UnityEngine.Random.Range(0f, chance) <= 5) ? weights[i][j][k] += UnityEngine.Random.Range(-val, val) : weights[i][j][k];

                }
            }
        }
    }
}
