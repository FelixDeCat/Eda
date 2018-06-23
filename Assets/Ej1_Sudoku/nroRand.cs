using System;
using System.Collections.Generic;

class nroRandom
{
    static Random _random = new Random();

    public static bool Unique(IEnumerable<int> collection)
    {
        var n = 0;

        for (var i = 1; i <= 9; i++)
        {
            foreach (var item in collection)
            {
                if (item != Cell.EMPTY)
                {
                    if (item == i)
                    {
                        n++;
                    }

                    if (n > 1)
                    {
                        return false;
                    }
                }
            }

            n = 0;
        }

        return true;
    }

    /*public static T ExtractRandom<T>(List<T> list)
    {
        var i = _random.Next(list.Count);

        if (!list[i].Equals(Cell.EMPTY))
        {
            return list[i];
        }

        else
        {
            return ExtractRandom(list);
        }
    }
    */
    public static void Shuffle<T>(IList<T> list)
    {
        var n = list.Count;

        for (var i = 0; i < n; i++)
        {
            var j = _random.Next(i, n);

            Swap(list, i, j);
        }
    }

    public static void Swap<T>(IList<T> list, int i, int j)
    {
        var aux = list[i];
        list[i] = list[j];
        list[j] = aux;
    }
    // convierte una lista a una matriz (simetrica)
    public static Matrix<int> ToMatrix(List<int> list)
    {
        if (list.Count == 0)
        {
            return null;
        }

        var sqrt = Math.Sqrt(list.Count);

        Matrix<int> _matrix;
        // checkeo que la raiz cuadrada sea un numero entero (o sea, que la matriz es simetica)
        if (sqrt % 1 == 0)
        {
            _matrix = new Matrix<int>((int)sqrt, (int)sqrt, Cell.EMPTY);

            var i = 0;
            // paso los elementos de la lista a la matriz (uno por uno de izq a der y de arriba a abajo)
            for (var y = 0; y < _matrix.Height; y++)
            {
                for (var x = 0; x < _matrix.Width; x++)
                {
                    _matrix[x, y] = list[i];

                    i++;
                }
            }
        }

        else
        {
            throw new Exception("The list cannot be converted into a symmetric matrix.");
        }

        return _matrix;
    }
}