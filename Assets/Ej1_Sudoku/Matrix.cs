﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;



public class Matrix<T>/* : IEnumerable<T> */{

    //   int _width;
    //   int _height;
    //   int _capacity;
    //   T[,] data;
    //   Matrix<T> auxData;
    //   public int Width { get { return data.GetLength(0); } }

    //   public int Height { get { return data.GetLength(1); } }

    //   public int Capacity { get { return _capacity; } }

    //   public Matrix(int width, int height) {
    //       _width = width;
    //       _height = height;
    //       _capacity = _width * _height;
    //       data = new T[_width, _height];
    //   }

    //public Matrix(int width, int height, T defaultValue) {
    //       /* _width = copyFrom.GetLength(0);
    //        _height = copyFrom.GetLength(1);
    //        _capacity = _width * _height;
    //        data = new T[_width, _height];
    //        for (int y = 0; y < _height; y++)
    //        {
    //            for (int x = 0; x < _width; x++)
    //            {
    //                data[x, y] = copyFrom[x, y];
    //            }
    //        }*/
    //       data = new T[width, height];

    //       for (var y = 0; y < height; y++)
    //       {
    //           for (var x = 0; x < width; x++)
    //           {
    //               data[x, y] = defaultValue;
    //           }
    //       }
    //   }

    //public Matrix<T> Clone() {
    //       Matrix<T> aux = new Matrix<T>(Width, Height);
    //       for (int y = 0; y < _height; y++)
    //       {
    //           for (int x = 0; x < _width; x++)
    //           {
    //               aux.data[x, y] = data[x, y];
    //           }
    //       }
    //       return aux;
    //   }

    //public void SetRangeTo(int x0, int y0, int x1, int y1, T item) {
    //       for (int y = y0; y < y1; y++)
    //       {
    //           for (int x = x0; x < x1; x++)
    //           {
    //               data[x, y] = item;
    //           }
    //       }
    //   }

    //public List<T> GetRange(int x0, int y0, int x1, int y1) {
    //       List<T> l = new List<T>();

    //       for (int y = y0; y < y1; y++)
    //       {
    //           for (int x = x0; x < x1; x++)
    //           {
    //               l.Add(data[x, y]);
    //           }
    //       }
    //       return l;
    //}


    //   public T this[int x, int y] {
    //	get {
    //		return data[x,y];
    //	}
    //	set {
    //           data[x, y] = value;
    //	}
    //}



    //public IEnumerator<T> GetEnumerator() {
    //       for (int y = 0; y < _height; y++)
    //       {
    //           for (int x = 0; x < _width; x++)
    //           {
    //               if (data[x, y] == null) yield return default(T);
    //               else yield return data[x, y];
    //           }
    //       }
    //   }

    //IEnumerator IEnumerable.GetEnumerator() {
    //	return GetEnumerator();
    //}

    T[,] _matrix;

    public int Width
    {
        get
        {
            return _matrix.GetLength(0);
        }
    }

    public int Height
    {
        get
        {
            return _matrix.GetLength(1);
        }
    }

    public Matrix(int width, int height, T defaultValue)
    {
        _matrix = new T[width, height];

        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
            {
                _matrix[x, y] = defaultValue;
            }
        }
    }

    public Matrix(T[,] copyFrom)
    {
        _matrix = new T[copyFrom.GetLength(0), copyFrom.GetLength(1)];

        for (var y = 0; y < _matrix.GetLength(1); y++)
        {
            for (var x = 0; x < _matrix.GetLength(0); x++)
            {
                _matrix[x, y] = copyFrom[x, y];
            }
        }
    }

    public Matrix(Matrix<T> copyFrom)
    {
        _matrix = new T[copyFrom.Width, copyFrom.Height];

        for (var y = 0; y < copyFrom.Height; y++)
        {
            for (var x = 0; x < copyFrom.Width; x++)
            {
                _matrix[x, y] = copyFrom[x, y];
            }
        }
    }

    public Matrix<T> Clone()
    {
        return new Matrix<T>(_matrix);
    }

    public void SetRangeTo(int x0, int y0, int x1, int y1, T item)
    {
        for (var y = y0; y <= y1; y++)
        {
            for (var x = x0; x <= x1; x++)
            {
                _matrix[x, y] = item;
            }
        }
    }

    public List<T> GetRange(int x0, int y0, int x1, int y1)
    {
        var _list = new List<T>();

        for (var y = y0; y <= y1; y++)
        {
            for (var x = x0; x <= x1; x++)
            {
                _list.Add(_matrix[x, y]);
            }
        }

        return _list;
    }

    public T this[int x, int y]
    {
        get
        {
            return _matrix[x, y];
        }

        set
        {
            _matrix[x, y] = value;
        }
    }
}
