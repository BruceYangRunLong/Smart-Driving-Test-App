using System.Collections.ObjectModel;
using MathNet.Numerics;

namespace Webiat;

public class PredictService
{
    public PredictService() { }

    public double Calculate(ObservableCollection<double> numbers)
    {
        int order = 3;
        int sum = numbers.Count;
        double[] s;
        double[] X = new double[sum];
        double[] Y = new double[sum];
        for (int i = 0; i < sum; i++)
        {
            X[i] = i + 1;
            Y[i] = numbers[i];
        }
        s = Fit.Polynomial(X, Y, order);
        double y = 0;
        for (int i = 0; i <= order; i++)
        {
            y+= s[i] * Math.Pow(sum+1, i);
        }
        return y;
    }
}