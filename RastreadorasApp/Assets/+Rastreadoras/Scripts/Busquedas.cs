using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using System;
using System.Net;
using System.IO;
using System.Collections;
using LoginProAsset;
using UnityEngine.UI;
using RenderHeads.Media.AVProVideo;

public class Busquedas : Pantalla
{

    [Header("Búsquedas")]
    public Transform containerBusquedasActivas;
    public Transform containerBusquedasInactivas;
    public GameObject prefabBusquedas;
    [Space(10)]

    public Transform containerUsuariosBusquedas;
    public GameObject prefabUsuarioBusqueda;

    public Transform containerAnadirUsuariosBusquedas;
    public GameObject prefabAnadirUsuarioBusqueda;

    public Transform containerTestimoniosBusquedas;
    public GameObject prefabTestimonioBusqueda;

    Busqueda busquedaActual;

    [Space(10)]
    [Header("Añadir testimonio")]

    public InputField tituloTestimonio;
    public InputField notasTestimonio;
    public Button btNotas;
    public Button btListpNotas;
    public GameObject panelNotas;

    public Button btusuarios;
    public Button bttestimonios;
    public Button btweb;
    public Button btcerrar;

    public Button btAnadirUsuario;
    public Button btAnadirTestimonio;

    public Button btMenu;

    public MultibuttonBehavior trimenu;

    public ScrollRect srBusquedas;

    string tokenKey;

    public Button btTomarFoto;
    public Button btGravarVideo;
    public Button btGrabarAudio;

    public GameObject slTomarFoto;
    public GameObject slGravarVideo;
    public GameObject slGrabarAudio;

    [Header("Media")]

    string media;
    string mediaName;
    public MediaPlayer mplayer;
    public RawImage imagenMedia;
	public DisplayUGUI uiVideo;
    public RawImage audioIcon;
    public Button btPlayMedia;

    public GameObject vpHoriz;
     public Button btVideoFull;
      public Button btCierraull;
      public Button btPlayMediaFull;

      public AudioSource audioMedia;
      string tipoMediaPlaying="";

    string userTimestamp;
   

    public Slider _videoSeekSlider;
    public Text textoVideoplayerSegundos;
     public Slider _videoSeekSliderFull;
    public Text textoVideoplayerSegundosFull;

    public Text datoTestimonios;
    public Text datoUsuarios;

    public RawImage imgMapa;
    public RawImage imgMapaDetalle;
    public CamCapManager camcap;

    //portada busquedas
    public RawImage portadaBusqueda;
    public Button btTomarFotoportadaBusqueda;

    //GPS
    public PlayerLocationService player_loc;
    GeoPoint localGeoPoint = new GeoPoint();

    //public FTPManager ftpMan;

    public VoiceRecorder VR;

    ResultadoGetTestimonio testimoniosSinsubir;
    Testimonio testimonioactual;

    void Start()
    {
        base.init();
        AppManager.instance.Subscribe(this, AppManager.PantallaEnum.Busquedas);

        prefabBusquedas.SetActive(false);
        prefabBusquedas.transform.SetParent(prefabBusquedas.transform.parent.parent);

        prefabUsuarioBusqueda.SetActive(false);
        prefabUsuarioBusqueda.transform.SetParent(prefabUsuarioBusqueda.transform.parent.parent);

        prefabAnadirUsuarioBusqueda.SetActive(false);
        prefabAnadirUsuarioBusqueda.transform.SetParent(prefabAnadirUsuarioBusqueda.transform.parent.parent);

        prefabTestimonioBusqueda.SetActive(false);
        prefabTestimonioBusqueda.transform.SetParent(prefabTestimonioBusqueda.transform.parent.parent);

        transform.Find("detalleBusqueda/svDetalleBusqueda/Viewport/Content/contenido/btBack").GetComponent<Button>().onClick.AddListener(regreso);

        btusuarios.onClick.AddListener(abreusuarios);
        bttestimonios.onClick.AddListener(abrehallazgos);

        //web
        btweb.onClick.AddListener(abreweb);

        //cerrar busqueda
        btcerrar.onClick.AddListener(advertenciaFaltaImagen);

        btMenu.onClick.AddListener(trimenu.toggle);

        //buscar
        transform.Find("anadirUsuario/ifbuscar").gameObject.SetActive(false);
        transform.Find("anadirUsuario/ifbuscar").GetComponent<InputField>().onValueChanged.AddListener(delegate { ValueChangeCheck(transform.Find("anadirUsuario/ifbuscar").GetComponent<InputField>()); });

        transform.Find("anadirUsuario/btBuscar").GetComponent<Button>().onClick.AddListener(abrirSearchBar);

        //listo

        //añadir usuario
        btAnadirUsuario.onClick.AddListener(abreanadirusuario);
        transform.Find("anadirUsuario/btBack").GetComponent<Button>().onClick.AddListener(cierraanadirusuario);

        //añadir Testimonio
        btAnadirTestimonio.onClick.AddListener(abreanadirTestimonio);
        transform.Find("anadirTestimonio/btanadir").GetComponent<Button>().onClick.AddListener(anadirTestimonio);
        transform.Find("anadirTestimonio/btBack").GetComponent<Button>().onClick.AddListener(cierraanadirTestimonio);

        //detalle Testimonio
        transform.Find("detalleTestimonio/btBack").GetComponent<Button>().onClick.AddListener(cierrahallazgo);

        btGrabarAudio.onClick.AddListener(GrabarAudioHandler);
        btGravarVideo.onClick.AddListener(leyendaEscogerVideoTake);
        btTomarFoto.onClick.AddListener(leyendaEscogerPickTake);

        //media
        mplayer.Events.AddListener(OnVideoEvent);
        btPlayMedia.onClick.AddListener(playMedia);
        btPlayMediaFull.onClick.AddListener(playMedia);
        btVideoFull.onClick.AddListener(goFull);
        btCierraull.onClick.AddListener(cierraFull);

        btNotas.onClick.AddListener(abreNotas);
        btListpNotas.onClick.AddListener(cierraNotas);
        notasTestimonio.onEndEdit.AddListener(endEditNotas); 

        player_loc.onGeoPosEvent.AddListener(UpdateLocationHandler);

        btTomarFotoportadaBusqueda.onClick.AddListener(leyendaEscogerPickTakeportadaBusqueda);

        testimoniosSinsubir=new ResultadoGetTestimonio();
        testimoniosSinsubir.testimonios=new List<Testimonio>(); 

        transform.Find("detalleTestimonio/btUpload").GetComponent<Button>().onClick.AddListener(UploadTestimonio);

        if(PlayerPrefs.HasKey("testimoniosSinsubir")){
            testimoniosSinsubir=JsonConvert.DeserializeObject<ResultadoGetTestimonio>(PlayerPrefs.GetString("testimoniosSinsubir"));
            Debug.Log(" si habia data sin subir");
        }
        Debug.Log(" start de busqueda");
    }

      void goFull()
    {
        vpHoriz.SetActive(true);
    }

    void cierraFull()
    {
        vpHoriz.SetActive(false);
    }

    public override void Show()
    {
        base.Show();

        trimenu.hide();
        transform.Find("detalleBusqueda").gameObject.SetActive(false);
        transform.Find("anadirUsuario").gameObject.SetActive(false);
        transform.Find("anadirTestimonio").gameObject.SetActive(false);
        transform.Find("detalleTestimonio").gameObject.SetActive(false);

        containerBusquedasActivas.Clear();
        containerBusquedasInactivas.Clear();

        
        if(LoginPro.Session.entroEnOffline){
            string[] datas=new string[1];
            datas[0]=PlayerPrefs.GetString("ResultadoGetBusquedas");
            Success(datas);
        }else{
            Alertas.instance.ShowLoading("Cargando...");
            LoginPro.Manager.GetBusquedas(Success, Failed);
        }
        

        slTomarFoto.SetActive(false);
        slGrabarAudio.SetActive(false);
        slGravarVideo.SetActive(false);
        
        panelNotas.SetActive(false);
        notasTestimonio.text="";

        textoVideoplayerSegundos.text="00:00";
        textoVideoplayerSegundosFull.text="00:00";
            
        _videoSeekSlider.value = 0;
        _videoSeekSliderFull.value = 0;

    }

    private void abreNotas()
    {
         panelNotas.SetActive(true);
         notasTestimonio.ActivateInputField();
    }

    private void endEditNotas(string textoNotas)
    {
         panelNotas.SetActive(false);
    }

    private void cierraNotas()
    {
         panelNotas.SetActive(false);
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
        media=obj;
        mediaName = "audio_testimonio_" + LoginPro.Manager.getEpoch().ToString() + Path.GetExtension(obj).ToLower();
		Debug.LogWarning("audioPickeadaHandler obj " + obj);
        Debug.LogWarning("audioPickeadaHandler mediaName " + mediaName);

        slTomarFoto.SetActive(false);
        slGrabarAudio.SetActive(true);
        slGravarVideo.SetActive(false);
    }

    private void leyendaEscogerPickTakeportadaBusqueda()
    {
		Alertas.instance.ShowMensajeCondicional("Cómo deseas añadir imagen?","", "TOMAR", GetTakePhotoportadaBusqueda, GetPickPhotoportadaBusqueda, "GALERÍA" );
	}

    
	 private void GetPickPhotoportadaBusqueda()
    {
		#if UNITY_EDITOR || UNITY_STANDLONE
		string filePath="/PictureOnDisk.png";
		filePath = Application.dataPath + filePath;
			fotoPickeadaImagenHandlerportadaBusqueda(filePath);
		#endif
       
	   camcap.pickPhoto(fotoPickeadaImagenHandlerportadaBusqueda, camcapError);
    }

	 private void GetTakePhotoportadaBusqueda()
    {
		#if UNITY_EDITOR || UNITY_STANDLONE
		string filePath="/PictureOnDisk.png";
		filePath = Application.dataPath + filePath;
			fotoPickeadaImagenHandlerportadaBusqueda(filePath);
		#endif
       
	   camcap.takePhoto(fotoPickeadaImagenHandlerportadaBusqueda, camcapError);
    }

     private void fotoPickeadaImagenHandlerportadaBusqueda(string obj = "")
    {
        string ext=Path.GetExtension(obj).ToLower();
        Alertas.instance.ShowLoadingPercentage("Subiendo foto");
        #if UNITY_ANDROID
            Alertas.instance.ShowLoading("Subiendo foto");
        #endif
        LoginPro.Manager.UploadFile(obj, "400_300_busqueda_avatar_"+busquedaActual.id+ext, fotoPickeadaHandlerportadaBusqueda, Failed, whileUploadingEventHandler,400, "400", "300");
    }

	private void fotoPickeadaHandlerportadaBusqueda(string obj="")
    {
        Debug.Log("fotoPickeadaHandlerportadaBusqueda "+obj);
        string ext=Path.GetExtension(obj).ToLower();
		Alertas.instance.ShowLoading("Cargando...");
		LoginPro.Manager.SaveBusquedaImage( "400_300_busqueda_avatar_"+busquedaActual.id+ext, busquedaActual.id,SendToServer_SuccessportadaBusqueda, Failed);
    }

    void whileUploadingEventHandler (float porcentaje)
	{
        Alertas.instance.UpdateLoadingPercentage(porcentaje);
	}

	public void SendToServer_SuccessportadaBusqueda(string[] datas)
        {
             Debug.Log("SendToServer_SuccessportadaBusqueda "+datas[0]);
			Alertas.instance.Hide();
			btTomarFotoportadaBusqueda.gameObject.GetComponent<CanvasGroup>().alpha=0;
			LoginPro.Manager.DownloadAndPlaceImage(datas[0], portadaBusqueda);
        }

    private void leyendaEscogerVideoTake()
    {
        Alertas.instance.ShowMensajeCondicional("Cómo deseas añadir video?", "", "TOMAR", GetTakeVideo, GetPickVideo, "GALERÍA");
    }

	private void leyendaEscogerPickTake()
    {
        Alertas.instance.ShowMensajeCondicional("Cómo deseas añadir imagen?", "", "TOMAR", GetTakePhoto, GetPickPhoto, "GALERÍA");
    }

	private void GetPickVideo() 
    {
#if UNITY_EDITOR || UNITY_STANDLONE
        string filePath = "/video.mp4";
        filePath = Application.dataPath + filePath;
        videoPickeadaHandler(filePath);
#endif

        camcap.pickVideo(videoPickeadaHandler, camcapError);
    }

    private void GetTakeVideo()
    {
#if UNITY_EDITOR || UNITY_STANDLONE
        string filePath = "/video.mp4";
        filePath = Application.dataPath + filePath;
        videoPickeadaHandler(filePath);
        //return;
#endif

		camcap.captureVideo(videoPickeadaHandler, camcapError);
    }

    private void GetPickPhoto()
    {
#if UNITY_EDITOR || UNITY_STANDLONE
        string filePath = "/PictureOnDisk.png";
        filePath = Application.dataPath + filePath;
        fotoPickeadaHandler(filePath);
#endif

        camcap.pickPhoto(fotoPickeadaHandler, camcapError);
    }

    private void GetTakePhoto()
    {
#if UNITY_EDITOR || UNITY_STANDLONE
        string filePath = "/PictureOnDisk.png";
        filePath = Application.dataPath + filePath;
        fotoPickeadaHandler(filePath);
#endif

        camcap.takePhoto(fotoPickeadaHandler, camcapError);
    }

    private void camcapError(string obj = "") 
    {
        Debug.Log(obj);
    }
    private void fotoPickeadaHandler(string obj = "") 
    {
        //byte[] bytes = { };

        if (File.Exists(obj))
        {
            //bytes = File.ReadAllBytes(obj);
            Debug.Log("si existe");
        }
        //media = Convert.ToBase64String(bytes);
        media=obj;
        mediaName = "750_570_img_testimonio_" + LoginPro.Manager.getEpoch().ToString() + Path.GetExtension(obj).ToLower();;
		Debug.LogWarning("fotoPickeadaHandler obj " + obj);
        Debug.LogWarning("fotoPickeadaHandler mediaName " + mediaName);

        slTomarFoto.SetActive(true);
        slGrabarAudio.SetActive(false);
        slGravarVideo.SetActive(false);
    }

    private void videoPickeadaHandler(string obj = "")
    {
        //md.startDowload(obj);
        //byte[] bytes = { };

        if (File.Exists(obj))
        {
            //bytes = File.ReadAllBytes(obj);
            Debug.Log("videoPickeadaHandler si existe");
        }
        //media = Convert.ToBase64String(bytes);
        media=obj;
        mediaName = "video_testimonio_" + LoginPro.Manager.getEpoch().ToString() + Path.GetExtension(obj).ToLower();;
        Debug.LogWarning("videoPickeadaHandler obj " + obj);
		Debug.LogWarning("videoPickeadaHandler mediaName " + mediaName);

        slTomarFoto.SetActive(false);
        slGrabarAudio.SetActive(false);
        slGravarVideo.SetActive(true);
    }

    void abrirSearchBar()
    {
        transform.Find("anadirUsuario/ifbuscar").gameObject.SetActive(true);
        transform.Find("anadirUsuario/ifbuscar").GetComponent<InputField>().ActivateInputField();
    }

    public void SuccessTest(string[] datas)
    {
        Debug.Log("Respuesta test" + datas[0]);
    }
    public void Success(string[] datas)
    {
        Alertas.instance.Hide();

        ResultadoGetBusquedas resultado = JsonConvert.DeserializeObject<ResultadoGetBusquedas>(datas[0]);

        int counterActivas = 0;
        int counterInactivas = 0;

        for (int i = 0; i < resultado.data.Count; i++)
        {
            GameObject gob;
            if (resultado.data[i].activa == "1")
            {
                gob = Instantiate(prefabBusquedas, containerBusquedasActivas);
                counterActivas++;
            }
            else
            {
                if(LoginPro.Session.entroEnOffline)
                    continue;
                gob = Instantiate(prefabBusquedas, containerBusquedasInactivas);
                counterInactivas++;
            }
            gob.transform.Find("Titulo").GetComponent<Text>().text = resultado.data[i].nombre;
            gob.transform.Find("Lugar").GetComponent<Text>().text = resultado.data[i].lugar;
            gob.transform.Find("grupo").GetComponent<Text>().text = resultado.data[i].nombre_grupo;

            if(resultado.data[i].avatar!="")
                LoginPro.Manager.DownloadAndPlaceImage(resultado.data[i].avatar, gob.transform.Find("Imagen/Imagen").GetComponent<RawImage>());
            //Debug.Log("resultado.data[i].color_grupo "+resultado.data[i].color_grupo);
            gob.transform.Find("color").GetComponent<Image>().color= hexToColor(resultado.data[i].color_grupo);

            DateTime date;
            DateTime.TryParse(resultado.data[i].creation_date, out date);
            System.Globalization.CultureInfo culture = new System.Globalization.CultureInfo("es-MX");
            gob.transform.Find("tiempo").GetComponent<Text>().text = date.ToString("d MMMM yyyy", culture).ToTitleCase();

            gob.SetActive(true);

            //RawImage imagen = gob.transform.Find("boton").GetComponent<RawImage>();

            //imagen.color = new Color(0, 0, 0, 0);
            gob.name = "Item" + i;

            Busqueda data2go = resultado.data[i];

            gob.GetComponent<Button>().onClick.AddListener(() => abreDetalleBusqueda(data2go, gob));

            //Uri ur = new Uri(resultado.data[i].avatar);

            //var request = new HTTPRequest(ur, ImageDownloaded);
            //request.Tag = gob;
            //request.Send();

        }
        containerBusquedasActivas.parent.Find("Dato").GetComponent<Text>().text = counterActivas.ToString();
        containerBusquedasInactivas.parent.Find("Dato").GetComponent<Text>().text = counterInactivas.ToString();

        containerBusquedasInactivas.parent.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -153 - (counterActivas * 170));
        Vector2 contentsize = containerBusquedasInactivas.parent.parent.GetComponent<RectTransform>().sizeDelta;
        Vector2 inactivasPos = containerBusquedasInactivas.parent.GetComponent<RectTransform>().anchoredPosition;
        containerBusquedasInactivas.parent.parent.GetComponent<RectTransform>().sizeDelta = new Vector2(contentsize.x, 153 + (counterInactivas * 170) + (counterActivas * 170));
        LayoutRebuilder.MarkLayoutForRebuild(srBusquedas.GetComponent<RectTransform>());

        PlayerPrefs.SetString("ResultadoGetBusquedas",datas[0]);
        Debug.Log("Canvas.ForceUpdateCanvases");
    }

    void abreDetalleBusqueda(Busqueda data, GameObject gob)
    {
        transform.Find("detalleBusqueda").gameObject.SetActive(true);
        transform.Find("detalleBusqueda/svDetalleBusqueda/Viewport/Content/contenido/Titulo").GetComponent<Text>().text = data.nombre;
        transform.Find("detalleBusqueda/svDetalleBusqueda/Viewport/Content/contenido/lugar").GetComponent<Text>().text = data.lugar;
        transform.Find("detalleBusqueda/svDetalleBusqueda/Viewport/Content/contenido/fecha").GetComponent<Text>().text = data.creation_date;

        datoTestimonios.text = "";
        datoUsuarios.text = "";

        containerUsuariosBusquedas.Clear();
        containerTestimoniosBusquedas.Clear();

         if(LoginPro.Session.entroEnOffline){
            string[] datas=new string[1];
            datas[0]= PlayerPrefs.GetString("ResultadoGetBusquedas"+data.id);
            SuccessGetBusquedaInfo(datas);
        }else{
           Alertas.instance.ShowLoading("Cargando");
        //todo: respuesta de GetBusqueda debe contener  data.rol_usuario
        LoginPro.Manager.GetBusqueda(data.id, SuccessGetBusquedaInfo, Failed);
        }
    }

    public void SuccessGetBusquedaInfo(string[] datas)
    {
        Alertas.instance.Hide();

        

        containerUsuariosBusquedas.Clear();
        containerTestimoniosBusquedas.Clear();

        transform.Find("detalleBusqueda/svDetalleBusqueda/").GetComponent<ScrollRect>().verticalScrollbar.value = 1;

        transform.Find("detalleBusqueda/svDetalleBusqueda/Viewport/Content/contenido/testimonios").gameObject.SetActive(true);
        transform.Find("detalleBusqueda/svDetalleBusqueda/Viewport/Content/contenido/usuarios").gameObject.SetActive(false);

        transform.Find("detalleBusqueda/svDetalleBusqueda/Viewport/Content/itemsUsuarios").gameObject.SetActive(false);
        transform.Find("detalleBusqueda/svDetalleBusqueda/Viewport/Content/itemTestimonios").gameObject.SetActive(true);

        ResultadoGetBusquedas resultado = JsonConvert.DeserializeObject<ResultadoGetBusquedas>(datas[0]);

        tokenKey = resultado.tokenKey;

        busquedaActual = resultado.data[0];
        PlayerPrefs.SetString("ResultadoGetBusquedas"+busquedaActual.id,datas[0]);

        portadaBusqueda.texture=null;
         if (busquedaActual.avatar != ""){
            string[] ss = new string[1];
            ss[0] = busquedaActual.avatar;
            SendToServer_SuccessportadaBusqueda(ss);
        }else{
            btTomarFotoportadaBusqueda.gameObject.GetComponent<CanvasGroup>().alpha = 1;
        }

        btAnadirTestimonio.gameObject.SetActive(busquedaActual.activa == "1");
        btcerrar.gameObject.SetActive(busquedaActual.activa == "1");
        btAnadirUsuario.gameObject.SetActive(busquedaActual.activa == "1");

         Debug.Log("busquedaActual.rol_enGrupo "+busquedaActual.rol_enGrupo);
        if (busquedaActual.rol_enGrupo.ToEnum<LoginPro_GroupRole>() == LoginPro_GroupRole.Usuario && LoginPro.Session.Role != LoginPro_UserRole.Administrador){
            btcerrar.gameObject.SetActive(false);
            btAnadirUsuario.gameObject.SetActive(false);
        }

         if(LoginPro.Session.entroEnOffline){
            btcerrar.gameObject.SetActive(false);
            btAnadirUsuario.gameObject.SetActive(false);
            btweb.gameObject.SetActive(false);
        }else{

        }

        datoUsuarios.text = busquedaActual.users.Count.ToString();
        foreach (Usuario usr in busquedaActual.users)
        {
            GameObject gobUsuario = Instantiate(prefabUsuarioBusqueda, containerUsuariosBusquedas);
            gobUsuario.transform.Find("nombre").GetComponent<Text>().text = usr.nombre_completo;
            gobUsuario.transform.Find("lugar").GetComponent<Text>().text = usr.rol;
            if (usr.rol.ToEnum<LoginPro_GroupRole>() == LoginPro_GroupRole.Administrador)
                gobUsuario.transform.SetAsFirstSibling();

            gobUsuario.SetActive(true);
            
            Usuario data2go = usr;
            if(data2go.avatar!="")
            LoginPro.Manager.DownloadAndPlaceImage(data2go.avatar, gobUsuario.transform.Find("Image").GetComponent<RawImage>());
            
            gobUsuario.transform.Find("btEliminar").GetComponent<Button>().onClick.AddListener(() => kickUserdeBusqueda(data2go, gobUsuario));
            gobUsuario.transform.Find("btEliminar").gameObject.SetActive(busquedaActual.activa == "1" );
            gobUsuario.transform.Find("btEliminar").gameObject.SetActive(!LoginPro.Session.entroEnOffline);
            if (busquedaActual.rol_enGrupo.ToEnum<LoginPro_GroupRole>() == LoginPro_GroupRole.Usuario)
                gobUsuario.transform.Find("btEliminar").gameObject.SetActive(false);
            
            Debug.Log("usr.rol_enGrupo "+usr.rol_enGrupo);
            if((usr.rol_enGrupo.ToEnum<LoginPro_GroupRole>() == LoginPro_GroupRole.Administrador ||usr.rol_enGrupo.ToEnum<LoginPro_GroupRole>() == LoginPro_GroupRole.SuperAdministrador) && LoginPro.Session.Role != LoginPro_UserRole.Administrador){
                gobUsuario.transform.Find("btEliminar").gameObject.SetActive(false);
                gobUsuario.transform.SetAsFirstSibling();
            }
            if(busquedaActual.rol_enGrupo.ToEnum<LoginPro_GroupRole>() == LoginPro_GroupRole.Usuario) 
                gobUsuario.transform.Find("btEliminar").gameObject.SetActive(false);

            //gobUsuario.transform.Find("btCambiaRol").GetComponent<Button>().onClick.AddListener(() => cambiaRolUser(data2go, gobUsuario));
            gobUsuario.name = "Item";
        }
        if(LoginPro.Session.entroEnOffline){
                busquedaActual.testimonios.Clear();
        }

        if(PlayerPrefs.HasKey("testimoniosSinsubir")){
            if(LoginPro.Session.entroEnOffline){
                busquedaActual.testimonios=testimoniosSinsubir.testimonios;
            }else{
                //nuevo y anadir al principio
                //busquedaActual.testimonios.AddRange(testimoniosSinsubir.testimonios);
                busquedaActual.testimonios=testimoniosSinsubir.testimonios;
            }
        }

        datoTestimonios.text = busquedaActual.testimonios.Count.ToString();
        foreach (Testimonio testimonio in busquedaActual.testimonios)
        {
            GameObject gobTestimonio = Instantiate(prefabTestimonioBusqueda, containerTestimoniosBusquedas);
            gobTestimonio.transform.Find("titulo").GetComponent<Text>().text = testimonio.titulo;
            gobTestimonio.transform.Find("user/nombre").GetComponent<Text>().text = testimonio.nombre_completo;
            gobTestimonio.transform.Find("hora/hora").GetComponent<Text>().text = Convert.ToDateTime(testimonio.user_creation_date).ToString("HH:mm");

            gobTestimonio.transform.Find("marcaNoSubido").gameObject.SetActive(testimonio.esNoSubido==true);

            if(testimonio.avatar_usuario!="" && testimonio.avatar_usuario!=null)
                LoginPro.Manager.DownloadAndPlaceImage(testimonio.avatar_usuario, gobTestimonio.transform.Find("user/Image").GetComponent<RawImage>());

            string exten = Path.GetExtension(testimonio.media).ToLower();
            if (exten == ".mov" || exten == ".mp4" )
                gobTestimonio.transform.Find("media/video").gameObject.SetActive(true);
            if (exten == ".wav")
                gobTestimonio.transform.Find("media/audio").gameObject.SetActive(true);
            if (exten == ".png" || exten == ".jpg" || exten == ".jpeg")
                gobTestimonio.transform.Find("media/imagen").gameObject.SetActive(true);

            gobTestimonio.SetActive(true);

            Testimonio data2go = testimonio;
            gobTestimonio.GetComponent<Button>().onClick.AddListener(() => abreTestimonio(data2go, gobTestimonio));
            //gobUsuario.transform.Find("btCambiaRol").GetComponent<Button>().onClick.AddListener(() => cambiaRolUser(data2go, gobUsuario));
            gobTestimonio.name = "Item";
        }

        
    }

    void kickUserdeBusqueda(Usuario data, GameObject gob)
    {
        Alertas.instance.ShowLoading("Cargando...");
        LoginPro.Manager.KickUserFromBusqueda(data.id, busquedaActual.id, SuccessKickUserdeBusqueda, Failed);
    }

    public void SuccessKickUserdeBusqueda(string[] datas)
    {
        containerUsuariosBusquedas.Clear();
        containerTestimoniosBusquedas.Clear();

        Alertas.instance.Hide();
        Alertas.instance.ShowLoading("Cargando...");
        LoginPro.Manager.GetBusqueda(busquedaActual.id, SuccessGetBusquedaInfo, Failed);

    }

    public void Failed(string errorMessage)
    {
        Alertas.instance.ShowMensaje(errorMessage);
        Debug.Log(errorMessage);
    }


    private void ValueChangeCheck(InputField textInField)
    {
        string textoAcomparar = textInField.text.ToLower();
        foreach (Transform child in containerAnadirUsuariosBusquedas)
        {
            if (textoAcomparar.Length > child.Find("nombre").GetComponent<Text>().text.Length)
                textoAcomparar = textoAcomparar.Substring(0, child.Find("nombre").GetComponent<Text>().text.Length);

            string textoCaja = child.Find("nombre").GetComponent<Text>().text.Substring(0, textoAcomparar.Length).ToLower();
            child.gameObject.SetActive(true);
            if (textoCaja != textoAcomparar)
                child.gameObject.SetActive(false);
        }
    }

    void abreweb()
    {
        Application.OpenURL("http://rastreadoras.vis-fuerzainnecesaria.org/?id=" + busquedaActual.id + "&session=" + tokenKey);
        Debug.Log("http://rastreadoras.vis-fuerzainnecesaria.org/?id=" + busquedaActual.id + "&session=" + tokenKey);
    }

    void abreanadirusuario()
    {
        transform.Find("anadirUsuario").gameObject.SetActive(true);
        transform.Find("anadirUsuario/Titulo").GetComponent<Text>().text = busquedaActual.nombre;

        transform.Find("anadirUsuario/ifbuscar").GetComponent<InputField>().text = "";
        transform.Find("anadirUsuario/ifbuscar").gameObject.SetActive(false);

        containerAnadirUsuariosBusquedas.Clear();

        //poblar de usuarios
        //filtrar a los que ya tienen asignacion, o a mi
        Alertas.instance.ShowLoading("Cargando...");

        LoginPro.Manager.GetUsersWithoutBusqueda(busquedaActual.id, busquedaActual.grupo,SuccessGetNonBusquedaUsers, Failed);

    }

    public void SuccessGetNonBusquedaUsers(string[] datas)
    {
        Alertas.instance.Hide();
        Debug.Log(datas[0]);
        ResultadoGetUsuarios resultado = JsonConvert.DeserializeObject<ResultadoGetUsuarios>(datas[0]);


        for (int i = 0; i < resultado.data.Count; i++)
        {
            GameObject gob = Instantiate(prefabAnadirUsuarioBusqueda, containerAnadirUsuariosBusquedas);
            gob.transform.Find("nombre").GetComponent<Text>().text = resultado.data[i].nombre_completo;

            gob.SetActive(true);
            //continue;
            //RawImage imagen = gob.transform.Find("boton").GetComponent<RawImage>();

            //imagen.color = new Color(0, 0, 0, 0);
            //gob.name = "instaItem" + i;

            Usuario data2go = resultado.data[i];
            gob.GetComponentInChildren<Button>().onClick.AddListener(() => AddUsuarioToBusqueda(data2go, gob));

            if(data2go.avatar!="")
            LoginPro.Manager.DownloadAndPlaceImage(data2go.avatar, gob.transform.Find("Image").GetComponent<RawImage>());


        }
    }

    void AddUsuarioToBusqueda(Usuario data, GameObject gob)
    {
        Alertas.instance.ShowLoading("Cargando...");
        LoginPro.Manager.AddUserToBusqueda(data.id, busquedaActual.id, SuccessAddUserToBusqueda, Failed);
    }

    public void SuccessAddUserToBusqueda(string[] datas)
    {
        Alertas.instance.Hide();
        transform.Find("anadirUsuario").gameObject.SetActive(false);
        Alertas.instance.ShowLoading("Cargando...");
        LoginPro.Manager.GetBusqueda(busquedaActual.id, SuccessGetBusquedaInfo, Failed);

        containerUsuariosBusquedas.Clear();
        containerTestimoniosBusquedas.Clear();
    }

    void anadirTestimonio()
    {
        if (string.IsNullOrEmpty(tituloTestimonio.text))
			{
				Failed("Falta título.");
				return;
			}

    float len = new FileInfo(media).Length/1024f/1024f;
    if (len>80f)
    {
        Failed("Archivo sobrepasa 80Mb.");
        return;
    }

     if (!Input.location.isEnabledByUser)
    {
        Failed("Falta Permiso para GPS.");
        return;
    }

    

     userTimestamp=DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");


        if(LoginPro.Session.entroEnOffline){
            Testimonio ts=new Testimonio();
            ts.id=busquedaActual.id;
            ts.media= media;
            Debug.Log("ts.media:"+ts.media);
            ts.esNoSubido= true;
            ts.mediaName= mediaName;
            ts.titulo=tituloTestimonio.text;
            ts.notas= notasTestimonio.text;
            ts.lat= player_loc.loc.ToUseStringLat();
            ts.lon= player_loc.loc.ToUseStringLon();
            ts.user_creation_date=userTimestamp;

            testimoniosSinsubir.testimonios.Add(ts);

            PlayerPrefs.SetString( "testimoniosSinsubir", JsonConvert.SerializeObject(testimoniosSinsubir));

            SuccessAnadirTestimonio(null);
            return;
        }

        Alertas.instance.ShowLoadingPercentage("Subiendo archivo");
        #if UNITY_ANDROID
            Alertas.instance.ShowLoading("Subiendo foto");
        #endif
        Debug.Log(File.Exists(media));
        Debug.Log(File.Exists(Path.GetFullPath(media)));

        if(media!=""){
            string exten = Path.GetExtension(media).ToLower();
            if (exten == ".png" || exten == ".jpg" || exten == ".jpeg")
                LoginPro.Manager.UploadFile(Path.GetFullPath(media), mediaName, SubirArchivo, Failed, whileDownloadingEventHandler, 750,"750", "570");
            else
                LoginPro.Manager.UploadFile(Path.GetFullPath(media), mediaName, SubirArchivo, Failed, whileDownloadingEventHandler);
        }
        else
        SubirArchivo("");
    }

    void SubirArchivooff(string link)
    {
        Debug.Log("<<<SubirArchivooff "+link);
        Alertas.instance.ShowLoading("Añadiendo testimonio offline");
        LoginPro.Manager.AnadirTestimonio(testimonioactual.id, testimonioactual.media, testimonioactual.mediaName, testimonioactual.titulo, "", testimonioactual.notas, testimonioactual.lat, testimonioactual.lon, testimonioactual.user_creation_date,SuccessAnadirTestimoniooff, Failed);
       
    }

    void SubirArchivo(string link)
    {
        Debug.Log("SubirArchivo "+link);
        Alertas.instance.ShowLoading("Añadiendo testimonio");
        LoginPro.Manager.AnadirTestimonio(busquedaActual.id, media, mediaName, tituloTestimonio.text, "", notasTestimonio.text, player_loc.loc.ToUseStringLat(), player_loc.loc.ToUseStringLon(), userTimestamp,SuccessAnadirTestimonio, Failed);
       
    }
    
    void whileDownloadingEventHandler (float porcentaje)
	{
        Alertas.instance.UpdateLoadingPercentage(porcentaje);
	}

    public void SuccessAnadirTestimoniooff(string[] datas)
    {
        tituloTestimonio.text = "";
        //lugarTestimonio.text = "";
        notasTestimonio.text = "";

        media="";

        if(PlayerPrefs.HasKey("testimoniosSinsubir")){
            foreach(Testimonio ts in testimoniosSinsubir.testimonios.ToArray()){
                if(ts.user_creation_date==testimonioactual.user_creation_date)
                testimoniosSinsubir.testimonios.Remove(ts);
            }
            if(testimoniosSinsubir.testimonios.Count==0)
                PlayerPrefs.DeleteKey("testimoniosSinsubir");
        }
       

        player_loc.StopLocationService();
        transform.Find("detalleTestimonio").gameObject.SetActive(false);

        Debug.Log("<<<SuccessAnadirTestimoniooff "+datas[0]);

        if(LoginPro.Session.entroEnOffline){
            string[] datas2=new string[1];
            datas2[0]= PlayerPrefs.GetString("ResultadoGetBusquedas"+busquedaActual.id);
            SuccessGetBusquedaInfo(datas2);
        }else{
          Alertas.instance.ShowLoading("Cargando...");
        LoginPro.Manager.GetBusqueda(busquedaActual.id, SuccessGetBusquedaInfo, Failed);
        }


    }

    public void SuccessAnadirTestimonio(string[] datas)
    {
        tituloTestimonio.text = "";
        //lugarTestimonio.text = "";
        notasTestimonio.text = "";

        Debug.Log("<<<SuccessAnadirTestimonio "+datas[0]);

        media="";

        player_loc.StopLocationService();
        transform.Find("anadirTestimonio").gameObject.SetActive(false);

        if(LoginPro.Session.entroEnOffline){
            string[] datas2=new string[1];
            datas2[0]= PlayerPrefs.GetString("ResultadoGetBusquedas"+busquedaActual.id);
            SuccessGetBusquedaInfo(datas2);
        }else{
            Alertas.instance.ShowLoading("Cargando...");
            LoginPro.Manager.GetBusqueda(busquedaActual.id, SuccessGetBusquedaInfo, Failed);
        }
    }

    void regreso()
    {
        transform.Find("detalleBusqueda").gameObject.SetActive(false);
    }

    void abrehallazgos()
    {
        transform.Find("detalleBusqueda/svDetalleBusqueda/Viewport/Content/contenido/testimonios").gameObject.SetActive(true);
        transform.Find("detalleBusqueda/svDetalleBusqueda/Viewport/Content/contenido/usuarios").gameObject.SetActive(false);

        transform.Find("detalleBusqueda/svDetalleBusqueda/Viewport/Content/itemsUsuarios").gameObject.SetActive(false);
        transform.Find("detalleBusqueda/svDetalleBusqueda/Viewport/Content/itemTestimonios").gameObject.SetActive(true);
    }
    void abreusuarios()
    {
        transform.Find("detalleBusqueda/svDetalleBusqueda/Viewport/Content/contenido/testimonios").gameObject.SetActive(false);
        transform.Find("detalleBusqueda/svDetalleBusqueda/Viewport/Content/contenido/usuarios").gameObject.SetActive(true);

        transform.Find("detalleBusqueda/svDetalleBusqueda/Viewport/Content/itemsUsuarios").gameObject.SetActive(true);
        transform.Find("detalleBusqueda/svDetalleBusqueda/Viewport/Content/itemTestimonios").gameObject.SetActive(false);
    }

    void advertenciaFaltaImagen()
    {
        if (busquedaActual.avatar == ""){
            Alertas.instance.ShowMensajeCondicional("Falta foto principal de búsqueda.", "Añade una imagen del carrete de fotos o toma una ahora.", "OK");
        }else
        {
            empezarCerrarBusqueda();
        }
    }

    void empezarCerrarBusqueda()
    {
        Alertas.instance.ShowMensajeCondicional("Deseas finalizar búsqueda?", "Si cierras la búsqueda no podrás volver a editarla. ¿Estás seguro que quieres finalizar búsqueda?", "SI", cerrarBusqueda);
    }

    void cerrarBusqueda()
    {
        Alertas.instance.ShowLoading("Cargando...");
        LoginPro.Manager.cerrarBusqueda(busquedaActual.id, SuccessCerrarBusqueda, Failed);
    }

    public void SuccessCerrarBusqueda(string[] datas)
    {
        Debug.Log(datas[0]);
        containerBusquedasActivas.Clear();
        containerBusquedasInactivas.Clear();

        Alertas.instance.ShowLoading("Cargando...");
        LoginPro.Manager.GetBusqueda(busquedaActual.id, SuccessGetBusquedaInfo, Failed);
        LoginPro.Manager.GetBusquedas(Success, Failed);
    }

    void abreTestimonio(Testimonio data2go, GameObject gobTestimonio)
    {
        transform.Find("detalleTestimonio").gameObject.SetActive(true);

        if(LoginPro.Session.entroEnOffline || PlayerPrefs.HasKey("testimoniosSinsubir")){
            string[] datas=new string[1];
            ResultadoGetTestimonio resultadoInventado=new ResultadoGetTestimonio();
            resultadoInventado.testimonios=new List<Testimonio>(); 
            resultadoInventado.testimonios.Add(data2go);
            datas[0]=JsonConvert.SerializeObject(resultadoInventado);
            SuccessGetTestimonioInfo(datas);
        }else{
            Alertas.instance.ShowLoading("Cargando...");
            LoginPro.Manager.GetTestimonio(data2go.id, SuccessGetTestimonioInfo, Failed);
        }        
    }

    public void SuccessGetTestimonioInfo(string[] datas)
    {
        Alertas.instance.Hide();

        vpHoriz.SetActive(false);
        btVideoFull.gameObject.SetActive(false);

        ResultadoGetTestimonio resultado = JsonConvert.DeserializeObject<ResultadoGetTestimonio>(datas[0]);

        testimonioactual=resultado.testimonios[0]; 
        Debug.Log(testimonioactual.media);
        
        string url = "https://api.mapbox.com/styles/v1/mapbox/outdoors-v10/static/" + testimonioactual.lon + "," + testimonioactual.lat + ",14.0,0,0/751x550?access_token=pk.eyJ1Ijoiam9yZ2VsdWlzIiwiYSI6ImNqY3FzdXgzYjA1ZHAycWtjcHZzY3l5MzYifQ.HuYtu0QitrUSx94SKmq78w";
        LoginPro.Manager.DownloadAndPlaceImage(url, imgMapaDetalle);

        transform.Find("detalleTestimonio/Titulo").GetComponent<Text>().text = testimonioactual.titulo;
        transform.Find("detalleTestimonio/notas").GetComponent<Text>().text = testimonioactual.notas;
        transform.Find("detalleTestimonio/lugar").GetComponent<Text>().text = testimonioactual.donde_estas;
        transform.Find("detalleTestimonio/coordenadas").GetComponent<Text>().text = testimonioactual.lat + "," + testimonioactual.lon;
        transform.Find("detalleTestimonio/user/nombre").GetComponent<Text>().text = testimonioactual.nombre_completo;

        transform.Find("detalleTestimonio/btUpload").gameObject.SetActive(testimonioactual.esNoSubido==true && !LoginPro.Session.entroEnOffline);
       
       if(testimonioactual.avatar_usuario!="" && testimonioactual.avatar_usuario!=null)
            LoginPro.Manager.DownloadAndPlaceImage(testimonioactual.avatar_usuario, transform.Find("detalleTestimonio/user/foto").GetComponent<RawImage>());

        if (testimonioactual.media != "")
            muestraMedia(testimonioactual.media);
    }

    void UploadTestimonio()
    {
        Alertas.instance.ShowLoadingPercentage("Subiendo archivo");
        #if UNITY_ANDROID
            Alertas.instance.ShowLoading("Subiendo foto");
        #endif
        if(testimonioactual.media!=""){
            string exten = Path.GetExtension(testimonioactual.media).ToLower();
            if (exten == ".png" || exten == ".jpg" || exten == ".jpeg")
                LoginPro.Manager.UploadFile(Path.GetFullPath(testimonioactual.media), testimonioactual.mediaName, SubirArchivooff, Failed, whileDownloadingEventHandler,750, "750", "570");
            else
                LoginPro.Manager.UploadFile(Path.GetFullPath(testimonioactual.media), testimonioactual.mediaName, SubirArchivooff, Failed, whileDownloadingEventHandler);
        }
        else
            SubirArchivooff("");
    }

    void muestraMedia(string media)
    {
       
		MediaPlayer.FileLocation _location = MediaPlayer.FileLocation.AbsolutePathOrURL;

        imagenMedia.gameObject.SetActive(false);
		uiVideo.gameObject.SetActive(false);

        if(mplayer.Control!=null)
		    mplayer.Control.Stop();

        audioIcon.gameObject.SetActive(false);
        btPlayMedia.gameObject.SetActive(false);
        btPlayMediaFull.gameObject.SetActive(false);
        btPlayMedia.GetComponent<CanvasGroup>().alpha = 1;
        btPlayMediaFull.GetComponent<CanvasGroup>().alpha = 1;

        string exten = Path.GetExtension(media).ToLower();

        Debug.Log("muestraMedia " + media + " - " + exten);
         if (exten == ".mov" || exten == ".mp4" )
        {
			uiVideo.gameObject.SetActive(true);
        	mplayer.m_VideoPath = media;

            if(mplayer.Control!=null)
                mplayer.Control.SetVolume(.2f);

            mplayer.OpenVideoFromFile(_location, media, false);
            btPlayMedia.gameObject.SetActive(true);
            btPlayMediaFull.gameObject.SetActive(true);
            btVideoFull.gameObject.SetActive(true);
			
        }
        if (exten == ".wav" || exten == ".mp3" )
        {
            uiVideo.gameObject.SetActive(true);
        	mplayer.m_VideoPath = media;
             if(mplayer.Control!=null)
                mplayer.Control.SetVolume(.6f);
            mplayer.OpenVideoFromFile(_location, media, false);
            audioIcon.gameObject.SetActive(true);
            btPlayMedia.gameObject.SetActive(true);
            btPlayMediaFull.gameObject.SetActive(true);
            //StartCoroutine(LoadTrack(media));
        }

        if (exten == ".png" || exten == ".jpg" || exten == ".jpeg")
        {
             if(LoginPro.Session.entroEnOffline || testimonioactual.esNoSubido){
                 LoginPro.Manager.LoadLocalImage(media, imagenMedia);
            }else{
                LoginPro.Manager.DownloadAndPlaceImage(media, imagenMedia);
            }
            
           imagenMedia.gameObject.SetActive(true);
        }


        /*Alertas.instance.Hide();
        byte[] fileBytes = Convert.FromBase64String(media);
        var tex = new Texture2D(1, 1);
        tex.LoadImage(fileBytes);

         DestroyImmediate (imagenMedia.texture, true);
        imagenMedia.texture = tex;
        tex = null;

        imagenMedia.gameObject.SetActive(true);
        Alertas.instance.Hide();*/
    }

    IEnumerator audioStream(string url){
         WWW www = new WWW(url);
     while (www.progress < 0.1)
     {
         Debug.Log(www.progress);
         yield return new WaitForSeconds(0.1f);
     }
     audioMedia.clip = www.GetAudioClip(false, true, AudioType.WAV);
     //streamPlayStart = true;

    }

    IEnumerator DownloadAndPlay(string url)
{
    WWW www = new WWW(url);
    yield return www;
    audioMedia.clip = www.GetAudioClip(false, true,AudioType.WAV);
    audioMedia.Play();
}

IEnumerator LoadTrack(string filename)
    {
        var www = new WWW(filename);

        //Wait for file finish loading
        while(!www.isDone)
        {
            Debug.LogFormat("Progress loading {0}: {1}", filename, www.progress);
            yield return null;
        }
        
        Debug.Log("loaded");
        var clip = www.GetAudioClip(false, true, AudioType.WAV);

        audioMedia.clip = clip;
        audioMedia.Play();
    }


    void playMedia()
    {
        if (!mplayer.Control.IsPlaying())
        {
            mplayer.Control.Play();
           /* string exten = Path.GetExtension(mplayer.m_VideoPath).ToLower();
         if (exten == ".mov" || exten == ".mp4" )
            {
                mplayer.Control.SetVolume(.2f);
                mplayer.Control.Seek(5000);
            }
            else
            {
                mplayer.Control.SetVolume(.6f);
            }*/
             mplayer.Control.SetVolume(.6f);
            btPlayMedia.GetComponent<CanvasGroup>().alpha = 0;
            btPlayMediaFull.GetComponent<CanvasGroup>().alpha = 0;
        }
        else
        {
            btPlayMedia.GetComponent<CanvasGroup>().alpha = 1;
            btPlayMediaFull.GetComponent<CanvasGroup>().alpha = 1;
            mplayer.Control.Pause();
        }
    }

    /*private IEnumerator LoadAsset(string url, RawImage _rawImage)
	{
		WWW www = new WWW(url);
		float elapsedTime = 0.0f;

		while (!www.isDone)
		{
			elapsedTime += Time.deltaTime;
			if (elapsedTime >= 10.0f) break;
			yield return null;
		}

		if (!www.isDone || !string.IsNullOrEmpty(www.error))
		{
			Debug.LogError("Load Failed");
			Alertas.instance.Hide();
			yield break;
		}

		_rawImage.GetComponent<RawImage>().texture = www.texture;
		imagenMedia.gameObject.SetActive(true);
		Alertas.instance.Hide();
	}*/

     public static Color hexToColor(string hex)
    {
        Color myColor = new Color();
        ColorUtility.TryParseHtmlString("#"+hex, out myColor);
        return myColor;
    }

    void cierrahallazgo()
    {
        transform.Find("detalleTestimonio").gameObject.SetActive(false);
        if(mplayer.Control!=null)
        mplayer.Control.Stop();
    }


    void UpdateLocationHandler(GeoPoint gp)
    {
        localGeoPoint = gp;
        Debug.Log("got geopoint " + gp.ToString());
        transform.Find("anadirTestimonio/lugar").GetComponent<Text>().text = player_loc.loc.ToUseStringBack();
        string url = "https://api.mapbox.com/styles/v1/mapbox/outdoors-v10/static/" + localGeoPoint.ToUseString() + ",14.0,0,0/751x550?access_token=pk.eyJ1Ijoiam9yZ2VsdWlzIiwiYSI6ImNqY3FzdXgzYjA1ZHAycWtjcHZzY3l5MzYifQ.HuYtu0QitrUSx94SKmq78w";
        LoginPro.Manager.DownloadAndPlaceImage(url, imgMapa);
    }

    void abreanadirTestimonio()
    {
        player_loc.StartLocationService();
        Debug.Log("abreanadirTestimonio " + player_loc.loc.ToString());

        slTomarFoto.SetActive(false);
        slGrabarAudio.SetActive(false);
        slGravarVideo.SetActive(false);

        media = "0";
        //btGravarVideo.gameObject.SetActive(true);
        btTomarFoto.gameObject.SetActive(true);
        //btGrabarAudio.gameObject.SetActive(true);

        transform.Find("anadirTestimonio/lugar").GetComponent<Text>().text = player_loc.loc.ToUseStringBack();

        transform.Find("anadirTestimonio").gameObject.SetActive(true);

        
        if (!Input.location.isEnabledByUser)
        {
            Alertas.instance.ShowMensaje("La aplicación no tiene permiso para acceder a GPS");
            return;
        }
    }

    void cierraanadirusuario()
    {
        transform.Find("anadirUsuario").gameObject.SetActive(false);
    }

    void cierraanadirTestimonio()
    {
        transform.Find("anadirTestimonio").gameObject.SetActive(false);
        player_loc.StopLocationService();
    }

    public void OnVideoEvent(MediaPlayer mp, MediaPlayerEvent.EventType et, ErrorCode errorCode)
    {
        switch (et)
        {
            case MediaPlayerEvent.EventType.ReadyToPlay:
                break;
            case MediaPlayerEvent.EventType.Started:
                break;
            case MediaPlayerEvent.EventType.FirstFrameReady:
                break;
            case MediaPlayerEvent.EventType.FinishedPlaying:
            btPlayMedia.GetComponent<CanvasGroup>().alpha = 1;
            btPlayMediaFull.GetComponent<CanvasGroup>().alpha = 1;
                break;
        }

        Debug.Log("Event: " + et.ToString());
    }
void Update()
    {
        if(mplayer.Control==null)
            return;
        if (mplayer.Control.IsPlaying())
        {
            float time = mplayer.Control.GetCurrentTimeMs();
            float duration = mplayer.Info.GetDurationMs();
            float d = Mathf.Clamp(time / duration, 0.0f, 1.0f);
            textoVideoplayerSegundos.text=(time/1000).ToString("00")+":"+(duration/1000).ToString("00");
            textoVideoplayerSegundosFull.text=(time/1000).ToString("00")+":"+(duration/1000).ToString("00");
            _videoSeekSlider.value = d;
            _videoSeekSliderFull.value = d;
        }
    }
    void LateUpdate()
    {
        /* takeHiResShot |= Input.GetKeyDown("k");
         if (takeHiResShot) {
			 //StartCoroutine(mapdl());
			 //return;
             RenderTexture rt = new RenderTexture(resWidth, resHeight, 24);
             mapCamera.targetTexture = rt;
             Texture2D screenShot = new Texture2D(resWidth, resHeight, TextureFormat.RGB24, false);
             mapCamera.Render();
             RenderTexture.active = rt;
             screenShot.ReadPixels(new Rect(0, 0, resWidth, resHeight), 0, 0);
			 screenShot.Apply();
			 mapaImage.texture=screenShot;
             mapCamera.targetTexture = null;
             RenderTexture.active = null; // JC: added to avoid errors
             Destroy(rt);
             takeHiResShot = false;

			 byte[] bytes = screenShot.EncodeToPNG();
             string filename = ScreenShotName(resWidth, resHeight);
             System.IO.File.WriteAllBytes(filename, bytes);
             Debug.Log(string.Format("Took screenshot to: {0}", filename));
             takeHiResShot = false;
			 Debug.Log("takeHiResShot");
         }*/
    }

   /* void dd2(){
using (WebClient wc = new WebClient())
{
    wc.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
    wc.UploadFileCompleted += new UploadFileCompletedEventHandler (UploadFileCallback);
    wc.UploadFileAsync(new System.Uri("path"), "POST", "filePath");
}
    }
private void UploadFileCallback(System.Object sender, UploadFileCompletedEventArgs e)
{
    if (e.Error != null)
         print("error");
    else
        print("success");
}*/
}

public class Busqueda
{
    public string rol_enGrupo { get; set; }
    public string id { get; set; }
    public string lugar { get; set; }
    public string avatar { get; set; }
    public string nombre { get; set; }
    public string grupo { get; set; }
    public string nombre_grupo { get; set; }
    public string color_grupo { get; set; }
    public string activa { get; set; }
    public string creation_date { get; set; }
    public List<Usuario> users { get; set; }
    public List<Testimonio> testimonios { get; set; }
}

public class Testimonio
{
    public string id { get; set; }
    public string creator_id { get; set; }
    public string creation_date { get; set; }
    public string user_creation_date { get; set; }
    public string titulo { get; set; }
    public string donde_estas { get; set; }
    public string lat { get; set; }
    public string lon { get; set; }
    public string notas { get; set; }
    public string media { get; set; }
    public string mediaName { get; set; }
    public string nombre_completo { get; set; }
    public string avatar_usuario { get; set; }
    public bool esNoSubido { get; set; }
}

public class ResultadoGetBusquedas
{
    public string titulo { get; set; }
    public string response { get; set; }
    public int resultados { get; set; }
    public string tokenKey { get; set; }

    public List<Busqueda> data { get; set; }
}


public class ResultadoGetTestimonio
{
    public string titulo { get; set; }
    public string response { get; set; }
    public int resultados { get; set; }
    public List<Testimonio> testimonios { get; set; }
}
