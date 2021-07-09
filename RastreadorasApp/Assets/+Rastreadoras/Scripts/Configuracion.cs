using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using UnityEngine.UI;
using LoginProAsset;

public class Configuracion : Pantalla
{

    public Button btLogout;
    public Button btGuardar;
    public Button btEstatus;

    public Button btMenu;
    public MultibuttonBehavior trimenu;

    public InputField Password;
    public InputField ConfirmPassword;
    public InputField Username;
    public InputField Mail;

    public RawImage imagenGrupo;
    public Button btPickImageGrupo;
    public CamCapManager camcap;
    public NativeCamCapManager Natcamcap;

     public GameObject pregunta;
     public Button btAudio;
     public Button btGuardarAudio;
    public GameObject slGrabarAudio;

    public Toggle siTg;
    public Toggle noTg;

    public VoiceRecorder VR;
     string media;
    string mediaName;

    void Start()
    {
        base.init();
        AppManager.instance.Subscribe(this, AppManager.PantallaEnum.Configuracion);

        btMenu.onClick.AddListener(trimenu.toggle);

        btEstatus.onClick.AddListener(abrecambioestatus);

        btGuardar.onClick.AddListener(UpdateUser);

        btLogout.onClick.AddListener(logout);

        Password.onValueChanged.AddListener(apareceBtSave);
        ConfirmPassword.onValueChanged.AddListener(apareceBtSave);
        Username.onValueChanged.AddListener(apareceBtSave);
        Mail.onValueChanged.AddListener(apareceBtSave);

        btPickImageGrupo.onClick.AddListener(leyendaEscogerPickTake);

        btGuardarAudio.onClick.AddListener(anadirAudio);
        btAudio.onClick.AddListener(GrabarAudioHandler);

          btGuardarAudio.gameObject.SetActive(false);
        btAudio.gameObject.SetActive(false);
        pregunta.gameObject.SetActive(false);
        slGrabarAudio.gameObject.SetActive(false);
    }

    void tgSiHandler(bool val)
    {
        if(val){
            btGuardarAudio.gameObject.SetActive(true);
        btAudio.gameObject.SetActive(true);
        pregunta.gameObject.SetActive(true);
        //slGrabarAudio.gameObject.SetActive(true);
        }else{
            btGuardarAudio.gameObject.SetActive(false);
        btAudio.gameObject.SetActive(false);
        pregunta.gameObject.SetActive(false);
        slGrabarAudio.gameObject.SetActive(false);
        }

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
        fotoPickeadaHandler(filePath);
        return;
#endif

        camcap.pickPhoto(fotoPickeadaHandler, camcapError);
    }

    private void GetTakePhoto()
    {
#if UNITY_EDITOR || UNITY_STANDLONE
        string filePath = "/PictureOnDisk.png";
        filePath = Application.dataPath + filePath;
        fotoPickeadaHandler(filePath);
        return;
#endif

        Natcamcap.takePhoto(-1,fotoPickeadaHandler, camcapError);
    }

    private void camcapError(string obj = "") 
    {
        Debug.Log(obj);
    }

    private void fotoPickeadaHandler(string obj = "")
    {
        string ext=Path.GetExtension(obj).ToLower();
        Alertas.instance.ShowLoadingPercentage("Subiendo foto");
         Debug.Log( "ShowLoadingPercentage: " + obj );
         //LoginPro.Manager.LoadLocalImage(obj, imagenGrupo);
         LoginPro.Manager.UploadFile(obj, "300_300_user_avatar_" + LoginPro.Session.idUser + ext,SendImageToServer_Success, Failed, whileUploadingEventHandler,300, "300", "300");
    }

    private void SendImageToServer_Success(string obj = "")
    {
        Alertas.instance.ShowLoading("Actualizando");
        Debug.Log(obj);
        string ext=Path.GetExtension(obj).ToLower();
        LoginPro.Manager.SaveUserImage(obj, LoginPro.Session.idUser, SendToServer_Success, Failed);
    }

    public void SendToServer_Success(string[] datas)
    {
        Alertas.instance.Hide();
         Debug.Log("datas:"+datas[0]);	
         LoginPro.Session.avatar = datas[0];
        btPickImageGrupo.gameObject.GetComponent<CanvasGroup>().alpha = 0;
        LoginPro.Manager.DownloadAndPlaceImage(datas[0], imagenGrupo);
    }

    public override void Show()
    {
        base.Show();
        Debug.Log("show configuracion");
        transform.Find("cuestionarioEstatus").gameObject.SetActive(false);


        btEstatus.gameObject.SetActive(LoginPro.Session.Role != LoginPro_UserRole.Administrador);

        Debug.Log("LoginPro.Session.Role " + LoginPro.Session.Role);

        Password.interactable = LoginPro.Session.Role != LoginPro_UserRole.Administrador;
        ConfirmPassword.interactable = LoginPro.Session.Role != LoginPro_UserRole.Administrador;
        Username.interactable = LoginPro.Session.Role != LoginPro_UserRole.Administrador;
        Mail.interactable = LoginPro.Session.Role != LoginPro_UserRole.Administrador;

        Username.text = LoginPro.Session.NombreCompleto;
        Mail.text = LoginPro.Session.Mail;
        Password.text = LoginPro.Session.Password;
        ConfirmPassword.text = LoginPro.Session.Password;

        trimenu.hide();
        btGuardar.gameObject.SetActive(false);
        if (LoginPro.Session.avatar != "")
        {
            btPickImageGrupo.gameObject.GetComponent<CanvasGroup>().alpha = 0;
            string[] ss = new string[1];
            ss[0] = LoginPro.Session.avatar;
            SendToServer_Success(ss);
        }
        else
        {
            btPickImageGrupo.gameObject.GetComponent<CanvasGroup>().alpha = 1;
        }

        btGuardarAudio.gameObject.SetActive(false);
        btAudio.gameObject.SetActive(false);
        pregunta.gameObject.SetActive(false);
        slGrabarAudio.gameObject.SetActive(false);

        siTg.onValueChanged.RemoveAllListeners();
        siTg.isOn=(LoginPro.Session.estado_busqueda_propia!="");
        noTg.isOn=(LoginPro.Session.estado_busqueda_propia=="");        
        if(LoginPro.Session.estado_busqueda_propia=="")
            siTg.onValueChanged.AddListener(tgSiHandler);
        siTg.interactable=LoginPro.Session.estado_busqueda_propia=="";
        noTg.interactable=LoginPro.Session.estado_busqueda_propia=="";


    }

    void apareceBtSave(string val)
    {
        btGuardar.gameObject.SetActive(true);
    }

    void UpdateUser()
    {
        if (Password.text != ConfirmPassword.text)
        {
            string errorMessage = "Las contraseñas no coinciden.";
            Debug.LogWarning(errorMessage);
            Failed(errorMessage);
            return;
        }
        else
        {

            Alertas.instance.ShowLoading("Actualizando");
            LoginPro.Manager.UpdateUser(Username.text, Mail.text, Password.text, SuccessActualizarUser, Failed);

        }
    }

    public void SuccessActualizarUser(string[] datas)
    {
        LoginPro.Session.NombreCompleto = Username.text;
        LoginPro.Session.Mail = Mail.text;
        LoginPro.Session.Password = Password.text;

        Alertas.instance.ShowMensaje("Usuario actualizado");
        btGuardar.gameObject.SetActive(false);
    }

    public void Failed(string errorMessage)
    {
        //errorMessage = errorMessage.Replace("ERROR: ", "Get achievements list failed: ");
        Alertas.instance.ShowMensaje(errorMessage);
        Debug.LogError(errorMessage);
    }


    void abrecambioestatus()
    {
        transform.Find("cuestionarioEstatus").gameObject.SetActive(true);
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
        mediaName = "audio_encontrousuario_" + LoginPro.Manager.getEpoch().ToString() + Path.GetExtension(obj).ToLower();
		Debug.LogWarning("audioPickeadaHandler obj " + obj);
        Debug.LogWarning("audioPickeadaHandler mediaName " + mediaName);

        slGrabarAudio.gameObject.SetActive(true);
    }

    void anadirAudio()
    {
        Alertas.instance.ShowLoadingPercentage("Subiendo archivo");
        if(media!="")
            LoginPro.Manager.UploadFile(Path.GetFullPath(media), mediaName,SubirArchivo, Failed, whileUploadingEventHandler);
        else
        SubirArchivo("");
    }

    void SubirArchivo(string link)
    {
        Alertas.instance.ShowLoading("Añadiendo testimonio");
        LoginPro.Manager.SaveUserEncontroAudio(LoginPro.Session.idUser, mediaName, SuccessAnadirTestimonioEncontro, Failed);
       
    }
    
    void whileUploadingEventHandler (float porcentaje)
	{
        Alertas.instance.UpdateLoadingPercentage(porcentaje);
	}

    public void SuccessAnadirTestimonioEncontro(string[] datas)
    {
        Alertas.instance.ShowMensaje("Estatus actualizado");
         btGuardarAudio.gameObject.SetActive(false);
        btAudio.gameObject.SetActive(false);
        pregunta.gameObject.SetActive(false);
        slGrabarAudio.gameObject.SetActive(false);

        siTg.interactable=false;
        noTg.interactable=false;
    }

    void regresar()
    {
        transform.Find("cuestionarioEstatus").gameObject.SetActive(false);

    }

    void logout()
    {
        PlayerPrefs.DeleteKey("user");
        PlayerPrefs.DeleteKey("password");
        PlayerPrefs.DeleteKey("entroEnOffline");

        LoginPro.Session.ClearSession();
        AppManager.instance.reset();

    }

}
