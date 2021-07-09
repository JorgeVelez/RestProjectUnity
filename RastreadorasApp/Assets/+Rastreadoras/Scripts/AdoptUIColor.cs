using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class AdoptUIColor : MonoBehaviour {

	 Color uicolor;
	void Start ()
    {
		//Debug.Log(AppManager.instance.colorUI);
		//uicolor=AppManager.instance.colorUI;
        //colorize();
    }

    public void colorize()
    {
		try{if (GetComponent<Graphic>() != null)
            GetComponent<Graphic>().color = AppManager.instance.colorUI;
			}catch(Exception e){}
        
    }

	void OnEnable(){
        colorize();
	}

    // Update is called once per frame
    public void setColor (Color _uicolor) {
		Debug.Log("setColor");
		uicolor=_uicolor;
		colorize();
	}
}
