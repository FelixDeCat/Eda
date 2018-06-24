using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PipesSim : MonoBehaviour
{
    public enum Content { Empty, Tears, Block }

    public Text txt_debug; object d { set { txt_debug.text = value.ToString(); } }

    public class Node
    {
        public string info;
        public Pipe pipe;
        public Content content = Content.Empty;
        public List<Node> neighbours = new List<Node>();
    };

    public Pipe prefabPipe;
    public int maxWidth, maxHeight;
    [Range(0.0f, 1.0f)]
    public float branchChance;
    [Range(0.0f, 1.0f)]
    public float blockChance;
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

    void Start()
    {
        visited = visited.Create(maxWidth, maxHeight, false);
        Iniciar();
        // transform.Rotate(0f, 0f, -5f);
         //StartCoroutine(Rellenar(0, 0));
    }
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            //InitRefill(GetNode(0,0));
        }
        else if (Input.GetMouseButtonDown(1)) { }
    }

    bool PositionOccupied(int x, int y)
    {
        if ((x >= 0 && x < maxWidth) && (y >= 0 && y < maxHeight)) return visited[x, y];
        return true;
    }
    Node CreateNode(int x, int y)
    {
        visited[x, y] = true; // Agregue que el mismo nodo diga en que X e Y se crea.
        var n = new Node();
        n.pipe = Instantiate(prefabPipe);
        n.pipe.transform.parent = transform;
        n.pipe.x = x;
        n.pipe.y = y;
        pipes.Add(n); // Se agrega a la lista de pipes del mapa
        return n;
    }

    public void RandomBranches()
    {

    }

    public bool Block() { return blockChance < (Random.Range(0, 1)); }
    public bool Branch() { return branchChance > (Random.Range(0, 1)); }

    public void Iniciar()
    {
        Debug.Log("--- INIT ---");
        var intial = new Node();
        intial = RecuBuildDFS(0, 0);

        Debug.Log("El node que me devuelve es el: " + intial.pipe.name);
        Debug.Log("--- END ---");
    }

    int debint = 0;

    bool first = true;

    //Creamos el sistema de cañerias
    Node RecuBuildDFS(int x, int y, Node prev = null, Direction comingFrom = Direction.Left)
    {
        Node myNode = CreateNode(x, y);
        myNode.content = Content.Empty;
        myNode.pipe.name = debint.ToString();
        //myNode.pipe.info = debint.ToString();
        debint++;

        // esto recibe mi posicion y me devuelve una tupla de nodos, (Item1 = Posicion del vecino), (Item2 = Direction del vecino)
        // filtra si uno de los que esta ahi es mi parent y randomiza
        var dir_posibles = GetPosiblePositions(new Vector2(x, y));

        //Debug.Log("NODE: " + myNode.pipe.name + "Es: " + myNode.pipe.x.ToString() + "," + myNode.pipe.y.ToString());
        //for (int i = 0; i < dir_posibles.Count; i++)
        //{
        //    Debug.Log(((int)(dir_posibles[i].Item1.x)).ToString() + "," + ((int)(dir_posibles[i].Item1.y)).ToString());
        //}

        if (first)
        {
            Debug.Log("El primero deveria tener maximo 2 direcciones" + dir_posibles.Count);
            first = false;
            myNode.content = Content.Empty;
            myNode.pipe.branches = dir_posibles.Count;
            //myNode.pipe.addinfo = "-" + myNode.pipe.branches.ToString();
        }
        else
        {
            myNode.content = blockChance > Random.Range(0f, 1f) ? Content.Block : Content.Empty;

            if (myNode.content == Content.Block) myNode.pipe.material = materialBlocked;
            else myNode.pipe.material = materialEmpty;

            myNode.pipe.branches = dir_posibles.Count;
            //myNode.pipe.addinfo = "-" + myNode.pipe.branches.ToString();
        }

        if (prev != null)// si no es el primero
        {
            //lo conecto con mi padre y lo agrego como vecino

            if (comingFrom == Direction.Top || comingFrom == Direction.Bottom) prev.pipe.SetConnection(comingFrom, true);
            if (comingFrom == Direction.Right || comingFrom == Direction.Left) prev.pipe.SetConnection(comingFrom.Inverted(), true);

            myNode.neighbours.Add(prev);

            if (comingFrom == Direction.Top || comingFrom == Direction.Bottom) myNode.pipe.SetConnection(comingFrom.Inverted(), true);
            if (comingFrom == Direction.Right || comingFrom == Direction.Left) myNode.pipe.SetConnection(comingFrom, true);

            //filtro y saco a mi padre de mis vecinos
            //randomizo
            dir_posibles = dir_posibles.Randomize();
        }
        else
        {
            //como es el primero tomo a todos los que estan alrededor
            dir_posibles = dir_posibles.Randomize();
        }

        for (int i = 0; i < myNode.pipe.branches; i++)
        {
            if (!PositionOccupied((int)dir_posibles[i].Item1.x, (int)dir_posibles[i].Item1.y))
            {
                myNode.neighbours.Add(RecuBuildDFS((int)dir_posibles[i].Item1.x, (int)dir_posibles[i].Item1.y, myNode, dir_posibles[i].Item2.Inverted()));
            }
        }

        return myNode;
    }

    List<Tuple<Vector2, Direction>> GetPosiblePositions(Vector2 pos)
    {
        List<Tuple<Vector2, Direction>> aux = new List<Tuple<Vector2, Direction>>();
        if (!PositionOccupied((int)(pos + Constantes.Left).x, (int)(pos + Constantes.Left).y)) { aux.Add(Tuple.Create(pos + Constantes.Left, Direction.Left)); }
        if (!PositionOccupied((int)(pos + Constantes.Top).x, (int)(pos + Constantes.Top).y)) { aux.Add(Tuple.Create(pos + Constantes.Top, Direction.Top)); }
        if (!PositionOccupied((int)(pos + Constantes.Right).x, (int)(pos + Constantes.Right).y)) { aux.Add(Tuple.Create(pos + Constantes.Right, Direction.Right)); }
        if (!PositionOccupied((int)(pos + Constantes.Bottom).x, (int)(pos + Constantes.Bottom).y)) { aux.Add(Tuple.Create(pos + Constantes.Bottom, Direction.Bottom)); }
        return aux;
    }
    //public IEnumerator Rellenar(int x, int y)
    //{
    //    var node = GetNode(x, y);

    //    node.content = node.content == Content.Block ? Content.Block : Content.Tears;
    //    node.pipe.material = node.content == Content.Block ? materialBlocked : materialTears;

    //    var vecinos = node.neighbours
    //        .Where(v => v.content != Content.Tears && v.content != Content.Block);

    //    foreach (Node n in vecinos)
    //    {
    //        yield return new WaitForSeconds(0.5f);
    //        yield return StartCoroutine(Rellenar(n.pipe.x, n.pipe.y));
    //    }
    //    Debug.Log("Segundo wait for seconds");
    //    yield return new WaitForSeconds(0.5f);
    //}

    public void InitRefill(Pipe pipe)
    {
        var node = pipes.Where( x => x.pipe.Equals(pipe) ).First();
        initialForFill = node;
        pendientes.Add(initialForFill);
        StartCoroutine(RecRefill());
    }

    public Node initialForFill;
    public List<Node> pendientes = new List<Node>();
    public List<Node> visitados = new List<Node>();
    public IEnumerator RecRefill()
    {
        d = pendientes.Count;

        if (pendientes.Count > 0)
        {
            var vecinos = new List<Node>();

            foreach (Node n in pendientes)
            {
                n.pipe.material = materialTears;
                foreach (Node v in n.neighbours) { if(v.content != Content.Block) vecinos.Add(v); }
                visitados.Add(n);
            }

            yield return new WaitForSeconds(0.1f);

            for (int i = 0; i < visitados.Count; i++)
                if (pendientes.Contains(visitados[i]))
                    pendientes.Remove(visitados[i]);

            foreach (Node v in vecinos)
            {
                if(!visitados.Contains(v))
                    pendientes.Add(v);
            }

            vecinos.Clear();

            StartCoroutine(RecRefill());
        }
    }

    Node GetNode(int _x, int _y)
    {
        return pipes.Where(current => current.pipe.x == _x && current.pipe.y == _y).First();
    }
    //public IEnumerator Contruir()
    //{
    //    yield return new WaitForSeconds(0.5f);
    //}

    //public IEnumerator RecursiveStep(int depth)
    //{
    //    if (depth > 0)
    //    {
    //        yield return StartCoroutine(RecursiveStep(depth - 1));
    //    }

    //    yield return new WaitForSeconds(0.5f);
    //    Debug.Log("MyCoroutine is now finished at depth " + depth);
    //}
}
public static class ExtensionsForPipes
{
    public static PipesSim.Node GetIn(this IEnumerable<PipesSim.Node> col, int x, int y)
    {
        foreach (PipesSim.Node p in col) if (p.pipe.x == x && p.pipe.y == y) return p;
        return null;
    }
    public static Direction GetRandomDirection(this List<Direction> dirs) { return dirs.GetRandomValue(); }
    public static List<Direction> GetDireccionesPosibles(this Pipe pipe)
    {
        var dirs = new List<Direction>();
        for (int i = 0; i < 4; i++)
        {
            if (pipe.parts[i] != null)
            {
                Direction dir = (Direction)i;
                dirs.Add(dir);
            }
        }
        return dirs;
    }
}

public static class FelitoExtensions
{
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
    public static List<T> Except<T>(this List<T> col, System.Func<T, bool> pred)
    {
        var newcol = new List<T>();
        for (int i = 0; i < col.Count; i++)
            if (!pred(col[i])) newcol.Add(col[i]);

        return newcol;
    }
    public static T Debug<T>(this T deb, string Msg = "")
    {
        UnityEngine.Debug.Log(Msg + deb.ToString());
        return deb;
    }
    public static List<T> Randomize<T>(this List<T> col)
    {
        for (int i = 0; i < col.Count; i++)
        {
            int indx_random = Random.Range(0, col.Count);
            var val_random = col[indx_random];

            col[indx_random] = col[i];
            col[i] = val_random;
        }
        return col;
    }
    public static T GetRandomValue<T>(this List<T> col) { return col[Random.Range(0, col.Count)]; }
    public static T GetRandomValue<T>(this T[] col) { return col[Random.Range(0, col.Length)]; }
    public static List<T> ToList<T>(this T[] col)
    {
        var aux = new List<T>();
        foreach (var t in col)
        {
            aux.Add(t);
        }
        return aux;
    }
}

