using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class CamCapManager : MonoBehaviour {

	 CameraCapture CamCap;

	public RawImage pickPreiveimage;

	private int mode = 1;

	Action<string> savedMethod;
	Action<string> errordMethod;

	private void Start()
	{
		if (CamCap == null) {
			CamCap = GameObject.FindObjectOfType<CameraCapture> ();
		}

		Debug.Log("camcap started");

		this.CamCap.CaptureVideoCompleted += new CameraCapture.MediaDelegate(this.Completetd);
		this.CamCap.TakePhotoCompleted += new CameraCapture.MediaDelegate(this.Completetd);
		this.CamCap.PickCompleted += new CameraCapture.MediaDelegate(this.Completetd);
		this.CamCap.Failed += new CameraCapture.ErrorDelegate(this.ErrorInfo);
	}

	private void Update()
	{
		
	}

	public void captureVideo(Action<string> methodForSuccess, Action<string> methodForError)
	{
		Debug.Log("captureVideo "+ methodForSuccess.ToString());
		this.CamCap.captureVideo();
		savedMethod=methodForSuccess;
		errordMethod=methodForError;
	}

	public void pickVideo(Action<string> methodForSuccess, Action<string> methodForError)
	{
		Debug.Log("pickVideo "+ methodForSuccess.ToString());
		this.CamCap.pickVideo();
		savedMethod=methodForSuccess;
	}

	public void takePhoto(Action<string> methodForSuccess, Action<string> methodForError)
	{
		Debug.Log("takePhoto "+ methodForSuccess.ToString());
		this.CamCap.takePhoto();
		savedMethod=methodForSuccess;
		errordMethod=methodForError;
	}

	public void pickPhoto(Action<string> methodForSuccess, Action<string> methodForError)
	{
		Debug.Log("pickPhoto "+ methodForSuccess.ToString());
		this.CamCap.pickPhoto();
		savedMethod=methodForSuccess;
		errordMethod=methodForError;
	}

	public void playVideo()
	{
		Debug.Log("playVideo ");
		this.CamCap.playVideo();
	}


	private void Completetd(string patha)
	{
		Debug.Log("Completetd camcap"+patha);
		savedMethod(patha);
		//base.StartCoroutine(this.LoadImage(patha));
		
	}

	private void ErrorInfo(string errorInfo)
	{
		//savedMethod(errorInfo);
		errordMethod(errorInfo);
		//pathText.text = pathText.text + "\n<color=#ff0000>" + errorInfo +"</color>";
		Debug.Log(errorInfo);
	}


	IEnumerator LoadImage(string path)
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
	}
}