using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour
{
    public GameObject slimePrefab;                      // Prefab du slime

    private bool isTraining = false;
    public int populationSize = 40;                     // Nombre d'agents a faire spawner
    public float timer = 7f;                            // Temps par generation
    private int generationNumber = 0;                   // Compteur de generation

    private List<NeuralNetwork> nets;                   // Liste de neural network de generation x
    private List<NeuralNetwork> newNets;                // Liste de neural network de generation x + 1
    public List<GameObject> slimeList = null;          // Liste des slimes de la generation x

    private int[] layers = new int[] { 6, 5, 4, 2 };    // Dimensions de nos reseaux de neurones

    private float fit = 0;                              // Fitness. On calculera la moyenne de la generation grace a ca

    //public Material myMaterial;

    // Met fin a la generation actuelle
    void Timer()
    {
        isTraining = false;
    }

    // Update is called once per frame
    void Update()
    {
        // Changement de generation
        if (isTraining == false)
        {
            // Si on est a la premiere generation, on instancie les slimes
            if (generationNumber == 0)
            {
                InitCarNeuralNetworks();
                CreateSlimeBodies();
            }
            else
            {
                // Transfere le fitness du controleur vers le reseau de neurones
                for (int i = 0; i < populationSize; i++)
                {
                    NNController script = slimeList[i].GetComponent<NNController>();
                    float fitness = script.fitness;
                    nets[i].SetFitness(fitness);
                }

                // Trie la liste de neural networks en fonstion de leurs fitness
                nets.Sort();
                nets.Reverse();

                fit = 0;
                for (int i = 0;i < populationSize; i++)
                {
                    fit += nets[i].GetFitness();
                }
                fit /= populationSize;
                Debug.Log(fit);

                // Instancie la liste de la generation suivante
                List<NeuralNetwork> newNets = new List<NeuralNetwork>();

                // Recupere le plus intelligent de nos slimes
                for (int i = 0; i < populationSize / 4; i++)
                {
                    NeuralNetwork net = new NeuralNetwork(nets[i]);
                    newNets.Add(net);
                }

                // Recupere le plus intelligent de nos slimes
                for (int i = 0; i < populationSize / 4; i++)
                {
                    NeuralNetwork net = new NeuralNetwork(nets[i]);
                    net.Mutate(0.5f);
                    newNets.Add(net);
                }

                // Recupere le plus intelligent de nos slimes
                for (int i = 0; i < populationSize / 4; i++)
                {
                    NeuralNetwork net = new NeuralNetwork(nets[i]);
                    net.Mutate(2f);
                    newNets.Add(net);
                }

                // Recupere le plus intelligent de nos slimes
                for (int i = 0; i < populationSize / 4; i++)
                {
                    NeuralNetwork net = new NeuralNetwork(nets[i]);
                    net.Mutate(9f);
                    newNets.Add(net);
                }

                // Changement entre les deux generations
                nets = newNets;
            }

            // A la fin du decompte du timer, passe a la generation suivante
            generationNumber++;
            Invoke("Timer", timer);
            CreateSlimeBodies();
            isTraining = true;
        }

        // Feedforward
        // transfers les informations du NNController vers les inputs de notre reseau de neurones
        for (int i = 0; i < populationSize; i++)
        {
            NNController script = slimeList[i].GetComponent<NNController>();

            float[] result;
            float vel = script.currentVelocity / script.maxDistance;
            float distForward = script.distForward / script.maxDistance;
            float distLeft = script.distLeft / script.maxDistance;
            float distRight = script.distRight / script.maxDistance;
            float distDiagLeft = script.distDiagLeft / script.maxDistance;
            float distDiagRight = script.distDiagRight / script.maxDistance;

            float[] tInput = new float[] { vel, distForward, distLeft, distRight, distDiagLeft, distDiagRight };
            result = nets[i].FeedForward(tInput);
            script.results = result; // Envoie le resultat au NNController
        }

        // Les generations ne meurent pas assez vite ? Voilà la solution
        if (Input.GetKeyDown(KeyCode.Space))
        {
            generationNumber++;
            isTraining = true;
            Timer();
            CreateSlimeBodies();
        }
    }

    // Initialise notre liste de neural networks
    void InitCarNeuralNetworks()
    {
        nets = new List<NeuralNetwork>();

        for (int i = 0; i < populationSize; i++)
        {
            NeuralNetwork net = new NeuralNetwork(layers);
            net.Mutate(0.5f);
            nets.Add(net);
        }
    }

    private void CreateSlimeBodies()
    {
        // Detruis tout nos slimes
        for (int i = 0; i < slimeList.Count; i++)
        {
            Destroy(slimeList[i]);
        }

        // Recree une generation de slimes
        slimeList = new List<GameObject> ();
        for (int i = 0; i < populationSize; i++)
        {
            GameObject slime = Instantiate(slimePrefab, new Vector3(4f, 0f, 2f), slimePrefab.transform.rotation);
            slimeList.Add(slime);
            slimeList[i] = slime;

            /*Renderer myRend = slimeList[i].GetComponent<Renderer>();
            myRend.enabled = true;

            myRend.sharedMaterial = myMaterial;*/
        }
    }
}
