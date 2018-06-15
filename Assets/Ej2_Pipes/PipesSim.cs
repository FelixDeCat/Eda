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
        pipes.Add(n); // Se agrega a la lista de pipes del mapa
        return n;
    }


    public void Iniciar()
    {
        var intial = _root;


    }

    //Creamos el sistema de cañerias
    Node RecuBuildDFS(int x, int y, Node prev = null, Direction comingFrom = Direction.Left)
    {
        return null;
    }

    // Obtiene la cantidad de ramas a dividir el nodo actual
    int RandomBranch()
    {
        return 0;
    }



    List<Direction> GetDireccionesPosibles(int x, int y) {
        var pipe = FindPipe(x, y);
        var dirs = new List<Direction>();
        for (int i = 0; i< 4; i++) {
            if (pipe.parts[i] != null) {
                Direction dir = (Direction)i;
                dirs.Add(dir);
            }
        }
        return dirs;
    }

    List<Direction> GetDireccionesPosibles(Pipe pipe) {
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

    


    public void GoToRandomDirection(int x, int y)
    {
        var pipe = FindPipe(x,y);
        var dir = pipe.ShutDownDirectionAndReturn(pipe.GetDireccionesPosibles().GetRandomDirection());
    }

    public Node FindNode(int x, int y) { return pipes.GetIn(x, y); }
    public Pipe FindPipe(int x, int y) { return pipes.GetIn(x, y).pipe; }


    void RandomBlock(Node node)
    {

    }


    void CheckErrors()
    {

    }

    void Start()
    {
        //Rotamos una pizca luego de agregar todos los pipes
        transform.Rotate(0f, 0f, -5f);
    }

    void StartFill()
    {

    }



    void Update(){
        if (Input.GetMouseButtonDown(0)) {}
        else if (Input.GetMouseButtonDown(1)){}
    }
}
public static class ext
{
    public static Vector2[] direction = new Vector2[4] {
        new Vector2(-1, 0),
        new Vector2(0, 1),
        new Vector2(1, 0),
        new Vector2(0, -1) };

    public static PipesSim.Node GetIn(this IEnumerable<PipesSim.Node> col, int x, int y) {
        foreach (PipesSim.Node p in col) if (p.pipe.x == x && p.pipe.y == y) return p;
        return null;
    }

    public static int ToInt(this Direction dir) { return (int)dir; }

    public static int ShutDownDirectionAndReturn(this Pipe pipe, Direction dirToShutDown) {
        pipe.SetConnection(dirToShutDown, false);
        return dirToShutDown.ToInt();
    }

    public static Direction GetRandomDirection(this List<Direction> dirs) { return dirs.GetRandomValue(); }

    public static List<Direction> GetDireccionesPosibles(this Pipe pipe)
    {
        var dirs = new List<Direction>();
        for (int i = 0; i < 4; i++) {
            if (pipe.parts[i] != null) {
                Direction dir = (Direction)i;
                dirs.Add(dir);
            }
        }
        return dirs;
    }

    public static T GetRandomValue<T>(this List<T> col) { return col[Random.Range(0, col.Count)]; }
    public static T GetRandomValue<T>(this T[] col) { return col[Random.Range(0, col.Length)]; }
}

