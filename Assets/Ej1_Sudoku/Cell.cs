using UnityEngine;
using System.Collections;
using UnityEngine.UI;

//IMPORTANTE: Esta clase esta completa, NO LA DEBE MODIFICAR -- USELA ASI
public class Cell : MonoBehaviour {

	public const int EMPTY = 0;

	public Text label;
	
	int _number;
	bool _locked;
	bool _invalid;
	Image _image;
	Color _prev;

    /// <summary> Get Normal, Set: Modifica el label directamente </summary>
	public int number {
		get { return _number; }
		set {
			_number = value;
			if(value == EMPTY) label.text = "";
			else label.text = value.ToString();
		}
	}

    /// <summary> esta vacio si _number es = 0 </summary>
	public bool isEmpty {
		get { return _number == 0; }
	}

    /// <summary> El Set hace un RefreshColor() </summary>
	public bool invalid {
		get { return _invalid; }
		set { _invalid = value; RefreshColor(); }
	}

    /// <summary> El Set hace un RefreshColor() </summary>
	public bool locked {
		get { return _locked; }
		set { _locked = value; RefreshColor(); }
	}

    /// <summary> Si es invalido lo pone Rojo, Si esta Lockeado 0.75f, sinó blanco </summary>
	void RefreshColor() {
		if(_invalid)
			_image.color = Color.red;
		else if(_locked)
			_image.color = new Color(0.75f, 0.75f, 0.75f);
		else
			_image.color = Color.white;
	}

	// Use this for initialization
	void Awake() {
		_image = GetComponent<Image>();
		RefreshColor();
	}
}
