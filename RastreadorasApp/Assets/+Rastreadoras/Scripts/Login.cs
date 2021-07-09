using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LoginProAsset;

public class Login : Pantalla
{

    public InputField Username;
    public InputField Password;

    public Button btGrabarAudio1;
    public Button btGrabarAudio2;
    public Button btGrabarAudio3;
    public Button btSiguienteAudio1;
    public Button btSiguienteAudio2;
    public Button btSiguienteAudio3;
    //public FTPManager ftpMan;
    public VoiceRecorder VR;

	public Toggle offlineToggle;

    string media1 = "";
    string mediaName1 = "";
    string media2 = "";
    string mediaName2 = "";
    string media3 = "";
    string mediaName3 = "";

    string cur_time;

    void Start()
    {
        //LoginPro.Manager.LoginSimple("sadmin", "admin", SuccessSimple, Error);
        transform.Find("Main/version").GetComponent<Text>().text="V"+Application.version;
        Debug.Log("Application Version : " + Application.version);

        btGrabarAudio1.onClick.AddListener(GrabarAudioHandler);
		btGrabarAudio2.onClick.AddListener(GrabarAudioHandler);
		btGrabarAudio3.onClick.AddListener(GrabarAudioHandler);

        transform.Find("Main/btLogin").GetComponent<Button>().onClick.AddListener(btLoginHandler);
        base.init();
        AppManager.instance.Subscribe(this, AppManager.PantallaEnum.Login);

        //7 inicia cuestionario usuario missing
        //13 inicia anadir cuestionario
        //12 termina cuestionario y esta boton finalizar
        //18 termina y regresa a 12

        transform.Find("cuestionarioInicial/pag1/btSiguiente").GetComponent<Button>().onClick.AddListener(pasaapag2);

        for (int i = 2; i < 7; i++)
        {
            int xx = i;
            transform.Find("cuestionarioInicial/pag" + xx + "/btSiguiente").GetComponent<Button>().onClick.AddListener(() => ProcessNext(xx));
            transform.Find("cuestionarioInicial/pag" + xx + "/btBack").GetComponent<Button>().onClick.AddListener(() => transform.Find("cuestionarioInicial/pag" + xx + "").gameObject.SetActive(false));
			transform.Find("cuestionarioInicial/pag" + xx + "/btBack").gameObject.SetActive(false);
		}

        for (int i = 7; i < 12; i++)
        {
            int xx = i;
            transform.Find("cuestionarioInicial/pag" + xx + "/btSiguiente").GetComponent<Button>().onClick.AddListener(() => transform.Find("cuestionarioInicial/pag" + (xx + 1) + "").gameObject.SetActive(true));
            transform.Find("cuestionarioInicial/pag" + xx + "/btBack").GetComponent<Button>().onClick.AddListener(() => transform.Find("cuestionarioInicial/pag" + xx + "").gameObject.SetActive(false));
        }

        transform.Find("cuestionarioInicial/pag12/btSiguiente").gameObject.SetActive(false);
        transform.Find("cuestionarioInicial/pag12/btAnadir").GetComponent<Button>().onClick.AddListener(() => transform.Find("cuestionarioInicial/pag13").gameObject.SetActive(true));
        transform.Find("cuestionarioInicial/pag12/btSiguiente").GetComponent<Button>().onClick.AddListener(() => finaliza());
        transform.Find("cuestionarioInicial/pag12/btBack").GetComponent<Button>().onClick.AddListener(() => transform.Find("cuestionarioInicial/pag12").gameObject.SetActive(false));

        for (int i = 13; i < 19; i++)
        {
            int xx = i;
            transform.Find("cuestionarioInicial/pag" + xx + "/btSiguiente").GetComponent<Button>().onClick.AddListener(() => transform.Find("cuestionarioInicial/pag" + (xx + 1) + "").gameObject.SetActive(true));
            transform.Find("cuestionarioInicial/pag" + xx + "/btBack").GetComponent<Button>().onClick.AddListener(() => transform.Find("cuestionarioInicial/pag" + xx + "").gameObject.SetActive(false));
        }

        transform.Find("cuestionarioInicial/pag19/btSiguiente").GetComponent<Button>().onClick.AddListener(() => pasaapag12());
        transform.Find("cuestionarioInicial/pag19/btBack").GetComponent<Button>().onClick.AddListener(() => transform.Find("cuestionarioInicial/pag19").gameObject.SetActive(false));

        Show();
		Username.onEndEdit.AddListener(enedithandler);

        if(PlayerPrefs.HasKey("lastloginwithBt")){
            DateTime epochStart = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            double curt = (int)((System.DateTime.UtcNow - epochStart).TotalSeconds);

            double curtSaved=double.Parse(PlayerPrefs.GetString("lastloginwithBt"));

            bool nohanpasado3horas=curt-curtSaved<10800;

            if(PlayerPrefs.HasKey("user") && PlayerPrefs.HasKey("entroEnOffline") && PlayerPrefs.HasKey("password") && nohanpasado3horas){
            Username.text=PlayerPrefs.GetString("user");
            Password.text=PlayerPrefs.GetString("password");
            offlineToggle.isOn=PlayerPrefs.GetString("entroEnOffline")=="true";
            Debug.Log("entro auto");

            btLoginHandler();
            }
        }

		

    }

    public void SuccessSimple(string[] datas)
    {
        Debug.Log("logged in "+ datas[0]);
        LoginPro.Manager.getData(datas[0], SuccessSimpleGet, Error);

        
    }

    public void SuccessSimpleGet(string[] datas)
    {
         Debug.Log("resouesta data " + datas[0]);

    }

	private void focusIF()
    {
		Username.ActivateInputField();
		Debug.Log("focusIF");
	}

	private void enedithandler(string end)
    {
		Debug.Log("enedithandler "+end);
	}

	private void ProcessNext(int page)
    {
		
        switch(page){
			case 2:
				Alertas.instance.ShowLoadingPercentage("Subiendo audio");
                LoginPro.Manager.UploadFile(Path.GetFullPath(media1), mediaName1, SubirArchivo1, Failed, whileUploadingEventHandler);
			break;
			case 3:
				Alertas.instance.ShowLoadingPercentage("Subiendo audio");
                LoginPro.Manager.UploadFile(Path.GetFullPath(media2), mediaName2, SubirArchivo2, Failed, whileUploadingEventHandler);
			break;
			case 4:
				Alertas.instance.ShowLoadingPercentage("Subiendo audio");
                LoginPro.Manager.UploadFile(Path.GetFullPath(media3), mediaName3, SubirArchivo3, Failed, whileUploadingEventHandler);
			break;
			case 5:
				string nombreCompleto=transform.Find("cuestionarioInicial/pag5/IFNombre").GetComponent<InputField>().text;
				string dia=transform.Find("cuestionarioInicial/pag5/fechanac/IFDia").GetComponent<InputField>().text;
				string mes=transform.Find("cuestionarioInicial/pag5/fechanac/IFMes").GetComponent<InputField>().text;
				string ano=transform.Find("cuestionarioInicial/pag5/fechanac/IFAno").GetComponent<InputField>().text;
				string estadoNacimiento=transform.Find("cuestionarioInicial/pag5/lugarnac/IFEstado").GetComponent<InputField>().text;
				string municipioNacimiento =transform.Find("cuestionarioInicial/pag5/lugarnac/IFMunicipio").GetComponent<InputField>().text;
				string localidadNacimiento =transform.Find("cuestionarioInicial/pag5/lugarnac/IFLocalidad").GetComponent<InputField>().text;
				
                if (int.Parse(dia)>31){
                    Failed("Dia inválido.");
                    return;
                }
                if (int.Parse(mes)>12){
                    Failed("Mes inválido.");
                    return;
                }
				string[] campos={nombreCompleto, dia, mes, ano, estadoNacimiento, municipioNacimiento, localidadNacimiento};
				foreach(string campo in campos)
					if (string.IsNullOrEmpty(campo)) 
						{
							Failed("Faltan campos.");
							return;
						}

				transform.Find("cuestionarioInicial/pag6").gameObject.SetActive(true);
			break;
			case 6:
			string estadoResidencia=transform.Find("cuestionarioInicial/pag6/lugarres/IFEstado").GetComponent<InputField>().text;
			string municipioResidencia=transform.Find("cuestionarioInicial/pag6/lugarres/IFMunicipio").GetComponent<InputField>().text;
			string localidadResidencia =transform.Find("cuestionarioInicial/pag6/lugarres/IFLocalidad").GetComponent<InputField>().text;
			string ocupacion =transform.Find("cuestionarioInicial/pag6/IFOcupacion").GetComponent<InputField>().text;
		
			string[] campos2={estadoResidencia, municipioResidencia, localidadResidencia, ocupacion};
				foreach(string campo in campos2)
					if (string.IsNullOrEmpty(campo)) 
						{
							Failed("Faltan campos.");
							return;
						}
				finaliza();
			break;
		}
    }
 	
	 void SubirArchivo1(string link){
         Debug.Log("se subio archivo "+link);
		 Alertas.instance.Hide();
       transform.Find("cuestionarioInicial/pag3").gameObject.SetActive(true);
    }
	void SubirArchivo2(string link){
		Alertas.instance.Hide();
       transform.Find("cuestionarioInicial/pag4").gameObject.SetActive(true);
    }
	void SubirArchivo3(string link){
		Alertas.instance.Hide();
       transform.Find("cuestionarioInicial/pag5").gameObject.SetActive(true);
    }

	public void Failed(string errorMessage)
    {
        Alertas.instance.ShowMensaje(errorMessage);
        Debug.Log(errorMessage);
    }

	void whileUploadingEventHandler (float porcentaje)
	{
        Alertas.instance.UpdateLoadingPercentage(porcentaje);
	}

    private void GrabarAudioHandler()
    {
        VR.openRecorder(audioPickeadaHandler);
    }

    private void audioPickeadaHandler(string obj = "")
    {
        if (File.Exists(obj))
        {
            Debug.Log("si existe");
        }
        if (media1 == "")
        {
            media1 = obj;
            mediaName1 = "audio_cuestionario_1_" + LoginPro.Manager.getEpoch().ToString() + Path.GetExtension(obj).ToLower();
            btGrabarAudio1.transform.Find("slFoto").gameObject.SetActive(true);
            btSiguienteAudio1.gameObject.SetActive(true);
			Debug.Log("audioPickeadaHandler media1");
        }else if (media2 == "")
        {
            media2 = obj;
            mediaName2 = "audio_cuestionario_2_" + LoginPro.Manager.getEpoch().ToString() + Path.GetExtension(obj).ToLower();
            btGrabarAudio2.transform.Find("slFoto").gameObject.SetActive(true);
            btSiguienteAudio2.gameObject.SetActive(true);
			Debug.Log("audioPickeadaHandler media2");
        }else if (media3 == "")
        {
            media3 = obj;
            mediaName3 = "audio_cuestionario_3_" + LoginPro.Manager.getEpoch().ToString() + Path.GetExtension(obj).ToLower();
            btGrabarAudio3.transform.Find("slFoto").gameObject.SetActive(true);
            btSiguienteAudio3.gameObject.SetActive(true);
			Debug.Log("audioPickeadaHandler media3");

        }
    }

    public override void Show()
    {
        base.Show();
        transform.Find("cuestionarioInicial").transform.HideChildren();
        transform.Find("cuestionarioInicial").gameObject.SetActive(true);
    }

    void btLoginHandler()
    {
        if (LoginPro.Manager != null)
        {
            DateTime epochStart = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        cur_time = ((System.DateTime.UtcNow - epochStart).TotalSeconds).ToString();

        PlayerPrefs.SetString("lastloginwithBt", cur_time);

            LoginPro.Session.entroEnOffline = false;
			if(offlineToggle.isOn){
                Debug.Log(" off");
                offlineToggle.isOn=false;
				if((Username.text==PlayerPrefs.GetString("user")) && (Password.text==PlayerPrefs.GetString("password") ) && PlayerPrefs.HasKey("contestoQuestionario")){
					//Alertas.instance.ShowMensajeCondicional("Quieres entrar en modo offline?","","SI",EntrarEnModoOffline);
					EntrarEnModoOffline();
					return;
				}else{
					Alertas.instance.ShowMensajeCondicional("Error en modo offline", "Las credenciales son diferentes a las guardadas o no se ha completado el perfil");
                    return;
                }
			}
            Debug.Log(" on");
            Alertas.instance.ShowLoading("Cargando");
            LoginPro.Manager.Login(Username.text, Password.text, Success, Error);
        }
    }

    void pasaapag12()
    {
        transform.Find("cuestionarioInicial").transform.HideChildren();
        transform.Find("cuestionarioInicial/pag12").gameObject.SetActive(true);
        transform.Find("cuestionarioInicial/pag12/btSiguiente").gameObject.SetActive(true);
    }

    void pasaapag2()
    {
        if (!transform.Find("cuestionarioInicial/pag1/TogglesSiNo/si").GetComponent<Toggle>().isOn)
            transform.Find("cuestionarioInicial/pag2").gameObject.SetActive(true);
        else
            transform.Find("cuestionarioInicial/pag2").gameObject.SetActive(true);
    }

    void finaliza()
    {
		string nombreCompleto=transform.Find("cuestionarioInicial/pag5/IFNombre").GetComponent<InputField>().text;
		int selSexo=transform.Find("cuestionarioInicial/pag5/DDSexo").GetComponent<Dropdown>().value;
		string sexo =transform.Find("cuestionarioInicial/pag5/DDSexo").GetComponent<Dropdown>().options[selSexo].text; 
		string dia=transform.Find("cuestionarioInicial/pag5/fechanac/IFDia").GetComponent<InputField>().text;
		string mes=transform.Find("cuestionarioInicial/pag5/fechanac/IFMes").GetComponent<InputField>().text;
		string ano=transform.Find("cuestionarioInicial/pag5/fechanac/IFAno").GetComponent<InputField>().text;
		string fechaNacimiento =dia+"/"+mes+"/"+ano; 
		string estadoNacimiento=transform.Find("cuestionarioInicial/pag5/lugarnac/IFEstado").GetComponent<InputField>().text;
		string municipioNacimiento =transform.Find("cuestionarioInicial/pag5/lugarnac/IFMunicipio").GetComponent<InputField>().text;
		string localidadNacimiento =transform.Find("cuestionarioInicial/pag5/lugarnac/IFLocalidad").GetComponent<InputField>().text;
		string estadoResidencia=transform.Find("cuestionarioInicial/pag6/lugarres/IFEstado").GetComponent<InputField>().text;
		string municipioResidencia=transform.Find("cuestionarioInicial/pag6/lugarres/IFMunicipio").GetComponent<InputField>().text;
		string localidadResidencia =transform.Find("cuestionarioInicial/pag6/lugarres/IFLocalidad").GetComponent<InputField>().text;
		string ocupacion =transform.Find("cuestionarioInicial/pag6/IFOcupacion").GetComponent<InputField>().text;
		string estadoCivil =""; 
		string parentezcoDesaparecido=""; 
		string familiarDesaparecido=transform.Find("cuestionarioInicial/pag1/TogglesSiNo/si").GetComponent<Toggle>().isOn?"1":"0";
		familiarDesaparecido="0";

		 if (string.IsNullOrEmpty(nombreCompleto) || string.IsNullOrEmpty(dia))
			{
				Failed("Faltan campos.");
				return;
			}
        
		Alertas.instance.ShowLoading("Guardando cuestionario");
        LoginPro.Manager.AddCuestionario(mediaName1,mediaName2,mediaName3, nombreCompleto, sexo, fechaNacimiento ,  estadoNacimiento,  municipioNacimiento ,  localidadNacimiento ,  estadoResidencia,  municipioResidencia,  localidadResidencia ,  ocupacion ,  estadoCivil ,   parentezcoDesaparecido,  familiarDesaparecido, SuccessUploadCuestionario, Error);
    }

    private void SuccessUploadCuestionario(string[] obj)
    {
        Alertas.instance.ShowMensaje("Cuestionario guardado.");
        AppManager.instance.intiApp();
    }

    public void Error(string errorMessage)
    {
        
        Debug.LogWarning(errorMessage);

        Alertas.instance.Hide();

        Alertas.instance.ShowMensaje(errorMessage);

    }

	public void EntrarEnModoOffline()
    {
		ColorUtility.TryParseHtmlString(PlayerPrefs.GetString("color"), out AppManager.instance.colorUI);

        if (AppManager.instance.colorUI == Color.white)
            AppManager.instance.colorUI = Color.gray;

        foreach (AdoptUIColor ac in FindObjectsOfType<AdoptUIColor>())
            ac.colorize();

            PlayerPrefs.SetString("entroEnOffline", "true");	

			LoginPro.Session.entroEnOffline = true;
			LoginPro.Session.LoggedIn = true;
			LoginPro.Session.Role = PlayerPrefs.GetString("role").ToEnum<LoginPro_UserRole>();
			LoginPro.Session.idUser = PlayerPrefs.GetString("iduser");
            AppManager.instance.intiApp();
	}

    public void Success(string[] datas)
    {
        PlayerPrefs.SetString("entroEnOffline", "false");

		PlayerPrefs.SetString("user", Username.text);	
		PlayerPrefs.SetString("password", Password.text);	
		PlayerPrefs.SetString("role", datas[2]);
		PlayerPrefs.SetString("iduser", datas[10]);

        //foreach (string ss in datas)
        //Debug.Log(ss);
        // Save information in session
        LoginPro.Session.Session_id = datas[1];
        LoginPro.Session.LoggedIn = true;
        LoginPro.Session.Role = datas[2].ToEnum<LoginPro_UserRole>();
        LoginPro.Session.Username = Username.text;
        LoginPro.Session.Password = Password.text;
        LoginPro.Session.Mail = datas[3];
        LoginPro.Session.RegistrationDate = datas[4];
        LoginPro.Session.CurrentConnectionDate = DateTime.Now;
        LoginPro.Session.PreviousConnectionDate = datas[5];
        double minutesPlayed = 0;
        double.TryParse(datas[6], out minutesPlayed);
        LoginPro.Session.MinutesPlayed = minutesPlayed;
        LoginPro.Session.NombreCompleto = datas[7];
        LoginPro.Session.idUser = datas[10];
        LoginPro.Session.avatar = datas[11];
        LoginPro.Session.estado_busqueda_propia = datas[12];
        Debug.Log("estado_busqueda " + LoginPro.Session.estado_busqueda_propia);

        LoginPro_ShowLogin.MenuShown = false;

        Debug.Log("Login succeeded. ");

        Alertas.instance.Hide();

        ColorUtility.TryParseHtmlString("#" + datas[9], out AppManager.instance.colorUI);
		PlayerPrefs.SetString("color", "#" + datas[9]);	

        if (AppManager.instance.colorUI == Color.white)
            AppManager.instance.colorUI = Color.gray;

        Debug.Log(AppManager.instance.colorUI);
        foreach (AdoptUIColor ac in FindObjectsOfType<AdoptUIColor>())
            ac.colorize();

        bool contestoQuestionario = datas[8] == "1";

		if(PlayerPrefs.HasKey("contestoQuestionario"))	
			PlayerPrefs.DeleteKey("contestoQuestionario");

        if (contestoQuestionario)
        {
			PlayerPrefs.SetString("contestoQuestionario", "true");	
			Debug.Log("contestoQuestionario");
            AppManager.instance.intiApp();
            return;
        }

        transform.Find("cuestionarioInicial/pag1").gameObject.SetActive(true);
    }



}