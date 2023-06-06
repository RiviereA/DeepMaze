using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeuralNetwork : IComparable<NeuralNetwork>
{
    private int[] layers;           // Liste de layers
    private float[][] neurons;      // Matrice des neurones
    private float[][][] weights;      // Matrice des poids
    private float fitness;          // Fitness du réseau

    // Initialise notre reseau de neurones
    public NeuralNetwork(int[] layers)
    {
        this.layers = new int[layers.Length];
        for (int i = 0; i < layers.Length; i++)
        {
            this.layers[i] = layers[i];
        }

        InitNeurons();
        InitWeight();
    }

    public NeuralNetwork(NeuralNetwork copyNetwork)
    {
        this.layers = new int[copyNetwork.layers.Length];
        for (int i = 0; i < copyNetwork.layers.Length; i++)
        {
            this.layers[i] = copyNetwork.layers[i];
        }

        InitNeurons();
        InitWeight();
    }

    // Copie tout les poids du reseau donne en parametre
    private void CopyWeights(float[][][] copyWeight)
    {
        for (int i = 0; i < weights.Length; i++)
        {
            for (int j = 0; j < weights[i].Length; j++)
            {
                for (int k = 0; k < weights[i][j].Length; k++)
                {
                    weights[i][j][k] = copyWeight[i][j][k];
                }
            }
        }
    }

    // Initialise notre matrice de neurones
    private void InitNeurons()
    {
        List<float[]> neuronsList = new List<float[]>();

        for (int i = 0; i < this.layers.Length; i++)
        {
            neuronsList.Add(new float[layers[i]]);
        }
        neurons = neuronsList.ToArray();
    }

    // Initialise notre matrice de poids
    private void InitWeight()
    {
        List<float[][]> weightList = new List<float[][]>();

        // Iteration sur tout les neurones ayant des connexions entrantes
        for (int i = 1; i < this.layers.Length; i++)
        {
            List<float[]> layerWeightsList = new List<float[]>();

            int neuronsInPreviousLayer = layers[i - 1];

            // Iteration sur tout les neurones du layer actuel
            for (int j = 0; j < neurons[i].Length; j++)
            {
                float[] neuronsWeights = new float[neuronsInPreviousLayer];

                // Iteration sur tout les neurones du layer precedent. Leurs cree un poids chacun -1 et 1
                for (int k = 0; k < neuronsInPreviousLayer; k++)
                {
                    neuronsWeights[k] = UnityEngine.Random.Range(-1f, 1f);
                }
                layerWeightsList.Add(neuronsWeights);
            }
            weightList.Add(layerWeightsList.ToArray()); // Convertis les poids de cette couche en liste 2D, et l'ajoute a la liste des poids
        }
        weights = weightList.ToArray(); // Convertis en tableau 3D
    }

    // Fonction de feedforward
    public float[] FeedForward(float[] inputs)
    {
        // Ajoute nos inputs dans la matrices des neurones d'entree
        for (int i = 0; i < inputs.Length; i++)
        {
            neurons[0][i] = inputs[i];
        }

        // Calculs
        // Iteration sur toutes les couches, tout les neurones, puis toutes les connexions entrantes, et fait les calculs du feedforward
        for (int i = 1; i < layers.Length; i++)
        {
            for (int j = 0; j < neurons[i].Length; j++)
            {
                float val = 0f;
                for (int k = 0; k < neurons[i - 1].Length; k++)
                {
                    val += weights[i - 1][j][k] * neurons[i - 1][k]; // Calcul la valeur de chaque neurones
                }
                neurons[i][j] = (float)Math.Tanh(val); // Fonction d'activation : tangente hyperbolique
            }
        }
        return neurons[neurons.Length - 1]; // retourne la valeur de l'output layer
    }

    // Fonction de mutation
    public void Mutate(float condition)
    {
        // Iteration sur tout les poids du reseau
        for (int i = 0; i < weights.Length; i++)
        {
            for (int j = 0; j < weights[i].Length; j++)
            {
                for (int k = 0; k < weights[i][j].Length; k++)
                {
                    // fait muter le poids actuel en fonction de la condition en entree
                    float weight = weights[i][j][k];
                    float randNum = UnityEngine.Random.Range(0f, 100f);
                    if (randNum <= condition)
                    {
                        float newWeight = UnityEngine.Random.Range(-1f, 1f);
                        weight = newWeight;
                    }
                    weights[i][j][k] = weight;
                }
            }
        }
    }

    public void AddFitness(float fit)
    {
        fitness += fit;
    }

    public void SetFitness(float fit)
    {
        fitness = fit;
    }

    public float GetFitness()
    {
        return fitness;
    }

    // Compare le fitness de nos reseaux
    public int CompareTo(NeuralNetwork other)
    {
        if (other == null) return 1;

        if (fitness > other.fitness)
            return 1;
        else if (fitness < other.fitness)
            return -1;
        else
            return 0;
    }
}
