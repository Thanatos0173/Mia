using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using IronPython.Runtime.Exceptions;
using Microsoft.Scripting.Utils;
using System.Text.RegularExpressions;
using NumSharp;
using NumSharp.Backends.Unmanaged;
using YamlDotNet.Serialization.TypeResolvers;

namespace Celeste.Mod.Mia.NeuralNetwork
{ 
public class NeuralNetwork
{
    public class FirstLayers
    {
        // Activation function: Relu
        public NDArray weights;
        public NDArray biases;
        public NDArray inputs;
        public NDArray outputNotActivated;
        public NDArray output;
        public NDArray outputGradient;
        public FirstLayers(NDArray weights, NDArray biases)
        {
            this.weights = weights;
            this.biases = biases;
        }

        public void Forward(NDArray inputs)
        {
            this.inputs = inputs;
            this.outputNotActivated = np.dot(inputs, this.weights) + this.biases;
            this.output = np.maximum(0, this.outputNotActivated);
        }

        public void FirstLayerBackward(NDArray inputGradient, double learningRate)
        {
            inputGradient = inputGradient * DerivRelu(this.outputNotActivated);
            this.outputGradient = np.dot(inputGradient, this.weights.T);
            this.weights -= np.dot(this.inputs.T, inputGradient) * learningRate * 1 / inputGradient.shape[0];
            this.biases -= np.mean(inputGradient, axis: 0) * learningRate;
        }

        public NDArray DerivRelu(NDArray x)
        {
            return x > 0;
        }
    }

    public class LastLayer
    {
        // Activation function: Softmax
        public NDArray weights;
        public NDArray biases;
        public NDArray inputs;
        public NDArray outputGradient;
        public NDArray output;

        public LastLayer(NDArray weights, NDArray biases)
        {
            this.weights = weights;
            this.biases = biases;
        }

        public void Forward(NDArray inputs)
        {
            this.inputs = inputs;
            NDArray outputNotActivated = np.dot(inputs, this.weights) + this.biases;
            NDArray expValues = np.exp(outputNotActivated - np.max(outputNotActivated, axis: 1, keepdims: true));
            this.output = expValues / np.sum(expValues, axis: 1, keepdims: true);
        }

        public void LastLayerBackward(NDArray yPred, NDArray yTrue, double learningRate)
        {
            NDArray inputGradient = yPred - yTrue;
            this.outputGradient = np.dot(inputGradient, this.weights.T);
            this.weights -= np.dot(this.inputs.T, inputGradient) * learningRate * 1 / inputGradient.shape[0];
            this.biases -= np.mean(inputGradient, axis: 0) * learningRate;
        }
    }
    
    private static Tuple<List<FirstLayers>,LastLayer> nn;
    private static NDArray accInput;
    private static NDArray accLabel;
    //private static NDArray dstest;

    public static void isWorking() { Console.WriteLine("Hey! It's working!"); }

    public static void Open()
    {
        string weightsPath = "weights.npy";
        string biasesPath = "biases.npy";
        string testPath = "test.npy";

        var weights = np.Load<double[]>(weightsPath).Select(arr => np.array(arr)).ToList();
        var biases = np.Load<double[]>(biasesPath).Select(arr => np.array(arr)).ToList();
        //dstest = np.Load<double[][]>(testPath).Select(arr => np.array(arr)).ToArray();
        int n = weights.Count;
        //accInput = np.array(dstest.Select(item => item[1]).ToArray()); // Comprendre le 
        //accLabel = np.array(dstest.Select(item => item[0]).ToArray()); // probleme

        nn = new Tuple<List<FirstLayers>, LastLayer>(new List<FirstLayers>(), new LastLayer(weights[n - 1], biases[n - 1])); // I gave it a new value... We'll see.
        for (int j = 0; j < n - 1; j++)
        {
            nn.Item1.Add(new FirstLayers(weights[j], biases[j]));
        }
    }

    public static void Create(int[] npc)
    {
        int n = npc.Length;
        var info = new NDArray[] { np.array(npc), 0, new NDArray[] { 0 } };
        var biases = npc.Skip(1).Select(size => np.random.randn(size)).ToArray();
        var weights = npc.Take(n - 1).Select((size, i) => 0.1 * np.random.randn(size, npc[i + 1])).ToArray();

        np.save("info.npy", info);
        np.save("weights.npy", weights);
        np.save("biases.npy", biases);
    }

    
    public static void Train(double lr, int tSamples, int nb, int nbbt = 1)
    {
        var dstrain = np.Load<double[][]>("train.npy").Select(arr => np.array(arr)).ToArray();
        var info = np.Load<NDArray[]>("info.npy").ToArray();
        double bestAcc = info[1];
        Open();
        for (int _ = 0; _ < nb; _++)
        {
            for (int __ = 0; __ < nbbt; __++)
            {
                List<int> choix = Enumerable.Range(0, 59999).OrderBy(x => new Random().Next()).Take(tSamples).ToList();
                NDArray inputs = np.array(choix.Select(i => dstrain[i][1]).ToArray());
                NDArray labels = np.eye(10)[choix.Select(i => dstrain[i][0]).ToArray()];
                NDArray output = ForPropagation(inputs);
                
                BackPropagation(output, labels, lr);
            }
            double acc = Accuracy();
            if (acc == 0 || bestAcc - acc > 0.3)
            {
                throw new ArgumentException("l'accuracy a chuté, diminuer le learning rate pourrait régler le problème");
            }
            info[2] = np.add(info[2],acc); // -> Trouver un moyen d'ajouter acc   
            
            if (acc > bestAcc)
            {
                Console.WriteLine($"l'accuracy a augmenté, la voici : {acc}");
                bestAcc = acc;
                info[1] = bestAcc;

                List<double[,]> bestWeights = new List<double[,]>();
                List<double[]> bestBiases = new List<double[]>();

                np.save("info.npy", info); for (int j = 0; j < nn.Item1.Count; j++)
                {
                    bestWeights.Add((double[,])nn.Item1[j].weights.Clone());
                    bestBiases.Add((double[])nn.Item1[j].biases.Clone());
                }
                bestWeights.Add((double[,])nn.Item2.weights.Clone());
                bestBiases.Add((double[])nn.Item2.biases.Clone());
                try
                {
                    np.save("weights.npy", bestWeights.ToArray()); // -> Faire en sorte qu'il croit qu'il est defini
                    np.save("biases.npy", bestBiases.ToArray()); // -> Idem
                }
                catch (NullReferenceException)
                {
                    throw new ArgumentException("pas de meilleure accuracy trouvé");
                }
            }
            else
            {
                Console.WriteLine($"l'accuracy n'a pas augmenté, la voici : {acc}");
            }
        }

        /*
         On enregistre le fichier ssi l'accuracy a augmentee
        info[1] = bestAcc;
        np.save("info.npy", info);
        try
        {
            np.save("weights.npy", bestWeights); // -> Faire en sorte qu'il croit qu'il est defini
            np.save("biases.npy", bestBiases); // -> Idem
        }
        catch (NullReferenceException)
        {
            throw new ArgumentException("pas de meilleure accuracy trouvé");
        }*/
    }

    public static double Accuracy()
    {
        NDArray output = ForPropagation(accInput);
        return np.mean(np.argmax(output, axis: 1) == accLabel);
    }


    public static NDArray ForPropagation(NDArray input)
    {
        nn.Item1[0].Forward(input);
        for (int i = 1; i < nn.Item1.Count-1; i++) // Adding all the values inside of FirstLayer
        {
            nn.Item1[i].Forward(nn.Item1[i - 1].output);
        }

        nn.Item2.Forward(nn.Item1[nn.Item1.Count - 1].output);

        return nn.Item2.output;
    }

    public static void BackPropagation(NDArray yPred, NDArray yTrue, double lr)
    {
        // Your implementation for the BackPropagation function
        nn.Item2.LastLayerBackward(yPred, yTrue, lr); // -> FAIRE EN SORTE QU'IL RECONNAISSE QUE LE TYPE DE nn.LAST SOIT LastLayer
        nn.Item1[nn.Item1.Count - 1].FirstLayerBackward(nn.Item2.outputGradient, lr);
        for (int i = nn.Item1.Count - 3 /*Pour pou*/; i >= 0; i--)
        {
            nn.Item1[i].FirstLayerBackward(nn.Item1[i + 1].outputGradient, lr);
        }
    }
}
}
