using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Alertas : MonoBehaviour {

	public static Alertas instance { get; private set; }

	public enum AlertType { Loading };

	GameObject alertaActual;


	void Awake()
	{
		if (instance == null)
			instance = this;

		transform.Find("waiting").gameObject.SetActive(false);
		transform.Find("mensaje").gameObject.SetActive(false);
	}

	public void ShowLoading(string msg)
	{
		Hide();
		alertaActual = transform.Find("waiting").gameObject;
		alertaActual.SetActive(true);
		alertaActual.transform.SetAsLastSibling();
		alertaActual.transform.Find("panel/mensaje").GetComponent<Text>().text = msg;
	}

	public void ShowLoadingPercentage(string msg)
	{
		Hide();
		alertaActual = transform.Find("loading").gameObject;
		alertaActual.SetActive(true);
		alertaActual.transform.SetAsLastSibling();
		alertaActual.transform.Find("panel/mensaje").GetComponent<Text>().text = msg;
		alertaActual.transform.Find("panel/Animator").GetComponent<Image>().fillAmount = 0;
	}

	public void UpdateLoadingPercentage(float val)
	{
		alertaActual.transform.Find("panel/Animator").GetComponent<Image>().fillAmount = val;
	}

	public void ShowMensaje(string msg,  string btMesage = "OK", Action action = null)
	{
		Hide();
		alertaActual = transform.Find("mensaje").gameObject;
		alertaActual.SetActive(true);
		alertaActual.transform.SetAsLastSibling();
		alertaActual.transform.Find("panel/mensaje").GetComponent<Text>().text = msg;
		alertaActual.transform.Find("panel/btok").GetComponentInChildren<Text>().text = btMesage;

		alertaActual.transform.Find("panel/btok").GetComponent<Button>().onClick.RemoveAllListeners();
		alertaActual.transform.Find("panel/btok").GetComponent<Button>().onClick.AddListener(() =>
		{	Hide();
			if (action != null)
				action();});
	}

	public void ShowMensajeCondicional(string msg,string texto="", string btMesage = "Eliminar", Action actionYes = null, Action actionNo = null,string btMesageCancel = "Cancelar")
	{
		Hide();
		alertaActual = transform.Find("mensajeCondicional").gameObject;
		alertaActual.SetActive(true);
		alertaActual.transform.SetAsLastSibling();
		alertaActual.transform.Find("panel/mensaje").GetComponent<Text>().text = msg;
		alertaActual.transform.Find("panel/texto").GetComponent<Text>().text = texto;
		alertaActual.transform.Find("panel/btok").GetComponentInChildren<Text>().text = btMesage;
		alertaActual.transform.Find("panel/btcancel").GetComponentInChildren<Text>().text = btMesageCancel;

		alertaActual.transform.Find("panel/btAnadir").gameObject.SetActive(btMesageCancel != "Cancelar");

		alertaActual.transform.Find("back").GetComponent<Button>().onClick.RemoveAllListeners();
		alertaActual.transform.Find("back").GetComponent<Button>().onClick.AddListener(Hide);

		alertaActual.transform.Find("panel/btok").GetComponent<Button>().onClick.RemoveAllListeners();
		alertaActual.transform.Find("panel/btok").GetComponent<Button>().onClick.AddListener(() => {
			Hide();
			if (actionYes != null)
				actionYes();
		});

		alertaActual.transform.Find("panel/btcancel").GetComponent<Button>().onClick.RemoveAllListeners();
		alertaActual.transform.Find("panel/btcancel").GetComponent<Button>().onClick.AddListener(() => {
			Hide();
			if (actionNo != null)
				actionNo();
		});
	}

	public void Hide()
	{
		if(alertaActual!=null)
		alertaActual.SetActive(false);
	}
}
