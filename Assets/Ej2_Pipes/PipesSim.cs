using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class PipesSim : MonoBehaviour
{
    public enum Content { Empty, Tears, Block }
    public class Node
    {
        public Pipe pipe;
        public Content content = Content.Empty;
        public List<Node> neighbours = new List<Node>();
    };

    public Pipe prefabPipe;
    public int maxWidth, maxHeight;
    [Range(0.0f, 1.0f)] public float branchChance;
    [Range(0.0f, 1.0f)] public float blockChance;
    public float fillDelaySeconds;

    public Material materialTears;
    public Material materialEmpty;
    public Material materialBlocked;
    public Material materialError;
    bool _filled = false;


    // Variables agregadas
    private Node _root;

    // Matrix Recargado de nodos visitados(nodos ya puestos en escena)
    private bool[,] visited;

    // Lista de todos los nodos en escena
    private List<Node> pipes = new List<Node>();

    //¿Esta la posicion ocupada?
    bool PositionOccupied(int x, int y)
    {
        if ((x > 0 && x < maxWidth) && (y > 0 && y < maxHeight)) return visited[x, y];
        return true;
    }

    //Creamos un nodo (esta función ya está completa)
    Node CreateNode(int x, int y)
    {
        visited[x, y] = true; // Agregue que el mismo nodo diga en que X e Y se crea.
        var n = new Node();
        n.pipe = Instantiate(prefabPipe);
        n.pipe.transform.parent = transform;
        n.pipe.x = x;
        n.pipe.y = y;
        n.pipe.name = x+","+y;
        pipes.Add(n); // Se agrega a la lista de pipes del mapa
        return n;
    }


    public bool Block() { return Random.Range(0, blockChance + branchChance) < blockChance; }

    public void Iniciar()
    {
        visited = visited.Create(maxWidth, maxHeight, false);

        var intial = _root;

        Node myNode = CreateNode(5, 5);

        var vecinos = GetNeighboursPositions(new Vector2(5, 5)).Randomize();


        for (int i = 0; i < vecinos.Count; i++)
        {
            //agregamos a nuestros vecinos...         (su posicion x)       (su posicion y)      (yo)    (la posicion del vecino invertida)
            myNode.neighbours.Add(RecuBuildDFS((int)vecinos[i].Item1.x, (int)vecinos[i].Item1.y, myNode, vecinos[i].Item2.Inverted()));
            myNode.pipe.SetConnection(vecinos[i].Item2, true);
        }

        Debug.Log("Terminó");

        
    }

    public IEnumerator Contruir()
    {
        yield return new WaitForSeconds(0.5f);
    }

    //public IEnumerator RecursiveStep(int depth)
    //{
    //    if (depth > 0)
    //    {
    //        yield return StartCoroutine(RecursiveStep(depth - 1));
    //    }

    //    yield return new WaitForSeconds(0.5f);
    //    Debug.Log("MyCoroutine is now finished at depth " + depth);
    //}

    public IEnumerator Rellenar(int x, int y)
    {
        var node = GetNode(x,y);

        node.content = node.content == Content.Block ? Content.Block : Content.Tears;
        node.pipe.material = node.content == Content.Block ? materialBlocked : materialTears;

        var vecinos = node.neighbours
            .Where(v => v.content != Content.Tears)
            .Where(v => v.pipe.transform.position.x > node.pipe.transform.position.x || v.pipe.transform.position.y < node.pipe.transform.position.y);

        foreach (Node n in vecinos)
        {
            
            yield return StartCoroutine(Rellenar(n.pipe.x, n.pipe.y));
        }
        //yield return new WaitForSeconds(0.5f);
    }

    Node GetNode(int _x,int _y)
    {
        return pipes.Where(current => current.pipe.x == _x && current.pipe.y == _y).First();
    }

    //Creamos el sistema de cañerias
    Node RecuBuildDFS(int x, int y, Node prev = null, Direction comingFrom = Direction.Left)
    {
        Node myNode = CreateNode(x,y);

        if (!Block())
        {
            myNode.content = Content.Empty;
            // esto recibe mi posicion y me devuelve una tupla de nodos, (Item1 = Posicion del vecino), (Item2 = Direction del vecino)
            //  filtra si uno de los que esta ahi es mi parent y randomiza
            var vecinos = GetNeighboursPositions(new Vector2(x, y)).Except(neigh => neigh.Item2 == comingFrom).Randomize();

            myNode.pipe.SetConnection(comingFrom, true);
            myNode.neighbours.Add(prev);

            for (int i = 0; i < vecinos.Count; i++)
            {
                //agregamos a nuestros vecinos...         (su posicion x)       (su posicion y)      (yo)    (la posicion del vecino invertida)
                myNode.neighbours.Add(RecuBuildDFS((int)vecinos[i].Item1.x, (int)vecinos[i].Item1.y, myNode, vecinos[i].Item2.Inverted()));
                myNode.pipe.SetConnection(vecinos[i].Item2, true);
            }
        }
        else
        {
            myNode.content = Content.Block;
        }

        return myNode;
    }

    List<Tuple<Vector2,Direction>> GetNeighboursPositions(Vector2 pos) {
        List<Tuple<Vector2, Direction>> aux = new List<Tuple<Vector2, Direction>>();
        if (!PositionOccupied((int)(pos + Constantes.Left).x, (int)(pos + Constantes.Left).y)) aux.Add(Tuple.Create(pos + Constantes.Left, Direction.Left));
        if (!PositionOccupied((int)(pos + Constantes.Top).x, (int)(pos + Constantes.Top).y)) aux.Add(Tuple.Create(pos + Constantes.Top, Direction.Top));
        if (!PositionOccupied((int)(pos + Constantes.Right).x, (int)(pos + Constantes.Right).y)) aux.Add(Tuple.Create(pos + Constantes.Right, Direction.Right));
        if (!PositionOccupied((int)(pos + Constantes.Bottom).x, (int)(pos + Constantes.Bottom).y)) aux.Add(Tuple.Create(pos + Constantes.Bottom, Direction.Bottom));
        return aux;
    }

    void Start() {
        Iniciar();
        transform.Rotate(0f, 0f, -5f);
        StartCoroutine(Rellenar(5, 5));
    }
    void Update(){
        if (Input.GetMouseButtonDown(0)) {}
        else if (Input.GetMouseButtonDown(1)){}
    }
}
public static class ExtensionsForPipes {
    public static PipesSim.Node GetIn(this IEnumerable<PipesSim.Node> col, int x, int y) {
        foreach (PipesSim.Node p in col) if (p.pipe.x == x && p.pipe.y == y) return p;
        return null;
    }
    public static Direction GetRandomDirection(this List<Direction> dirs) { return dirs.GetRandomValue(); }
    public static List<Direction> GetDireccionesPosibles(this Pipe pipe) {
        var dirs = new List<Direction>();
        for (int i = 0; i < 4; i++) {
            if (pipe.parts[i] != null) {
                Direction dir = (Direction)i;
                dirs.Add(dir);
            }
        }
        return dirs;
    }
}

public static class FelitoExtensions {
    public static List<T> Where<T>(this List<T> col, System.Func<T, bool> pred)
    {
        List<T> aux = new List<T>();

        foreach (T v in col)
        {
            if (pred(v))
            {
                aux.Add(v);
            }
        }

        return aux;
    }
    public static T First<T>(this List<T> col) { return col[0] != null ? col[0] : default(T); }
    public static T[,] Create<T>(this T[,] matrix, int width, int height, T def_val = default(T))
    {
        matrix = new T[width, height];
        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                matrix[x, y] = def_val;

        return matrix;
    }
    public static List<T> Except<T>(this List<T> col, System.Func<T, bool> pred) {
        var newcol = new List<T>();
        for (int i = 0; i < col.Count; i++) 
            if (!pred(col[i])) newcol.Add(col[i]);

        return newcol;
    }
    public static List<T> Randomize<T>(this List<T> col) {
        for (int i = 0; i < col.Count; i++) {
            int indx_random = Random.Range(0,col.Count-1);
            var val_random = col[indx_random];

            col[indx_random] = col[i];
            col[i] = val_random;
        }
        return col;
    }
    public static T GetRandomValue<T>(this List<T> col) { return col[Random.Range(0, col.Count)]; }
    public static T GetRandomValue<T>(this T[] col) { return col[Random.Range(0, col.Length)]; }
}

