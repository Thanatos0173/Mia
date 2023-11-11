using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using YamlDotNet.Core.Tokens;

namespace Celeste.Mod.Mia.Custom
{
    public class CustomMatrix
    {
        public List<List<float>> List {  get; set; }
        public int I {  get; set; }
        public int J { get; set; }


        public static void EditMatrix(CustomMatrix matrix, CustomMatrix newMatrix)
        {
            matrix.List.Clear();
            for(int i = 0; i < newMatrix.I; i++)
            {
                matrix.List.Add(newMatrix.List[i]);
            }
        }

        public static CustomMatrix MultiplyMatrix(CustomMatrix matrix1, CustomMatrix matrix2)
        {
            // Cause we don't know the size of the matrix, we'll just iterate on all the values until we get an NaN
            // Size of the first matrix : (i,j). Size of the second matrix : (k,l)
            // We'll always check if j is equal to k. If it's not, we'll send an error
            if (matrix1.J != matrix2.I) throw new Exception("Math error : Thoses matrix can't be multiplied : Nonmatching size");
            CustomMatrix toReturn = new CustomMatrix
            {
                List = new List<List<float>>(),
                I = matrix1.I,
                J = matrix2.J,
            };

            for (int i = 0; i < matrix1.I; i++)
            {
                toReturn.List.Add(new List<float>() { });
                for (int j = 0; j < matrix2.J; j++)
                {
                    float value = 0;
                    for (int k = 0; k < matrix1.J; k++)
                    {
                        if (GetValue(matrix1, i, k) == float.NaN || GetValue(matrix2, k, j) == float.NaN)
                        {
                            throw new Exception("Math error : Thoses matrix can't be multiplied : NaN where a value should be present");
                        }
                        value += GetValue(matrix1, i, k) * GetValue(matrix2, k, j);
                    }
                    toReturn.List[i].Add(value);
                }
            }
            return toReturn;
        }

        public static CustomMatrix MultiplyElemByElemMatrix(CustomMatrix matrix1, CustomMatrix matrix2)
        {
            // Cause we don't know the size of the matrix, we'll just iterate on all the values until we get an NaN
            // Size of the first matrix : (i,j). Size of the second matrix : (k,l)
            // We'll always check if j is equal to k. If it's not, we'll send an error
            if (matrix1.I != matrix2.I || matrix2.J != matrix2.J) throw new Exception("Math error : Thoses matrix can't be summed : Nonmatching size");
            CustomMatrix toReturn = new CustomMatrix
            {
                List = new List<List<float>>(),
                I = matrix1.I,
                J = matrix1.J,
            };

            for (int i = 0; i < matrix1.I; i++)
            {
                toReturn.List.Add(new List<float>() { });
                for (int j = 0; j < matrix1.J; j++)
                {
                    if (GetValue(matrix1, i, j) == float.NaN || GetValue(matrix2, i, j) == float.NaN)
                    {
                        throw new Exception("Math error : Thoses matrix can't be summed : NaN where a value should be present");
                    }
                    toReturn.List[i].Add(GetValue(matrix1, i, j) * GetValue(matrix2, i, j));
                }
            }
            return toReturn;
        }

        public static CustomMatrix MultiplyByFloat(CustomMatrix matrix, float factor)
        {
            CustomMatrix toReturn = new CustomMatrix
            {
                List = new List<List<float>>(),
                I = matrix.I,
                J = matrix.J,
            };

            for (int i = 0; i < matrix.I; i++)
            {
                toReturn.List.Add(new List<float>() { });
                for (int j = 0; j < matrix.J; j++)
                {
                    if (GetValue(matrix, i, j) == float.NaN)
                    {
                        throw new Exception("Math error : Thoses matrix can't be summed : NaN where a value should be present");
                    }
                    toReturn.List[i].Add(factor* GetValue(matrix, i, j));
                }
            }
            return toReturn;
        }

        public static CustomMatrix MeanAxis0(CustomMatrix matrix)
        {
            CustomMatrix toReturn = new CustomMatrix
            {
                List = new List<List<float>>(),
                I = matrix.I,
                J = 1,
            };

            for (int i = 0; i < matrix.I; i++)
            {
                toReturn.List.Add(new List<float>() { });
                for (int j = 0; j < matrix.J; j++)
                {
                    if (GetValue(matrix, i, j) == float.NaN)
                    {
                        throw new Exception("Math error : Thoses matrix can't be summed : NaN where a value should be present");
                    }
                    toReturn.List[i].Add(factor * GetValue(matrix, i, j));
                }
            }
            return toReturn;
        }



        public static CustomMatrix ReluMaxMatrix(int value,CustomMatrix matrix1)
        {
            // Cause we don't know the size of the matrix, we'll just iterate on all the values until we get an NaN
            // Size of the first matrix : (i,j). Size of the second matrix : (k,l)
            // We'll always check if j is equal to k. If it's not, we'll send an error
            CustomMatrix toReturn = new CustomMatrix
            {
                List = new List<List<float>>(),
                I = matrix1.I,
                J = matrix1.J,
            };

            for (int i = 0; i < matrix1.I; i++)
            {
                toReturn.List.Add(new List<float>() { });
                for (int j = 0; j < matrix1.J; j++)
                {
                    if (GetValue(matrix1, i, j) == float.NaN)
                    {
                        throw new Exception("Math error : Thoses matrix can't be summed : NaN where a value should be present");
                    }
                    toReturn.List[i].Add(value > GetValue(matrix1, i, j) ? value : GetValue(matrix1,i,j));
                }
            }
            return toReturn;
        }

        public static CustomMatrix DerivReluMaxMatrix(CustomMatrix matrix1)
        {
            // Cause we don't know the size of the matrix, we'll just iterate on all the values until we get an NaN
            // Size of the first matrix : (i,j). Size of the second matrix : (k,l)
            // We'll always check if j is equal to k. If it's not, we'll send an error
            CustomMatrix toReturn = new CustomMatrix
            {
                List = new List<List<float>>(),
                I = matrix1.I,
                J = matrix1.J,
            };

            for (int i = 0; i < matrix1.I; i++)
            {
                toReturn.List.Add(new List<float>() { });
                for (int j = 0; j < matrix1.J; j++)
                {
                    if (GetValue(matrix1, i, j) == float.NaN)
                    {
                        throw new Exception("Math error : Thoses matrix can't be summed : NaN where a value should be present");
                    }
                    toReturn.List[i].Add(0 > GetValue(matrix1, i, j) ? 1 : 0);
                }
            }
            return toReturn;
        }

        public static CustomMatrix Transpose(CustomMatrix matrix1)
        {
            // Cause we don't know the size of the matrix, we'll just iterate on all the values until we get an NaN
            // Size of the first matrix : (i,j). Size of the second matrix : (k,l)
            // We'll always check if j is equal to k. If it's not, we'll send an error
            CustomMatrix toReturn = new CustomMatrix
            {
                List = new List<List<float>>(),
                I = matrix1.I,
                J = matrix1.J,
            };

            for (int i = 0; i < matrix1.I; i++)
            {
                toReturn.List.Add(new List<float>() { });
                for (int j = 0; j < matrix1.J; j++)
                {
                    if (GetValue(matrix1, i, j) == float.NaN)
                    {
                        throw new Exception("Math error : Thoses matrix can't be summed : NaN where a value should be present");
                    }
                    toReturn.List[i].Add(GetValue(matrix1, j, i)) ;
                }
            }
            return toReturn;
        }

        public static float GetValue(CustomMatrix list, int i, int j)
        {
            if (i > list.I || j > list.J) return float.NaN;
            try { return list.List[i][j]; }
            catch (ArgumentOutOfRangeException) { return float.NaN; }
        }

    }
}
