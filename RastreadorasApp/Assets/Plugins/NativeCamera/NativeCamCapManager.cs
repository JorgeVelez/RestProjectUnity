using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class NativeCamCapManager : MonoBehaviour {


	public RawImage pickPreiveimage;

	private int mode = 1;

	Action<string> savedMethod;
	Action<string> errordMethod;

	private void Start()
	{
		Debug.Log("camcap started");

		//this.CamCap.CaptureVideoCompleted += new CameraCapture.MediaDelegate(this.Completetd);
		//this.CamCap.TakePhotoCompleted += new CameraCapture.MediaDelegate(this.Completetd);
		//this.CamCap.PickCompleted += new CameraCapture.MediaDelegate(this.Completetd);
		//this.CamCap.Failed += new CameraCapture.ErrorDelegate(this.ErrorInfo);
	}

	private void Update()
	{
		
	}

	public void captureVideo(Action<string> methodForSuccess, Action<string> methodForError)
	{
		Debug.Log("captureVideo "+ methodForSuccess.ToString());
		//this.CamCap.captureVideo();
		savedMethod=methodForSuccess;
		errordMethod=methodForError;
	}



	public void takePhoto(int size,Action<string> methodForSuccess, Action<string> methodForError)
	{
		Debug.Log("takePhoto "+ methodForSuccess.ToString());
		savedMethod=methodForSuccess;
		errordMethod=methodForError;
		TakePicture(size);
		
	}



	public void playVideo()
	{
		Debug.Log("playVideo ");
		//this.CamCap.playVideo();
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

private void TakePicture(int maxSize)
{
	NativeCamera.Permission permission = NativeCamera.TakePicture( ( path ) =>
	{
		Debug.Log( "img path: " + path );
		if( path != null )
		{
			savedMethod(path);
		}else
		errordMethod("img path not valid");
	} );

	Debug.Log( "Permission result: " + permission );
}
private void TakePictureAndLoad( int maxSize )
{
	Debug.Log( "TakePicture" );
	NativeCamera.Permission permission = NativeCamera.TakePicture( ( path ) =>
	{
		Debug.Log( "Image path: " + path );
		if( path != null )
		{
			// Create a Texture2D from the captured image
			Texture2D texture = NativeCamera.LoadImageAtPath( path, maxSize );
			if( texture == null )
			{
				Debug.Log( "Couldn't load texture from " + path );
				return;
			}

			// Assign texture to a temporary quad and destroy it after 5 seconds
			GameObject quad = GameObject.CreatePrimitive( PrimitiveType.Quad );
			quad.transform.position = Camera.main.transform.position + Camera.main.transform.forward * 2.5f;
			quad.transform.forward = Camera.main.transform.forward;
			quad.transform.localScale = new Vector3( 1f, texture.height / (float) texture.width, 1f );
			
			Material material = quad.GetComponent<Renderer>().material;
			if( !material.shader.isSupported ) // happens when Standard shader is not included in the build
				material.shader = Shader.Find( "Legacy Shaders/Diffuse" );

			material.mainTexture = texture;
				
			Destroy( quad, 5f );

			// If a procedural texture is not destroyed manually, 
			// it will only be freed after a scene change
			Destroy( texture, 5f );
		}
	}, maxSize );

	Debug.Log( "Permission result: " + permission );
}

private void RecordVideo()
{
	NativeCamera.Permission permission = NativeCamera.RecordVideo( ( path ) =>
	{
		Debug.Log( "Video path: " + path );
		if( path != null )
		{
			// Play the recorded video
			//Handheld.PlayFullScreenMovie( "file://" + path );
			savedMethod("file://" + path);
		}else
		errordMethod("img path not valid");
	} );

	Debug.Log( "Permission result: " + permission );
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