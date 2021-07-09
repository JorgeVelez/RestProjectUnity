using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LoginProAsset;
using System;

public class Usuarios : Pantalla
{

	Transform containerUsers;
	GameObject prefabUser;

	public InputField Password;
	public InputField ConfirmPassword;
	public InputField Username;
	public InputField Mail;

	public Button btBackDeAgregarUsuario;
	public Button btAgregarUsuario;

	void Awake()
	{
		base.init();
		//AppManager.instance.Subscribe(this, AppManager.PantallaEnum.Usuarios);

		containerUsers = transform.Find("Main/svUsers/Viewport/Content").GetComponent<Transform>();
		prefabUser = transform.Find("Main/svUsers/Viewport/Content/Usuario").gameObject;
		prefabUser.SetActive(false);
		prefabUser.transform.SetParent(prefabUser.transform.parent.parent);

		transform.Find("Main/btAnadir").GetComponent<Button>().onClick.AddListener(() => { openAddUser();});
		
		//transform.Find("Main/btBloquear").GetComponent<Button>().onClick.AddListener(btbloquearHandler);

		transform.Find("anadirUsuario/btBack").GetComponent<Button>().onClick.AddListener(cerrarAddUser);
		transform.Find("anadirUsuario/btAgregar").GetComponent<Button>().onClick.AddListener(() => { addUser();});

	}

	public override void Show()
	{
		base.Show();
		Debug.Log("show usuarios");

		transform.Find("anadirUsuario").gameObject.SetActive(false);

		Alertas.instance.ShowLoading("Cargando...");
		LoginPro.Manager.GetUsers(Success, Failed);
	}

	void openAddUser()
	{
		transform.Find("anadirUsuario").gameObject.SetActive(true);
		Password.text = "";
		ConfirmPassword.text = "";
		Username.text = "";
		Mail.text = "";
	}

	void addUser()
	{
		if (Password.text != ConfirmPassword.text)
		{
			string errorMessage = "Your passwords don't match.";
			Debug.LogWarning(errorMessage);
			Failed(errorMessage);
			return;
		}
		else {

			Debug.Log("Registration launched.");
			Alertas.instance.ShowLoading("Registrando");
			LoginPro.Manager.Register(Username.text, Mail.text, Password.text,"", SuccessRegister, Failed);

		}
	}

	public void SuccessRegister(string[] datas)
	{
		Alertas.instance.ShowMensaje("Usuario registrado", "OK", Show);
	}

	void btbloquearHandler()
	{
		Alertas.instance.ShowMensajeCondicional("Bloquear usuario?","",  "Bloquear");
	}

	void cerrarAddUser()
	{
		transform.Find("anadirUsuario").gameObject.SetActive(false);
	}



	public void Success(string[] datas)
	{
		Alertas.instance.Hide();

		ResultadoGetUsuarios resultado = JsonConvert.DeserializeObject<ResultadoGetUsuarios>(datas[0]);
		containerUsers.Clear();

		for (int i = 0; i < resultado.data.Count; i++)
		{
			GameObject gob = Instantiate (prefabUser, containerUsers);
			gob.transform.Find("nombre").GetComponent<Text>().text=resultado.data[i].nombre_completo;

			gob.SetActive(true);
			continue;
			RawImage imagen = gob.transform.Find("boton").GetComponent<RawImage>();

			imagen.color = new Color(0, 0, 0, 0);
			gob.name = "instaItem" + i;

			Usuario data2go = resultado.data[i];
			gob.GetComponentInChildren<Button>().onClick.AddListener(() => accionBt(data2go, gob));

			Uri ur = new Uri(resultado.data[i].avatar);

			//var request = new HTTPRequest(ur, ImageDownloaded);
			//request.Tag = gob;
			//request.Send();

		}
	}

	public void Failed(string errorMessage)
	{
		//errorMessage = errorMessage.Replace("ERROR: ", "Get achievements list failed: ");
		Alertas.instance.ShowMensaje(errorMessage);
		Debug.LogError(errorMessage);
	}

	void accionBt(Usuario data, GameObject gob)
	{

	}
}

public class Usuario
{
	public string id { get; set; }
	public string nombre_completo { get; set; }
	public string avatar { get; set; }
	public string rol { get; set; }
	public string rol_enGrupo { get; set; }
	
}

public class ResultadoGetUsuarios
{
	public string titulo { get; set; }
	public string response { get; set; }
	public int resultados { get; set; }
	public List<Usuario> data { get; set; }
}