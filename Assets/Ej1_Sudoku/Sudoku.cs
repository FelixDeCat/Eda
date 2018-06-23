using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

public class Sudoku : MonoBehaviour
{
    public Cell prefabCell;
    public Canvas canvas;
    public Text feedback;
    public float stepDuration = 0.05f;
    [Range(1, 81)] public int difficulty = 20;

    Matrix<Cell> _board;
    Matrix<int> _createdMatrix;
    List<int> posibles = new List<int>();
    int _smallSide;
    int _bigSide;
    string memory = "";
    string canSolve = "";
    bool canPlayMusic = false;
    List<int> nums = new List<int>();

    int watchdog = 0;

    float r = 1.0594f;
    float frequency = 440;
    float gain = 0.5f;
    float increment;
    float phase;
    float samplingF = 48000;


    void Start()
    {
        long mem = System.GC.GetTotalMemory(true);
        feedback.text = string.Format("MEM: {0:f2}MB", mem / (1024f * 1024f));
        memory = feedback.text;
        _smallSide = 3;
        _bigSide = _smallSide * 3;
        frequency = frequency * Mathf.Pow(r, 2);
        CreateEmptyBoard();
        ClearBoard();
    }

    void ClearBoard()
    {
        _createdMatrix = new Matrix<int>(_bigSide, _bigSide, Cell.EMPTY);
        SetBoardFromIntMatrix(_createdMatrix, false);

        for (var y = 0; y < _board.Height; y++)
        {
            for (int x = 0; x < _board.Width; x++)
            {
                var cell = _board[x, y];
                cell.number = Cell.EMPTY;
                cell.locked = cell.invalid = false;
            }

        }
        /*foreach(var cell in _board) {
			cell.number = 0;
			cell.locked = cell.invalid = false;
		}*/
    }
    void SetBoardFromIntMatrix(Matrix<int> mtx, bool lockNonEmpty)
    {
        for (var y = 0; y < mtx.Height; y++)
        {
            for (var x = 0; x < mtx.Width; x++)
            {
                _board[x, y].number = mtx[x, y];

                if (!_board[x, y].isEmpty && lockNonEmpty)
                {
                    _board[x, y].locked = true;
                }
            }
        }
    }

    void CreateEmptyBoard()
    {
        float spacing = 68f;
        float startX = -spacing * 4f;
        float startY = spacing * 4f;

        _board = new Matrix<Cell>(_bigSide, _bigSide, null);
        for (int x = 0; x < _board.Width; x++)
        {
            for (int y = 0; y < _board.Height; y++)
            {
                var cell = _board[x, y] = Instantiate(prefabCell);
                cell.transform.SetParent(canvas.transform, false);
                cell.transform.localPosition = new Vector3(startX + x * spacing, startY - y * spacing, 0);
            }
        }
    }
    bool CanPlaceValue(Matrix<int> matrix, Matrix<bool> invalids = null)
    {
        for (var j = 0; j < matrix.Height; j = j + _smallSide)
        {
            for (var i = 0; i < matrix.Width; i = i + _smallSide)
            {
                if (!nroRandom.Unique(matrix.GetRange(i, j, i + _smallSide - 1, j + _smallSide - 1)))
                {
                    return false;
                }
            }
        }

        for (var y = 0; y < matrix.Height; y++)
        {
            for (var x = 0; x < matrix.Width; x++)
            {
                if (!nroRandom.Unique(matrix.GetRange(0, y, matrix.Width - 1, y)) || !nroRandom.Unique(matrix.GetRange(x, 0, x, matrix.Height - 1)))
                {
                    return false;
                }
            }
        }

        return true;
    }



    //int watchdog = 0;
    bool RecuSolve(Matrix<int> matrixParent, int x, int y/*, int protectMaxDepth*/, List<Matrix<int>> solution)
    {
        if (watchdog < 1)
        {
            return false;
        }
        else
        {
            watchdog--;
        }
        var _clone = matrixParent.Clone();

        solution.Add(_clone);

        var _numbers = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 };

        nroRandom.Shuffle(_numbers);
        if (!CanPlaceValue(_clone, x, y))
        {
            return false;
        }
        else if (x == (_clone.Width - 1) && y == (_clone.Height - 1) && CanPlaceValue(_clone, x, y) && _clone[x, y] != Cell.EMPTY)
        {
            return true;
        }
        else
        {
            if (_clone[x, y] == Cell.EMPTY)
            {
                for (var i = 0; i < _numbers.Length; i++)
                {
                    _clone[x, y] = _numbers[i];
                    if (RecuSolve(_clone, x, y, solution))
                    {
                        return true;
                    }
                }
            }
            else
            {
                if (x < (_clone.Width - 1))
                {
                    x = x + 1;
                }
                else
                {
                    x = 0;
                    if (y < (_clone.Height - 1))
                    {
                        y = y + 1;
                    }
                }
            }
            return RecuSolve(_clone, x, y, solution);
        }
    }



    void OnAudioFilterRead(float[] array, int channels)
    {
        if (canPlayMusic)
        {
            increment = frequency * Mathf.PI / samplingF;
            for (int i = 0; i < array.Length; i++)
            {
                phase = phase + increment;
                array[i] = (float)(gain * Mathf.Sin((float)phase));
            }
        }

    }
    void changeFreq(int num)
    {
        frequency = 440 + num * 80;
    }

    IEnumerator ShowSequence(List<Matrix<int>> seq)
    {
        yield return new WaitForSeconds(0);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R) || Input.GetMouseButtonDown(1))
        {
            //StopAllCoroutines();
            //watchdog = 100000;
            //var _solution = new List<Matrix<int>>();
            //RecuSolve(_createdMatrix, 0, 0, _solutions);

            // StartCoroutine(ShowSequence(_solutions));

        }
        // SolvedSudoku();
        else if (Input.GetKeyDown(KeyCode.C) || Input.GetMouseButtonDown(0))
            CreateSudoku();


    }

    void SolvedSudoku()
    {
        StopAllCoroutines();
        nums = new List<int>();
        var solution = new List<Matrix<int>>();
        watchdog = 100000;
        var result = false;//????
        long mem = System.GC.GetTotalMemory(true);
        memory = string.Format("MEM: {0:f2}MB", mem / (1024f * 1024f));
        canSolve = result ? " VALID" : " INVALID";
    }

    void CreateSudoku()
    {
        StopAllCoroutines();
        canPlayMusic = false;
        ClearBoard();
        watchdog = 100000;
        var _solutions = new List<Matrix<int>>();
        RecuSolve(_createdMatrix, 0, 0, _solutions);
        _createdMatrix = ApplyDifficulty(_solutions[_solutions.Count - 1], difficulty);
        SetBoardFromIntMatrix(_createdMatrix, true);
        // GenerateValidLine(_createdMatrix, 0, 0);
        //var result =false;//??????
        //_createdMatrix = l[_createdMatrix.Capacity - 1].Clone();//??????
        // LockRandomCells();
        // ClearUnlocked(_createdMatrix);
        //TranslateAllValues(_createdMatrix);
        // long mem = System.GC.GetTotalMemory(true);
        //memory = string.Format("MEM: {0:f2}MB", mem / (1024f * 1024f));
        //canSolve = result ? " VALID" : " INVALID";
        //feedback.text = "Pasos: " + l.Count + "/" + l.Count + " - " + memory + " - " + canSolve;
        feedback.text = CanPlaceValue(_createdMatrix) ? "Valid board." : "Invalid board.";
    }

    Matrix<int> ApplyDifficulty(Matrix<int> _matrix, int _difficulty)
    {
        if (_difficulty > 81)
        {
            throw new ArgumentOutOfRangeException("Difficulty");
        }

        var _list = _matrix.GetRange(0, 0, 8, 8);

        var _positions = new int[81];

        for (var i = 0; i < _positions.Length; i++)
        {
            _positions[i] = i;
        }

        nroRandom.Shuffle(_positions);

        for (var i = 0; i < _difficulty; i++)
        {
            var n = _positions[i];

            _list[n] = Cell.EMPTY;
        }

        return _matrix = nroRandom.ToMatrix(_list);
    }

    void GenerateValidLine(Matrix<int> mtx, int x, int y)
    {
        int[] aux = new int[9];
        for (int i = 0; i < 9; i++)
        {
            aux[i] = i + 1;
        }
        int numAux = 0;
        for (int j = 0; j < aux.Length; j++)
        {
            int r = 1 + UnityEngine.Random.Range(j, aux.Length);
            numAux = aux[r - 1];
            aux[r - 1] = aux[j];
            aux[j] = numAux;
        }
        for (int k = 0; k < aux.Length; k++)
        {
            mtx[k, 0] = aux[k];
        }
    }


    void ClearUnlocked(Matrix<int> mtx)
    {
        for (int i = 0; i < _board.Height; i++)
        {
            for (int j = 0; j < _board.Width; j++)
            {
                if (!_board[j, i].locked)
                    mtx[j, i] = Cell.EMPTY;
            }
        }
    }

    void LockRandomCells()
    {
        List<Vector2> posibles = new List<Vector2>();
        for (int i = 0; i < _board.Height; i++)
        {
            for (int j = 0; j < _board.Width; j++)
            {
                if (!_board[j, i].locked)
                    posibles.Add(new Vector2(j, i));
            }
        }
        for (int k = 0; k < 82 - difficulty; k++)
        {
            int r = UnityEngine.Random.Range(0, posibles.Count);
            _board[(int)posibles[r].x, (int)posibles[r].y].locked = true;
            posibles.RemoveAt(r);
        }
    }

    void TranslateAllValues(Matrix<int> matrix)
    {
        for (int y = 0; y < _board.Height; y++)
        {
            for (int x = 0; x < _board.Width; x++)
            {
                _board[x, y].number = matrix[x, y];
            }
        }
    }

    void TranslateSpecific(int value, int x, int y)
    {
        _board[x, y].number = value;
    }

    void TranslateRange(int x0, int y0, int xf, int yf)
    {
        for (int x = x0; x < xf; x++)
        {
            for (int y = y0; y < yf; y++)
            {
                _board[x, y].number = _createdMatrix[x, y];
            }
        }
    }
    /*void CreateNew()
    {
        _createdMatrix = new Matrix<int>(Tests.validBoards[1]);
        TranslateAllValues(_createdMatrix);
    }*/

    bool CanPlaceValue(Matrix<int> mtx, /*int value,*/ int x, int y)
    {
        List<int> fila = new List<int>();
        List<int> columna = new List<int>();
        List<int> area = new List<int>();
        List<int> total = new List<int>();

        Vector2 cuadrante = Vector2.zero;

        /* for (int i = 0; i < mtx.height; i++)
         {
             for (int j = 0; j < mtx.width; j++)
             {
                 if (i != y && j == x) columna.Add(mtx[j, i]);
                 else if(i == y && j != x) fila.Add(mtx[j,i]);
             }
         }*/
        //
        if (!nroRandom.Unique(mtx.GetRange(0, y, mtx.Width - 1, y)))
            return false;

        if (!nroRandom.Unique(mtx.GetRange(x, 0, x, mtx.Height - 1)))
            return false;

        cuadrante.x = (int)(x / 3);

        if (x < 3)
        {
            //cuadrante.x = 0;
            if (!nroRandom.Unique(mtx.GetRange(0, 0, 2, 2)))
            {
                return false;
            }
        }

        else if (x < 6)
        {
            // cuadrante.x = 3;
            if (!nroRandom.Unique(mtx.GetRange(3, 0, 5, 2)))
            {
                return false;
            }
        }
        else
        {
            //cuadrante.x = 6;            
            if (!nroRandom.Unique(mtx.GetRange(6, 0, 8, 2)))
            {
                return false;
            }
        }
        //cuadrante.x = 6;

        if (y < 3)
        {
            //cuadrante.y = 0;
            if (!nroRandom.Unique(mtx.GetRange(0, 0, 2, 2)))
            {
                return false;
            }
        }

        else if (y < 6)
        {
            //cuadrante.y = 3;
            if (x < _smallSide)
            {
                if (!nroRandom.Unique(mtx.GetRange(0, 3, 2, 5)))
                {
                    return false;
                }
            }
            else if (x < _smallSide * 2)
            {
                if (!nroRandom.Unique(mtx.GetRange(3, 3, 5, 5)))
                {
                    return false;
                }
            }
            else

            // cuadrante.y = 6;
            {
                if (!nroRandom.Unique(mtx.GetRange(6, 3, 8, 5)))
                {
                    return false;
                }
            }

        }
        else
        {
            if (x < _smallSide)
            {
                if (!nroRandom.Unique(mtx.GetRange(0, 6, 2, 8)))
                {
                    return false;
                }
            }

            else if (x < _smallSide * 2)
            {
                if (!nroRandom.Unique(mtx.GetRange(3, 6, 5, 8)))
                {
                    return false;
                }
            }

            else
            {
                if (!nroRandom.Unique(mtx.GetRange(6, 6, 8, 8)))
                {
                    return false;
                }
            }
        }



        //  esto nose para que es
        area = mtx.GetRange((int)cuadrante.x, (int)cuadrante.y, (int)cuadrante.x + 3, (int)cuadrante.y + 3);
        total.AddRange(fila);
        total.AddRange(columna);
        total.AddRange(area);
        total = FilterZeros(total);

        // if (total.Contains(value))
        //    return false;
        //else
        return true;
    }


    List<int> FilterZeros(List<int> list)
    {
        List<int> aux = new List<int>();
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i] != 0) aux.Add(list[i]);
        }
        return aux;
    }
}
