using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LoginProAsset;
using System;
using System.IO;
using System.Collections;


public class Grupos : Pantalla
{

    public Transform container;
    public GameObject prefab;

    public Transform containerUsuarios;
    public GameObject prefabUsuario;

    public InputField nombre;
    public InputField ubicacion;

    Grupo grupoConsultado;

    public InputField nombreBusquedaAnadir;
    public InputField locationBusquedaAnadir;

    public Button btAnadirUsuario;
    public Button btAnadirBusqueda;
    public Button btConfigurarGrupo;

    public Transform containerAddUsuarios;
    public GameObject prefabAddUsuario;

    public Button btMenu;
    public MultibuttonBehavior trimenu;

    public InputField Password;
    public InputField ConfirmPassword;
    public InputField Username;
    public InputField Mail;

    public Button btBackDeAgregarUsuario;
    public Button btAgregarUsuario;

    public Button btVerUsuariosExistentes;
    public Button btBackDeAfilliate;

    public ScrollRect usuariosGrupo;

    public Transform colorToggles;
    Toggle chosencolorToggle;

    public RawImage imagenGrupo;
    public Button btPickImageGrupo;
    public CamCapManager camcap;

    void Start()
    {
        base.init();
        AppManager.instance.Subscribe(this, AppManager.PantallaEnum.Grupos);

        prefab.SetActive(false);
        prefab.transform.SetParent(prefab.transform.parent.parent);

        prefabAddUsuario.SetActive(false);
        prefabAddUsuario.transform.SetParent(prefabAddUsuario.transform.parent.parent);

        prefabUsuario.SetActive(false);
        prefabUsuario.transform.SetParent(prefabUsuario.transform.parent.parent);

        transform.Find("Main/btAnadir").GetComponent<Button>().onClick.AddListener(openAddGrupo);

        transform.Find("anadirGrupo/btBack").GetComponent<Button>().onClick.AddListener(Show);
        transform.Find("anadirGrupo/btAdd").GetComponent<Button>().onClick.AddListener(addGrupo);

        transform.Find("detalleGrupo/btBack").GetComponent<Button>().onClick.AddListener(regreso);

        btBackDeAgregarUsuario.onClick.AddListener(cerrarAddUser);
        btAgregarUsuario.onClick.AddListener(() => { addUser(); });


        btMenu.onClick.AddListener(trimenu.toggle);

        btConfigurarGrupo.onClick.AddListener(AbrirEditarGrupo);
        btAnadirUsuario.onClick.AddListener(abreanadirusuario);

        transform.Find("anadirUsuario/btBack").GetComponent<Button>().onClick.AddListener(cierraanadirusuario);

        transform.Find("editarGrupo/btGuardar").GetComponent<Button>().onClick.AddListener(EditarGrupo);
        transform.Find("editarGrupo/btBack").GetComponent<Button>().onClick.AddListener(CerrarEditarGrupo);

        btAnadirBusqueda.onClick.AddListener(abrisAnadirBusqueda);

        transform.Find("anadirBusqueda/btBack").GetComponent<Button>().onClick.AddListener(regreso);
        transform.Find("anadirBusqueda/btGenerico").GetComponent<Button>().onClick.AddListener(AnadirBusqueda);

        btVerUsuariosExistentes.onClick.AddListener(abreanadirusuarioDeLista);
        btBackDeAfilliate.GetComponent<Button>().onClick.AddListener(cierraAffilliateUser);

        btPickImageGrupo.onClick.AddListener(leyendaEscogerPickTake);
    }

    private void leyendaEscogerPickTake()
    {
        Alertas.instance.ShowMensajeCondicional("Cómo deseas añadir imagen?", "", "TOMAR", GetTakePhoto, GetPickPhoto, "GALERÍA");
    }
    private void GetPickPhoto()
    {
#if UNITY_EDITOR || UNITY_STANDLONE
        string filePath = "/PictureOnDisk.png";
        filePath = Application.dataPath + filePath;
        fotoPickeadaImagenHandler(filePath);
        return;
#endif

        camcap.pickPhoto(fotoPickeadaImagenHandler, camcapError);
    }

    private void GetTakePhoto()
    {
#if UNITY_EDITOR || UNITY_STANDLONE
        string filePath = "/PictureOnDisk.png";
        filePath = Application.dataPath + filePath;
        fotoPickeadaImagenHandler(filePath);
        return;
#endif

        camcap.takePhoto(fotoPickeadaImagenHandler, camcapError);
    }

    private void camcapError(string obj = "") 
    {
        Debug.Log(obj);
    }

    private void fotoPickeadaImagenHandler(string obj = "")
    {
        string ext=Path.GetExtension(obj).ToLower();

        Alertas.instance.ShowLoadingPercentage("Subiendo foto");
        LoginPro.Manager.UploadFile(obj, "300_300_grupo_avatar_" + grupoConsultado.id + ext, fotoPickeadaHandler, Failed, whileUploadingEventHandler,300,"300","300");
    }

    void whileUploadingEventHandler (float porcentaje)
	{
        Alertas.instance.UpdateLoadingPercentage(porcentaje);
	}

    private void fotoPickeadaHandler(string obj = "")
    {
        Alertas.instance.ShowLoading("Cargando...");
        Debug.Log("fotoPickeadaHandler " + obj);
        string ext=Path.GetExtension(obj).ToLower();
        LoginPro.Manager.SaveGroupImage( "300_300_grupo_avatar_" + grupoConsultado.id + ext, grupoConsultado.id, SendToServer_Success, Failed);

    }

    public void SendToServer_Success(string[] datas)
    {
        Alertas.instance.Hide();
        Debug.Log("SendToServer_Success " + datas[0]);
        btPickImageGrupo.gameObject.GetComponent<CanvasGroup>().alpha = 0;
        LoginPro.Manager.DownloadAndPlaceImage(datas[0], imagenGrupo);
    }


    /*IEnumerator LoadImage(string path, RawImage pickPreiveimage)
	{

		var url = "file://" + path;
		#if UNITY_EDITOR || UNITY_STANDLONE
		url = "file:/"+path;
		#endif
		Debug.Log ("current path is " + url);
		var www = new WWW(url);
		yield return www;

		var texture = www.texture;
		if (texture == null)
		{
			Debug.LogError("Failed to load texture url:" + url);
		}

		DestroyImmediate (pickPreiveimage.texture);
		pickPreiveimage.texture = texture;
		texture = null;
	}*/

    private void colorChanged(Toggle tg)
    {
        if (tg.isOn)
        {
            chosencolorToggle = tg;
            ValueChangeUpdateGroupInfo(null);
        }
    }

    public override void Show()
    {
        base.Show();
        Debug.Log("show grupos");

        trimenu.hide();

        transform.Find("affiliateUsuario").gameObject.SetActive(false);
        transform.Find("anadirBusqueda").gameObject.SetActive(false);
        transform.Find("anadirGrupo").gameObject.SetActive(false);
        transform.Find("detalleGrupo").gameObject.SetActive(false);
        transform.Find("anadirUsuario").gameObject.SetActive(false);
        transform.Find("editarGrupo").gameObject.SetActive(false);

        container.Clear();

        Alertas.instance.ShowLoading("Cargando...");
        LoginPro.Manager.GetGroups(Success, Failed);

        if (LoginPro.Session.Role == LoginPro_UserRole.Usuario)
        {
            transform.Find("Main/btAnadir").gameObject.SetActive(false);
        }

    }


    void cierraAffilliateUser()
    {
        transform.Find("affiliateUsuario").gameObject.SetActive(false);
    }
    void cerrarAddUser()
    {
        transform.Find("anadirUsuario").gameObject.SetActive(false);
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
        else
        {

            Alertas.instance.ShowLoading("Registrando");
            LoginPro.Manager.Register(Username.text, Mail.text, Password.text, grupoConsultado.id,SuccessRegister, Failed);

        }
    }

    public void SuccessRegister(string[] datas)
    {
        Alertas.instance.ShowMensaje(datas[0], "OK", abreanadirusuarioDeLista);
    }

    void AnadirBusqueda()
    {
        Alertas.instance.ShowLoading("Cargando...");
        LoginPro.Manager.CreateBusqueda(nombreBusquedaAnadir.text, locationBusquedaAnadir.text, grupoConsultado.id, SuccessAddBusqueda, Failed);
    }

    public void SuccessAddBusqueda(string[] datas)
    {
        Alertas.instance.Hide();
        transform.Find("anadirBusqueda").gameObject.SetActive(false);

        Debug.Log(datas[0]);
        Alertas.instance.ShowLoading("Cargando...");
        LoginPro.Manager.GetBusquedas(Success, Failed);

    }

    void abrisAnadirBusqueda()
    {
        transform.Find("anadirBusqueda").gameObject.SetActive(true);
    }

    void CerrarEditarGrupo()
    {
        transform.Find("editarGrupo").gameObject.SetActive(false);
    }

    void openAddGrupo()
    {
        transform.Find("anadirGrupo").gameObject.SetActive(true);
        nombre.text = "";
        ubicacion.text = "";
    }

    void AbrirEditarGrupo()
    {
        transform.Find("editarGrupo").gameObject.SetActive(true);
        transform.Find("editarGrupo/titulo").GetComponent<Text>().text = grupoConsultado.name;

        transform.Find("editarGrupo/IFNombre").GetComponent<InputField>().text = grupoConsultado.name;
        transform.Find("editarGrupo/IFUbicacion").GetComponent<InputField>().text = grupoConsultado.location;

        transform.Find("editarGrupo/IFNombre").GetComponent<InputField>().onValueChanged.RemoveAllListeners();
        transform.Find("editarGrupo/IFNombre").GetComponent<InputField>().onValueChanged.AddListener(delegate { ValueChangeUpdateGroupInfo(transform.Find("editarGrupo/IFNombre").GetComponent<InputField>()); });

        transform.Find("editarGrupo/IFUbicacion").GetComponent<InputField>().onValueChanged.RemoveAllListeners();
        transform.Find("editarGrupo/IFUbicacion").GetComponent<InputField>().onValueChanged.AddListener(delegate { ValueChangeUpdateGroupInfo(transform.Find("editarGrupo/IFUbicacion").GetComponent<InputField>()); });

        foreach (Toggle tg in colorToggles.GetComponentsInChildren<Toggle>())
        {
            tg.onValueChanged.RemoveAllListeners();
            tg.isOn = false;
            Debug.Log(grupoConsultado.color + " - " + ColorUtility.ToHtmlStringRGB(tg.transform.Find("Background").GetComponent<Image>().color));
            if (grupoConsultado.color == ColorUtility.ToHtmlStringRGB(tg.transform.Find("Background").GetComponent<Image>().color))
                tg.isOn = true;
            //Debug.Log("color changed to "+ColorUtility.ToHtmlStringRGB(chosencolorToggle.transform.Find("Background").GetComponent<Image>().color));
            tg.onValueChanged.AddListener((val) => { colorChanged(tg); });
        }


        transform.Find("editarGrupo/btGuardar").gameObject.SetActive(false);

    }

    void abreanadirusuarioDeLista()
    {
        transform.Find("affiliateUsuario").gameObject.SetActive(true);
        transform.Find("affiliateUsuario/Titulo").GetComponent<Text>().text = grupoConsultado.name;
        containerAddUsuarios.Clear();
        //poblar de usuarios
        //filtrar a los que ya tienen asignacion, o a mi
        Alertas.instance.ShowLoading("Cargando...");
        LoginPro.Manager.GetNonAffiliatedUsers(grupoConsultado.id, SuccessGetNonAffiliatedUsers, Failed);

        transform.Find("affiliateUsuario/ifbuscar").GetComponent<InputField>().text = "";
        transform.Find("affiliateUsuario/ifbuscar").gameObject.SetActive(false);
    }

    public void SuccessGetNonAffiliatedUsers(string[] datas)
    {
        Alertas.instance.Hide();
        Debug.Log(datas[0]);
        ResultadoGetUsuarios resultado = JsonConvert.DeserializeObject<ResultadoGetUsuarios>(datas[0]);

        containerAddUsuarios.Clear();
        for (int i = 0; i < resultado.data.Count; i++)
        {
            GameObject gob = Instantiate(prefabAddUsuario, containerAddUsuarios);
            gob.transform.Find("nombre").GetComponent<Text>().text = resultado.data[i].nombre_completo;

            gob.SetActive(true);
            //continue;
            //RawImage imagen = gob.transform.Find("boton").GetComponent<RawImage>();

            //imagen.color = new Color(0, 0, 0, 0);
            //gob.name = "instaItem" + i;

            Usuario data2go = resultado.data[i];
            gob.GetComponentInChildren<Button>().onClick.AddListener(() => AddUsuarioToGroup(data2go, gob));

            //Uri ur = new Uri(resultado.data[i].avatar);

            //var request = new HTTPRequest(ur, ImageDownloaded);
            //request.Tag = gob;
            //request.Send();

        }
    }

    void AddUsuarioToGroup(Usuario data, GameObject gob)
    {
        Alertas.instance.ShowLoading("Cargando...");
        LoginPro.Manager.AfilliateUser(data.id, grupoConsultado.id, LoginPro_GroupRole.Usuario.ToString(), SuccessAffiliateUser, Failed);


    }

    public void SuccessAffiliateUser(string[] datas)
    {
        Alertas.instance.Hide();
        transform.Find("anadirUsuario").gameObject.SetActive(false);
        transform.Find("affiliateUsuario").gameObject.SetActive(false);
        Alertas.instance.ShowLoading("Cargando...");
        LoginPro.Manager.GetGroup(grupoConsultado.id, SuccessGetGroupInfo, Failed);

    }

    private void ValueChangeCheck(InputField textInField)
    {
        string textoAcomparar = textInField.text.ToLower();
        foreach (Transform child in containerAddUsuarios)
        {
            if (textoAcomparar.Length > child.Find("nombre").GetComponent<Text>().text.Length)
                textoAcomparar = textoAcomparar.Substring(0, child.Find("nombre").GetComponent<Text>().text.Length);

            string textoCaja = child.Find("nombre").GetComponent<Text>().text.Substring(0, textoAcomparar.Length).ToLower();
            child.gameObject.SetActive(true);
            if (textoCaja != textoAcomparar)
                child.gameObject.SetActive(false);
        }
    }


    private void ValueChangeUpdateGroupInfo(InputField textInField)
    {
        transform.Find("editarGrupo/btGuardar").gameObject.SetActive(true);
    }


    void EditarGrupo()
    {
        Alertas.instance.ShowLoading("Cargando");
        string color = ColorUtility.ToHtmlStringRGB(chosencolorToggle.transform.Find("Background").GetComponent<Image>().color);
        LoginPro.Manager.UpdateGroup(transform.Find("editarGrupo/IFNombre").GetComponent<InputField>().text, color, transform.Find("editarGrupo/IFUbicacion").GetComponent<InputField>().text, grupoConsultado.id, SuccessEditGrupo, Failed);
        Debug.Log("grupoConsultado.id " + grupoConsultado.id);
    }

    void addGrupo()
    {
        Debug.Log("add grupo launched.");
        Alertas.instance.ShowLoading("Creando grupo");
        LoginPro.Manager.CreateGroup(nombre.text, ubicacion.text, SuccessaddGrupo, Failed);
    }

    public void SuccessEditGrupo(string[] datas)
    {
        Alertas.instance.Hide();

        Debug.Log(datas[0]);
        Alertas.instance.ShowLoading("Cargando...");

        transform.Find("editarGrupo").gameObject.SetActive(false);

        LoginPro.Manager.GetGroup(grupoConsultado.id, SuccessGetGroupInfo, Failed);


    }

    public void SuccessaddGrupo(string[] datas)
    {
        Alertas.instance.Hide();

        Debug.Log(datas[0]);

        ResultadoGenerico respuesta = JsonConvert.DeserializeObject<ResultadoGenerico>(datas[0]);

        Show();
    }

    void regreso()
    {
        transform.Find("anadirGrupo").gameObject.SetActive(false);
        transform.Find("detalleGrupo").gameObject.SetActive(false);
        transform.Find("anadirUsuario").gameObject.SetActive(false);
        transform.Find("anadirBusqueda").gameObject.SetActive(false);
    }

    void abreanadirusuario()
    {
        transform.Find("anadirUsuario").gameObject.SetActive(true);
        Password.text = "";
        ConfirmPassword.text = "";
        Username.text = "";
        Mail.text = "";
    }

    void cierraanadirusuario()
    {
        transform.Find("anadirUsuario").gameObject.SetActive(false);
    }

    void eliminarusuario()
    {
        Alertas.instance.ShowMensajeCondicional("Eliminar como administrador?", "", "Eliminar");
    }

    public void Success(string[] datas)
    {
        Alertas.instance.Hide();

        Debug.Log(datas[0]);

        ResultadoGetGrupos resultado = JsonConvert.DeserializeObject<ResultadoGetGrupos>(datas[0]);

        transform.Find("Main/Dato").GetComponent<Text>().text = resultado.resultados.ToString();

        container.Clear();

        for (int i = 0; i < resultado.data.Count; i++)
        {
            GameObject gob = Instantiate(prefab, container);
            gob.transform.Find("nombre").GetComponent<Text>().text = resultado.data[i].name;
            gob.transform.Find("Lugar").GetComponent<Text>().text = resultado.data[i].location;

			gob.transform.Find("color").GetComponent<Image>().color= hexToColor(resultado.data[i].color);

            if(resultado.data[i].avatar!="")
            LoginPro.Manager.DownloadAndPlaceImage(resultado.data[i].avatar, gob.transform.Find("Image").GetComponent<RawImage>());

            gob.SetActive(true);

            //RawImage imagen = gob.transform.Find("boton").GetComponent<RawImage>();

            //imagen.color = new Color(0, 0, 0, 0);
            gob.name = "Item" + i;

            Grupo data2go = resultado.data[i];
            gob.GetComponent<Button>().onClick.AddListener(() => abreDetalleGrupo(data2go, gob));

            //Uri ur = new Uri(resultado.data[i].avatar);

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

    void abreDetalleGrupo(Grupo data, GameObject gob)
    {
        transform.Find("detalleGrupo").gameObject.SetActive(true);

        containerUsuarios.Clear();
        grupoConsultado = data;

        Alertas.instance.ShowLoading("Cargando");
        LoginPro.Manager.GetGroup(data.id, SuccessGetGroupInfo, Failed);

    }

    public void SuccessGetGroupInfo(string[] datas)
    {
        Alertas.instance.Hide();

        ResultadoGetGrupos resultado = JsonConvert.DeserializeObject<ResultadoGetGrupos>(datas[0]);


        Grupo gp = resultado.data[0];

        grupoConsultado = gp;
        if (gp.avatar != "")
        {
            btPickImageGrupo.gameObject.GetComponent<CanvasGroup>().alpha = 0;
            string[] ss = new string[1];
            ss[0] = gp.avatar;
            SendToServer_Success(ss);
        }
        else
        {
            btPickImageGrupo.gameObject.GetComponent<CanvasGroup>().alpha = 1;
        }

        LoginPro_GroupRole role = gp.esAdmin.ToEnum<LoginPro_GroupRole>();
        Debug.Log("rolsss  " + role.ToString()+ " - "+LoginPro.Session.Role);
        
        btMenu.gameObject.SetActive(true);

        if(role == LoginPro_GroupRole.Usuario && LoginPro.Session.Role != LoginPro_UserRole.Administrador)
            btMenu.gameObject.SetActive(false);

        transform.Find("detalleGrupo/titulo").GetComponent<Text>().text = gp.name;
        transform.Find("detalleGrupo/lugar").GetComponent<Text>().text = gp.location;
        transform.Find("detalleGrupo/fecha").GetComponent<Text>().text = gp.creation_date;

        containerUsuarios.Clear();

        Transform userLocal = null;

        foreach (Usuario usr in gp.users)
        {
            GameObject gobUsuario = Instantiate(prefabUsuario, containerUsuarios);
            gobUsuario.transform.Find("nombre").GetComponent<Text>().text = usr.nombre_completo;
            gobUsuario.transform.Find("lugar").GetComponent<Text>().text = usr.rol;

            gobUsuario.SetActive(true);

            Usuario data2go = usr;
            gobUsuario.transform.Find("btUnaffiliate").GetComponent<Button>().onClick.AddListener(() => unafilliateUser(data2go, gobUsuario));
            gobUsuario.transform.Find("btCambiaRol").GetComponent<Button>().onClick.AddListener(() => cambiaRolUser(data2go, gobUsuario));
            gobUsuario.name = "Item";

             if(usr.avatar!="")
            LoginPro.Manager.DownloadAndPlaceImage(usr.avatar, gobUsuario.transform.Find("Image").GetComponent<RawImage>());

            if (usr.rol.ToEnum<LoginPro_GroupRole>() == LoginPro_GroupRole.Administrador || usr.rol.ToEnum<LoginPro_GroupRole>() == LoginPro_GroupRole.SuperAdministrador)
            {
                gobUsuario.transform.Find("btCambiaRol").GetComponent<Image>().color = AppManager.instance.colorUI;

                gobUsuario.transform.Find("btUnaffiliate").gameObject.SetActive(false);
            }

            if(role == LoginPro_GroupRole.Usuario && LoginPro.Session.Role != LoginPro_UserRole.Administrador){
                gobUsuario.transform.Find("btCambiaRol").GetComponent<Button>().interactable = false;
                gobUsuario.transform.Find("btUnaffiliate").gameObject.SetActive(false);
            }
            
            if (usr.rol.ToEnum<LoginPro_GroupRole>() == LoginPro_GroupRole.SuperAdministrador && LoginPro.Session.Role != LoginPro_UserRole.Administrador)
                    gobUsuario.transform.Find("btCambiaRol").GetComponent<Button>().interactable = false;
        }

        usuariosGrupo.verticalScrollbar.value = 1;

        if (userLocal != null)
            userLocal.SetAsFirstSibling();
    }

    void unafilliateUser(Usuario data, GameObject gob)
    {
        Alertas.instance.ShowLoading("Cargando...");
        LoginPro.Manager.unAfilliateUser(data.id, SuccessunAffiliateUser, Failed);
    }

    public void SuccessunAffiliateUser(string[] datas)
    {
        Alertas.instance.Hide();
        containerUsuarios.Clear();

        //transform.Find("anadirUsuario").gameObject.SetActive(false);
        Alertas.instance.ShowLoading("Cargando...");
        LoginPro.Manager.GetGroup(grupoConsultado.id, SuccessGetGroupInfo, Failed);

    }

    void cambiaRolUser(Usuario data, GameObject gob)
    {
        Alertas.instance.ShowLoading("Cargando...");

        bool isUserAdmin = false;
        if (data.rol.ToEnum<LoginPro_GroupRole>() == LoginPro_GroupRole.Administrador || data.rol.ToEnum<LoginPro_GroupRole>() == LoginPro_GroupRole.SuperAdministrador)
            isUserAdmin = true;

        if (LoginPro.Session.Role == LoginPro_UserRole.Administrador)
        {
            LoginPro.Manager.ChangeUserGroupRole(data.id, isUserAdmin ? LoginPro_GroupRole.Usuario.ToString() : LoginPro_GroupRole.SuperAdministrador.ToString(), SuccessCambiaRolUser, Failed);
        }
        else
        {
            LoginPro.Manager.ChangeUserGroupRole(data.id, isUserAdmin ? LoginPro_GroupRole.Usuario.ToString() : LoginPro_GroupRole.Administrador.ToString(), SuccessCambiaRolUser, Failed);
        }

    }

    public void SuccessCambiaRolUser(string[] datas)
    {
        Alertas.instance.Hide();
        //transform.Find("anadirUsuario").gameObject.SetActive(false);
        Alertas.instance.ShowLoading("Cargando...");
        LoginPro.Manager.GetGroup(grupoConsultado.id, SuccessGetGroupInfo, Failed);

    }

    public static string colorToHex(Color32 color)
    {
        string hex = color.r.ToString("X2") + color.g.ToString("X2") + color.b.ToString("X2");
        return hex;
    }

    public static Color hexToColor(string hex)
    {
        Color myColor = new Color();
        ColorUtility.TryParseHtmlString("#"+hex, out myColor);
        return myColor;
    }

}

public class Grupo
{
    public string color { get; set; }
    public string avatar { get; set; }
    public string id { get; set; }
    public string name { get; set; }
    public string location { get; set; }
    public string creation_date { get; set; }
    public List<Usuario> users { get; set; }
    public string esAdmin { get; set; }

}

public class ResultadoGetGrupos
{
    public string titulo { get; set; }
    public string response { get; set; }
    public int resultados { get; set; }
    public List<Grupo> data { get; set; }
}

public class ResultadoGenerico
{
    public string titulo { get; set; }
    public string response { get; set; }
    public int resultados { get; set; }
}
