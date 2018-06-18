using UnityEngine;

public enum Direction {
	Left = 0,
	Top = 1,
	Right = 2,
	Bottom = 3,
	Center = 4,
    Count = 5
}

public static class Constantes
{


    public static readonly int[] dirX = { -1, 0, 1, 0 };
    public static readonly int[] dirY = { 0, 1, 0, -1 };

    public static Vector2 Left { get { return new Vector2(dirX[(int)Direction.Left],dirY[(int)Direction.Left]);} }
    public static Vector2 Right { get { return new Vector2(dirX[(int)Direction.Right], dirY[(int)Direction.Right]); } }
    public static Vector2 Top { get { return new Vector2(dirX[(int)Direction.Top], dirY[(int)Direction.Top]); } }
    public static Vector2 Bottom { get { return new Vector2(dirX[(int)Direction.Bottom], dirY[(int)Direction.Bottom]); } }

    static int op1;
    public static Direction Inverted(this Direction currentDir)
    {
        if ((int)currentDir == 0) op1 = 2;
        if ((int)currentDir == 1) op1 = 3;
        if ((int)currentDir == 2) op1 = 0;
        if ((int)currentDir == 3) op1 = 1;
        return (Direction)op1;
    }

    static int op;
    public static int GetOpuesto(int id) {
        if (id == 0) op = 2;
        if (id == 1) op = 3;
        if (id == 2) op = 0;
        if (id == 3) op = 1;
        return op;
    }
}