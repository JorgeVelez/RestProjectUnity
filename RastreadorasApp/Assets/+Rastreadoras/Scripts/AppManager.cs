using UnityEngine;
using UnityEngine.UI;
using LoginProAsset;

using System.Collections;

using System.Collections.Generic;       //Allows us to use Lists. 

/*  
‌properties list  ⇧⌘O   & :
extract method ⌘.
rename F2
go to definition F12 ⌘ click
go to implementation ⌘F12
peek definition ⌥F12
find all references ⇧F12
opensymbol by name ⌘T
abrir archivo ⌘P
menu ctrl click
*/

public class AppManager : MonoBehaviour
{

	public static AppManager instance { get; private set; }

	public enum PantallaEnum { Login, Busquedas, Configuracion, Grupos };

	public Dictionary<PantallaEnum, Pantalla> pantallas = new Dictionary<PantallaEnum, Pantalla>();

	public Color colorUI=Color.green;

	public Texture2D texForLoading;
	public Texture2D texForError;

	public bool borrarPlayerPrefs=false;

	Transform canvas;

	void Awake()
	{

		Application.targetFrameRate = 60;

		if (instance == null)
			instance = this;
		else if (instance != this)
			Destroy(gameObject);

		DontDestroyOnLoad(gameObject);

		InitGame();
	}

	void InitGame()
	{
		canvas = GameObject.Find("Canvas").transform;
		canvas.Find("Menu/TogglesMenu/busquedas").GetComponent<Toggle>().onValueChanged.AddListener((isSelected) => { if (isSelected)ShowScreen(PantallaEnum.Busquedas); });
		canvas.Find("Menu/TogglesMenu/configuracion").GetComponent<Toggle>().onValueChanged.AddListener((isSelected) => { if (isSelected)ShowScreen(PantallaEnum.Configuracion); });
		canvas.Find("Menu/TogglesMenu/grupos").GetComponent<Toggle>().onValueChanged.AddListener((isSelected) => { if(isSelected)ShowScreen(PantallaEnum.Grupos); });

		canvas.Find("Menu").gameObject.SetActive(false);

		//foreach (ScrollRect sv in FindObjectsOfType<ScrollRect>())
			//sv.scrollSensitivity = .3f;

	}

	void Update(){
		 foreach(KeyCode vKey in System.Enum.GetValues(typeof(KeyCode))){
             if(Input.GetKey(vKey)){
             //Debug.LogWarning(vKey);
             }
         }
		if (Application.platform == RuntimePlatform.Android)
        {
            if (Input.GetKey(KeyCode.Home) || Input.GetKey(KeyCode.Escape) || Input.GetKey(KeyCode.Menu))
            {
                Application.Quit();
				//System.Diagnostics.Process.GetCurrentProcess().Kill();
                return;
            }
        }
	}

	public void Subscribe(Pantalla pn, PantallaEnum enumP)
	{
		pantallas.Add(enumP, pn);
	}

	public void ShowScreen(PantallaEnum pe)
	{
		Pantalla pantalla = null;

		if (pantallas.TryGetValue(pe, out pantalla)){
			pantalla.gameObject.SetActive(true);
			pantalla.Show();
		}
			

		foreach(KeyValuePair<PantallaEnum, Pantalla> pn in pantallas)
		{
			if(pn.Value!=pantalla)
				pn.Value.gameObject.SetActive(false);
		}
	}
	public void reset()
	{
		canvas.Find("Menu").gameObject.SetActive(false);
		ShowScreen(PantallaEnum.Login);
	}

	public void intiApp()
	{
		ShowMenu();
		canvas.Find("Menu/TogglesMenu/busquedas").GetComponent<Toggle>().isOn = true;
	}

	public void ShowMenu()
	{
		if (LoginPro.Session.entroEnOffline)
		{
			//canvas.Find("Menu/TogglesMenu/grupos").GetComponent<RectTransform>().anchoredPosition = new Vector2(0,canvas.Find("Menu/TogglesMenu/grupos").GetComponent<RectTransform>().anchoredPosition.y);
			canvas.Find("Menu/TogglesMenu/grupos").gameObject.SetActive(false);
			//canvas.Find("Menu/TogglesMenu/configuracion").gameObject.SetActive(false);
		}
		canvas.Find("Menu").gameObject.SetActive(true);
	}

}
