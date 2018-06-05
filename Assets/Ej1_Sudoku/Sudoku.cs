using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class Sudoku : MonoBehaviour
{
    public Cell prefabCell;
    public Canvas canvas;
    public Text feedback;
    public float stepDuration = 0.05f;
    [Range(1, 82)]
    public int difficulty = 40;

    Matrix<Cell> _board;
    Matrix<int> _createdMatrix;
    List<int> posibles = new List<int>();
    int _smallSide;
    int _bigSide;
    string memory = "";
    string canSolve = "";
    bool canPlayMusic = false;
    List<int> nums = new List<int>();

    float r = 1.0594f;
    float frequency = 440;
    float gain = 0.5f;
    float increment;
    float phase;
    float samplingF = 48000;


    void Start() {

        //bla bla
        long mem = System.GC.GetTotalMemory(true);
        feedback.text = string.Format("MEM: {0:f2}MB", mem / (1024f * 1024f));
        memory = feedback.text;

        // esto es simplemente para formalizar la longitud del sector
        // tranquilamente se podía hacer hecho con un CONST, pero al profe no le gustan los CONST porque rompe con Solid... creo :p
        _smallSide = 3;
        _bigSide = _smallSide * 3;

        //bla bla 
        frequency = frequency * Mathf.Pow(r, 2);

        // Crea las celdas en el escenario.
        // Las limpia por si las pulgas
        CreateEmptyBoard();
        ClearBoard();
    }

    /// <summary>
    /// hace dos cosas distintas
    /// * crea una matriz nueva
    /// * del board limpia todo, desbloquea, valida y zera
    /// </summary>
    void ClearBoard() {
        _createdMatrix = new Matrix<int>(_bigSide, _bigSide);
        foreach (var cell in _board) {
            cell.number = 0;
            cell.locked = cell.invalid = false;
        }
    }

    /// <summary>
    /// Funcion FRONTEND que crea las celdas físicamente en el escenario
    /// </summary>
    void CreateEmptyBoard() {
        float spacing = 68f;// 68 es la separacion entre cells
        float startX = -spacing * 4f;//(-272) *4 es xq desde el centro hasta la punta hay 4 casilleros
        float startY = spacing * 4f;//(272) *4 es xq desde el centro hasta la punta hay 4 casilleros
        _board = new Matrix<Cell>(_bigSide, _bigSide);
        for (int x = 0; x < _board.width; x++) {
            for (int y = 0; y < _board.height; y++) {
                var cell = _board[x, y] = Instantiate(prefabCell);
                cell.transform.SetParent(canvas.transform, false);
                cell.transform.localPosition = new Vector3(startX + x * spacing, startY - y * spacing, 0);
            }
        }
    }

    #region no nos interesa
    int watchdog = 0;
    bool RecuSolve(Matrix<int> matrixParent, int x, int y, int protectMaxDepth, List<Matrix<int>> solution) { return false; }
    void OnAudioFilterRead(float[] array, int channels) {
        if (canPlayMusic) {
            increment = frequency * Mathf.PI / samplingF;
            for (int i = 0; i < array.Length; i++) {
                phase = phase + increment;
                array[i] = (float)(gain * Mathf.Sin((float)phase));
            }
        }
    }
    void changeFreq(int num) { frequency = 440 + num * 80; }
    IEnumerator ShowSequence(List<Matrix<int>> seq) { yield return new WaitForSeconds(0); }
    #endregion

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R) || Input.GetMouseButtonDown(1)) SolvedSudoku();
        else if (Input.GetKeyDown(KeyCode.C) || Input.GetMouseButtonDown(0)) CreateSudoku();
    }

    void SolvedSudoku()
    {
        // bla bla bla
        StopAllCoroutines();
        watchdog = 100000;

        //coleccion que podemos usar
        nums = new List<int>();

        //una lista de Matrix's
        var solution = new List<Matrix<int>>();

        // esto se supone que lo tenemos que usar nosotros, pequeña pista
        var result = false;

        //bla bla bla
        long mem = System.GC.GetTotalMemory(true);
        memory = string.Format("MEM: {0:f2}MB", mem / (1024f * 1024f));

        //FRONTEND FEEDBACK ETC ETC
        canSolve = result ? " VALID" : " INVALID";
    }

    void CreateSudoku()
    {
        // bla bla bla
        StopAllCoroutines();
        canPlayMusic = false;
        watchdog = 100000;

        //coleccion que podemos usar
        nums = new List<int>();

        //limpia el tablero, (ZERA, VALIDA, DESBLOQUEA)
        ClearBoard();

        //una lista de Matrix's
        var l = new List<Matrix<int>>();
        
        //crea un la primer fila random
        GenerateValidLine(_createdMatrix, 0, 0);

        // esto se supone que lo tenemos que usar nosotros, pequeña pista
        var result = false;

        //Obtiene la primer matriz, pero esta vacia, la tenemos que rellenar con algo
        _createdMatrix = l[0].Clone();

        //Esto supongo que será para luego borrar las que no estan bloqueadas y que tengamos que resolverlo con fuerza bruta
        LockRandomCells();

        //jaja si efectivamente es para que luego lo resolvamos a fuerza bruta
        ClearUnlocked(_createdMatrix);

        // esta funcion lo que hace es que vos le pasas la matriz que rellenamos mas arriba y se la 
        // copia toooda al board, para que laburemos con el board y no con esta _createdMatrix
        TranslateAllValues(_createdMatrix);

        //bla bla bla
        long mem = System.GC.GetTotalMemory(true);
        memory = string.Format("MEM: {0:f2}MB", mem / (1024f * 1024f));

        //FRONTEND FEEDBACK ETC ETC
        canSolve = result ? " VALID" : " INVALID";
        feedback.text = "Pasos: " + l.Count + "/" + l.Count + " - " + memory + " - " + canSolve;
    }

    // Le pasas una Matriz y te crea en el primer indice una linea desordenada
    // es solo en la primera, supongo que es para iniciar el calculo
    // ademas recibe x e y, que no se usan, algo habra borrado, o flasheo que
    // iba a hacer otra cosa y no lo hizo
    void GenerateValidLine(Matrix<int> mtx, int x, int y)
    {
        // rellena aux[] con... { 1,2,3,4,5,6,7,8,9 }
        int[] aux = new int[9];
        for (int i = 0; i < 9; i++) aux[i] = i + 1; 
        
        // desordena aux[]
        int numAux = 0;
        for (int j = 0; j < aux.Length; j++) {
            int r = 1 + Random.Range(j, aux.Length);
            numAux = aux[r - 1];
            aux[r - 1] = aux[j];
            aux[j] = numAux;
        }

        // va generando una linea horizontal con ese array desordenado
        for (int k = 0; k < aux.Length; k++) mtx[k, 0] = aux[k];
    }

    /// <summary> Le pasas una matriz, y comienza a recorrer por el alto/ancho del board, si encuentra una
    /// celda que no esta bloqueada, a la matriz le asigna en esa coordenada un cero </summary>
    void ClearUnlocked(Matrix<int> mtx) {
        for (int i = 0; i < _board.height; i++)
            for (int j = 0; j < _board.width; j++)
                if (!_board[j, i].locked)
                    mtx[j, i] = Cell.EMPTY;
    }

    void LockRandomCells()
    {
        List<Vector2> posibles = new List<Vector2>();
        for (int i = 0; i < _board.height; i++)
        {
            for (int j = 0; j < _board.width; j++)
            {
                if (!_board[j, i].locked)
                    posibles.Add(new Vector2(j, i));
            }
        }
        for (int k = 0; k < 82 - difficulty; k++)
        {
            int r = Random.Range(0, posibles.Count);
            _board[(int)posibles[r].x, (int)posibles[r].y].locked = true;
            posibles.RemoveAt(r);
        }
    }


    #region Funciones Translate
    /////////////////////////////////////////////////////////////////////////////
    // Estas funciones Modifican de alguna manera el Board Principal actual...
    // La primera le pasas una matrix con algun contenido, entonces esta se encarga de pasarle todo ese contenido al Board
    // La segunda vos le pasas el valor en especifico que queres modificar y se lo das a una coordenada (Recomendacion, poner un filtro antes de llamar a esta funcion)
    // La tercera Le pasas un rango de donde hasta donde y la rellena con el contenido de _createdMatrix
    // La cuarta Llama a la primera, pero antes, crea una matriz, porque la primera te pide una matriz... esta matriz es una de las que estan en la base de datos
    /////////////////////////////////////////////////////////////////////////////
    void TranslateAllValues(Matrix<int> matrix) {
        for (int y = 0; y < _board.height; y++)
            for (int x = 0; x < _board.width; x++)
                _board[x, y].number = matrix[x, y];
    }

    void TranslateSpecific(int value, int x, int y) { _board[x, y].number = value; }
    void TranslateRange(int x0, int y0, int xf, int yf) {
        for (int x = x0; x < xf; x++)
            for (int y = y0; y < yf; y++)
                _board[x, y].number = _createdMatrix[x, y];
    }
    void CreateNew() {
        _createdMatrix = new Matrix<int>(Tests.validBoards[1]);
        TranslateAllValues(_createdMatrix);
    }
    #endregion


    /// <summary>
    /// No Tocar, funcion copada sin SIDE EFFECTS, le pasas la matriz que queres chequear, un valor y una coordenada
    /// y ella sola te dice... podes poner ese numerito en esa coordenada de esa matriz
    /// (TODO ESTO ES SOLO PARA CHECKEAR, NO MODIFICA NADA)
    /// </summary>
    /// <returns> Retorna true en el caso de que puedas poner ese valor ahí </returns>
    bool CanPlaceValue(Matrix<int> mtx, int value, int x, int y)
    {
        List<int> fila = new List<int>();
        List<int> columna = new List<int>();
        List<int> area = new List<int>();
        List<int> total = new List<int>();

        Vector2 cuadrante = Vector2.zero;

        //este for va acumulando los valores que se encuentran en las filas y columnas adyacentes
        //exeptuando la misma coordenada que le pasamos por X e Y
        /* ejemplo
         001000
         001000
         11E111
         001000
         */
        for (int i = 0; i < mtx.height; i++) {
            for (int j = 0; j < mtx.width; j++) {
                if (i != y && j == x) columna.Add(mtx[j, i]);
                else if (i == y && j != x) fila.Add(mtx[j, i]);
            }
        }

        // Obtiene el cuadrante sabiendo que el primer cuadrante 
        // empieza en el index 0, el segundo en 3 y el tercero en 6
        cuadrante.x = x < 3 ? 0 : (x < 6 ? 3 : 6);
        cuadrante.y = y < 3 ? 0 : (y < 6 ? 3 : 6);

        //obtiene todos los valores comprendidos entre el cuadrante actual y los devuelve en una lista
        area = mtx.GetRange((int)cuadrante.x, (int)cuadrante.y, (int)cuadrante.x + 3, (int)cuadrante.y + 3);

        //aca empieza a sumar toooooodo a la lista para que luego podamos checquear si nuestro valor esta entre ellas
        total.AddRange(fila);
        total.AddRange(columna);
        total.AddRange(area);
        total = FilterZeros(total); //esto es porque simplemente el cero no lo necesitamos calcular y sacamos de la lista

        //checkeamos, si no lo contiene es porque podemos poner ese valor ahi
        return !total.Contains(value);
    }

    /// <summary> A partir de una lista pasada por parametros devuelve otra nueva pero sin ceros </summary>
    List<int> FilterZeros(List<int> list) {
        List<int> aux = new List<int>();
        for (int i = 0; i < list.Count; i++) { if (list[i] != 0) aux.Add(list[i]); }
        return aux;
    }
}
