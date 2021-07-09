using System.Collections;
using System;
using System.IO;
using UnityEngine;

public class FTPManager:MonoBehaviour {

	FTPClient ftpc;
	public string servidorString;
	public string userString;
	public string passString;

	private string fileString;

	private long fileSizeVar;

	//download
	bool isMeasuringDownload=false;
	float porcentageBajado=0;

	bool isMeasuringUpload=false;
	float porcentageSubido=0;

	bool fireOnComplete=false;
	bool fireOnError=false;

	string daslink;

	//counter archivos
	int counterArchivos=0;

	int counterTimerAfterDownload=0;
	int waitingtime=0;

	Action<string> methodForSuccess;
	 Action<string> methodFail;
	  Action<float> methodForUpdate;

	void Start () {
		//ftp://s112763.gridserver.com:21/
		//acervo@thisisnotanumber.org
		//@Marzo1981
		ftpc = new FTPClient (servidorString, userString, passString);

		ftpc.onCompleteEvent += onCompleteEventHandler;
		ftpc.onErrorEvent += onErrorEventHandler;
	}

	void onCompleteEventHandler ( string link, long fsize){
		fileSizeVar = fsize;
		daslink = link;

		fireOnComplete = true;
		counterTimerAfterDownload=0;
		//waitingtime = (int)(((fileSizeVar / 1000000f) / 10.0f) * 60);
		waitingtime = 1;
		//Debug.Log ("onCompleteEventHandler ftpmanager");
	}

	void onErrorEventHandler ( string link){
		daslink = link;
		fireOnError = true;
		counterTimerAfterDownload=0;
	}

	public void Update () {
		if (isMeasuringDownload && ftpc != null) {
			porcentageBajado=ftpc.getPorcentajeBajado();
			//Debug.Log(porcentageBajado);
			if (methodForUpdate != null)
				methodForUpdate(porcentageBajado);
		}

		if (isMeasuringUpload && ftpc != null) {
			porcentageSubido=ftpc.getPorcentajeSubido();
			//Debug.Log(porcentageBajado);
			if (methodForUpdate != null)
				methodForUpdate(porcentageSubido);
		}

		if (fireOnComplete && counterTimerAfterDownload > waitingtime) {
			if (methodForSuccess != null)
				methodForSuccess (daslink);
			fireOnComplete = false;
			isMeasuringDownload = false;
			isMeasuringUpload = false;
		} else {
			counterTimerAfterDownload++;
		}

		if (fireOnError) {
			if (methodFail != null)
				methodFail (daslink);
			fireOnError=false;
			isMeasuringDownload=false;
			isMeasuringUpload = false;
		}
	}
	public void AbortRequest () {
		if (ftpc.isDownloading) {
			isMeasuringDownload=false;
			isMeasuringUpload = false;
			ftpc.abortDownloadRequest ();
		}
	}

	public string DirList (string folder) {
			return ftpc.DirectoryListing (folder);
	}

	public void Upload (string file, string output, Action<string> _methodForSuccess, Action<string> _methodFail, Action<float> _methodForUpdate) {
			ftpc.StartUploadHandler (file, output);

			porcentageSubido= 0;
			isMeasuringUpload = true;

			methodForSuccess=_methodForSuccess;
			methodFail=_methodFail;
			methodForUpdate=_methodForUpdate;
	}


	public void bajaArchivoProcess (string stringUrl, string dondeGuardar) {
		string ext = (Path.GetExtension (stringUrl) as String).ToLower ();
		ftpc.StartDownloadHandler(stringUrl, dondeGuardar+counterArchivos+ext);
		counterArchivos++;
		porcentageBajado = 0;
		isMeasuringDownload=true;
	}



}
