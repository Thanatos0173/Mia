using System;

namespace Celeste.Mod.Mia.Plage
{
    public static class PlageExtensions
    {
        public static void FillPlage<T>(this T[,] originalArray, int x, int y, int width, int height, T value)
        {
            int maxWidth = originalArray.GetLength(1);
            int maxHeight = originalArray.GetLength(0);
            for (int j = y; j < Math.Min(y + height + 1,maxHeight); j++)
            {
                for (int i = x; i < Math.Min(x + width + 1,maxWidth); i++)
                {
                    originalArray[i, j] = value;
                }
            }
        }
    }
}
