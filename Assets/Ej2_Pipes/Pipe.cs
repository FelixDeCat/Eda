using UnityEngine;

//IMPORTANTE: Esta clase esta completa, NO LA DEBE MODIFICAR -- USELA ASI
public class Pipe : MonoBehaviour {

    public GameObject[] parts;
    Material _material;
    int _x, _y;

    void Awake() {
        foreach (var p in parts) { p.SetActive(false); }
        SetConnection(Direction.Center, true);
    }

    // + //
    public int x { get { return _x; } set { _x = value; RefreshPos(); } }
    public int y { get { return _y; } set { _y = value; RefreshPos(); } }
    public Material material { get { return _material; } set { _material = value; foreach (var p in parts) p.GetComponent<Renderer>().material = _material; } }
    public Material center { set { parts.ToList().Where(x => x.gameObject.GetComponent<BoxCollider>() != null).First(); } }
    public void SetConnection(Direction which, bool visible) { parts[(int)which].SetActive(visible); }

    // - //
    void RefreshPos() { gameObject.transform.localPosition = new Vector3(_x * 3f, -_y * 3f, 0f); }
}
