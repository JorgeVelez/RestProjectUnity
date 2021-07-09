using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pantalla : MonoBehaviour{
		int hp;

		public static Pantalla instance { get; private set; }


	public void init()
	{
		if (instance == null)
			instance = this;
		/*else if (instance != this)
			Destroy(gameObject);

		DontDestroyOnLoad(gameObject);*/

		Hide();
	}

		public virtual void Show()
	{

		transform.Find("Main").gameObject.SetActive(true);
		gameObject.SetActive(true);
		transform.SetAsLastSibling();
	}

	public virtual void Hide()
	{
		//Debug.Log("hide "+gameObject.name);
		//enabled = false;
		gameObject.SetActive(false);
	}
}
