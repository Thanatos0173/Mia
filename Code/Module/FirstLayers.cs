using FMOD;
using NumSharp;

class FirstLayer
{
    private NDArray weights;
    private NDArray biases;
    private NDArray inputs;
    private NDArray outputNotActivated;
    private NDArray output;
    private NDArray outputGradient;
    public FirstLayer(double[,] weights, double[] biases)
    {
        this.weights = np.array(weights, dtype: np.float64);
        this.biases = np.array(biases, dtype: np.float64);
    }

    public void Forward(NDArray inputs)
    {
        this.inputs = inputs;
        this.outputNotActivated = np.dot(inputs, this.weights) + this.biases;
        this.output = np.maximum(0, this.outputNotActivated);
    }

    public void Backward(NDArray inputGradient, double learningRate)
    {
        inputGradient = inputGradient * 
            DerivRelu(this.outputNotActivated);
        this.outputGradient = np.dot(inputGradient, this.weights.T);
        this.weights -=           
            np.dot(
            this.inputs.T,
            inputGradient) * learningRate / inputGradient.shape[0];
        this.biases -= np.mean(inputGradient, axis: 0) * learningRate;
    }

    private NDArray DerivRelu(NDArray x)
    {
        return np.zeros_like(x)[x > 0] = 1;
    }

}

class LastLayer
{
    private NDArray weights;
    private NDArray biases;
    private NDArray inputs;
    private NDArray output;
    private NDArray outputGradient;


    public LastLayer(double[,] weights, double[] biases)
    {
        this.weights = np.array(weights, dtype: np.float64);
        this.biases = np.array(biases, dtype: np.float64);
    }

    public void Forward(NDArray inputs)
    {
        this.inputs = inputs;
        NDArray outputNotActivated = np.dot(inputs, this.weights) + this.biases;
        NDArray maxValues = np.max(outputNotActivated, axis: 1, keepdims: true);
        NDArray expValues = np.exp(outputNotActivated - maxValues);
        this.output = expValues / np.sum(expValues, axis: 1, keepdims: true);
    }

    public void Backward(NDArray yPred, NDArray yTrue, double learningRate)
    {
        NDArray inputGradient = yPred - yTrue;
        this.outputGradient = np.dot(inputGradient, this.weights.T);
        this.weights -= np.dot(this.inputs.T, inputGradient) * learningRate * 1.0 / inputGradient.shape[0];
        this.biases -= np.mean(inputGradient, axis: 0) * learningRate;
    }
}
