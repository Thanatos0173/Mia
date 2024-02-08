using NumSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Celeste.Mod.Mia;

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
                this.inputs = inputs;
                this.outputNotActivated = np.dot(inputs, this.weights) + this.biases; //np.dot
                this.output = np.maximum(0, this.outputNotActivated);
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
        private static NDArray accInput;
        private static NDArray accLabel;
        //private static NDArray dstest;

        private static void CreateFileIfNotExist(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    // Create the file and close it immediately
                    using (FileStream fs = File.Create(filePath)) { }
                    
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating file {filePath}: {ex.Message}");
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

            //            np.Load<double[]>(weightsPath).Select(arr => np.array(arr)).ToList();
            //          var biases = np.Load<double[]>(biasesPath).Select(arr => np.array(arr)).ToList();
            //dstest = np.Load<double[][]>(testPath).Select(arr => np.array(arr)).ToArray();
            int n = weights.Count;
            //accInput = np.array(dstest.Select(item => item[1]).ToArray()); // Comprendre le 
            //accLabel = np.array(dstest.Select(item => item[0]).ToArray()); // probleme

            nn = new Tuple<List<FirstLayers>, LastLayer>(
                new List<FirstLayers>(), 
                new LastLayer(weights[n - 1], biases[n - 1])); // I gave it a new value... We'll see.
            for (int j = 0; j < n - 1; j++)
            {
                nn.Item1.Add(new FirstLayers(weights[j], biases[j]));
            }


        }

        public static void Create(int[] npc, Monocle.Commands command)
        {
            int n = npc.Length;
            // Adjust the info object to be a more straightforward structure.
            // Since it's a mix of different types, consider breaking it down or ensuring it's compatible with NumSharp's capabilities.
            var npcArray = np.array(npc); // Convert to NDArray if needed.
            var zeroArray = np.array(new int[] { 0 }); // Convert to NDArray if needed.
                                                       // Combine into a 2D object array if NumSharp supports it. Otherwise, consider saving these parts separately.
            Tuple<int[], int[], int[]> info = new Tuple<int[], int[], int[]>
            (
                npc,
                new int[] { },
                new int[] {0}
            );

            //            var biases = new NDArray[n - 1];
            //var weights = np.zeros(n - 1,n-1);

            command.Log("Checking if all dirs exist...");
            Directory.CreateDirectory("Mia");
            Directory.CreateDirectory("Mia/weights");
            Directory.CreateDirectory("Mia/biases");
            Directory.CreateDirectory("Mia/info");
            command.Log("Check terminated. Directory might have been added.");

            command.Log("Making shit for weights and biases");
            for (int j = 0; j < n - 1; j++)
            {
                var biases = np.random.randn(npc[j + 1]);
                var weights = 0.1 * np.random.randn(npc[j], npc[j + 1]);
                Console.WriteLine($"Weights number {j} : {weights.Shape}");
                CreateFileIfNotExist($"Mia/weights/weights_{j}.npy");
                np.Save((Array)weights, $"Mia/weights/weights_{j}.npy");
                CreateFileIfNotExist($"Mia/biases/biaises_{j}.npy");
                np.Save((Array)biases, $"Mia/biases/biaises_{j}.npy");  

            }
            command.Log("Done");
            command.Log("Doing some shit with info");
            np.Save(info.Item1,"Mia/info/info_npc");
            np.Save(info.Item2, "Mia/info/info_old_scores");
            np.Save((Array)info.Item3, "Mia/info/info_actual_score");

            command.Log("Finished.");
           
            
            /*try
            {
                // Read the JSON from the file
                string json = File.ReadAllText("Mia/weights.npy");

                // Deserialize the JSON to NDArray[]
                NDArray[] loadedArray = JsonConvert.DeserializeObject<NDArray[]>(json);
                Console.WriteLine(loadedArray[0].GetType());

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            //np.save("Mia/weights.npy", weights);
            np.save("Mia/biases.npy", biases);*/
        }


        public static void Train(double lr, int[]  allTiles, int[] keypress)
        {
            //            CreateFileIfNotExist("Mia/info.npy");
            //            var info = np.Load<NDArray[]>("Mia/info.npy").ToArray();
            //double bestAcc = info[1];

            Console.WriteLine(Main.UwuInt);
            Main.UwuInt++;

            NDArray inputs = new NDArray(allTiles);
            NDArray trueInputs = inputs.reshape(1,400);
            NDArray labels = new NDArray(keypress);
            CreateFileIfNotExist($"Mia/inputs.npy");
            np.Save((Array)inputs, "Mia/inputs.npy");

            NDArray output = ForPropagation(trueInputs);

           BackPropagation(output, labels, lr);

            
            /*double acc = Accuracy();
            if (acc == 0 || bestAcc - acc > 0.3)
            {
                throw new ArgumentException("l'accuracy a chuté, diminuer le learning rate pourrait régler le problème");
            }
            info[2] = np.add(info[2], acc);
               
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
            }*/


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

        public static void Save()
        {
            DateTime currentDateTime = DateTime.Now;

            // Extract date and time components
            string currentDate = currentDateTime.ToString("yyyyMMdd");
            string currentTime = currentDateTime.ToString("HHmmss");
            
            Directory.CreateDirectory($"Mia/mia_save{currentDate}_{currentTime}");
            UtilsClass.Utils.MoveDirectory("Mia/weights", $"Mia/mia_save{currentDate}_{currentTime}");
            UtilsClass.Utils.MoveDirectory("Mia/biases", $"Mia/mia_save{currentDate}_{currentTime}");
            Directory.CreateDirectory("Mia/weights");
            Directory.CreateDirectory("Mia/biases");
            for (int j = 0; j < nn.Item1.Count ; j++)
            {
                var biases = nn.Item1[j].biases;
                var weights = nn.Item1[j].weights;
                CreateFileIfNotExist($"Mia/weights/weights_{j}.npy");
                np.Save((Array)weights, $"Mia/weights/weights_{j}.npy");
                CreateFileIfNotExist($"Mia/biases/biaises_{j}.npy");
                np.Save((Array)biases, $"Mia/biases/biaises_{j}.npy");

            }
            CreateFileIfNotExist($"Mia/weights/weights_{nn.Item1.Count}.npy");
            np.Save((Array)nn.Item2.weights, $"Mia/weights/weights_{nn.Item1.Count}.npy");
            CreateFileIfNotExist($"Mia/biases/biaises_{nn.Item1.Count}.npy");
            np.Save((Array)nn.Item2.biases, $"Mia/biases/biaises_{nn.Item1.Count}.npy");



        }

        public static NDArray ForPropagation(NDArray input)
        {
            nn.Item1[0].Forward(input);
            Console.WriteLine(nn.Item1.Count);
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
