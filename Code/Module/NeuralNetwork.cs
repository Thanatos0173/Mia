using NumSharp;
using System;
using System.Collections.Generic;
using System.IO;
using Celeste.Mod.Mia.UtilsClass;

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
                //   { 400, 2048, 1024, 56 }
                try 
                {
                    this.inputs = inputs;
                    this.outputNotActivated = np.dot(inputs, this.weights) + this.biases; //np.dot
                    this.output = np.maximum(0, this.outputNotActivated);

                }
                catch (Exception)
                {
                    CreateFileIfNotExist("Mia/test.npy");
                    np.Save((Array)inputs,"Mia/test.npy");
                }
            }

            public void FirstLayerBackward(NDArray inputGradient, double learningRate)
            {
                bool one = DerivRelu(this.outputNotActivated) == null;
                inputGradient = inputGradient * DerivRelu(this.outputNotActivated);
                this.outputGradient = np.dot(inputGradient, this.weights.T);
                this.weights -= np.dot(this.inputs.T, inputGradient) * learningRate / inputGradient.shape[0];
                this.biases -= np.mean(inputGradient, axis: 0) * learningRate;
            }

            public NDArray DerivRelu(NDArray x)
            {
                NDArray result = np.zeros(x.Shape);
                for (int i = 0; i < x.Shape[0]; i++)
                {
                    for (int j = 0; j < x.Shape[1]; j++)
                    {
                        result[i, j] = (x[i, j].Data<double>()[0] > 0 ? 1 : 0);
                    }
                }
                return result;
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
                this.output = expValues / np.sum(expValues.astype(NPTypeCode.Float), axis: 1, keepdims: true);
            }

            public void LastLayerBackward(NDArray yPred, NDArray yTrue, double learningRate)
            {
                NDArray inputGradient = yPred - yTrue;
                this.outputGradient = np.dot(inputGradient, this.weights.T);
                this.weights -= np.dot(this.inputs.T, inputGradient) * learningRate  / inputGradient.shape[0];
                this.biases -= np.mean(inputGradient, axis: 0) * learningRate;
            }
        }

        private static Tuple<List<FirstLayers>, LastLayer> nn;

        private static void CreateFileIfNotExist(string filePath)
        {
                if (!File.Exists(filePath))
                {
                    // Create the file and close it immediately
                    using (FileStream fs = File.Create(filePath)) { }
                    
                }

        }
        public static void Open()
        {



            var weights = new List<NDArray> ();
            var biases = new List<NDArray> ();
            foreach(string file in Directory.GetFiles("Mia/weights"))
            {
                weights.Add(np.load (file));
            }
            foreach (string file in Directory.GetFiles("Mia/biases"))
            {
                biases.Add(np.load(file));
            }
            int n = weights.Count;

            nn = new Tuple<List<FirstLayers>, LastLayer>(
                new List<FirstLayers>(), 
                new LastLayer(weights[n - 1], biases[n - 1])); // I gave it a new value... We'll see.
            for (int j = 0; j < n - 1; j++)
            {
                nn.Item1.Add(new FirstLayers(weights[j], biases[j]));
            }


        }

        public static void Create(List<int> npc, Monocle.Commands command)
        {
            int n = npc.Count;
            var npcArray = np.array(npc); // Convert to NDArray if needed.
            var zeroArray = np.array(new int[] { 0 }); // Convert to NDArray if needed.
                                                       // Combine into a 2D object array if NumSharp supports it. Otherwise, consider saving these parts separately.
            Tuple<int[], int[], int[]> info = new Tuple<int[], int[], int[]>
            (
                npc.ToArray(),
                new int[] { },
                new int[] {0}
            );


            Directory.CreateDirectory("Mia");
            if(Directory.Exists("Mia/weights") || Directory.Exists("Mia/biases")) 
            {
                command.Log("Some directories already exist.");
                command.Log("The old structure will be saved, and a new one will be created");
                DateTime currentDateTime = DateTime.Now;

                string currentDate = currentDateTime.ToString("yyyyMMdd");
                string currentTime = currentDateTime.ToString("HHmmss");

                Directory.CreateDirectory($"Mia/mia_save{currentDate}_{currentTime}");

                Utils.MoveDirectoryAndDeleteOld("Mia/weights", $"Mia/mia_save{currentDate}_{currentTime}");
                Utils.MoveDirectoryAndDeleteOld("Mia/biases", $"Mia/mia_save{currentDate}_{currentTime}");

            }


            Directory.CreateDirectory("Mia/weights");
            Directory.CreateDirectory("Mia/biases");
            Directory.CreateDirectory("Mia/info");

            for (int j = 0; j < n - 1; j++)
            {
                var biases = np.random.randn(npc[j + 1]);
                var weights = 0.1 * np.random.randn(npc[j], npc[j + 1]);
                Console.WriteLine($"Weights number {j} : {weights.Shape}");
                CreateFileIfNotExist($"Mia/weights/weights_{j}.npy");
                np.Save((Array)weights, $"Mia/weights/weights_{j}.npy");
                CreateFileIfNotExist($"Mia/biases/biases_{j}.npy");
                np.Save((Array)biases, $"Mia/biases/biases_{j}.npy");
            }
            np.Save(info.Item1, "Mia/info/info_npc");
            np.Save(info.Item2, "Mia/info/info_old_scores");
            np.Save((Array)info.Item3, "Mia/info/info_actual_score");

            command.Log("Finished.");



            
            

        }

        public static void Save()
        {
            DateTime currentDateTime = DateTime.Now;

            string currentDate = currentDateTime.ToString("yyyyMMdd");
            string currentTime = currentDateTime.ToString("HHmmss");
            
            Directory.CreateDirectory($"Mia/mia_save{currentDate}_{currentTime}");
            UtilsClass.Utils.CopyDirectory("Mia/weights", $"Mia/mia_save{currentDate}_{currentTime}");
            UtilsClass.Utils.CopyDirectory("Mia/biases", $"Mia/mia_save{currentDate}_{currentTime}");
        }

        public static NDArray ForPropagation(NDArray input)
        {
            nn.Item1[0].Forward(input);
            for (int i = 1; i < nn.Item1.Count; i++) // Adding all the values inside of FirstLayer
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
